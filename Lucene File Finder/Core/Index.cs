using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis.MyAnalyzer;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using LuceneFileFinder.Type;
using LuceneFileFinder.Util;
using LuceneFileFinder.Forms;

namespace LuceneFileFinder.Core
{
    /// <summary>
    /// 索引文件 （普通文件 MP3文件） 
    /// <remarks>
    /// 创建时间：2010/4/29 更新时间：2010/6/21 
    /// 三个线程来处理文件索引操作：分扫描磁盘文件，处理扫描获取的文件数据，将文件信息加入到索引中。
    /// 三个线程来处理MP3索引操作：获取MP3文件，获取MP3Tag信息，将信息加入到索引中。
    /// </remarks>
    /// </summary>
    public class Index : IDisposable
    {
        #region  成员变量

        #region ....

        private List<string> fileScanPath = new List<string>();//文件要扫描的路径
        private List<string> mp3ScanPath = new List<string>();//MP3要扫描的路径

        private List<string> mp3Artist = new List<string>();//收集的MP3艺术家列表
        private List<string> mp3Directorys = new List<string>();//收集的MP3文件夹列表

        private IndexMode indexType;//当前索引的类型

        private int fileCounter = 0;//文件计数
        private int folderCounter = 0;//文件夹计数
        private int mp3FileCounter = 0;//mp3文件计数
        private int mp3FolderCounter = 0;//mp3文件夹计数

        private string fileStorePath = Static.FileIndexStorePath;//文件索引存放的路径
        private string mp3StorePath = Static.MP3IndexStorePath;//mp3索引存放的路径
        private string currentDirectory = "";//当前正在索引的文件夹

        private IndexWriter fileIndexWriter;//文件索引
        private IndexWriter mp3IndexWriter;//mp3文件索引

        private Thread threadGetFile;//扫描获取文件线程
        private Thread threadDealFile;//处理扫描获取的数据的线程
        private Thread threadAnalyzeFile;//将处理后的文件数据加入到索引的线程

        private Thread threadGetMP3;//扫描获取MP3文件线程
        private Thread threadGetMP3Tag;//获取MP3TAG数据的线程
        private Thread threadAnalyzeMP3;//将MP3TAG数据加入到索引的线程

        private bool isScanFileEnd = false;//扫描磁盘获取文件或目录是否结束
        private bool isDealDataEnd = false;//处理扫描获取的文件数据是否结束

        private bool isScanMP3End = false;//扫描获取MP3文件是否结束
        private bool isGetMP3TagEnd = false;//获取MP3Tag信息是否结束

        private bool isStopIndexFile = false;//是否要停止索引文件
        private bool isStopIndexMP3 = false;//是否要停止索引MP3

        private bool isIndexFileComplete = false;//索引文件是否完成
        private bool isIndexMP3Complete = false;//索引MP3是否完成

        private DateTime dtFileStart;//文件索引开始时间
        private DateTime dtFileStop;//文件索引结束时间

        private DateTime dtMP3Start;//MP3索引开始时间
        private DateTime dtMP3Stop;//MP3索引结束时间

        private FileFinder form;//主窗
        /// <summary>
        /// 定义一个代理，用于跨线程访问，线程回调修改主窗体。
        /// </summary>
        private delegate void CrossThreadOperateControl();

        #region FileQueueStack
        /// <summary>
        /// FileDirectoryFinder执行查找的路径的栈
        /// </summary>
        private Stack<string> scanDirectoryStack = new Stack<string>(5000);

        /// <summary>
        /// FileDirectoryFinder找到的文件数据队列
        /// </summary>
        private Queue<FindFileData> findFileDataQueue = new Queue<FindFileData>(1000);

        /// <summary>
        /// 将findFileData处理后的数据队列
        /// </summary>
        private Queue<FileData> fileDataQueue = new Queue<FileData>(1000);

        #endregion

        #region MP3QueueStack

        /// <summary>
        /// 扫描的MP3文件夹栈
        /// </summary>
        private Stack<string> mp3DirectoryStack = new Stack<string>();

        /// <summary>
        /// 找到的MP3文件属性队列
        /// </summary>
        private Queue<MP3FileData> mp3FileQueue = new Queue<MP3FileData>();

        /// <summary>
        /// 读取到的MP3Tag信息队列
        /// </summary>
        private Queue<MP3TagData> mp3TagQueue = new Queue<MP3TagData>();

        #endregion

        #endregion

        #region MP3Tag 变量
        byte[] tagBody = new byte[128];
        string tagFlag;
        string[] GENRE = {"Blues","Classic Rock","Country","Dance","Disco","Funk","Grunge","Hip-Hop",
            "Jazz","Metal","New Age","Oldies","Other", "Pop", "R&B", "Rap", "Reggae", "Rock", "Techno", 
            "Industrial", "Alternative"};
        #endregion

        #region FileFiled
        private Field f_Name = new Field("Name", "", Field.Store.YES, Field.Index.ANALYZED);
        private Field f_Directory = new Field("Directory", "", Field.Store.YES, Field.Index.NOT_ANALYZED);
        private Field f_Length = new Field("Length", "", Field.Store.YES, Field.Index.NO);
        private Field f_LastWriteTime = new Field("LastWriteTime", "", Field.Store.YES, Field.Index.NO);
        private Field f_Extension = new Field("Extension", "", Field.Store.YES, Field.Index.NOT_ANALYZED);
        #endregion

        #region MP3Field
        private Field m_SongName = new Field("SongName", "", Field.Store.YES, Field.Index.ANALYZED);
        private Field m_Artist = new Field("Artist", "", Field.Store.YES, Field.Index.ANALYZED);
        private Field m_Album = new Field("Album", "", Field.Store.YES, Field.Index.ANALYZED);
        private Field m_PubYear = new Field("PubYear", "", Field.Store.YES, Field.Index.NO);
        private Field m_Genre = new Field("Genre", "", Field.Store.YES, Field.Index.NO);
        //此项存储音乐家，用于TermQuery搜索艺术家曲目数。
        private Field m_ArtistStore = new Field("ArtistStore", "", Field.Store.YES, Field.Index.NOT_ANALYZED);
        #endregion

        #endregion

        #region 构造函数
        /// <summary>
        /// 索引前需要重新初始化
        /// </summary>
        public Index(FileFinder _form, IndexMode type)
        {
            this.form = _form;
            this.indexType = type;
        }
        #endregion

        #region IndexFile

        #region GetFilesDirectorys
        /// <summary>
        /// 根据目录队列逐个扫描，获取文件或目录。
        /// </summary>
        private void GetFilesDirectorys()
        {
            DateTime dt1 = DateTime.Now;
            this.isScanFileEnd = false;
            FileDirectoryFinder finder = new FileDirectoryFinder();
            bool isQueueEmpty = false;
            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexFile == true)
                    break;
                lock (this.scanDirectoryStack)
                {
                    if (this.scanDirectoryStack.Count > 0)
                    {
                        isQueueEmpty = false;
                        finder.SearchPath = this.scanDirectoryStack.Pop();
                        finder.Reset();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }
                if (isQueueEmpty == false)
                {
                    while (finder.MoveNext())
                    {
                        //将找到的文件数据放入findFileData队列。
                        lock (this.findFileDataQueue)
                        {
                            this.findFileDataQueue.Enqueue(finder.CurrentFileData);
                        }

                        //当前对象为目录
                        if (finder.IsFile == false)
                        {
                            lock (this.scanDirectoryStack)
                            {
                                this.scanDirectoryStack.Push(finder.FullName);
                            }
                            this.folderCounter++;//文件夹计数
                        }
                        else
                        {
                            this.fileCounter++;//文件计数
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("扫描磁盘" + d);
            this.isScanFileEnd = true;//完成扫描目录获取文件
        }
        #endregion

        #region DealFindData
        /// <summary>
        /// 处理找到的文件数据，将文件数据转成文件信息。
        /// </summary>
        private void DealFindData()
        {
            DateTime dt1 = DateTime.Now;
            this.isDealDataEnd = false;
            FindFileData findFileData = new FindFileData();
            FileData fileData = new FileData();
            bool isQueueEmpty = true;
            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexFile == true)
                    break;

                lock (this.findFileDataQueue)
                {
                    if (this.findFileDataQueue.Count > 0)
                    {
                        isQueueEmpty = false;
                        findFileData = this.findFileDataQueue.Dequeue();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }

                if (isQueueEmpty == false)
                {
                    fileData.Name = findFileData.name;
                    fileData.Directory = findFileData.directory;
                    if (findFileData.isFile)
                    {
                        fileData.Length = Deal.ToLong(findFileData.nFileSizeHigh,
                            findFileData.nFileSizeLow).ToString();
                        fileData.Extension = Deal.GetExtension(fileData.Name);//扩展名
                    }
                    else
                    {
                        fileData.Length = "-1";//文件夹标志
                        fileData.Extension = ".f";//文件夹
                    }

                    fileData.LastWriteTime = Deal.ToDateTimeString(findFileData.ftLastWriteTime_dwHighDateTime,
                        findFileData.ftLastWriteTime_dwLowDateTime);

                    lock (this.fileDataQueue)
                    {
                        this.fileDataQueue.Enqueue(fileData);
                    }
                }
                else if (this.isScanFileEnd == false)
                {
                    //Console.Out.WriteLine("FindFileDataQueue 空");
                    Thread.Sleep(0);//让出时间片给其他线程
                    continue;
                }
                else
                {
                    break;
                }
            }

            this.isDealDataEnd = true;//处理数据结束
            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("处理" + d);
        }
        #endregion

        #region AnalyzeFileData
        /// <summary>
        /// 使用lucene对获取的文件数据进行分析
        /// </summary>
        private void AnalyzeFileData()
        {
            DateTime dt1 = DateTime.Now;
            FileData fileData = new FileData();
            bool isQueueEmpty = true;

            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexFile == true)
                    break;

                //为fileDataQueue获取互斥锁来实现同步
                lock (this.fileDataQueue)
                {
                    if (this.fileDataQueue.Count > 0)
                    {
                        isQueueEmpty = false;
                        fileData = this.fileDataQueue.Dequeue();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }

                if (isQueueEmpty == false)
                {
                    //设置当前正在索引的文件夹
                    lock (this.currentDirectory)
                    {
                        this.currentDirectory = fileData.Directory;
                    }
                    this.f_Name.SetValue(fileData.Name);
                    this.f_Length.SetValue(fileData.Length);
                    this.f_Directory.SetValue(fileData.Directory);
                    this.f_LastWriteTime.SetValue(fileData.LastWriteTime);
                    this.f_Extension.SetValue(fileData.Extension);

                    Document doc = new Document();
                    doc.Add(this.f_Name);
                    doc.Add(this.f_Length);
                    doc.Add(this.f_Directory);
                    doc.Add(this.f_LastWriteTime);
                    doc.Add(this.f_Extension);

                    this.fileIndexWriter.AddDocument(doc);
                }
                else if (this.isDealDataEnd == false)
                {
                    //Console.Out.WriteLine("FileDataQueue 空");
                    Thread.Sleep(0);//让出时间片给其他线程
                    continue;
                }
                else
                {
                    break;
                }
            }

            //对索引优化，将多个段文件合并成一个文件，减少搜索时打开的文件数，提高搜索速度。
            this.fileIndexWriter.Optimize();
            this.fileIndexWriter.Close();

            this.dtFileStop = DateTime.Now;//索引结束
            this.isIndexFileComplete = true;
            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("分析" + d);
            //Console.Out.WriteLine(this.fileCounter + this.folderCounter);

            //为索引全部，在文件索引末尾继续索引MP3
            if (this.indexType == IndexMode.All)
            {
                this.IndexMP3();
            }
            else
            {
                this.SaveIndexInfo();//保存索引信息到索引信息文件
                this.CallBack();
            }
        }
        #endregion

        #region IndexFile
        /// <summary>
        /// 索引文件
        /// </summary>
        private void IndexFile()
        {
            this.dtFileStart = DateTime.Now;//文件索引开始
            this.isIndexFileComplete = false;

            this.fileIndexWriter = new IndexWriter(this.fileStorePath, new MyAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED);
            this.fileIndexWriter.SetMergeFactor(10000);//达到10000个doc才合并

            for (int i = this.fileScanPath.Count - 1; i >= 0; i--)
            {
                if (Directory.Exists(this.fileScanPath[i].ToString()))
                {
                    this.scanDirectoryStack.Push(this.fileScanPath[i].ToString());
                }
            }

            //扫描磁盘获取文件数据的线程
            this.threadGetFile = new Thread(new ThreadStart(this.GetFilesDirectorys));
            this.threadGetFile.Start();

            //处理扫描磁盘获取的文件数据的线程
            this.threadDealFile = new Thread(new ThreadStart(this.DealFindData));
            this.threadDealFile.Start();

            //将分析后的文件数据加入索引的线程。
            this.threadAnalyzeFile = new Thread(new ThreadStart(this.AnalyzeFileData));
            this.threadAnalyzeFile.Start();
        }
        #endregion

        #endregion

        #region IndexMP3

        #region GetMP3File
        /// <summary>
        /// 获取MP3文件
        /// </summary>
        private void GetMP3File()
        {
            DateTime dt1 = DateTime.Now;
            FileDirectoryFinder finder = new FileDirectoryFinder();
            this.isScanMP3End = false;
            MP3FileData mp3File = new MP3FileData();
            bool isQueueEmpty = false;
            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexMP3 == true)
                    break;
                lock (this.mp3DirectoryStack)
                {
                    if (this.mp3DirectoryStack.Count > 0)
                    {
                        isQueueEmpty = false;
                        finder.SearchPath = this.mp3DirectoryStack.Pop();
                        finder.Reset();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }
                if (isQueueEmpty == false)
                {
                    while (finder.MoveNext())
                    {
                        //当前对象为目录
                        if (finder.IsFile == true)
                        {
                            //不是mp3文件，继续。
                            if (finder.CurrentFileData.name.ToLower().EndsWith(".mp3") == false)
                            {
                                continue;
                            }

                            lock (this.mp3FileQueue)
                            {
                                mp3File.name = finder.CurrentFileData.name;
                                mp3File.directory = finder.CurrentFileData.directory;

                                mp3File.length = Deal.ToLong(finder.CurrentFileData.nFileSizeHigh,
                                        finder.CurrentFileData.nFileSizeLow).ToString();

                                mp3File.lastWriteTime = Deal.ToDateTimeString(
                                    finder.CurrentFileData.ftLastWriteTime_dwHighDateTime,
                                   finder.CurrentFileData.ftLastWriteTime_dwLowDateTime);

                                lock (this.mp3FileQueue)
                                {
                                    this.mp3FileQueue.Enqueue(mp3File);
                                }
                                this.mp3FileCounter++;
                            }
                        }
                        else
                        {
                            lock (this.mp3DirectoryStack)
                            {
                                this.mp3DirectoryStack.Push(finder.FullName);
                            }
                            this.mp3FolderCounter++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("GetMP3File" + d);
            this.isScanMP3End = true;
        }
        #endregion

        #region GetMP3Tag
        /// <summary>
        /// 获取MP3的Tag信息
        /// </summary>
        private void GetMP3Tag()
        {   
            DateTime dt1 = DateTime.Now;
            this.isGetMP3TagEnd = false;
            string fullName = "";//文件全名
            MP3FileData mp3File = new MP3FileData();
            MP3TagData mp3Tag = new MP3TagData();
            bool isQueueEmpty = false;
            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexMP3 == true)
                    break;

                lock (this.mp3FileQueue)
                {
                    if (this.mp3FileQueue.Count > 0)
                    {
                        isQueueEmpty = false;
                        mp3File = this.mp3FileQueue.Dequeue();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }

                if (isQueueEmpty == false)
                {
                    //文件信息
                    mp3Tag.name = mp3File.name;
                    mp3Tag.length = mp3File.length;
                    mp3Tag.directory = mp3File.directory;
                    mp3Tag.lastWriteTime = mp3File.lastWriteTime;

                    fullName = mp3Tag.directory + mp3Tag.name;

                    //读取MP3文件的最后128个字节的内容
                    try
                    {
                        using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read))
                        {
                            fs.Seek(-128, SeekOrigin.End);
                            fs.Read(tagBody, 0, 128);
                            fs.Close();
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    //取TAG段的前三个字节
                    tagFlag = Encoding.Default.GetString(tagBody, 0, 3);
                    //如果没有TAG信息，则置为null
                    if (!"TAG".Equals(tagFlag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        mp3Tag.album = "";
                        mp3Tag.artist = "";
                        mp3Tag.genre = "";
                        mp3Tag.pubYear = "";
                        mp3Tag.songName = "";
                    }
                    else
                    {
                        //按照MP3 ID3 V1 的tag定义，依次读取相关的信息
                        mp3Tag.songName = Encoding.Default.GetString(tagBody, 3, 30);
                        mp3Tag.songName = (mp3Tag.songName.Trim()).Trim('\0');
                        mp3Tag.artist = Encoding.Default.GetString(tagBody, 33, 30);
                        mp3Tag.artist = (mp3Tag.artist.Trim()).Trim('\0');
                        mp3Tag.album = Encoding.Default.GetString(tagBody, 62, 30);
                        mp3Tag.album = (mp3Tag.album.Trim()).Trim('\0');
                        mp3Tag.pubYear = Encoding.Default.GetString(tagBody, 93, 4);
                        mp3Tag.pubYear = (mp3Tag.pubYear.Trim()).Trim('\0');
                        Int16 g = (Int16)tagBody[127];
                        mp3Tag.genre = g >= this.GENRE.Length ? "" : this.GENRE[g];
                    }

                    lock (this.mp3TagQueue)
                    {
                        this.mp3TagQueue.Enqueue(mp3Tag);
                    }
                }
                else if (this.isScanMP3End == false)
                {
                    //Console.WriteLine("mp3FileQueue 空");
                    Thread.Sleep(0);//让出时间片给其他线程
                    continue;
                }
                else
                {
                    break;
                }
            }

            this.isGetMP3TagEnd = true;//获取MP3Tag结束
            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("GetMP3TagEnd" + d);      
        }
        #endregion

        #region AnalyzeMP3Tag
        /// <summary>
        /// 分析MP3Tag并加入索引
        /// </summary>
        private void AnalyzeMP3Tag()
        {
            DateTime dt1 = DateTime.Now;
            MP3TagData mp3Tag = new MP3TagData();
            bool isQueueEmpty = true;
            while (true)
            {
                //如果用户执行了停止索引，结束。
                if (this.isStopIndexMP3 == true)
                    break;

                lock (this.mp3TagQueue)
                {
                    if (this.mp3TagQueue.Count > 0)
                    {
                        isQueueEmpty = false;
                        mp3Tag = this.mp3TagQueue.Dequeue();
                    }
                    else
                    {
                        isQueueEmpty = true;
                    }
                }

                if (isQueueEmpty == false)
                {
                    //设置当前正在索引的文件夹
                    lock (this.currentDirectory)
                    {
                        this.currentDirectory = mp3Tag.directory;
                    }

                    //收集音乐家
                    if (this.mp3Artist.Contains(mp3Tag.artist) == false)
                    {
                        this.mp3Artist.Add(mp3Tag.artist);
                        Console.WriteLine(mp3Tag.artist);
                    }

                    //收集文件夹
                    if (this.mp3Directorys.Contains(mp3Tag.directory) == false)
                    {
                        this.mp3Directorys.Add(mp3Tag.directory);
                    }

                    this.f_Name.SetValue(mp3Tag.name);
                    this.f_Length.SetValue(mp3Tag.length);
                    this.f_Directory.SetValue(mp3Tag.directory);
                    this.f_LastWriteTime.SetValue(mp3Tag.lastWriteTime);
                    this.m_Album.SetValue(mp3Tag.album);
                    this.m_Artist.SetValue(mp3Tag.artist);
                    this.m_Genre.SetValue(mp3Tag.genre);
                    this.m_PubYear.SetValue(mp3Tag.pubYear);
                    this.m_SongName.SetValue(mp3Tag.songName);
                    this.m_ArtistStore.SetValue(mp3Tag.artist);

                    Document doc = new Document();
                    doc.Add(this.f_Name);
                    doc.Add(this.f_Length);
                    doc.Add(this.f_Directory);
                    doc.Add(this.f_LastWriteTime);
                    doc.Add(this.m_Album);
                    doc.Add(this.m_Artist);
                    doc.Add(this.m_Genre);
                    doc.Add(this.m_PubYear);
                    doc.Add(this.m_SongName);
                    doc.Add(this.m_ArtistStore);

                    this.mp3IndexWriter.AddDocument(doc);
                }
                else if (this.isGetMP3TagEnd == false)
                {
                    //Console.Out.WriteLine("mp3TagQueue 空");
                    Thread.Sleep(0);//让出时间片给其他线程
                    continue;
                }
                else
                {
                    break;
                }
            }

            //对索引优化，将多个段文件合并成一个文件，减少搜索时打开的文件数，提高搜索速度。
            this.mp3IndexWriter.Optimize();
            this.mp3IndexWriter.Close();
            
            this.dtMP3Stop = DateTime.Now;//MP3索引结束
            this.isIndexMP3Complete = true;
            double d = (DateTime.Now - dt1).TotalSeconds;
            //Console.Out.WriteLine("分析MP3" + d);
            //Console.Out.WriteLine(this.mp3FileCounter + this.mp3FolderCounter);

            //执行索引后可能修改mp3Artist，更新mp3Artist。
            Static.MP3Artist = this.mp3Artist;
            //执行索引后可能修改mp3Artist，置MP3ArtistInfo为null，在搜索时重新更新。
            Static.MArtistInfo = null;

            this.SaveIndexInfo();//保存索引信息
            this.CallBack();
        }
        #endregion

        #region IndexMP3
        private void IndexMP3()
        {
            this.dtMP3Start = DateTime.Now;//MP3索引开始
            this.isIndexMP3Complete = false;

            this.mp3IndexWriter = new IndexWriter(this.mp3StorePath, new MyAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED);
            this.mp3IndexWriter.SetMergeFactor(10000);//达到10000个doc才合并

            for (int i = 0; i < this.mp3ScanPath.Count; i++)
            {
                if (Directory.Exists(this.mp3ScanPath[i].ToString()))
                {
                    this.mp3DirectoryStack.Push(this.mp3ScanPath[i].ToString());
                }
            }

            this.threadGetMP3 = new Thread(new ThreadStart(this.GetMP3File));
            this.threadGetMP3.Start();

            this.threadGetMP3Tag = new Thread(new ThreadStart(this.GetMP3Tag));
            this.threadGetMP3Tag.Start();

            this.threadAnalyzeMP3 = new Thread(new ThreadStart(this.AnalyzeMP3Tag));
            this.threadAnalyzeMP3.Start();
        }
        #endregion

        #endregion

        #region SaveIndexInfo
        /// <summary>
        /// 保存文件索引信息的索引信息文件
        /// </summary>
        private void SaveIndexInfo()
        {
            IndexInfoDB indexInfoDB = new IndexInfoDB();//索引数据库信息
            indexInfoDB.ReadIndexInfoDB();
            string strNow = DateTime.Now.ToString();
            switch (indexType)
            {
                case IndexMode.All:
                    {
                        if (indexInfoDB.FCreationTime == "")
                        {
                            indexInfoDB.FCreationTime = strNow;
                        }
                        indexInfoDB.FUpdateTime = strNow;
                        indexInfoDB.FFileNums = this.fileCounter;
                        indexInfoDB.FFolderNums = this.folderCounter;
                        indexInfoDB.FileIndexPath = this.fileScanPath;

                        if (indexInfoDB.MCreationTime == "")
                        {
                            indexInfoDB.MCreationTime = strNow;
                        }

                        indexInfoDB.MUpdateTime = strNow;
                        indexInfoDB.MFileNums = this.mp3FileCounter;
                        indexInfoDB.MFolderNums = this.mp3FolderCounter;
                        indexInfoDB.MP3IndexPath = Deal.ListStringClone(this.mp3ScanPath);
                        indexInfoDB.MP3Artist = Deal.ListStringClone(this.mp3Artist);
                        indexInfoDB.MP3Directorys = Deal.ListStringClone(this.mp3Directorys); 
                        break;
                    }
                case IndexMode.File:
                    {
                        if (indexInfoDB.FCreationTime == "")
                        {
                            indexInfoDB.FCreationTime = strNow;
                        }
                        indexInfoDB.FUpdateTime = strNow;
                        indexInfoDB.FFileNums = this.fileCounter;
                        indexInfoDB.FFolderNums = this.folderCounter;
                        indexInfoDB.FileIndexPath = Deal.ListStringClone(this.fileScanPath);
                        break;
                    }
                case IndexMode.MP3:
                    {
                        if (indexInfoDB.MCreationTime == "")
                        {
                            indexInfoDB.MCreationTime = strNow;
                        }

                        indexInfoDB.MUpdateTime = strNow;
                        indexInfoDB.MFileNums = this.mp3FileCounter;
                        indexInfoDB.MFolderNums = this.mp3FolderCounter;
                        indexInfoDB.MP3IndexPath = Deal.ListStringClone(this.mp3ScanPath);
                        indexInfoDB.MP3Artist = Deal.ListStringClone(this.mp3Artist);
                        indexInfoDB.MP3Directorys = Deal.ListStringClone(this.mp3Directorys);

                        //更新全局变量中的艺术家信息与曲目数。
                        Static.MP3Artist = Deal.ListStringClone(indexInfoDB.MP3Artist);
                        Static.MP3SongNums = indexInfoDB.MFileNums;
                        break;
                    }
            }
            indexInfoDB.WriteIndexInfoDB();
        }
        #endregion

        #region CallBack
        /// <summary>
        /// 线程回调，执行完索引线程后对主线程做的操作。
        /// </summary>
        private void CallBack()
        {
            CrossThreadOperateControl CrossOperate = delegate()
            {
                lock (this.form)
                {
                    this.form.IsIndex = false;
                    this.form.索引ToolStripMenuItem.Enabled = true;
                    this.form.停止更新ToolStripMenuItem.Enabled = false;
                }
            };
            this.form.Invoke(CrossOperate);
        }
        #endregion

        #region 属性

        #region IndexFileFolderNum
        /// <summary>
        /// 索引文件数，文件夹数字符串
        /// </summary>
        public string IndexFileFolderNum
        {
            get 
            {
                if (this.IsIndexComplete == false)
                    return "";
                switch (this.indexType)
                {
                    case IndexMode.All:
                        return "文件：" + Deal.ToEnglishNumString(this.fileCounter) + " 目录：" +
                            Deal.ToEnglishNumString(this.folderCounter) + "   " +
                            "MP3：" + Deal.ToEnglishNumString(this.mp3FileCounter) + " MP3目录：" +
                             Deal.ToEnglishNumString(this.mp3FolderCounter);
                    case IndexMode.File:
                        return "文件：" + Deal.ToEnglishNumString(this.fileCounter) + " 目录：" +
                            Deal.ToEnglishNumString(this.folderCounter);
                    case IndexMode.MP3:
                        return "MP3：" + Deal.ToEnglishNumString(this.mp3FileCounter) + " MP3目录：" +
                             Deal.ToEnglishNumString(this.mp3FolderCounter);
                }
                return "";
            }
        }
        #endregion

        #region IsIndexComplete
        /// <summary>
        /// 索引是否完成
        /// </summary>
        public bool IsIndexComplete
        {
            get
            {
                switch (this.indexType)
                {
                    case IndexMode.All: 
                        return this.isIndexFileComplete && this.isIndexMP3Complete;
                    case IndexMode.File:
                        return this.isIndexFileComplete;
                    case IndexMode.MP3:
                        return this.isIndexMP3Complete;
                }
                return false;
            }
        }
        #endregion

        #region IsIndexFileComplete
        /// <summary>
        /// 是否完成了文件索引
        /// </summary>
        public bool IsFileIndexComplete
        {
            get { return this.isIndexFileComplete; }
        }
        #endregion

        #region IsIndexMP3Complete
        /// <summary>
        /// 是否完成了MP3索引
        /// </summary>
        public bool IsIndexMP3Complete
        {
            get { return this.isIndexMP3Complete; }
        }
        #endregion

        #region IndexTime
        /// <summary>
        /// 索引用时
        /// </summary>
        public double IndexTime
        {
            get
            {
                if (this.IsIndexComplete == false)
                    return 0.0;
                switch (this.indexType)
                {
                    case IndexMode.All:
                        return this.IndexFileTime + this.IndexMP3Time;
                    case IndexMode.File:
                        return this.IndexFileTime;
                    case IndexMode.MP3:
                        return this.IndexMP3Time;
                }
                return 0.0;
            }
        }
        #endregion

        #region IndexFileTime
        /// <summary>
        /// 文件索引时间
        /// </summary>
        public double IndexFileTime
        {
            get
            {
                if (this.isIndexFileComplete)
                    return (this.dtFileStop - this.dtFileStart).TotalSeconds;
                else
                    return 0.0;
            }
        }
        #endregion

        #region IndexMP3Time
        /// <summary>
        /// MP3索引时间
        /// </summary>
        public double IndexMP3Time
        {
            get
            {
                if (this.isIndexMP3Complete)
                    return (this.dtMP3Stop - this.dtMP3Start).TotalSeconds;
                else
                    return 0.0;
            }
        }
        #endregion

        #region FileScanPath
        /// <summary>
        /// 获取或设置文件索引路径
        /// </summary>
        public List<string> FileScanPath
        {
            set 
            {
                this.fileScanPath.Clear();
                foreach (string item in value)
                {
                    this.fileScanPath.Add(item);
                } 
            }
            get { return this.fileScanPath; }
        }
        #endregion

        #region MP3ScanPath
        /// <summary>
        /// 获取或设置MP3索引路径
        /// </summary>
        public List<string> MP3ScanPath
        {
            set
            {
                this.mp3ScanPath.Clear();
                foreach (string item in value)
                {
                    this.mp3ScanPath.Add(item);
                }
            }
            get { return this.mp3ScanPath; }
        }
        #endregion

        #endregion

        #region 公共方法

        #region GetCurrentDirectory
        /// <summary>
        /// 获取正在索引的文件夹。
        /// </summary>
        /// <returns>正在索引的文件夹</returns>
        public string GetCurrentDirectory()
        {
            lock (this.currentDirectory)
            {
                return this.currentDirectory;
            }
        }
        #endregion

        #region StartIndexFile
        /// <summary>
        /// 开始索引文件
        /// </summary>
        public void StartIndexFile()
        {
            if (this.indexType != IndexMode.File)
                return;
            else
                this.IndexFile();
        }
        #endregion

        #region StartIndexMP3
        /// <summary>
        /// 开始索引MP3
        /// </summary>
        public void StartIndexMP3()
        {
            if (this.indexType != IndexMode.MP3)
                return;
            else
                this.IndexMP3();
        }
        #endregion

        #region StartIndexAll
        /// <summary>
        /// 开始索引文件和MP3，先索引文件再索引MP3
        /// </summary>
        public void StartIndexAll()
        {
            if (this.indexType != IndexMode.All)
                return;
            else
                this.IndexFile(); //在indexFile()方法最后判断是否要继续索引MP3          
        }
        #endregion

        #region StopIndexFile
        /// <summary>
        /// 停止索引文件
        /// </summary>
        public void StopIndexFile()
        {
            this.isStopIndexFile = true;
        }
        #endregion

        #region StopIndexMP3
        /// <summary>
        /// 停止索引MP3
        /// </summary>
        public void StopIndexMP3()
        {
            this.isStopIndexMP3 = true;
        }
        #endregion

        #region StopIndexAll
        /// <summary>
        /// 停止所有索引
        /// </summary>
        public void StopIndexALl()
        {
            this.isStopIndexFile = true;
            this.isStopIndexMP3 = true;
        }
        #endregion

        #region Reset
        /// <summary>
        /// 重置索引器
        /// </summary>
        public void Reset(IndexMode _type)
        {
            this.indexType = _type;

            this.currentDirectory = "";

            this.scanDirectoryStack.Clear();
            this.findFileDataQueue.Clear();
            this.fileDataQueue.Clear();

            this.mp3DirectoryStack.Clear();
            this.mp3FileQueue.Clear();
            this.mp3TagQueue.Clear();

            this.mp3Artist.Clear();
            this.mp3Directorys.Clear();

            this.fileScanPath.Clear();
            this.mp3ScanPath.Clear();

            this.isScanFileEnd = false;
            this.isDealDataEnd = false;

            this.isScanMP3End = false;
            this.isGetMP3TagEnd = false;

            this.isStopIndexFile = false;
            this.isStopIndexMP3 = false;

            this.isIndexFileComplete = false;
            this.isIndexMP3Complete = false;

            this.fileCounter = 0;
            this.folderCounter = 0;
            this.mp3FileCounter = 0;
            this.mp3FolderCounter = 0;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            //this.fileScanPath = null;
            //this.mp3ScanPath = null;
            //this.mp3Artist = null;
            //this.mp3Directorys = null;

            //this.scanDirectoryStack = null;
            //this.fileDataQueue = null;
            //this.findFileDataQueue = null;

            //this.mp3DirectoryStack = null;
            //this.mp3FileQueue = null;
            //this.mp3TagQueue = null;
 
            GC.SuppressFinalize(this);          
            GC.Collect();
        }
        #endregion

        #endregion
    }
}

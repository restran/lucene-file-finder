using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.MyAnalyzer;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using LuceneFileFinder.Forms;
using LuceneFileFinder.Type;
using LuceneFileFinder.Util;

namespace LuceneFileFinder.Core
{
    /// <summary>
    /// 搜索文件 (普通文件 MP3 JPG TXT)
    /// <remarks>
    /// 创建日期 2010/5/4 更新日期 2010/5/16
    /// </remarks>
    /// </summary>
    public class Search
    {
        #region 成员变量

        private int pageNums;//搜索结果的总页数
        private int currentPage;//当前的页数
        private IndexSearcher indexSearcher;
        private DateTime dtStart = DateTime.Now;//开始时间
        private DateTime dtStop = DateTime.Now;//结束时间
        private Analyzer analyzer = new MyAnalyzer();//分词方法
        private KeywordStruct keyword;//关键字
        private FileFinder form;//主窗
        private BooleanQuery boolQuery;//搜索采用布尔搜索，可以匹配多个关键字。
        private BooleanQuery oldBoolQuery;//上次的搜索
        private TopDocs topDocs;//搜索到的doc。
        private String[] mp3Fields = { "Name", "SongName", "Artist", "Album" };//mp3分词搜索的field
        private SearchMode searchMode;//搜索模式
        private bool isSearchWithResult;//是否在结果中搜索
        private bool isStopSave = false;//是否要停止保存结果。
        private bool isStopSearch = false;//是否要停止搜索。
        /// <summary>
        /// 定义一个代理，用于跨线程访问
        /// </summary>
        private delegate void CrossThreadOperateControl();
        /// <summary>
        /// 定义一个代理作为UpdateFileListView和UpdateMP3ListView的函数指针。
        /// </summary>
        private delegate void UpdateLVMethod(out ListViewItem[] items, ref ScoreDoc[] scoreDocs, ref int indexRecord, int times);

        #endregion

        #region 构造函数
        /// <summary>
        /// Search构造函数
        /// </summary>
        /// <param name="_form">主窗</param>
        /// <param name="_mode">搜索模式</param>
        public Search(FileFinder _form, SearchMode _mode)
        {
            //修改maxCluseCount是为了用PrefixQuery时没有数量的限制，
            //PrefixQuery会将找到的数据用TermQuery，然后用booleanQuery.add()方法。
            BooleanQuery.SetMaxClauseCount(int.MaxValue);

            this.oldBoolQuery = new BooleanQuery();
            this.form = _form;
            this.SetSearcher(_mode);
        }
        #endregion

        #region Search

        #region SearchFile
        /// <summary>
        /// 搜索文件
        /// </summary>
        public void SearchFile()
        {
            this.isStopSearch = false;
            this.dtStart = DateTime.Now;//开始时间

            if (this.indexSearcher == null)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            Query query;
 
            if (this.keyword.Name != "")
            {
                try
                {
                    QueryParser parser = new QueryParser("Name", this.analyzer);
                    query = parser.Parse(this.keyword.Name);
                    boolQuery.Add(query, BooleanClause.Occur.MUST);
                }
                catch (Exception)
                { }
            }

            if (this.keyword.Directory != null && this.keyword.Directory != "")
            {
                PrefixQuery pq = new PrefixQuery(new Term("Directory", this.keyword.Directory));
                boolQuery.Add(pq, BooleanClause.Occur.MUST);
            }

            if (this.keyword.Extension != null && this.keyword.Extension != "")
            {
                TermQuery termQuery = new TermQuery(new Term("Extension", this.keyword.Extension));
                boolQuery.Add(termQuery, BooleanClause.Occur.MUST);
            }
                             
            try
            {
                //搜索前Static.PageItems项
                this.topDocs = this.indexSearcher.Search(boolQuery, Static.PageItems);
            }
            catch (Exception)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            this.dtStop = DateTime.Now;//完成时间
            if (this.topDocs.totalHits > 0)
            {
                CrossThreadOperateControl CrossUpdateStatus = delegate()
                {
                    lock (this.form.tsslStatus)
                    {
                        this.form.OrginalStatusText = Deal.ToEnglishNumString(this.topDocs.totalHits) 
                            + " 个对象     用时：" + (this.dtStop - this.dtStart).TotalSeconds + "秒";
                        this.form.tsslStatus.Text = Deal.LimitStringLength(this.form.OrginalStatusText, 
                            this.form.TsslLength);
                    }
                };
                this.form.Invoke(CrossUpdateStatus);
            }
            else
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            this.pageNums = topDocs.totalHits / Static.PageItems + 1;//总的页数
            this.currentPage = 1;//每次搜索时当前页数为1

            UpdateLVMethod method = new UpdateLVMethod(this.UpdateFileListView);
            this.UpdateListView(method, this.currentPage);
            this.CallBack();//线程执行完，修改主窗的相关属性。
        }
        #endregion

        #region SearchMP3
        /// <summary>
        /// 搜索MP3
        /// </summary>
        public void SearchMP3()
        {
            this.isStopSearch = false;
            this.dtStart = DateTime.Now;//搜索开始

            if (this.indexSearcher == null)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            Query query;

            if (this.keyword.Name != "")
            {
                try
                {                 
                    MultiFieldQueryParser parser = new MultiFieldQueryParser(this.mp3Fields, this.analyzer);
                    query = parser.Parse(this.keyword.Name);
                    boolQuery.Add(query, BooleanClause.Occur.MUST);
                }
                catch (Exception)
                { }
            }

            if (this.keyword.Directory != null && this.keyword.Directory != "")
            {
                PrefixQuery pq = new PrefixQuery(new Term("Directory", this.keyword.Directory));
                boolQuery.Add(pq, BooleanClause.Occur.MUST);
            }

            if (this.keyword.Artist != null)
            {
                TermQuery term = new TermQuery(new Term("ArtistStore", this.keyword.Artist));
                boolQuery.Add(term, BooleanClause.Occur.MUST);
            }

            try
            {
                //搜索前Static.PageItems项
                this.topDocs = this.indexSearcher.Search(boolQuery, Static.PageItems);
            }
            catch (Exception)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            this.dtStop = DateTime.Now;//完成时间
            if (this.topDocs.totalHits > 0)
            {
                CrossThreadOperateControl CrossUpdateStatus = delegate()
                {
                    this.form.OrginalStatusText = Deal.ToEnglishNumString(this.topDocs.totalHits)
                        + " 个对象     用时：" + (this.dtStop - this.dtStart).TotalSeconds + "秒";
                    this.form.tsslStatus.Text = Deal.LimitStringLength(this.form.OrginalStatusText, 
                        this.form.TsslLength);
                };
                this.form.Invoke(CrossUpdateStatus);
            }
            else
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            this.pageNums = topDocs.totalHits / Static.PageItems + 1;//总的页数
            this.currentPage = 1;//每次搜索时当前页数为1

            UpdateLVMethod method = new UpdateLVMethod(this.UpdateMP3ListView);
            this.UpdateListView(method, this.currentPage);
            this.CallBack();//线程执行完，修改主窗的相关属性。
        }
        #endregion

        #region SearchJPG
        /// <summary>
        /// 搜索JPG
        /// </summary>
        public void SearchJPG()
        {
            this.isStopSearch = false;
            this.dtStart = DateTime.Now;//搜索开始

            if (this.indexSearcher == null)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            TermQuery termQuery = new TermQuery(new Term("Extension", "jpg"));
            this.boolQuery.Add(termQuery, BooleanClause.Occur.MUST);
            this.SearchFile();
        }
        #endregion

        #region SearchTXT
        /// <summary>
        /// 搜索TXT
        /// </summary>
        public void SearchTXT()
        {
            this.isStopSearch = false;
            this.dtStart = DateTime.Now;//搜索开始

            if (this.indexSearcher == null)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            TermQuery termQuery = new TermQuery(new Term("Extension", "txt"));
            this.boolQuery.Add(termQuery, BooleanClause.Occur.MUST);
            this.SearchFile();
        }
        #endregion

        #region ShowMP3Artist
        /// <summary>
        /// 用搜索的方式获取并显示艺术家和曲目数。
        /// </summary>
        public void ShowMP3Artist()
        {
            this.isStopSearch = false;
            if (this.indexSearcher == null)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }

            CrossThreadOperateControl CrossUpdateStatus = delegate()
            {
                lock (this.form)
                {
                    this.form.OrginalStatusText = Deal.ToEnglishNumString(Static.MP3Artist.Count)
                        + " 名艺术家   " + Deal.ToEnglishNumString(Static.MP3SongNums) + " 首曲目";
                    this.form.tsslStatus.Text = Deal.LimitStringLength(this.form.OrginalStatusText,
                        this.form.TsslLength);
                }
            };
            this.form.Invoke(CrossUpdateStatus);

            //该模式下只有1页，不用分页。
            this.pageNums = 1;
            this.currentPage = 1;

            int TotalRecord; //总的数据项数
            int EveryTimeRecord; //每次批量增加的数量
            int TotalTimes;//批量添加的执行次数
            int indexRecord = 0;

            if (Static.MArtistInfo == null)
            {
                Static.MArtistInfo = new List<MP3ArtistInfo>();
                Term termField = new Term("ArtistStore");

                TotalRecord = Static.MP3Artist.Count; //总的数据项数
                EveryTimeRecord = 50; //每次批量增加的数量
                TotalTimes = TotalRecord / EveryTimeRecord;//批量添加的执行次数

                for (int i = 0; i <= TotalTimes; i++)
                {
                    if (this.isStopSearch == true)
                        break;

                    ListViewItem[] items;

                    if (i < TotalTimes) //i不是最后一次执行
                    {
                        this.UpadateArtistListView(out items, ref indexRecord, EveryTimeRecord, ref termField);
                    }
                    else
                    {
                        int lastQuantity = TotalRecord - indexRecord;
                        this.UpadateArtistListView(out items, ref indexRecord, lastQuantity, ref termField);
                    }

                    CrossThreadOperateControl CrossUpdateListView = delegate()
                    {
                        lock (this.form.lvwFiles)
                        {
                            this.form.lvwFiles.BeginUpdate();
                            //利用AddRange批量增加，目的是减少刷屏次数，减轻不断闪烁问题。
                            this.form.lvwFiles.Items.AddRange(items);
                            this.form.lvwFiles.EndUpdate();
                        }
                    };
                    this.form.lvwFiles.Invoke(CrossUpdateListView);
                }
            }
            else
            {
                TotalRecord = Static.MArtistInfo.Count; //总的数据项数
                EveryTimeRecord = 500; //每次批量增加的数量
                TotalTimes = TotalRecord / EveryTimeRecord;//批量添加的执行次数

                for (int i = 0; i <= TotalTimes; i++)
                {
                    if (this.isStopSearch == true)
                        break;

                    ListViewItem[] items;

                    if (i < TotalTimes) //i不是最后一次执行
                    {
                        this.UpadateArtistListView(out items, ref indexRecord, EveryTimeRecord);
                    }
                    else
                    {
                        int lastQuantity = TotalRecord - indexRecord;
                        this.UpadateArtistListView(out items, ref indexRecord, lastQuantity);
                    }

                    CrossThreadOperateControl CrossUpdateListView = delegate()
                    {
                        lock (this.form.lvwFiles)
                        {
                            this.form.lvwFiles.BeginUpdate();
                            //利用AddRange批量增加，目的是减少刷屏次数，减轻不断闪烁问题。
                            this.form.lvwFiles.Items.AddRange(items);
                            this.form.lvwFiles.EndUpdate();
                        }
                    };
                    this.form.lvwFiles.Invoke(CrossUpdateListView);
                }             
            }

            this.CallBack();//线程执行完，修改主窗的相关属性。

        }
        #endregion

        #region UpdateListView
        private void UpdateListView(UpdateLVMethod method, int currentPage)
        {
            int TotalRecord = this.topDocs.scoreDocs.Length - ((this.currentPage - 1) * Static.PageItems);//总的数据项数
            int EveryTimeRecord = 40; //每次批量增加的数量
            int TotalTimes = TotalRecord / EveryTimeRecord;//批量添加的执行次数
            int indexStart = (this.currentPage - 1) * Static.PageItems;//开始的index
            int indexRecord = indexStart;
            ScoreDoc[] scoreDocs = this.topDocs.scoreDocs;
            for (int i = 0; i <= TotalTimes; i++)
            {
                if (this.isStopSearch == true)
                    return;

                ListViewItem[] items;

                if (i < TotalTimes) //i不是最后一次执行
                {
                    method(out items, ref scoreDocs, ref indexRecord, EveryTimeRecord);
                }
                else
                {
                    int lastQuantity = TotalRecord + indexStart - indexRecord;
                    method(out items, ref scoreDocs, ref indexRecord, lastQuantity);
                }

                CrossThreadOperateControl CrossUpdateListView = delegate()
                {
                    lock (this.form.lvwFiles)
                    {
                        this.form.lvwFiles.BeginUpdate();
                        //利用AddRange批量增加，目的是减少刷屏次数，减轻不断闪烁问题。
                        this.form.lvwFiles.Items.AddRange(items);
                        this.form.lvwFiles.EndUpdate();
                    }
                };
                this.form.lvwFiles.Invoke(CrossUpdateListView);
            }
        }
        #endregion

        #region UpadateFileListView
        /// <summary>
        /// 更新ListView中的项
        /// </summary>
        private void UpdateFileListView(out ListViewItem[] items, ref ScoreDoc[] scoreDocs, ref int indexRecord, int times)
        {
            //GetSystemIcon getIcon = new GetSystemIcon();//获取图标
            string extenName = null;//扩展名
            string name = null;//文件名
            string length = null;
            int k;
            items = new ListViewItem[times];
            for (int j = 0; j < times; j++)
            {
                Document doc = this.indexSearcher.Doc(scoreDocs[indexRecord++].doc);
                name = doc.Get("Name");
                length = doc.Get("Length");
                //文件夹
                if (length == "-1")
                {
                    string[] subItem = { name, doc.Get("Directory"), "", doc.Get("LastWriteTime") };
                          
                    items[j] = new ListViewItem(subItem, 0);//0为文件夹
                }
                else
                {
                    string[] subItem = { name, doc.Get("Directory"), 
                        Deal.FormatFileLength(length), doc.Get("LastWriteTime") };
                    k = name.LastIndexOf('.');
                    if (k > 0)
                    {
                        extenName = name.Substring(k, name.Length - k).ToLower();
                        //如果imageList里还没有该格式的图标，就添加
                        //文件夹，未知文件，mp3，jpg的图标已在主窗体创建时就加入ImageList中
                        if (this.form.imgIcon.Images.ContainsKey(extenName) == false)
                        {
                            Icon icon = GetSystemIcon.GetIconByFileType(extenName, false);
                            if (icon != null)
                            {
                                CrossThreadOperateControl CrossUpdateImageIcon = delegate()
                                {
                                    this.form.imgIcon.Images.Add(extenName, icon);
                                };
                                this.form.Invoke(CrossUpdateImageIcon);//使用创建ImageList的线程调用
                                items[j] = new ListViewItem(subItem, extenName);
                            }
                            else
                                items[j] = new ListViewItem(subItem, 1);//1为未知文件
                        }
                        else
                            items[j] = new ListViewItem(subItem, extenName);
                    }
                    else
                    {
                        items[j] = new ListViewItem(subItem, 1);//1为未知文件
                    }
                }
            }
        }
        #endregion

        #region UpadateMP3ListView
        /// <summary>
        /// 更新MP3的ListView中的项
        /// </summary>
        private void UpdateMP3ListView(out ListViewItem[] items, ref ScoreDoc[] scoreDocs, ref int indexRecord, int times)
        {
            string length;//文件大小
            string name;//文件名
            items = new ListViewItem[times];
            for (int j = 0; j < times; j++)
            {
                Document doc = this.indexSearcher.Doc(scoreDocs[indexRecord++].doc);
                length = doc.Get("Length");
                name = doc.Get("Name");
                if (length == "-1")
                {
                    continue;//过滤文件夹
                }
                else
                {
                    length = Deal.FormatFileLength(length);//将1000转换成1,000 KB
                    string[] subItem = { name, doc.Get("SongName"), doc.Get("Artist"), doc.Get("Album"), 
                      doc.Get("PubYear"), doc.Get("Genre"), length, doc.Get("LastWriteTime"), doc.Get("Directory")};

                    items[j] = new ListViewItem(subItem, 2);//mp3在imagelist中的imagekey是2。
                }
            }
        }
        #endregion

        #region UpadateArtistListView
        /// <summary>
        /// 更新MP3为Artist模式下的ListView中的项
        /// </summary>
        private void UpadateArtistListView(out ListViewItem[] items, ref int indexRecord, int times, ref Term termField)
        {
            int nums;
            items = new ListViewItem[times];
            for (int i = 0; i < times; i++)
            {
                try
                {
                    Term term = termField.CreateTerm(Static.MP3Artist[indexRecord]);
                    TermQuery query = new TermQuery(term);
                    this.topDocs = this.indexSearcher.Search(query, 1);
                    nums = this.topDocs.totalHits;
                }
                catch (Exception)
                {
                    nums = 0;
                    continue;
                }

                Static.MArtistInfo.Add(new MP3ArtistInfo(Static.MP3Artist[indexRecord], nums));
                string[] subItem = { Static.MArtistInfo[indexRecord].artist, 
                              Deal.ToEnglishNumString(Static.MArtistInfo[indexRecord].songNums)};
                indexRecord++;
                //artis图标在imagelist中的imagekey是5。
                items[i] = new ListViewItem(subItem, 5);
            }
        }
        #endregion

        #region UpadateArtistListView
        /// <summary>
        /// 更新MP3为Artist模式下的ListView中的项，数据已获取过。
        /// </summary>
        private void UpadateArtistListView(out ListViewItem[] items, ref int indexRecord, int times)
        {
            items = new ListViewItem[times];
            for (int i = 0; i < times; i++)
            {
                string[] subItem = { Static.MArtistInfo[indexRecord].artist, 
                      Deal.ToEnglishNumString(Static.MArtistInfo[indexRecord].songNums)};
                //artis图标在imagelist中的imagekey是5。
                items[i] = new ListViewItem(subItem, 5);

                indexRecord++;
            }
        }
        #endregion

        #endregion

        #region GetCurrentPage
        /// <summary>
        /// 获取当前页码所在页的数据并更新ListView
        /// </summary>
        public void GetCurrentPage()
        {
            try
            {
                this.topDocs = this.indexSearcher.Search(boolQuery, Static.PageItems * this.currentPage);
            }
            catch (Exception)
            {
                this.NullResult();//空结果，修改主窗的status状态。
                this.CallBack();//线程执行完，修改主窗的相关属性。
                return;
            }
            UpdateLVMethod method;
            switch (this.searchMode)
            {
                case SearchMode.File:
                    method = new UpdateLVMethod(this.UpdateFileListView);
                    break;
                case SearchMode.MP3:
                    method = new UpdateLVMethod(this.UpdateMP3ListView);
                    break;
                default:
                    method = new UpdateLVMethod(this.UpdateMP3ListView);
                    break;
            }
            
            this.UpdateListView(method, this.currentPage);

            CrossThreadOperateControl CrossOperate = delegate()
            {
                this.form.IsSearch = false;
                this.form.索引ToolStripMenuItem.Enabled = true;
                this.form.保存结果ToolStripMenuItem.Enabled = true;
            };
            this.form.Invoke(CrossOperate);
        }
        #endregion

        #region GetRangePage
        /// <summary>
        /// 供保存结果时使用，不更新主窗的ListView。
        /// </summary>
        public List<string> GetRangePage(int paegBegin, int pageEnd)
        {
            //刚开始设置为false。
            this.isStopSave = false;

            List<string> result = new List<string>();

            if (paegBegin <= 0 || pageEnd > this.pageNums)
                return result;
            try
            {
                this.topDocs = this.indexSearcher.Search(boolQuery, Static.PageItems * pageEnd);
            }
            catch (Exception)
            {
                return result;
            }

            int TotalRecord = this.topDocs.scoreDocs.Length - ((paegBegin - 1) * Static.PageItems);//总的数据项数
            int EveryTimeRecord = 200; //每次批量增加的数量
            int TotalTimes = TotalRecord / EveryTimeRecord;//批量添加的执行次数
            int indexStart = (paegBegin - 1) * Static.PageItems;//开始的index
            int indexRecord = indexStart;
            ScoreDoc[] scoreDocs = this.topDocs.scoreDocs;
            for (int i = 0; i <= TotalTimes; i++)
            {
                if (this.isStopSave == true)
                    break;

                if (i < TotalTimes) //i不是最后一次执行
                {
                    this.GetResultData(ref result, ref scoreDocs, ref indexRecord, EveryTimeRecord);
                }
                else
                {
                    int lastQuantity = TotalRecord + indexStart - indexRecord;
                    this.GetResultData(ref result, ref scoreDocs, ref indexRecord, lastQuantity);
                }
            }
            return result;
        }
        #endregion

        #region GetResultData
        /// <summary>
        /// 获取结果数据，只包含fullName，即name + directory。
        /// </summary>
        private void GetResultData(ref List<string> result, ref ScoreDoc[] scoreDocs, ref int indexRecord, int times)
        {
            string name;
            string directory;
            for (int i = 0; i < times; i++)
            {
                Document doc = this.indexSearcher.Doc(scoreDocs[indexRecord++].doc);
                directory = doc.Get("Directory");
                name = doc.Get("Name");
                result.Add(directory + name);
            }
        }
        #endregion

        #region 属性

        #region Keyword
        /// <summary>
        /// 关键字
        /// </summary>
        public KeywordStruct Keyword
        {
            set 
            { 
                this.keyword.Directory = value.Directory;
                //这里的扩展名索引内存的是没有“.”开头的，文件夹是“.f”，无扩展名的是“.u”。
                if (value.Extension != null && value.Extension != "")
                {
                    if (value.Extension[0] == '.')
                        this.keyword.Extension = value.Extension.TrimStart('.');
                    else
                        this.keyword.Extension = value.Extension.Insert(0, ".");
                }
                else
                {
                    this.keyword.Extension = "";
                }

                this.keyword.Name = value.Name;
                this.keyword.Artist = value.Artist;
            }
            get { return this.keyword; }
        }
        #endregion

        #region IsSearchWithResult
        /// <summary>
        /// 是否在结果中搜索
        /// </summary>
        public bool IsSearchWithResult
        {
            set { this.isSearchWithResult = value; }
            get { return this.isSearchWithResult; }
        }
        #endregion

        #region IsSearcherNull
        /// <summary>
        /// 搜索器是否为空，执行索引前判断是否为空，不为空就关闭indexSearcher
        /// </summary>
        public bool IsSearcherNull
        {
            get 
            {
                if (this.indexSearcher == null)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #region SMode
        /// <summary>
        /// 搜索模式
        /// </summary>
        public SearchMode SMode
        {
            set { this.searchMode = value; }
            get { return this.searchMode; }
        }
        #endregion

        #region CurrentPage
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage
        {
            set { this.currentPage = value; }
            get { return this.currentPage; }
        }
        #endregion

        #region PageNums
        /// <summary>
        /// 搜索结果的总页数
        /// </summary>
        public int PageNums
        {
            set { this.pageNums = value; }
            get { return this.pageNums; }
        }
        #endregion

        #region IsStopSave
        /// <summary>
        /// 是否要停止保存结果
        /// </summary>
        public bool IsStopSave
        {
            set { this.isStopSave = value; }
            get { return this.isStopSave; }
        }
        #endregion

        #region IsStopSearch
        /// <summary>
        /// 是否停止搜索
        /// </summary>
        public bool IsStopSearch
        {
            set { this.isStopSearch = true; }
            get { return this.isStopSearch; }
        }
        #endregion

        #endregion

        #region 私有方法

        #region CallBack
        /// <summary>
        /// 线程执行完后回调
        /// </summary>
        private void CallBack()
        {
            if (this.isSearchWithResult == false)
            {
                this.oldBoolQuery = (BooleanQuery)this.boolQuery.Clone();
            }
            else
            {
                //当前为结果中搜索，保留为原始的boolQuery。
            }

            CrossThreadOperateControl CrossOperate = delegate()
            {
                lock (this.form)
                {
                    this.form.IsSearch = false;
                    this.form.SearchResult = this.form.OrginalStatusText;//更新搜索结果
                    this.form.索引ToolStripMenuItem.Enabled = true;
                    this.form.保存结果ToolStripMenuItem.Enabled = true;
                    this.form.CurrentPage = 1;
                    this.form.PageNums = this.pageNums;
                    if (this.pageNums > 1)
                    {
                        this.form.btnPrevious.Visible = true;
                        this.form.tsslPages.Visible = true;
                        this.form.btnNext.Visible = true;

                        this.form.btnPrevious.Enabled = false;
                        this.form.btnNext.Enabled = true;

                        this.form.tsslPages.Text = "  " + this.currentPage + "/" + this.pageNums + "  ";
                    }
                    else
                    {
                        this.form.btnPrevious.Visible = false;
                        this.form.tsslPages.Visible = false;
                        this.form.btnNext.Visible = false;
                    }
                }
            };
            this.form.Invoke(CrossOperate);
        }
        #endregion

        #region NullResult
        /// <summary>
        /// 无结果或搜索时发生异常，修改主窗的status状态。
        /// </summary>
        private void NullResult()
        {
            this.dtStop = DateTime.Now;
            //无结果时也是只有1页。
            this.currentPage = 1;
            this.pageNums = 1;
            CrossThreadOperateControl CrossUpdateStatus = delegate()
            {
                lock (this.form)
                {
                    this.form.IsSearch = false;
                    ////无结果无法保存。
                    //this.form.保存结果ToolStripMenuItem.Enabled = false;
                    this.form.OrginalStatusText = "0 个对象     用时：" 
                        + (this.dtStop - this.dtStart).TotalSeconds + "秒";
                    this.form.tsslStatus.Text = Deal.LimitStringLength(this.form.OrginalStatusText, 
                        this.form.TsslLength);
                }
            };
            this.form.Invoke(CrossUpdateStatus);
        }
        #endregion

        #endregion

        #region 公共方法

        #region SetSearcher
        /// <summary>
        /// 设置索引器的搜索模式，即设置indexSearcher指向的索引数据库。
        /// </summary>
        /// <param name="_mode">搜索模式</param>
        public void SetSearcher(SearchMode mode)
        {
            string indexStorePath;
            switch (mode)
            {
                case SearchMode.File:
                    indexStorePath = Static.FileIndexStorePath;
                    break;
                case SearchMode.MP3:
                    indexStorePath = Static.MP3IndexStorePath;
                    break;
                default:
                    indexStorePath = Static.FileIndexStorePath;
                    break;
            }

            try
            {
                this.indexSearcher = new IndexSearcher(indexStorePath);
            }
            catch (Exception)
            {
                this.indexSearcher = null;
            }
        }
        #endregion

        #region Reset
        /// <summary>
        /// 重置Search
        /// </summary>
        public void Reset()
        {
            if (this.isSearchWithResult)
            {
                //如果为结果中搜索，继续用结果中搜索前保存的boolQuery。
                this.boolQuery = (BooleanQuery)this.oldBoolQuery.Clone();
            }
            else
            {
                this.boolQuery = new BooleanQuery();
            }
        }
        #endregion

        #region CloseSearcher
        /// <summary>
        /// 关闭indexSearcher
        /// </summary>
        public void CloseSearcher()
        {
            try
            {
                this.indexSearcher.Close();
            }
            catch
            { }
        }
        #endregion

        #region Warmup
        /// <summary>
        /// 预热
        /// </summary>
        public void Warmup()
        {
            if (this.indexSearcher == null)
            {
                return;
            }

            Query query;
            try
            {
                QueryParser parser = new QueryParser("Name", this.analyzer);
                query = parser.Parse("test");
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                //搜索前Static.PageItems项
                this.topDocs = this.indexSearcher.Search(query, Static.PageItems);
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        #endregion
    }
}

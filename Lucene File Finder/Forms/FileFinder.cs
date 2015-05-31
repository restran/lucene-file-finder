using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LuceneFileFinder.Control;
using LuceneFileFinder.Core;
using LuceneFileFinder.Type;
using LuceneFileFinder.Util;

namespace LuceneFileFinder.Forms
{
    /// <summary>
    /// File Finder 主窗
    /// <remarks>
    /// 创建时间：2010/4/27 更新时间：2010/6/21
    /// </remarks>
    /// </summary>
    public partial class FileFinder : Form
    {
        #region 内部变量

        private Index index;//索引
        private Search search;//搜索

        private IndexMode indexMode;//索引模式
        private SearchMode searchMode;//搜索模式

        private Thread threadSearch;//搜索线程      

        private ModeStatus modeStatus;//模式状态
        private MagicMirrorStatus magicMirrorStatus;//魔镜状态
        private ListViewStatus lvStatus;//ListView的显示状态，有文件，MP3和MP3艺术家三种。

        private MagicMirror magicMirror;//魔镜

        private ListViewColumnSorter lvwColumnSorter;

        private string currentDirectory = "";//正在扫描的目录
        private string searchResult = "";//搜索结果，在搜索线程的callback中更新
        private string lastKeyword = "";//上次的搜索关键词，供结果中搜索时使用。
        private string orginalStatusText;//原始的状态栏文本

        private bool isIndex;//是否正在索引
        private bool isSearch;//是否正在搜索
        private bool isSearchWithResult;//是否是在结果中搜索
        private bool isSWRFirst;//在结果中搜索时，第一次让cmbKeyword.Text为""，不触发搜索。
        private int currentPage = 1;//当前页码
        private int pageNums = 1;//总的页数
        private int tsslLength;//状态栏文字的长度

        #region ListView中MP3的列
        private System.Windows.Forms.ColumnHeader m_fileName;
        private System.Windows.Forms.ColumnHeader m_fullName;
        private System.Windows.Forms.ColumnHeader m_length;
        private System.Windows.Forms.ColumnHeader m_lastWriteTime;
        private System.Windows.Forms.ColumnHeader m_songName;
        private System.Windows.Forms.ColumnHeader m_artist;
        private System.Windows.Forms.ColumnHeader m_album;
        private System.Windows.Forms.ColumnHeader m_pubYear;
        private System.Windows.Forms.ColumnHeader m_genre;

        //当关键字为空时显示。
        private System.Windows.Forms.ColumnHeader m_listArtist;//艺术家
        private System.Windows.Forms.ColumnHeader m_songNums;//艺术家的曲目数
        #endregion

        #endregion

        #region 构造函数
        public FileFinder()
        {
            InitializeComponent();
            this.InitSettings();
            this.InitMP3Column();
            //创建一个ListView排序类的对象，并设置listView1的排序器
            this.lvwColumnSorter = new ListViewColumnSorter();
            this.lvwFiles.ListViewItemSorter = this.lvwColumnSorter;
            this.search.Warmup();
        }
        #endregion

        #region Button

        #region btnMode_MouseDown
        /// <summary>
        /// 模式选择
        /// </summary>
        private void btnMode_MouseDown(object sender, MouseEventArgs e)
        {
            Point location = this.btnMode.PointToScreen(this.btnMode.Location);
            this.cmsMode.Show(location.X - 5, location.Y + this.btnMode.Height - 3);
        }
        #endregion

        #region btnSearchWR_Click
        private void btnSearchWR_Click(object sender, EventArgs e)
        {
            if (this.isSearchWithResult == false)
            {
                this.btnSearchWR.Image = Properties.Resources.swr_press;
                this.isSearchWithResult = true;
                this.toolTip.SetToolTip(this.btnSearchWR, "关闭结果中搜索");
                //设置搜索线程的搜索状态是正常搜索还是结果中搜索
                this.search.IsSearchWithResult = this.isSearchWithResult;

                this.lastKeyword = this.cmbKeyword.Text;
                this.isSWRFirst = true;
                this.cmbKeyword.Text = "";
            }
            else
            {
                this.btnSearchWR.Image= Properties.Resources.swr;
                this.isSearchWithResult = false;
                this.toolTip.SetToolTip(this.btnSearchWR, "开启结果中搜索");
                this.search.IsSearchWithResult = this.isSearchWithResult;
                this.cmbKeyword.Text = this.lastKeyword;
            }
        }
        #endregion

        #region btnMagicMirror_Click
        private void btnMagicMirror_Click(object sender, EventArgs e)
        {
            if (this.pnlMagicMirror.Visible == false)
            {
                this.btnMagicMirror.Image = Properties.Resources.magicMirror_press;
                this.pnlMagicMirror.Visible = true;
                this.magicMirrorStatus = MagicMirrorStatus.Open;
                this.toolTip.SetToolTip(this.btnMagicMirror, "关闭魔镜");
                this.ShowMagicProperty();//打开魔镜，显示属性。
            }
            else
            {
                this.btnMagicMirror.Image = Properties.Resources.magicMirror;
                this.pnlMagicMirror.Visible = false;
                this.magicMirrorStatus = MagicMirrorStatus.Close;
                this.toolTip.SetToolTip(this.btnMagicMirror, "打开魔镜");
            }
        }
        #endregion

        #endregion

        #region 模式选择菜单
        private void 所有类型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnMode.Image = Properties.Resources.all;
            this.toolTip.SetToolTip(this.btnMode, "所有类型");
            this.modeStatus = ModeStatus.FILE;//设置搜索线程的搜索模式，是FILE或MP3或JPG或TXT。
            this.lvwFiles.Items.Clear();//清除数据
            if (this.lvStatus != ListViewStatus.File)
            {
                this.SetFileColumn();//将listview设置为显示文件的列
            }
            this.searchMode = SearchMode.File;//设置搜索的模式
            this.search.SetSearcher(SearchMode.File);
            this.cmbKeyword_TextChanged(this, new EventArgs());//重新搜索
        }

        private void mP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnMode.Image = Properties.Resources.mp3;
            this.toolTip.SetToolTip(this.btnMode, "MP3音乐");
            this.modeStatus = ModeStatus.MP3;
            this.lvwFiles.Items.Clear();//清除数据
            if (this.cmbKeyword.Text != "" || this.isSearchWithResult == true)
            {
                if (this.lvStatus != ListViewStatus.MP3)
                {
                    this.SetMP3Column();//将listview设置为显示MP3的列
                }
            }
            else
            {
                if (this.lvStatus != ListViewStatus.MP3Artist)
                {
                    this.SetMP3ArtistColumn();//将listview设置为显示MP3艺术家的列
                }
            }
            this.searchMode = SearchMode.MP3;
            this.search.SetSearcher(SearchMode.MP3);
            this.cmbKeyword_TextChanged(this, new EventArgs());//重新搜索
        }

        private void jPG图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnMode.Image = Properties.Resources.jpg;
            this.toolTip.SetToolTip(this.btnMode, "JPG图片");
            this.modeStatus = ModeStatus.JPG;
            this.lvwFiles.Items.Clear();//清除数据
            if (this.lvStatus != ListViewStatus.File)
            {
                this.SetFileColumn();//将listview设置为显示文件的列
            }
            this.searchMode = SearchMode.File;
            this.search.SetSearcher(SearchMode.File);
            this.cmbKeyword_TextChanged(this, new EventArgs());//重新搜索
        }

        private void 纯文本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnMode.Image = Properties.Resources.txt;
            this.toolTip.SetToolTip(this.btnMode, "纯文本文件");
            this.modeStatus = ModeStatus.TXT;
            this.lvwFiles.Items.Clear();//清除数据
            if (this.lvStatus != ListViewStatus.File)
            {
                this.SetFileColumn();//将listview设置为显示文件的列
            }
            this.searchMode = SearchMode.File;
            this.search.SetSearcher(SearchMode.File);
            this.cmbKeyword_TextChanged(this, new EventArgs());//重新搜索
        }
        #endregion
        
        #region ToolStripMenuItem_Click
        
        #region 文件

        #region 停止更新
        private void 停止更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.index.StopIndexALl();
        }
        #endregion

        #region 关闭
        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isSearch || this.isIndex)
            {
                this.CloseThread();
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region 保存结果
        private void 保存结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveForm save = new SaveForm(this, this.search);
            save.Show(this);
        }
        #endregion

        #region 索引

        #region 索引管理
        private void 索引管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //设置allowDrop为false是因为，indexManager中也有可以拖动的。
            this.AllowDrop = false;
            IndexManager indexManager = new IndexManager();
            indexManager.Show(this);
        }
        #endregion

        #region 更新文件
        private void 更新文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartIndex(IndexMode.File);
        }
        #endregion

        #region 更新所有
        private void 更新所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartIndex(IndexMode.All);
        }
        #endregion

        #region 更新MP3
        private void 更新MP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartIndex(IndexMode.MP3);
        }
        #endregion

        #endregion

        #endregion

        #region 查看

        #region 显示
        private void 小图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwFiles.View = View.SmallIcon;
        }

        private void 列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwFiles.View = View.List;
        }

        private void 详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwFiles.View = View.Details;
        }
        #endregion

        #endregion

        #region 帮助

        #region 帮助ToolStripMenuItem1_Click
        private void 帮助ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //打开帮助
            try
            {
                System.Diagnostics.Process.Start(Application.StartupPath 
                    + @"\Lucene File Finder 使用说明.chm");
            }
            catch
            {
                MessageBox.Show("帮助文档打开失败！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #endregion

        #endregion

        #region cmbKeyword_TextChanged
        /// <summary>
        /// cmbKeyword_Text发生改变时执行搜索
        /// </summary>
        private void cmbKeyword_TextChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                //如果正在索引，直接返回。
                if (this.isIndex)
                {
                    return;
                }
                else if (this.isSearchWithResult && this.isSWRFirst)
                {
                    //在结果中搜索时，第一次让cmbKeyword.Text为""，不触发搜索。
                    this.isSWRFirst = false;
                    return;
                }
            }

            //如果延迟搜索定时器已启动，直接返回。
            if (this.tmrDelaySearch.Enabled == true)
            {
                this.tmrDelaySearch.Enabled = false;
                this.tmrDelaySearch.Enabled = true;
                return;
            }

            //如果查找还未结束，结束线程，并启动延迟搜索定时器。
            if (this.threadSearch != null && this.threadSearch.IsAlive)
            {
                this.search.IsStopSearch = true;
                this.tmrDelaySearch.Enabled = true;
                return;
            }

            //开始搜索
            this.StartSearch();
        }
        #endregion

        #region ListView事件

        #region lvwFiles_MouseDoubleClick
        /// <summary>
        /// 鼠标双击ListView
        /// </summary>
        private void lvwFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            if (this.lvwFiles.SelectedItems.Count > 0)
            {
                if (this.lvStatus == ListViewStatus.MP3Artist)
                {
                    this.cmbKeyword.Text = "\"artist: " + this.lvwFiles.SelectedItems[0].Text + "\"";
                    return;
                }
                else
                {
                    //直接打开文件
                    string path = "";
                    switch(this.lvStatus)
                    {
                        case ListViewStatus.File:
                            path = this.lvwFiles.SelectedItems[0].SubItems[1].Text;//文件夹
                            path += this.lvwFiles.SelectedItems[0].Text;//文件名
                            break;
                        case ListViewStatus.MP3:
                            path = this.lvwFiles.SelectedItems[0].SubItems[8].Text;
                            path += this.lvwFiles.SelectedItems[0].Text;
                            break;
                    }

                    //打开并运行文件
                    try
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    catch
                    {
                        MessageBox.Show(" 文件无法打开！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    } 
                }
            }
        }
        #endregion

        #region lvwFiles_SelectedIndexChanged
        /// <summary>
        /// ListView选中项发生改变时发生
        /// </summary>
        private void lvwFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvwFiles.SelectedItems.Count > 0)
            {
                //当有选择项时，点击该项则状态栏显示文件全名。
                string str = "";
                switch (this.lvStatus)
                {
                    case ListViewStatus.File:
                        str = this.lvwFiles.SelectedItems[0].SubItems[1].Text +
                            this.lvwFiles.SelectedItems[0].SubItems[0].Text;
                        break;
                    case ListViewStatus.MP3:
                        str = this.lvwFiles.SelectedItems[0].SubItems[8].Text +
                            this.lvwFiles.SelectedItems[0].SubItems[0].Text;
                        break;
                    case ListViewStatus.MP3Artist:
                        str = this.lvwFiles.SelectedItems[0].Text;
                        break;
                }
                this.orginalStatusText = ((this.CurrentPage - 1) * Static.PageItems
                    + this.lvwFiles.SelectedItems[0].Index + 1) + "： " + str;
                this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
            }
            else
            {
                //若没有选中项，则状态栏显示当前搜索结果。
                this.orginalStatusText = this.searchResult;
                this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
            }

            if (this.magicMirrorStatus == MagicMirrorStatus.Open)
            {
                //在ShowMagicProperty函数中判断listview中的selectedItem是否大于0。
                this.ShowMagicProperty();
            }
        }
        #endregion

        #region lvwFiles_ColumnClick
        private void lvwFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 检查点击的列是不是现在的排序列.
            if (e.Column == this.lvwColumnSorter.SortColumn)
            {
                // 重新设置此列的排序方法.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                //设置排序列，默认为正向排序
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // 用新的排序方法对ListView排序
            this.lvwFiles.Sort();
        }
        #endregion

        #region lvwFiles_MouseClick
        private void lvwFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.lvwFiles.SelectedItems.Count > 0)
            {
                //艺术家模式下不显示右键菜单
                if (this.lvStatus != ListViewStatus.MP3Artist)
                {
                    this.contextMenuStrip.Show(this.lvwFiles, e.X, e.Y);
                }
            }
        }
        #endregion

        #endregion

        #region 拖动文件到窗体

        #region FileFinder_DragEnter
        /// <summary>
        /// 拖放进入
        /// </summary>
        private void FileFinder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion

        #region FileFinder_DragDrop
        /// <summary>
        /// 拖放完成
        /// </summary>
        private void FileFinder_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Link)
            {
                string str = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                KeywordStruct keyword = Deal.GetKeyword(this.cmbKeyword.Text.Trim());

                if (Directory.Exists(str))
                {
                    //文件夹
                    keyword.Directory = str;
                }
                else
                {
                    //文件
                    try
                    {
                        FileInfo fileInfo = new FileInfo(str);
                        keyword.Extension = fileInfo.Extension;
                    }
                    catch
                    { }
                }

                //如果是MP3文件可以也设置音乐家（暂时不做）

                this.SetKeywordText(keyword);
            }
        }
        #endregion

        #endregion

        #region Timer

        #region tmrIndexStatus_Tick
        /// <summary>
        /// 每隔1秒显示正在扫描的文件夹
        /// </summary>
        private void tmrIndexStatus_Tick(object sender, EventArgs e)
        {
            if (this.index.IsIndexComplete == true)
            {
                this.orginalStatusText = "索引完成 用时：" + index.IndexTime + " 秒  " 
                    + this.index.IndexFileFolderNum;
                this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
                this.tmrIndexStatus.Enabled = false;
                this.search.SetSearcher(this.searchMode);
                this.btnUpdate.Visible = false;
                this.tmrReSearch.Enabled = true;
                this.index.Dispose();
            }
            else
            {
                this.currentDirectory = this.index.GetCurrentDirectory();
                if (currentDirectory != "")
                {
                    this.orginalStatusText = "正在扫描 " + this.currentDirectory; 
                    this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
                }
            }
        }
        #endregion

        #region tmrWaitThreadClose_Tick
        /// <summary>
        /// 等待索引线程结束的定时器
        /// </summary>
        private void tmrWaitThreadClose_Tick(object sender, EventArgs e)
        {
            if (this.isIndex == true)
            {
                //线程还未结束，继续等待。
                return;
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region tmrReSearch_Tick
        /// <summary>
        /// 索引完成后启动，4秒后触发重新搜索。
        /// </summary>
        private void tmrReSearch_Tick(object sender, EventArgs e)
        {
            this.tmrReSearch.Enabled = false;
            if (this.modeStatus == ModeStatus.FILE && this.cmbKeyword.Text == "")
            {
                return;
            }
            else
            {
                this.cmbKeyword_TextChanged(this, new EventArgs());
            }
        }
        #endregion

        #region tmrDelaySearch_Tick
        /// <summary>
        /// 如果搜索线程未结束，结束搜索线程，并启动定时器，延迟新的搜索。
        /// </summary>
        private void tmrDelaySearch_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                if (this.threadSearch != null && this.threadSearch.IsAlive)
                    return;
            }

            this.tmrDelaySearch.Enabled = false;
            //开始搜索
            this.StartSearch();
        }
        #endregion

        #endregion

        #region 窗体事件

        #region FileFinderForm_FormClosing
        private void FileFinderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.isSearch || this.isIndex)
            {
                e.Cancel = true;//还有线程在运行，取消关闭，等待线程结束
                this.CloseThread();
            }
            else
            {
                Application.Exit();
            }
        }
        #endregion

        #region FileFinder_SizeChanged
        private void FileFinder_SizeChanged(object sender, EventArgs e)
        {
            this.tsslLength = this.statusStrip.Width - 120;
            this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, 
                this.tsslLength);
        }
        #endregion

        #region FileFinder_Activated
        /// <summary>
        /// 窗体激活时，让cmbKeyword获得焦点
        /// </summary>
        private void FileFinder_Activated(object sender, EventArgs e)
        {
            this.cmbKeyword.Focus();
        }
        #endregion

        #endregion

        #region 私有方法

        #region StartIndex
        /// <summary>
        /// 开始索引，根据传入的索引类型设置索引，并设置相关属性。
        /// </summary>
        /// <param name="type">索引模式</param>
        private void StartIndex(IndexMode mode)
        {
            //在开始索引前判断搜索是否结束，保存结果是否结束，如果未结束，尝试关闭线程。
            if (this.isSearch == true)
            {
                try
                {
                    this.threadSearch.Abort();
                    this.threadSearch.Join(500);
                    this.isSearch = false;
                }
                catch
                { }
            }

            //如果搜索器不为空就先关闭搜索器 
            if (this.search.IsSearcherNull == false)
            {
                this.search.CloseSearcher();
            }

            this.btnUpdate.Visible = true;
            this.isIndex = true;
            this.orginalStatusText = "正在扫描";
            this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
            this.保存结果ToolStripMenuItem.Enabled = false;
            this.索引ToolStripMenuItem.Enabled = false;
            this.停止更新ToolStripMenuItem.Enabled = true;
            this.index = new Index(this, mode);
            //this.index.Reset(mode);
            Thread thread;
            switch (mode)
            {
                case IndexMode.MP3:
                    {
                        index.MP3ScanPath = Static.MP3IndexPath;
                        thread = new Thread(new ThreadStart(index.StartIndexMP3));
                        break;
                    }
                case IndexMode.File:
                    {
                        index.FileScanPath = Static.FileIndexPath;
                        thread = new Thread(new ThreadStart(index.StartIndexFile));
                        break;
                    }
                default:
                    {
                        index.FileScanPath = Static.FileIndexPath;
                        index.MP3ScanPath = Static.MP3IndexPath;
                        thread = new Thread(new ThreadStart(index.StartIndexAll));
                        break;
                    }
            }

            thread.Start();
            this.tmrIndexStatus.Enabled = true;//启动定时器，定时显示正在扫描的文件夹。
        }
        #endregion

        #region InitSettings
        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitSettings()
        {
            //初始化设置信息
            Static.Settings.ReadSettings();
            Static.FileIndexPath = Static.Settings.FileIndexPath;
            Static.MP3IndexPath = Static.Settings.MP3IndexPath;
            IndexInfoDB indexInfo = new IndexInfoDB();
            indexInfo.ReadIndexInfoDB();
            Static.MP3Artist = indexInfo.MP3Artist;
            Static.MP3SongNums = indexInfo.MFileNums;

            this.停止更新ToolStripMenuItem.Enabled = false;
            this.保存结果ToolStripMenuItem.Enabled = false;
            this.tsslLength = this.statusStrip.Width - 120;
            this.orginalStatusText = "永远别说永远，凡事都有可能。";
            this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);

            this.modeStatus = ModeStatus.FILE;
            this.magicMirrorStatus = MagicMirrorStatus.Close;
            this.lvStatus = ListViewStatus.File;

            this.isSearchWithResult = false;
            this.indexMode = IndexMode.All;//默认索引模式为所有
            this.searchMode = SearchMode.File;//默认搜索模式为文件
            //this.index = new Index(this, this.indexMode);
            this.search = new Search(this, this.searchMode);
            this.magicMirror = new MagicMirror();
            this.magicMirror.SetProperties(MagicMode.None, null);
            this.pnlMagicMirror.Controls.Add(this.magicMirror);
            this.magicMirror.Dock = DockStyle.Fill;

            this.btnNext.Visible = false;
            this.btnPrevious.Visible = false;
            this.tsslPages.Visible = false;

            //加入文件夹图标，未知文件的图标，mp3图标，jpg图标。
            this.imgIcon.Images.Add("f", GetSystemIcon.GetFolderIcon(false));
            this.imgIcon.Images.Add("u", GetSystemIcon.GetUnknownFileIcon(false));
            this.imgIcon.Images.Add(".mp3", GetSystemIcon.GetIconByFileType(".mp3", false));
            this.imgIcon.Images.Add(".jpg", GetSystemIcon.GetIconByFileType(".jpg", false));
            this.imgIcon.Images.Add(".txt", GetSystemIcon.GetIconByFileType(".txt", false));
            this.imgIcon.Images.Add("artist", Properties.Resources.artist);
        }
        #endregion

        #region InitMP3Column
        /// <summary>
        /// 初始化MP3的列
        /// </summary>
        private void InitMP3Column()
        {
            this.m_fileName = new ColumnHeader();
            this.m_album = new ColumnHeader();
            this.m_artist = new ColumnHeader();
            this.m_fullName = new ColumnHeader();
            this.m_genre = new ColumnHeader();
            this.m_lastWriteTime = new ColumnHeader();
            this.m_length = new ColumnHeader();
            this.m_pubYear = new ColumnHeader();
            this.m_songName = new ColumnHeader();

            this.m_listArtist = new ColumnHeader();
            this.m_songNums = new ColumnHeader();

            this.m_fileName.Text = "文件名";
            this.m_fileName.Width = 200;

            this.m_songName.Text = "歌曲名";
            this.m_songName.Width = 150;

            this.m_artist.Text = "艺术家";
            this.m_artist.Width = 100;

            this.m_album.Text = "专辑";
            this.m_album.Width = 180;

            this.m_pubYear.Text = "发布年份";
            this.m_pubYear.Width = 65;

            this.m_genre.Text = "流派";
            this.m_genre.Width = 60;

            this.m_fullName.Text = "路径";
            this.m_fullName.Width = 400;

            this.m_length.Text = "大小";
            this.m_length.Width = 80;

            this.m_lastWriteTime.Text = "修改时间";
            this.m_lastWriteTime.Width = 130;

            this.m_listArtist.Text = "艺术家";
            this.m_listArtist.Width = 400;

            this.m_songNums.Text = "曲目数";
            this.m_songNums.Width = 150;
        }
        #endregion

        #region CloseThread
        /// <summary>
        /// 关闭线程
        /// </summary>
        private void CloseThread()
        {
            if (this.isSearch == true)
            {
                try
                {
                    this.threadSearch.Abort();
                    this.threadSearch.Join(500);
                    this.isSearch = false;
                }
                catch
                { }
            }

            if (this.isIndex == true)
            {
                this.index.StopIndexALl();
            }

            //启动线程关闭等待定时器
            this.tmrWaitThreadClose.Enabled = true;
        }
        #endregion

        #region SetFileColumn
        /// <summary>
        /// 设置lvwFiles的列为文件内容。
        /// </summary>
        private void SetFileColumn()
        {
            this.lvwFiles.Columns.Clear();
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.fullName,
            this.length,
            this.lastWriteTime});

            this.lvStatus = ListViewStatus.File;
        }
        #endregion

        #region SetMP3Column
        /// <summary>
        /// 设置lvwFiles的列为MP3内容。
        /// </summary>
        private void SetMP3Column()
        {
            this.lvwFiles.Columns.Clear();
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_fileName,
            this.m_songName,
            this.m_artist,
            this.m_album,
            this.m_pubYear,
            this.m_genre,
            this.m_length,
            this.m_lastWriteTime,
            this.m_fullName});

            this.lvStatus = ListViewStatus.MP3;
        }
        #endregion

        #region SetMP3ArtistColumn
        /// <summary>
        /// 设置MP3艺术家，曲目数列。
        /// </summary>
        private void SetMP3ArtistColumn()
        {
            this.lvwFiles.Columns.Clear();
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_listArtist,
            this.m_songNums});

            this.lvStatus = ListViewStatus.MP3Artist;
        }
        #endregion

        #region SetKeywordText
        /// <summary>
        /// 设置cmbKeyword的Text
        /// </summary>
        private void SetKeywordText(KeywordStruct keyword)
        {
            //写回cmbKeyword
            string str = "";
            if (keyword.Directory != null && keyword.Directory != "")
            {
                str += "\"" + keyword.Directory + "\"" + " ";
            }

            if (this.modeStatus == ModeStatus.FILE)
            {
                if (keyword.Extension != null && keyword.Extension != "")
                {
                    str += "\"" + keyword.Extension + "\"" + " ";
                }
            }

            if (this.modeStatus == ModeStatus.MP3)
            {
                if (keyword.Artist != null && keyword.Artist != "")
                {
                    str += keyword.Artist + " ";
                }
            }

            if (keyword.Name != "")
            {
                str += keyword.Name;
            }
            this.cmbKeyword.Text = str;
        }
        #endregion

        #region StartSearch
        /// <summary>
        /// 开始搜索
        /// </summary>
        public void StartSearch()
        {
            this.lvwFiles.Items.Clear();
            //下一次搜索不采用排序，使用Lucene给的结果的顺序。
            this.lvwColumnSorter.SortColumn = 0;
            this.lvwColumnSorter.Order = SortOrder.None;
            this.isSearch = true;
            this.索引ToolStripMenuItem.Enabled = false;
            this.保存结果ToolStripMenuItem.Enabled = false;

            this.search.Reset();
            this.search.Keyword = Deal.GetKeyword(this.cmbKeyword.Text.Trim());

            switch (this.modeStatus)
            {
                case ModeStatus.FILE:
                    this.threadSearch = new Thread(new ThreadStart(this.search.SearchFile));
                    this.search.SMode = SearchMode.File;
                    break;
                case ModeStatus.MP3:
                    if (this.cmbKeyword.Text == "")
                    {
                        //如果当前listview为MP3状态，修改为MP3艺术家状态。
                        if (this.lvStatus == ListViewStatus.MP3)
                        {
                            this.SetMP3ArtistColumn();
                        }
                        else
                        {
                            //this.SetMP3Column();
                        }
                        this.threadSearch = new Thread(new ThreadStart(this.search.ShowMP3Artist));
                    }
                    else
                    {
                        //如果当前listview列不是MP3状态，修改为MP3状态。
                        if (this.lvStatus != ListViewStatus.MP3)
                        {
                            this.SetMP3Column();
                        }

                        this.threadSearch = new Thread(new ThreadStart(this.search.SearchMP3));
                    }
                    this.search.SMode = SearchMode.MP3;
                    break;
                case ModeStatus.JPG:
                    this.threadSearch = new Thread(new ThreadStart(this.search.SearchJPG));
                    this.search.SMode = SearchMode.File;
                    break;
                case ModeStatus.TXT:
                    this.threadSearch = new Thread(new ThreadStart(this.search.SearchTXT));
                    this.search.SMode = SearchMode.File;
                    break;
            }

            this.threadSearch.Start();
        }
        #endregion

        #region ShowMagicProperty
        /// <summary>
        /// 在魔镜中显示属性，在调用前已确保魔镜处于打开状态。
        /// </summary>
        private void ShowMagicProperty()
        {
            if (this.lvwFiles.SelectedItems.Count <= 0)
            {
                return;
            }

            if (this.lvStatus == ListViewStatus.MP3Artist)
            {
                //显示无扩展信息的控件。
                this.magicMirror.SetProperties(MagicMode.None, null);
                return;
            }

            string fullName = "";
            string extension = Deal.GetExtension(this.lvwFiles.SelectedItems[0].Text);
            if (this.lvwFiles.SelectedItems[0].ImageIndex != 0)
            {
                //非文件夹
                if (this.lvStatus == ListViewStatus.File)
                {
                    fullName = this.lvwFiles.SelectedItems[0].SubItems[1].Text +
                        this.lvwFiles.SelectedItems[0].Text;

                    switch (extension)
                    {
                        case "jpg":
                            this.magicMirror.SetProperties(MagicMode.JPG, GetProperty.GetJPGExif(fullName));                           
                            break;
                        case "mp3":
                            this.magicMirror.SetProperties(MagicMode.MP3,GetProperty.GetMP3Tag(fullName));
                            break;
                        default:
                            this.magicMirror.SetProperties(MagicMode.File, GetProperty.GetFileProperty(fullName));
                            break;
                    }
                }
                else if (this.lvStatus == ListViewStatus.MP3)
                {
                    fullName = this.lvwFiles.SelectedItems[0].SubItems[8].Text +
                        this.lvwFiles.SelectedItems[0].Text;
                    this.magicMirror.SetProperties(MagicMode.MP3, GetProperty.GetMP3Tag(fullName));
                }
            }
            else
            {
                //文件夹只在文件模式中出现，所以这里不用再判断。
                fullName = this.lvwFiles.SelectedItems[0].SubItems[1].Text +
                    this.lvwFiles.SelectedItems[0].Text;
                //文件夹
                this.magicMirror.SetProperties(MagicMode.File, GetProperty.GetDirectoryProperty(fullName));
            }
        }
        #endregion

        #region MemoryOptimization
        /// <summary>
        /// 整理内存
        /// </summary>
        private void MemoryOptimization()
        {
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.MaxWorkingSet = currentProcess.MaxWorkingSet;
        }
        #endregion

        #endregion

        #region 属性

        #region IsIndex
        /// <summary>
        /// 是否正在索引
        /// </summary>
        public bool IsIndex
        {
            set { this.isIndex = value; }
            get { return this.isIndex; }
        }
        #endregion

        #region IsSearch
        /// <summary>
        /// 是否正在搜索
        /// </summary>
        public bool IsSearch
        {
            set { this.isSearch = value; }
            get { return this.isSearch; }
        }
        #endregion

        #region SearchResult
        /// <summary>
        /// 搜索结果，在搜索线程的callback中更新
        /// </summary>
        public string SearchResult
        {
            set { this.searchResult = value; }
            get { return this.searchResult; }
        }
        #endregion

        #region IsSearchWithResult
        /// <summary>
        /// 是否在结果中搜索
        /// </summary>
        public bool IsSearchWithResult
        {
            get { return this.isSearchWithResult; }
        }
        #endregion

        #region CurrentKeyword
        /// <summary>
        /// 当前的搜索关键字
        /// </summary>
        public string CurrentKeyword
        {
            get 
            {
                if (this.isSearchWithResult)
                {
                    return this.lastKeyword + " + (" +
                        this.cmbKeyword.Text + ")";
                }
                else
                {
                    return this.cmbKeyword.Text;
                }
            }
        }
        #endregion

        #region OrginalStatusText
        /// <summary>
        /// 原始的状态栏文本
        /// </summary>
        public string OrginalStatusText
        {
            set { this.orginalStatusText = value; }
            get { return this.orginalStatusText; }
        }
        #endregion

        #region TsslLength
        /// <summary>
        /// 状态栏文字的长度
        /// </summary>
        public int TsslLength
        {
            set { this.tsslLength = value; }
            get { return this.tsslLength; }
        }
        #endregion

        #region SMode
        /// <summary>
        /// 搜索模式
        /// </summary>
        public SearchMode SMode
        {
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

        #region LVStatus
        /// <summary>
        /// listview的列状态
        /// </summary>
        public ListViewStatus LVStatus
        {
            get { return this.lvStatus; }
        }
        #endregion

        #region CurrentPage

        #endregion

        #endregion

        #region btnNext_Click
        /// <summary>
        /// 下一页
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.isIndex == true || this.isSearch == true)
                return;

            if (this.search.CurrentPage < this.search.PageNums)
            {
                this.btnPrevious.Enabled = true;
                this.currentPage++;
                this.search.CurrentPage++;
                this.tsslPages.Text = "  " + this.search.CurrentPage + "/" + this.search.PageNums + "  ";
                if (this.search.CurrentPage == this.search.PageNums)
                {
                    this.btnNext.Enabled = false;
                }
                //状态栏显示当前搜索结果。
                this.orginalStatusText = this.searchResult;
                this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);
                this.lvwFiles.Items.Clear();
                this.search.GetCurrentPage();
                //this.threadSearch = new Thread(new ThreadStart(this.search.GetCurrentPage));
                //this.threadSearch.Start();
            }
        }
        #endregion

        #region btnPrevious_Click
        /// <summary>
        /// 上一页
        /// </summary>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (this.isIndex == true || this.isSearch == true)
                return;

            if (this.search.CurrentPage > 1)
            {
                this.btnNext.Enabled = true;
                this.currentPage--;
                this.search.CurrentPage--;
                this.tsslPages.Text = "  " + this.search.CurrentPage + "/" + this.search.PageNums + "  ";
                if (this.search.CurrentPage == 1)
                {
                    this.btnPrevious.Enabled = false;
                }
                //状态栏显示当前搜索结果。
                this.orginalStatusText = this.searchResult;
                this.tsslStatus.Text = Deal.LimitStringLength(this.orginalStatusText, this.tsslLength);

                this.lvwFiles.Items.Clear();
                this.search.GetCurrentPage();
                //this.threadSearch = new Thread(new ThreadStart(this.search.GetCurrentPage));
                //this.threadSearch.Start();
            }
        }
        #endregion

        #region 右键菜单

        #region 打开ToolStripMenuItem_Click
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //直接打开文件
            string path = "";
            switch (this.lvStatus)
            {
                case ListViewStatus.File:
                    path = this.lvwFiles.SelectedItems[0].SubItems[1].Text;//文件夹
                    path += this.lvwFiles.SelectedItems[0].Text;//文件名
                    break;
                case ListViewStatus.MP3:
                    path = this.lvwFiles.SelectedItems[0].SubItems[8].Text;
                    path += this.lvwFiles.SelectedItems[0].Text;
                    break;
            }

            //打开并运行文件
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                MessageBox.Show(" 文件无法打开！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region 打开路径ToolStripMenuItem_Click
        private void 打开路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "";
            //按住Ctrl时打开文件所在的文件夹，并选中文件。
            if (this.lvStatus == ListViewStatus.File)
            {
                path = this.lvwFiles.SelectedItems[0].SubItems[1].Text;
                path += this.lvwFiles.SelectedItems[0].Text;
            }
            else if (this.lvStatus == ListViewStatus.MP3)
            {
                path = this.lvwFiles.SelectedItems[0].SubItems[8].Text;
                path += this.lvwFiles.SelectedItems[0].Text;
            }

            try
            {
                DirectoryOpener.OpenDirectory(path);
            }
            catch
            {
                MessageBox.Show(" 文件无法打开！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region 作为搜索路径ToolStripMenuItem_Click
        private void 作为搜索路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeywordStruct keyword = Deal.GetKeyword(this.cmbKeyword.Text);
            string str = "";
            if (this.lvStatus == ListViewStatus.File)
            {
                if (this.lvwFiles.SelectedItems[0].ImageIndex == 0)
                {
                    //文件夹的imageindex为0。
                    str = this.lvwFiles.SelectedItems[0].SubItems[1].Text
                        + this.lvwFiles.SelectedItems[0].Text;
                    keyword.Directory = str;
                    //写回cmbKeyword
                    this.SetKeywordText(keyword);
                }
                else
                {
                    //如果是文件，则将文件所在的目录作为搜索目录
                    str = this.lvwFiles.SelectedItems[0].SubItems[1].Text;
                    keyword.Directory = str;
                    //写回cmbKeyword
                    this.SetKeywordText(keyword);
                }
            }
            else if (this.lvStatus == ListViewStatus.MP3)
            {
                if (this.lvwFiles.SelectedItems[0].ImageIndex != 0)
                {
                    //如果是文件，则将文件所在的目录作为搜索目录
                    str = this.lvwFiles.SelectedItems[0].SubItems[8].Text;
                    keyword.Directory = str;
                    //写回cmbKeyword
                    this.SetKeywordText(keyword);
                }
            }
        }
        #endregion

        #region 作为搜索后缀ToolStripMenuItem_Click
        private void 作为搜索后缀ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.modeStatus == ModeStatus.FILE)
            {
                KeywordStruct keyword = Deal.GetKeyword(this.cmbKeyword.Text);
                string str = "";
                int i;
                if (this.lvwFiles.SelectedItems[0].ImageIndex != 0)
                {
                    str = this.lvwFiles.SelectedItems[0].Text;
                    i = str.LastIndexOf('.');
                    keyword.Extension = str.Substring(i, str.Length - i);
                    //写回cmbKeyword
                    this.SetKeywordText(keyword);
                }
            }
        }
        #endregion

        #region 复制
        private void 文件名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.lvwFiles.SelectedItems[0].SubItems[0].Text);
        }

        private void 完整路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fullName = "";
            if (this.lvStatus == ListViewStatus.File)
            {
                fullName = this.lvwFiles.SelectedItems[0].SubItems[1].Text;
            }
            else
            {
                fullName = this.lvwFiles.SelectedItems[0].SubItems[8].Text;
            }

            Clipboard.SetDataObject(fullName);
        }

        private void 文件大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string length = "";
            if (this.lvStatus == ListViewStatus.File)
            {
                length = this.lvwFiles.SelectedItems[0].SubItems[2].Text;
            }
            else
            {
                length = this.lvwFiles.SelectedItems[0].SubItems[6].Text;
            }

            Clipboard.SetDataObject(length);
        }

        private void 修改日期ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string lastWriteTime = "";
            if (this.lvStatus == ListViewStatus.File)
            {
                lastWriteTime = this.lvwFiles.SelectedItems[0].SubItems[3].Text;
            }
            else
            {
                lastWriteTime = this.lvwFiles.SelectedItems[0].SubItems[7].Text;
            }

            Clipboard.SetDataObject(lastWriteTime);
        }
        #endregion

        #endregion
    }
}

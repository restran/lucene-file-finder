using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LuceneFileFinder.Type;
using LuceneFileFinder.Util;

namespace LuceneFileFinder.Forms
{
    /// <summary>
    /// 索引管理窗口
    /// </summary>
    public partial class IndexManager : Form
    {
        #region 成员变量
        private bool isChanged = false;//设置是否发生改变
        private List<string> fileInPath = new List<string>();
        private List<string> fileOutPath = new List<string>();
        private List<string> mp3InPath = new List<string>();
        private List<string> mp3OutPath = new List<string>();
        private ListViewColumnSorter lvwColumnSorter;
        #endregion

        #region 构造函数
        public IndexManager()
        {
            InitializeComponent();
            //创建一个ListView排序类的对象，并设置listView1的排序器
            this.lvwColumnSorter = new ListViewColumnSorter();
            this.lvwFIn.ListViewItemSorter = this.lvwColumnSorter;
            this.lvwFOut.ListViewItemSorter = this.lvwColumnSorter;
            this.lvwMIn.ListViewItemSorter = this.lvwColumnSorter;
            this.lvwMOut.ListViewItemSorter = this.lvwColumnSorter;

            this.lvwColumnSorter.Order = SortOrder.Ascending;
            this.lvwColumnSorter.SortColumn = 0;

            this.lvwFIn.Sort();
            this.lvwFOut.Sort();
            this.lvwMIn.Sort();
            this.lvwMOut.Sort();

            this.folderBrowserDialog.SelectedPath = "";
            //GetSystemIcon getIcon = new GetSystemIcon();//获取图标
            //加入文件夹图标
            this.imgIcon.Images.Add("folder", GetSystemIcon.GetFolderIcon(false));
            this.lblWarning.Text = "";
            this.InitData();//初始化索引数据
        }
        #endregion

        #region InitData
        /// <summary>
        /// 从索引文件及设置文件中读取数据，初始化数据。
        /// </summary>
        private void InitData()
        {
            #region 包含路径 排除路径
            this.fileInPath = Deal.ListStringClone(Static.Settings.FileIndexInPath);//文件索引包含路径
            this.fileOutPath = Deal.ListStringClone(Static.Settings.FileIndexOutPath);//文件索引排除路径
            this.mp3InPath = Deal.ListStringClone(Static.Settings.MP3IndexInPath);//MP3索引包含路径
            this.mp3OutPath = Deal.ListStringClone(Static.Settings.MP3IndexOutPath);//MP3索引排除路径

            this.UpdateListView(this.lvwFIn, this.fileInPath);
            this.UpdateListView(this.lvwFOut, this.fileOutPath);
            this.UpdateListView(this.lvwMIn, this.mp3InPath);
            this.UpdateListView(this.lvwMOut, this.mp3OutPath);
            #endregion

            #region 索引信息

            IndexInfoDB indexInfo = new IndexInfoDB();
            indexInfo.ReadIndexInfoDB();//读取索引信息
            this.lblFCreation.Text = indexInfo.FCreationTime;
            this.lblFUpdate.Text = indexInfo.FUpdateTime;
            this.lblFFiles.Text = Deal.ToEnglishNumString(indexInfo.FFileNums);
            this.lblFPaths.Text = Deal.ToEnglishNumString(indexInfo.FFolderNums);

            this.lblMCreation.Text = indexInfo.MCreationTime;
            this.lblMUpdate.Text = indexInfo.MUpdateTime;
            this.lblMFiles.Text = Deal.ToEnglishNumString(indexInfo.MFileNums);
            this.lblMPaths.Text = Deal.ToEnglishNumString(indexInfo.MFolderNums);

            #endregion
        }
        #endregion

        #region 文件按钮

        #region 包含路径按钮
        private void btnFInAdd_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.Description = "添加文件索引包含路径";
            this.folderBrowserDialog.ShowDialog();
            if (this.folderBrowserDialog.SelectedPath != "")
            {
                string path = this.folderBrowserDialog.SelectedPath;
                if (path.EndsWith("\\") == false)
                {
                    path += "\\";
                }

                if (this.CheckIn(this.fileInPath, this.fileOutPath, path) == true)
                {
                    //通过检测，更新ListView。
                    this.UpdateListView(this.lvwFIn, this.fileInPath);
                }
                this.folderBrowserDialog.SelectedPath = "";
            }
        }

        private void btnFInDelete_Click(object sender, EventArgs e)
        {
            if (this.lvwFIn.SelectedItems.Count > 0)
            {
                this.isChanged = true;
                this.fileInPath.Remove(this.lvwFIn.SelectedItems[0].Text);
                //删掉该目录在排除目录中的子目录
                this.RemoveChildOutFolder(this.fileOutPath, this.lvwFIn.SelectedItems[0].Text);
                //更新排除目录
                this.UpdateListView(this.lvwFOut, this.fileOutPath);
                this.lvwFIn.Items.Remove(this.lvwFIn.SelectedItems[0]);
                this.lblWarning.Text = "";
            }
        }
        #endregion

        #region 排除路径按钮
        private void btnFOutAdd_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.Description = "添加文件索引排除路径";
            this.folderBrowserDialog.ShowDialog();
            if (this.folderBrowserDialog.SelectedPath != "")
            {
                string path = this.folderBrowserDialog.SelectedPath;
                if (path.EndsWith("\\") == false)
                {
                    path += "\\";
                }

                if (this.CheckOut(this.fileInPath, this.fileOutPath, path) == true)
                {
                    //通过检测，更新ListView。
                    this.UpdateListView(this.lvwFOut, this.fileOutPath);
                }
                this.folderBrowserDialog.SelectedPath = "";
            }
        }

        private void btnFOutDelete_Click(object sender, EventArgs e)
        {
            if (this.lvwFOut.SelectedItems.Count > 0)
            {
                this.isChanged = true;
                this.fileOutPath.Remove(this.lvwFOut.SelectedItems[0].Text);
                this.lvwFOut.Items.Remove(this.lvwFOut.SelectedItems[0]);
                this.lblWarning.Text = "";
            }
        }
        #endregion

        #endregion

        #region MP3按钮

        #region 包含路径按钮
        private void btnMIAdd_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.Description = "添加MP3索引包含路径";
            this.folderBrowserDialog.ShowDialog();
            if (this.folderBrowserDialog.SelectedPath != "")
            {
                string path = this.folderBrowserDialog.SelectedPath;
                if (path.EndsWith("\\") == false)
                {
                    path += "\\";
                }

                if (this.CheckIn(this.mp3InPath, this.mp3OutPath, path) == true)
                {
                    //通过检测，更新ListView。
                    this.UpdateListView(this.lvwMIn, this.mp3InPath);
                }
                this.folderBrowserDialog.SelectedPath = "";
            }
        }

        private void btnMIDelete_Click(object sender, EventArgs e)
        {
            if (this.lvwMIn.SelectedItems.Count > 0)
            {
                this.isChanged = true;
                this.mp3InPath.Remove(this.lvwMIn.SelectedItems[0].Text);
                //删除该目录在排除目录中的子目录。
                this.RemoveChildOutFolder(this.mp3OutPath, this.lvwMIn.SelectedItems[0].Text);
                //更新排除目录。
                this.UpdateListView(this.lvwMOut, this.mp3OutPath);
                this.lvwMIn.Items.Remove(this.lvwMIn.SelectedItems[0]);
                this.lblWarning.Text = "";
            }
        }
        #endregion

        #region 排除路径按钮
        private void btnMOAdd_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.Description = "添加MP3索引排除路径";
            this.folderBrowserDialog.ShowDialog();
            if (this.folderBrowserDialog.SelectedPath != "")
            {
                string path = this.folderBrowserDialog.SelectedPath;
                if (path.EndsWith("\\") == false)
                {
                    path += "\\";
                }

                if (this.CheckOut(this.mp3InPath, this.mp3OutPath, path) == true)
                {
                    //通过检测，更新ListView。
                    this.UpdateListView(this.lvwMOut, this.mp3OutPath);
                }
                this.folderBrowserDialog.SelectedPath = "";
            }
        }

        private void btnMODelete_Click(object sender, EventArgs e)
        {
            if (this.lvwMOut.SelectedItems.Count > 0)
            {
                this.isChanged = true;
                this.mp3OutPath.Remove(this.lvwMOut.SelectedItems[0].Text);
                this.lvwMOut.Items.Remove(this.lvwMOut.SelectedItems[0]);
                this.lblWarning.Text = "";
            }
        }
        #endregion

        #endregion

        #region IndexManager_FormClosed
        /// <summary>
        /// 关闭了IndexManager窗口 判断设置是否发生改变 提示用户是否保存设置
        /// </summary>
        private void IndexManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.isChanged == true)
            {
                DialogResult dr = MessageBox.Show("设置发生更改是否保存", "关闭索引管理", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == dr)
                {
                    this.SaveSettings();
                }
                else if (DialogResult.No == dr)
                {
                    
                }
            }

            //恢复主窗的allowdrop为true
            this.RecoverMainForm();
        }
        #endregion

        #region 私有方法

        #region 路径检测

        #region CheckIn
        /// <summary>
        /// 检测是否是inPath的子目录，是否是inPath的父目录，是否是outPath的子目录。
        /// </summary>
        private bool CheckIn(List<string> inPath, List<string> outPath, string path)
        {
            foreach (string item in inPath)
            {
                if (path.StartsWith(item))
                {
                    this.lblWarning.Text = "当前目录已在包含目录中";
                    return false;
                }
            }

            foreach (string item in outPath)
            {
                //是否是outPath的子目录
                if (path.StartsWith(item))
                {
                    this.lblWarning.Text = "当前目录为排除目录的子目录";
                    return false;
                }
            }

            List<string> temp = Deal.ListStringClone(inPath);
            foreach (string item in temp)
            {
                //为inPath中某项的父目录，删除该项。
                if (item.StartsWith(path))
                {
                    inPath.Remove(item);
                }
            }

            this.isChanged = true;
            this.lblWarning.Text = "";
            inPath.Add(path);
            return true;
        }
        #endregion

        #region CheckOut
        /// <summary>
        /// 检测是否是outPath的子目录，是否是outPath的父目录，
        /// 是否是inPath中的父目录，必须是inPath中的子目录。
        /// </summary>
        /// <returns></returns>
        private bool CheckOut(List<string> inPath, List<string> outPath, string path)
        {
            foreach (string item in outPath)
            {
                if (path.StartsWith(item))
                {
                    this.lblWarning.Text = "当前目录已在排除目录中";
                    return false;
                }
            }

            bool isFInChild = false;
            foreach (string item in inPath)
            {
                if (path.StartsWith(item))
                {
                    isFInChild = true;
                    break;
                }
            }

            if (isFInChild == false)
            {
                this.lblWarning.Text = "当前目录不是包含目录的子目录";
                return false;
            }

            foreach (string item in inPath)
            {
                //是否是inPath的父目录
                if (item.StartsWith(path))
                {
                    this.lblWarning.Text = "当前目录为包含目录的父目录";
                    return false;
                }
            }

            List<string> temp = Deal.ListStringClone(outPath);
            foreach (string item in temp)
            {
                //为outPath中某项的父目录，删除该项。
                if (item.StartsWith(path))
                {
                    outPath.Remove(item);
                }
            }

            this.isChanged = true;
            this.lblWarning.Text = "";
            outPath.Add(path);
            return true;
        }
        #endregion

        #endregion

        #region RemoveChildFolder
        /// <summary>
        /// 移除排除目录中是该目录的子目录。
        /// </summary>
        /// <param name="outPath">排除目录</param>
        /// <param name="folder">包含目录中的一个目录</param>
        private void RemoveChildOutFolder(List<string> outPath, string folder)
        {
            List<string> temp = Deal.ListStringClone(outPath);
            foreach (string item in temp)
            {
                if (item.StartsWith(folder))
                {
                    outPath.Remove(item);
                }
            }
        }
        #endregion

        #region SaveSettings
        /// <summary>
        /// 保存设置文件
        /// </summary>
        private void SaveSettings()
        {
            Static.Settings.FileIndexInPath = Deal.ListStringClone(this.fileInPath);
            Static.Settings.FileIndexOutPath = Deal.ListStringClone(this.fileOutPath);
            Static.Settings.MP3IndexInPath = Deal.ListStringClone(this.mp3InPath);
            Static.Settings.MP3IndexOutPath = Deal.ListStringClone(this.mp3OutPath);
            Static.Settings.FileIndexPath = Deal.CombineInOutPath(this.fileInPath, this.fileOutPath);
            Static.Settings.MP3IndexPath = Deal.CombineInOutPath(this.mp3InPath, this.mp3OutPath);

            Static.Settings.WriteSettings();

            Static.FileIndexPath = Deal.ListStringClone(Static.Settings.FileIndexPath);
            Static.MP3IndexPath = Deal.ListStringClone(Static.Settings.MP3IndexPath);
        }
        #endregion

        #region UpdateListView
        /// <summary>
        /// 更新ListView
        /// </summary>
        private void UpdateListView(ListView lv, List<string> folders)
        {
            lv.Items.Clear();
            foreach (string str in folders)
            {
                lv.Items.Add(str, 0);//0为文件夹图标
            }
        }
        #endregion

        #endregion

        #region btnOK_Click
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.isChanged == true)
            {
                this.SaveSettings();
            }

            //恢复主窗的allowdrop为true
            this.RecoverMainForm();
            this.Dispose();
        }
        #endregion

        #region btnCancel_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //恢复主窗的allowdrop为true
            this.RecoverMainForm();
            this.Dispose();
        }
        #endregion

        #region 拖动文件夹到ListView中
        private void lvwFIn_DragDrop(object sender, DragEventArgs e)
        {
            for (int i = 0; i < ((System.Array)e.Data.GetData(DataFormats.FileDrop)).Length; i++)
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString();
                if (System.IO.Directory.Exists(path))
                {
                    if (sender.Equals(this.lvwFIn))
                    {
                        if (this.CheckIn(this.fileInPath, this.fileOutPath, path) == true)
                        {
                            //通过检测，更新ListView。
                            this.UpdateListView(this.lvwFIn, this.fileInPath);
                        }
                    }
                    else if (sender.Equals(this.lvwFOut))
                    {
                        if (this.CheckOut(this.fileInPath, this.fileOutPath, path) == true)
                        {
                            //通过检测，更新ListView。
                            this.UpdateListView(this.lvwFOut, this.fileOutPath);
                        }
                    }
                    else if (sender.Equals(this.lvwMIn))
                    {
                        if (this.CheckIn(this.mp3InPath, this.mp3OutPath, path) == true)
                        {
                            //通过检测，更新ListView。
                            this.UpdateListView(this.lvwMIn, this.mp3InPath);
                        }
                    }
                    else if (sender.Equals(this.lvwMOut))
                    {
                        if (this.CheckOut(this.mp3InPath, this.mp3OutPath, path) == true)
                        {
                            //通过检测，更新ListView。
                            this.UpdateListView(this.lvwMOut, this.mp3OutPath);
                        }
                    }
                }
            }
        }

        private void lvwFIn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion

        #region tabManager_Selected
        /// <summary>
        /// 当选项卡切换时，另lblWarning为""。
        /// </summary>
        private void tabManager_Selected(object sender, TabControlEventArgs e)
        {
            this.lblWarning.Text = "";
        }
        #endregion

        #region RecoverMainForm
        /// <summary>
        /// 关闭窗体时，设置主窗的AllowDrop为true
        /// </summary>
        private void RecoverMainForm()
        {
            FileFinder fileFinder = (FileFinder)this.Owner;
            fileFinder.AllowDrop = true;
        }
        #endregion
    }
}

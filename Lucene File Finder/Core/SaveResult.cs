using System;
using System.Collections.Generic;
using System.IO;
using LuceneFileFinder.Forms;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Core
{
    /// <summary>
    /// 保存搜索结果
    /// 创建时间：2010/5/15 更新时间：2010/5/15
    /// </summary>
    public class SaveResult
    {
        #region 内部变量
        private string fullName;//保存的文件全名
        private FileFinder form;
        private ListViewStatus lvStatus;
        private StreamWriter sw;
        private string strLine;//一行的数据
        private PageRange pageRange;//页码的范围
        private SaveForm saveForm;
        private Search search;
        private List<string> result;
        private int setpValue;//进度条每次增加的量
        private int lastValue;//进度条最后一次增加的量
        /// <summary>
        /// 定义一个代理，用于跨线程访问
        /// </summary>
        private delegate void CrossThreadOperateControl();
        #endregion 

        #region 构造函数
        public SaveResult(FileFinder _form, SaveForm _saveForm, Search _serach, string _fullName, 
            PageRange _pageRange)
        {
            this.saveForm = _saveForm;
            this.form = _form;
            this.lvStatus = this.form.LVStatus;
            this.fullName = _fullName;
            this.search = _serach;
            this.pageRange = _pageRange;
            this.setpValue = 100 / (this.pageRange.End - this.pageRange.Begin + 2);
            this.lastValue = 100 - this.setpValue * (this.pageRange.End - this.pageRange.Begin + 1);
        }
        #endregion

        #region StartSave
        public void StartSave()
        {
            DateTime dtStart = DateTime.Now;//开始时间
            this.sw = new StreamWriter(this.fullName, false);
            //输出保存时间
            this.strLine = "保存时间：" + DateTime.Now.ToString();
            this.sw.WriteLine(strLine);
            this.sw.WriteLine();//换行

            CrossThreadOperateControl CrossOperate = delegate()
            {
                if (this.lvStatus != ListViewStatus.MP3Artist)
                {
                    //输出搜索关键字
                    this.strLine = "关键字：" + this.form.CurrentKeyword;
                    this.sw.WriteLine(strLine);
                    this.sw.WriteLine();//换行

                    //输出搜索结果信息
                    this.strLine = "搜索结果：" + this.form.SearchResult;
                    this.sw.WriteLine(strLine);
                    this.sw.WriteLine();//换行
                }
                else
                {
                    this.strLine = "MP3索引数据库艺术家信息：";
                    this.sw.WriteLine(strLine);
                    this.sw.WriteLine();//换行

                    //输出搜索结果信息
                    this.strLine = this.form.SearchResult;
                    this.sw.WriteLine(strLine);
                    this.sw.WriteLine();//换行
                }

                //输出搜索结果
                switch (this.lvStatus)
                {
                    case ListViewStatus.File:
                        this.OutPutFile();
                        break;
                    case ListViewStatus.MP3:
                        this.OutPutMP3();
                        break;
                    case ListViewStatus.MP3Artist:
                        this.OutPutMP3Artist();
                        break;
                }

                this.sw.Close();
            };
            this.form.lvwFiles.Invoke(CrossOperate);

            DateTime dtStop = DateTime.Now;//完成时间
            string time = (dtStop - dtStart).TotalSeconds.ToString();
            CrossThreadOperateControl CrossOperate2 = delegate()
            {
                this.saveForm.CallBack(time);
            };
            this.saveForm.Invoke(CrossOperate2);
        }
        #endregion

        #region OutPutFile
        private void OutPutFile()
        {
            this.result = this.search.GetRangePage(this.pageRange.Begin, this.pageRange.End);
            foreach (string item in this.result)
            {
                this.strLine = item;
                this.sw.WriteLine(strLine);
            }
        }
        #endregion

        #region OutPutMP3
        private void OutPutMP3()
        {
            this.result = this.search.GetRangePage(this.pageRange.Begin, this.pageRange.End);
            foreach (string item in this.result)
            {
                this.strLine = item;
                this.sw.WriteLine(strLine);
            }
        }
        #endregion

        #region OutPutMP3Artist
        private void OutPutMP3Artist()
        {
            string blank = "          ";
            for (int i = 0; i < this.form.lvwFiles.Items.Count; i++)
            {
                this.strLine = (i + 1).ToString() + "：" + this.form.lvwFiles.Items[i].Text +
                    blank + this.form.lvwFiles.Items[i].SubItems[1].Text;
                this.sw.WriteLine(strLine);
            }
        }
        #endregion
    }
}

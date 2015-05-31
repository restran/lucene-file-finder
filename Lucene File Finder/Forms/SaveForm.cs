using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using LuceneFileFinder.Core;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Forms
{
    /// <summary>
    /// 保存搜索结果
    /// 创建时间：2010/5/15 更新时间：2010/5/15
    /// </summary>
    public partial class SaveForm : Form
    {
        #region 内部变量
        private SaveResult saveResult;
        private Search search;
        private FileFinder form;
        private PageRange pageRange = new PageRange();
        private Thread thread;
        #endregion

        #region 构造函数
        public SaveForm(FileFinder _form, Search _search)
        {
            InitializeComponent();
            this.form = _form;
            this.search = _search;
            this.lblWarning.Visible = false;
            this.rdoFirst.Checked = true;

            this.lbl1.Enabled = false;
            this.lbl2.Enabled = false;
            this.lbl3.Enabled = false;
            this.cmbBegin.Enabled = false;
            this.cmbEnd.Enabled = false;

            for (int i = 1; i <= this.form.PageNums; i++)
            {
                this.cmbBegin.Items.Add(i);
                this.cmbEnd.Items.Add(i);
            }
            this.cmbBegin.SelectedIndex = 0;
            this.cmbEnd.SelectedIndex = 0;
        }
        #endregion

        #region btnStart_Click
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.rdoRange.Checked == true)
            {
                if (this.cmbBegin.SelectedIndex > this.cmbEnd.SelectedIndex)
                {
                    return;
                }
            }

            string fileName = DateTime.Now.ToString();
            fileName = fileName.Replace("/", "-");
            fileName = fileName.Replace(":", ".");
            this.saveFileDialog.FileName = fileName;
            this.saveFileDialog.ShowDialog();
            //如果按下保存按钮触发saveFileDialog_FileOk事件，执行保存结果。
        }
        #endregion

        #region btnCancel_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.thread != null && this.thread.IsAlive)
            {
                try
                {
                    this.thread.Abort(500);
                    this.thread.Join(500);
                    this.Dispose();
                }
                catch
                { }
            }
            else
            {
                this.Dispose();
            }
        }
        #endregion

        #region saveFileDialog_FileOk
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.btnStart.Enabled = false;
            string fileName = this.saveFileDialog.FileName;
            this.saveResult = new SaveResult(this.form, this, this.search,
                fileName, this.pageRange);
            this.thread = new Thread(new ThreadStart(this.saveResult.StartSave));
            this.thread.Start();
        }
        #endregion

        #region cmbBegin_SelectedIndexChanged
        private void cmbBegin_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pageRange.Begin = this.cmbBegin.SelectedIndex + 1;
            if (this.pageRange.Begin > this.pageRange.End)
            {
                this.lblWarning.Visible = true;
            }
            else
            {
                this.lblWarning.Visible = false;
            }
        }
        #endregion

        #region cmbEnd_SelectedIndexChanged
        private void cmbEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pageRange.End = this.cmbEnd.SelectedIndex + 1;
            if (this.pageRange.Begin > this.pageRange.End)
            {
                this.lblWarning.Visible = true;
            }
            else
            {
                this.lblWarning.Visible = false;
            }
        }
        #endregion

        #region RadioButton_CheckedChanged
        private void rdoFirst_CheckedChanged(object sender, EventArgs e)
        {
            this.pageRange.Begin = 1;
            this.pageRange.End = 1;
        }

        private void rdoCurrent_CheckedChanged(object sender, EventArgs e)
        {
            this.pageRange.Begin = this.form.CurrentPage;
            this.pageRange.End = this.form.CurrentPage;
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            this.pageRange.Begin = 1;
            this.pageRange.End = this.form.PageNums;
        }

        #region rdoRange_CheckedChanged
        private void rdoRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoRange.Checked == true)
            {
                this.lbl1.Enabled = true;
                this.lbl2.Enabled = true;
                this.lbl3.Enabled = true;
                this.cmbBegin.Enabled = true;
                this.cmbEnd.Enabled = true;
            }
            else
            {
                this.lbl1.Enabled = false;
                this.lbl2.Enabled = false;
                this.lbl3.Enabled = false;
                this.cmbBegin.Enabled = false;
                this.cmbEnd.Enabled = false;
            }
        }
        #endregion

        #endregion

        #region CallBack
        /// <summary>
        /// 保存结果线程执行完，回调弹出提示。
        /// </summary>
        public void CallBack(string time)
        {
            MessageBox.Show("保存完成 用时：" + time, 
                "保存完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Dispose();//关闭窗口。
        }
        #endregion

        #region SaveForm_FormClosing
        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.thread != null && this.thread.IsAlive)
            {
                try
                {
                    this.thread.Abort(500);
                    this.thread.Join(500);
                }
                catch
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion
    }
}

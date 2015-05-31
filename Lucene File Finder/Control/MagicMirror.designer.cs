namespace LuceneFileFinder.Control
{
    partial class MagicMirror
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlPic = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dblProList = new LuceneFileFinder.Control.DoubleBufferListView();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.pnlPic.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(6, 356);
            this.pnlLeft.TabIndex = 22;
            // 
            // picPreview
            // 
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 4);
            this.picPreview.Margin = new System.Windows.Forms.Padding(0);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(214, 103);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 30;
            this.picPreview.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 4);
            this.panel1.TabIndex = 38;
            // 
            // pnlPic
            // 
            this.pnlPic.Controls.Add(this.panel4);
            this.pnlPic.Controls.Add(this.panel3);
            this.pnlPic.Controls.Add(this.picPreview);
            this.pnlPic.Controls.Add(this.panel1);
            this.pnlPic.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPic.Location = new System.Drawing.Point(6, 249);
            this.pnlPic.Name = "pnlPic";
            this.pnlPic.Size = new System.Drawing.Size(214, 107);
            this.pnlPic.TabIndex = 39;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 103);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(209, 4);
            this.panel4.TabIndex = 40;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(209, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(5, 103);
            this.panel3.TabIndex = 39;
            // 
            // dblProList
            // 
            this.dblProList.AllowColumnReorder = true;
            this.dblProList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dblProList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dblProList.Location = new System.Drawing.Point(6, 0);
            this.dblProList.Margin = new System.Windows.Forms.Padding(0);
            this.dblProList.Name = "dblProList";
            this.dblProList.Size = new System.Drawing.Size(214, 249);
            this.dblProList.TabIndex = 44;
            this.dblProList.UseCompatibleStateImageBehavior = false;
            this.dblProList.View = System.Windows.Forms.View.Details;
            // 
            // MagicMirror
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dblProList);
            this.Controls.Add(this.pnlPic);
            this.Controls.Add(this.pnlLeft);
            this.Name = "MagicMirror";
            this.Size = new System.Drawing.Size(220, 356);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.pnlPic.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlPic;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private DoubleBufferListView dblProList;
    }
}

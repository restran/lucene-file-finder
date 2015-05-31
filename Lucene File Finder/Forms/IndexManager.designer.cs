namespace LuceneFileFinder.Forms
{
    partial class IndexManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader chMOutFolder;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexManager));
            this.tabManager = new System.Windows.Forms.TabControl();
            this.tpFile = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblFPaths = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblFFiles = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFUpdate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFCreation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFInAdd = new System.Windows.Forms.Button();
            this.btnFInDelete = new System.Windows.Forms.Button();
            this.lvwFOut = new System.Windows.Forms.ListView();
            this.chFOFolder = new System.Windows.Forms.ColumnHeader();
            this.imgIcon = new System.Windows.Forms.ImageList(this.components);
            this.lvwFIn = new System.Windows.Forms.ListView();
            this.chFIFolder = new System.Windows.Forms.ColumnHeader();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnFOutAdd = new System.Windows.Forms.Button();
            this.btnFOutDelete = new System.Windows.Forms.Button();
            this.tpMP3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lvwMOut = new System.Windows.Forms.ListView();
            this.lvwMIn = new System.Windows.Forms.ListView();
            this.chMInFolder = new System.Windows.Forms.ColumnHeader();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnMOAdd = new System.Windows.Forms.Button();
            this.btnMODelete = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnMIAdd = new System.Windows.Forms.Button();
            this.btnMIDelete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblMPaths = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblMFiles = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lblMUpdate = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblMCreation = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.listView4 = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.listView5 = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            chMOutFolder = new System.Windows.Forms.ColumnHeader();
            this.tabManager.SuspendLayout();
            this.tpFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tpMP3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // chMOutFolder
            // 
            chMOutFolder.Text = "排除目录";
            chMOutFolder.Width = 312;
            // 
            // tabManager
            // 
            this.tabManager.Controls.Add(this.tpFile);
            this.tabManager.Controls.Add(this.tpMP3);
            this.tabManager.Location = new System.Drawing.Point(0, 0);
            this.tabManager.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabManager.Name = "tabManager";
            this.tabManager.SelectedIndex = 0;
            this.tabManager.Size = new System.Drawing.Size(460, 380);
            this.tabManager.TabIndex = 0;
            this.tabManager.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabManager_Selected);
            // 
            // tpFile
            // 
            this.tpFile.BackColor = System.Drawing.SystemColors.Control;
            this.tpFile.Controls.Add(this.groupBox1);
            this.tpFile.Controls.Add(this.tableLayoutPanel1);
            this.tpFile.Location = new System.Drawing.Point(4, 26);
            this.tpFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpFile.Name = "tpFile";
            this.tpFile.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpFile.Size = new System.Drawing.Size(452, 350);
            this.tpFile.TabIndex = 0;
            this.tpFile.Text = "文件";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblFPaths);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblFFiles);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblFUpdate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblFCreation);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 70);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件索引数据库信息";
            // 
            // lblFPaths
            // 
            this.lblFPaths.AutoSize = true;
            this.lblFPaths.Location = new System.Drawing.Point(313, 44);
            this.lblFPaths.Name = "lblFPaths";
            this.lblFPaths.Size = new System.Drawing.Size(39, 17);
            this.lblFPaths.TabIndex = 7;
            this.lblFPaths.Text = "1,000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(238, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "目录总数：";
            // 
            // lblFFiles
            // 
            this.lblFFiles.AutoSize = true;
            this.lblFFiles.Location = new System.Drawing.Point(313, 22);
            this.lblFFiles.Name = "lblFFiles";
            this.lblFFiles.Size = new System.Drawing.Size(46, 17);
            this.lblFFiles.TabIndex = 5;
            this.lblFFiles.Text = "10,000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(238, 22);
            this.label5.Margin = new System.Windows.Forms.Padding(4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "文件总数：";
            // 
            // lblFUpdate
            // 
            this.lblFUpdate.AutoSize = true;
            this.lblFUpdate.Location = new System.Drawing.Point(82, 44);
            this.lblFUpdate.Name = "lblFUpdate";
            this.lblFUpdate.Size = new System.Drawing.Size(109, 17);
            this.lblFUpdate.TabIndex = 3;
            this.lblFUpdate.Text = "2010/04/29 18:00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 44);
            this.label3.Margin = new System.Windows.Forms.Padding(4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "更新时间：";
            // 
            // lblFCreation
            // 
            this.lblFCreation.AutoSize = true;
            this.lblFCreation.Location = new System.Drawing.Point(82, 22);
            this.lblFCreation.Name = "lblFCreation";
            this.lblFCreation.Size = new System.Drawing.Size(109, 17);
            this.lblFCreation.TabIndex = 1;
            this.lblFCreation.Text = "2010/04/29 18:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "创建时间：";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.41958F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.58042F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lvwFOut, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lvwFIn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 85);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.57471F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.42529F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(429, 257);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFInAdd);
            this.panel1.Controls.Add(this.btnFInDelete);
            this.panel1.Location = new System.Drawing.Point(347, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(79, 71);
            this.panel1.TabIndex = 7;
            // 
            // btnFInAdd
            // 
            this.btnFInAdd.Location = new System.Drawing.Point(0, 0);
            this.btnFInAdd.Name = "btnFInAdd";
            this.btnFInAdd.Size = new System.Drawing.Size(75, 23);
            this.btnFInAdd.TabIndex = 2;
            this.btnFInAdd.Text = "添加";
            this.btnFInAdd.UseVisualStyleBackColor = true;
            this.btnFInAdd.Click += new System.EventHandler(this.btnFInAdd_Click);
            // 
            // btnFInDelete
            // 
            this.btnFInDelete.Location = new System.Drawing.Point(0, 29);
            this.btnFInDelete.Name = "btnFInDelete";
            this.btnFInDelete.Size = new System.Drawing.Size(75, 23);
            this.btnFInDelete.TabIndex = 3;
            this.btnFInDelete.Text = "删除";
            this.btnFInDelete.UseVisualStyleBackColor = true;
            this.btnFInDelete.Click += new System.EventHandler(this.btnFInDelete_Click);
            // 
            // lvwFOut
            // 
            this.lvwFOut.AllowDrop = true;
            this.lvwFOut.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFOFolder});
            this.lvwFOut.Location = new System.Drawing.Point(3, 132);
            this.lvwFOut.Name = "lvwFOut";
            this.lvwFOut.Size = new System.Drawing.Size(331, 122);
            this.lvwFOut.SmallImageList = this.imgIcon;
            this.lvwFOut.TabIndex = 1;
            this.toolTip.SetToolTip(this.lvwFOut, "使用排除目录会导致父目录上的非目录文件也将会被排除");
            this.lvwFOut.UseCompatibleStateImageBehavior = false;
            this.lvwFOut.View = System.Windows.Forms.View.Details;
            this.lvwFOut.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragDrop);
            this.lvwFOut.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragEnter);
            // 
            // chFOFolder
            // 
            this.chFOFolder.Text = "排除目录";
            this.chFOFolder.Width = 312;
            // 
            // imgIcon
            // 
            this.imgIcon.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgIcon.ImageSize = new System.Drawing.Size(16, 16);
            this.imgIcon.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvwFIn
            // 
            this.lvwFIn.AllowDrop = true;
            this.lvwFIn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFIFolder});
            this.lvwFIn.Location = new System.Drawing.Point(3, 3);
            this.lvwFIn.Name = "lvwFIn";
            this.lvwFIn.Size = new System.Drawing.Size(331, 123);
            this.lvwFIn.SmallImageList = this.imgIcon;
            this.lvwFIn.TabIndex = 0;
            this.toolTip.SetToolTip(this.lvwFIn, "只包含重要的目录可减少索引时间");
            this.lvwFIn.UseCompatibleStateImageBehavior = false;
            this.lvwFIn.View = System.Windows.Forms.View.Details;
            this.lvwFIn.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragDrop);
            this.lvwFIn.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragEnter);
            // 
            // chFIFolder
            // 
            this.chFIFolder.Text = "包含目录";
            this.chFIFolder.Width = 313;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnFOutAdd);
            this.panel2.Controls.Add(this.btnFOutDelete);
            this.panel2.Location = new System.Drawing.Point(347, 132);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(79, 75);
            this.panel2.TabIndex = 8;
            // 
            // btnFOutAdd
            // 
            this.btnFOutAdd.Location = new System.Drawing.Point(0, 0);
            this.btnFOutAdd.Name = "btnFOutAdd";
            this.btnFOutAdd.Size = new System.Drawing.Size(75, 23);
            this.btnFOutAdd.TabIndex = 4;
            this.btnFOutAdd.Text = "添加";
            this.toolTip.SetToolTip(this.btnFOutAdd, "排除的目录必须是包含目录中某项的子目录");
            this.btnFOutAdd.UseVisualStyleBackColor = true;
            this.btnFOutAdd.Click += new System.EventHandler(this.btnFOutAdd_Click);
            // 
            // btnFOutDelete
            // 
            this.btnFOutDelete.Location = new System.Drawing.Point(0, 29);
            this.btnFOutDelete.Name = "btnFOutDelete";
            this.btnFOutDelete.Size = new System.Drawing.Size(75, 23);
            this.btnFOutDelete.TabIndex = 5;
            this.btnFOutDelete.Text = "删除";
            this.btnFOutDelete.UseVisualStyleBackColor = true;
            this.btnFOutDelete.Click += new System.EventHandler(this.btnFOutDelete_Click);
            // 
            // tpMP3
            // 
            this.tpMP3.BackColor = System.Drawing.SystemColors.Control;
            this.tpMP3.Controls.Add(this.tableLayoutPanel4);
            this.tpMP3.Controls.Add(this.groupBox2);
            this.tpMP3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tpMP3.Location = new System.Drawing.Point(4, 26);
            this.tpMP3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpMP3.Name = "tpMP3";
            this.tpMP3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tpMP3.Size = new System.Drawing.Size(452, 350);
            this.tpMP3.TabIndex = 1;
            this.tpMP3.Text = "MP3";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.41958F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.58042F));
            this.tableLayoutPanel4.Controls.Add(this.lvwMOut, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lvwMIn, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel5, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(8, 85);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.57471F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.42529F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(429, 257);
            this.tableLayoutPanel4.TabIndex = 9;
            // 
            // lvwMOut
            // 
            this.lvwMOut.AllowDrop = true;
            this.lvwMOut.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            chMOutFolder});
            this.lvwMOut.Location = new System.Drawing.Point(3, 132);
            this.lvwMOut.Name = "lvwMOut";
            this.lvwMOut.Size = new System.Drawing.Size(331, 122);
            this.lvwMOut.SmallImageList = this.imgIcon;
            this.lvwMOut.TabIndex = 1;
            this.toolTip.SetToolTip(this.lvwMOut, "使用排除目录会导致父目录上的非目录文件也将会被排除");
            this.lvwMOut.UseCompatibleStateImageBehavior = false;
            this.lvwMOut.View = System.Windows.Forms.View.Details;
            this.lvwMOut.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragDrop);
            this.lvwMOut.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragEnter);
            // 
            // lvwMIn
            // 
            this.lvwMIn.AllowDrop = true;
            this.lvwMIn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chMInFolder});
            this.lvwMIn.Location = new System.Drawing.Point(3, 3);
            this.lvwMIn.Name = "lvwMIn";
            this.lvwMIn.Size = new System.Drawing.Size(331, 123);
            this.lvwMIn.SmallImageList = this.imgIcon;
            this.lvwMIn.TabIndex = 0;
            this.toolTip.SetToolTip(this.lvwMIn, "只包含重要的目录可减少索引时间");
            this.lvwMIn.UseCompatibleStateImageBehavior = false;
            this.lvwMIn.View = System.Windows.Forms.View.Details;
            this.lvwMIn.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragDrop);
            this.lvwMIn.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvwFIn_DragEnter);
            // 
            // chMInFolder
            // 
            this.chMInFolder.Text = "包含目录";
            this.chMInFolder.Width = 316;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnMOAdd);
            this.panel5.Controls.Add(this.btnMODelete);
            this.panel5.Location = new System.Drawing.Point(347, 132);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(79, 75);
            this.panel5.TabIndex = 8;
            // 
            // btnMOAdd
            // 
            this.btnMOAdd.Location = new System.Drawing.Point(0, 0);
            this.btnMOAdd.Name = "btnMOAdd";
            this.btnMOAdd.Size = new System.Drawing.Size(75, 23);
            this.btnMOAdd.TabIndex = 4;
            this.btnMOAdd.Text = "添加";
            this.toolTip.SetToolTip(this.btnMOAdd, "排除的目录必须是包含目录中某项的子目录");
            this.btnMOAdd.UseVisualStyleBackColor = true;
            this.btnMOAdd.Click += new System.EventHandler(this.btnMOAdd_Click);
            // 
            // btnMODelete
            // 
            this.btnMODelete.Location = new System.Drawing.Point(0, 29);
            this.btnMODelete.Name = "btnMODelete";
            this.btnMODelete.Size = new System.Drawing.Size(75, 23);
            this.btnMODelete.TabIndex = 5;
            this.btnMODelete.Text = "删除";
            this.btnMODelete.UseVisualStyleBackColor = true;
            this.btnMODelete.Click += new System.EventHandler(this.btnMODelete_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnMIAdd);
            this.panel6.Controls.Add(this.btnMIDelete);
            this.panel6.Location = new System.Drawing.Point(347, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(79, 75);
            this.panel6.TabIndex = 9;
            // 
            // btnMIAdd
            // 
            this.btnMIAdd.Location = new System.Drawing.Point(0, 0);
            this.btnMIAdd.Name = "btnMIAdd";
            this.btnMIAdd.Size = new System.Drawing.Size(75, 23);
            this.btnMIAdd.TabIndex = 4;
            this.btnMIAdd.Text = "添加";
            this.btnMIAdd.UseVisualStyleBackColor = true;
            this.btnMIAdd.Click += new System.EventHandler(this.btnMIAdd_Click);
            // 
            // btnMIDelete
            // 
            this.btnMIDelete.Location = new System.Drawing.Point(0, 29);
            this.btnMIDelete.Name = "btnMIDelete";
            this.btnMIDelete.Size = new System.Drawing.Size(75, 23);
            this.btnMIDelete.TabIndex = 5;
            this.btnMIDelete.Text = "删除";
            this.btnMIDelete.UseVisualStyleBackColor = true;
            this.btnMIDelete.Click += new System.EventHandler(this.btnMIDelete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblMPaths);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.lblMFiles);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.lblMUpdate);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.lblMCreation);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Location = new System.Drawing.Point(8, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(429, 70);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MP3索引数据库信息";
            // 
            // lblMPaths
            // 
            this.lblMPaths.AutoSize = true;
            this.lblMPaths.Location = new System.Drawing.Point(313, 44);
            this.lblMPaths.Name = "lblMPaths";
            this.lblMPaths.Size = new System.Drawing.Size(39, 17);
            this.lblMPaths.TabIndex = 7;
            this.lblMPaths.Text = "1,000";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(238, 44);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(68, 17);
            this.label18.TabIndex = 6;
            this.label18.Text = "目录总数：";
            // 
            // lblMFiles
            // 
            this.lblMFiles.AutoSize = true;
            this.lblMFiles.Location = new System.Drawing.Point(313, 22);
            this.lblMFiles.Name = "lblMFiles";
            this.lblMFiles.Size = new System.Drawing.Size(46, 17);
            this.lblMFiles.TabIndex = 5;
            this.lblMFiles.Text = "10,000";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(238, 22);
            this.label20.Margin = new System.Windows.Forms.Padding(4);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(68, 17);
            this.label20.TabIndex = 4;
            this.label20.Text = "文件总数：";
            // 
            // lblMUpdate
            // 
            this.lblMUpdate.AutoSize = true;
            this.lblMUpdate.Location = new System.Drawing.Point(82, 44);
            this.lblMUpdate.Name = "lblMUpdate";
            this.lblMUpdate.Size = new System.Drawing.Size(109, 17);
            this.lblMUpdate.TabIndex = 3;
            this.lblMUpdate.Text = "2010/04/29 18:00";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(7, 44);
            this.label22.Margin = new System.Windows.Forms.Padding(4);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(68, 17);
            this.label22.TabIndex = 2;
            this.label22.Text = "更新时间：";
            // 
            // lblMCreation
            // 
            this.lblMCreation.AutoSize = true;
            this.lblMCreation.Location = new System.Drawing.Point(82, 22);
            this.lblMCreation.Name = "lblMCreation";
            this.lblMCreation.Size = new System.Drawing.Size(109, 17);
            this.lblMCreation.TabIndex = 1;
            this.lblMCreation.Text = "2010/04/29 18:00";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(7, 22);
            this.label24.Margin = new System.Windows.Forms.Padding(4);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(68, 17);
            this.label24.TabIndex = 0;
            this.label24.Text = "创建时间：";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.41958F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.58042F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.button2);
            this.panel3.Location = new System.Drawing.Point(163, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(34, 66);
            this.panel3.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "添加";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "删除";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView3.Location = new System.Drawing.Point(3, 132);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(331, 122);
            this.listView3.TabIndex = 1;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "排除路径";
            this.columnHeader1.Width = 312;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(313, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "1,000";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "目录总数：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(313, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "10,000";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(238, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(4);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 4;
            this.label12.Text = "文件总数：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(82, 44);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 12);
            this.label13.TabIndex = 3;
            this.label13.Text = "2010/04/29 18:00";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 44);
            this.label14.Margin = new System.Windows.Forms.Padding(4);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 2;
            this.label14.Text = "更新时间：";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(82, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(101, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "2010/04/29 18:00";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 22);
            this.label16.Margin = new System.Windows.Forms.Padding(4);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "创建时间：";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.41958F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.58042F));
            this.tableLayoutPanel3.Controls.Add(this.listView4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.listView5, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listView4.Location = new System.Drawing.Point(3, 23);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(154, 74);
            this.listView4.TabIndex = 1;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "排除路径";
            this.columnHeader2.Width = 312;
            // 
            // listView5
            // 
            this.listView5.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView5.Location = new System.Drawing.Point(3, 3);
            this.listView5.Name = "listView5";
            this.listView5.Size = new System.Drawing.Size(154, 14);
            this.listView5.TabIndex = 0;
            this.listView5.UseCompatibleStateImageBehavior = false;
            this.listView5.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "包含路径";
            this.columnHeader3.Width = 174;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "文件数";
            this.columnHeader4.Width = 82;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "文件夹数";
            this.columnHeader5.Width = 64;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button3);
            this.panel4.Controls.Add(this.button4);
            this.panel4.Location = new System.Drawing.Point(347, 132);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(79, 75);
            this.panel4.TabIndex = 8;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(0, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "添加";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1, 29);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "删除";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblWarning.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblWarning.Location = new System.Drawing.Point(24, 388);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(164, 17);
            this.lblWarning.TabIndex = 18;
            this.lblWarning.Text = "当前目录为排除目录的子目录";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(359, 384);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(271, 384);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "确定";
            this.toolTip.SetToolTip(this.btnOK, "保存设置");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // IndexManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(460, 413);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabManager);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "IndexManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "索引管理";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.IndexManager_FormClosed);
            this.tabManager.ResumeLayout(false);
            this.tpFile.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tpMP3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabManager;
        private System.Windows.Forms.TabPage tpFile;
        private System.Windows.Forms.TabPage tpMP3;
        private System.Windows.Forms.Button btnFOutDelete;
        private System.Windows.Forms.Button btnFOutAdd;
        private System.Windows.Forms.Button btnFInDelete;
        private System.Windows.Forms.Button btnFInAdd;
        private System.Windows.Forms.ListView lvwFOut;
        private System.Windows.Forms.ColumnHeader chFOFolder;
        private System.Windows.Forms.ListView lvwFIn;
        private System.Windows.Forms.ColumnHeader chFIFolder;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFUpdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFCreation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFPaths;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblFFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblMPaths;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblMFiles;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lblMUpdate;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblMCreation;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ListView lvwMOut;
        private System.Windows.Forms.ListView lvwMIn;
        private System.Windows.Forms.ColumnHeader chMInFolder;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnMOAdd;
        private System.Windows.Forms.Button btnMODelete;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnMIAdd;
        private System.Windows.Forms.Button btnMIDelete;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listView5;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ImageList imgIcon;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
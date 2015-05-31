using System.Windows.Forms;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Control
{
    public partial class MagicMirror : UserControl
    {
        public MagicMirror()
        {
            InitializeComponent();
        }

        #region SetProperties
        public void SetProperties(MagicMode mode, object properties)
        {
            switch (mode)
            {
                case MagicMode.File:
                    this.SetFile((FileProperties)properties);
                    break;
                case MagicMode.JPG:
                    this.SetJPG((JPGExif)properties);
                    break;
                case MagicMode.MP3:
                    this.SetMP3((MP3Tag)properties);
                    break;
                default:
                    this.SetNone();
                    break;
            }
        }
        #endregion

        #region SetNone
        private void SetNone()
        {
            this.pnlPic.Visible = false;
            if (this.picPreview.Image != null)
                this.picPreview.Image.Dispose();

            this.dblProList.Items.Clear();
            this.dblProList.Columns.Clear();
            ColumnHeader property = new ColumnHeader();
            property.Text = "";
            property.Width = 200;

            this.dblProList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            property});
        }
        #endregion

        #region SetFile
        private void SetFile(FileProperties properties)
        {
            this.pnlPic.Visible = false;
            if (this.picPreview.Image != null)
                this.picPreview.Image.Dispose();
            this.dblProList.Items.Clear();
            this.dblProList.Columns.Clear();
            ColumnHeader property = new ColumnHeader();
            property.Text = "文件属性";
            property.Width = 200;

            this.dblProList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            property});
            ListViewItem[] items = new ListViewItem[14];
            items[0] = new ListViewItem("文件名：");
            items[1] = new ListViewItem("name");
            items[2] = new ListViewItem("文件大小：");
            items[3] = new ListViewItem("length");
            items[4] = new ListViewItem("创建时间：");
            items[5] = new ListViewItem("creationTime");
            items[6] = new ListViewItem("修改时间：");
            items[7] = new ListViewItem("lastWriteTime");
            items[8] = new ListViewItem("最近访问时间：");
            items[9] = new ListViewItem("lastAccessTime");
            items[10] = new ListViewItem("文件属性：");
            items[11] = new ListViewItem("attributies");
            items[12] = new ListViewItem("文件类型：");
            items[13] = new ListViewItem("extension");

            this.dblProList.Items.AddRange(items);

            this.dblProList.Items[1].SubItems[0].Text = properties.Name;
            this.dblProList.Items[3].SubItems[0].Text = properties.Length;
            this.dblProList.Items[5].SubItems[0].Text = properties.CreationTime;
            this.dblProList.Items[7].SubItems[0].Text = properties.LastWriteTime;
            this.dblProList.Items[9].SubItems[0].Text = properties.LastAccessTime;
            this.dblProList.Items[11].SubItems[0].Text = properties.Attributies;
            this.dblProList.Items[13].SubItems[0].Text = properties.Extension;
        }
        #endregion

        #region SetJPG
        private void SetJPG(JPGExif jpgExif)
        {
            this.dblProList.Items.Clear();
            this.dblProList.Columns.Clear();
            ColumnHeader property = new ColumnHeader();
            ColumnHeader value = new ColumnHeader();
            property.Text = "属性";
            value.Text = "值";
            property.Width = 80;
            value.Width = 120;

            this.dblProList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            property,
            value});

            ListViewItem[] items = new ListViewItem[10];
            items[0] = new ListViewItem(new string[] {
            "相机制造商", ""});
            items[1] = new ListViewItem(new string[] {
            "相机型号", ""});
            items[2] = new ListViewItem(new string[] {
            "拍摄时间", ""});
            items[3] = new ListViewItem(new string[] {
            "光圈值", ""});
            items[4] = new ListViewItem(new string[] {
            "曝光时间", ""});
            items[5] = new ListViewItem(new string[] {
            "ISO速度", ""});
            items[6] = new ListViewItem(new string[] {
            "闪光灯", ""});
            items[7] = new ListViewItem(new string[] {
            "焦距", ""});
            items[8] = new ListViewItem(new string[] {
            "宽度", ""});
            items[9] = new ListViewItem(new string[] {
            "高度", ""});

            this.pnlPic.Visible = true;
            if (this.picPreview.Image != null)
                this.picPreview.Image.Dispose();
            this.dblProList.Items.AddRange(items);

            this.dblProList.Items[0].SubItems[1].Text = jpgExif.Maker;
            this.dblProList.Items[1].SubItems[1].Text = jpgExif.Model;
            this.dblProList.Items[2].SubItems[1].Text = jpgExif.ShootingTime;
            this.dblProList.Items[3].SubItems[1].Text = jpgExif.F_Number;
            this.dblProList.Items[4].SubItems[1].Text = jpgExif.ExposureTime;
            this.dblProList.Items[5].SubItems[1].Text = jpgExif.ISO;
            this.dblProList.Items[6].SubItems[1].Text = jpgExif.FlashLamp;
            this.dblProList.Items[7].SubItems[1].Text = jpgExif.FocalLength;
            this.dblProList.Items[8].SubItems[1].Text = jpgExif.Width.ToString();
            this.dblProList.Items[9].SubItems[1].Text = jpgExif.Height.ToString();
            this.picPreview.Image = jpgExif.PicPreview;
        }
        #endregion

        #region SetMP3
        private void SetMP3(MP3Tag mp3Tag)
        {
            this.pnlPic.Visible = false;
            if (this.picPreview.Image != null)
                this.picPreview.Image.Dispose();
            this.dblProList.Items.Clear();
            this.dblProList.Columns.Clear();
            ColumnHeader property = new ColumnHeader();
            ColumnHeader value = new ColumnHeader();
            property.Text = "属性";
            value.Text = "值";
            property.Width = 80;
            value.Width = 120;

            this.dblProList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            property,
            value});
            ListViewItem[] items = new ListViewItem[10]; 
            items[0] = new ListViewItem(new string[] {
            "标题", ""});
            items[1] = new ListViewItem(new string[] {
            "艺术家", ""});
            items[2] = new ListViewItem(new string[] {
            "作曲家", ""});
            items[3] = new ListViewItem(new string[] {
            "专辑", ""});
            items[4] = new ListViewItem(new string[] {
            "音轨", ""});
            items[5] = new ListViewItem(new string[] {
            "流派", ""});
            items[6] = new ListViewItem(new string[] {
            "年代", ""});
            items[7] = new ListViewItem(new string[] {
            "长度",""});
            items[8] = new ListViewItem(new string[] {
            "比特率", ""});
            items[9] = new ListViewItem(new string[] {
            "备注", ""});

            this.dblProList.Items.AddRange(items);

            this.dblProList.Items[0].SubItems[1].Text = mp3Tag.Title;
            this.dblProList.Items[1].SubItems[1].Text = mp3Tag.Artist;
            this.dblProList.Items[2].SubItems[1].Text = mp3Tag.Composer;
            this.dblProList.Items[3].SubItems[1].Text = mp3Tag.Album;
            this.dblProList.Items[4].SubItems[1].Text = mp3Tag.TrackNumber;
            this.dblProList.Items[5].SubItems[1].Text = mp3Tag.Genre;
            this.dblProList.Items[6].SubItems[1].Text = mp3Tag.Date;
            this.dblProList.Items[7].SubItems[1].Text = mp3Tag.Length;
            this.dblProList.Items[8].SubItems[1].Text = mp3Tag.Kbps;
            this.dblProList.Items[9].SubItems[1].Text = mp3Tag.Comment;
        }
        #endregion
    }
}

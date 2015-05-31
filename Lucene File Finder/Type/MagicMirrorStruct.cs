using System.Drawing;

namespace LuceneFileFinder.Type
{
    //此文件用于存放要在魔镜中显示的数据的结构

    #region FileProperties
    /// <summary>
    /// 用于在FilePropertiesControl中显示文件属性
    /// </summary>
    public struct FileProperties
    {
        #region 成员变量
        private string name;
        private string length;
        private string creationTime;
        private string lastWriteTime;
        private string lastAccessTime;
        private string attributies;
        private string extension;
        #endregion

        #region 构造函数
        public FileProperties(string _name, string _length, string _creationTime,
            string _lastWriteTime, string _lastAccessTime, string _attributies, string _extension)
        {
            this.name = _name;
            this.length = _length;
            this.creationTime = _creationTime;
            this.lastWriteTime = _lastWriteTime;
            this.lastAccessTime = _lastAccessTime;
            this.attributies = _attributies;
            this.extension = _extension;
        }

        public FileProperties(FileProperties fileProperties)
        {
            this.name = fileProperties.name;
            this.length = fileProperties.length;
            this.creationTime = fileProperties.creationTime;
            this.lastWriteTime = fileProperties.lastWriteTime;
            this.lastAccessTime = fileProperties.lastAccessTime;
            this.attributies = fileProperties.attributies;
            this.extension = fileProperties.extension;
        }
        #endregion

        #region 属性
        public string Name
        {
            set { this.name = value; }
            get { return name; }
        }

        public string Length
        {
            set { this.length = value; }
            get { return length; }
        }

        public string CreationTime
        {
            set { this.creationTime = value; }
            get { return creationTime; }
        }

        public string LastWriteTime
        {
            set { this.lastWriteTime = value; }
            get { return lastWriteTime; }
        }

        public string LastAccessTime
        {
            set { this.lastAccessTime = value; }
            get { return lastAccessTime; }
        }

        public string Attributies
        {
            set { this.attributies = value; }
            get { return attributies; }
        }

        public string Extension
        {
            set { this.extension = value; }
            get { return extension; }
        }
        #endregion
    }
    #endregion

    #region MP3Tag
    /// <summary>
    /// 用于在MP3TagControl中显示的Mp3Tag信息
    /// </summary>
    public struct MP3Tag
    {
        #region 成员变量
        private string album;//专辑
        private string artist;//艺术家
        private string comment;//备注
        private string composer;//作曲家
        private string date;//年代
        private string genre;//流派
        private string publisher;//发布者
        private string title;//标题
        private string trackNumber;//音轨
        private string length;//时间长度
        private string kbps;//比特率
        #endregion

        #region 构造函数
        public MP3Tag(string _album, string _artist, string _comment, string _composer,
            string _date, string _genre, string _publisher,
            string _title, string _trackNumber, string _length, string _kbps)
        {
            this.album = _album;
            this.artist = _artist;
            this.comment = _comment;
            this.composer = _composer;
            this.date = _date;
            this.genre = _genre;
            this.publisher = _publisher;
            this.title = _title;
            this.trackNumber = _trackNumber;
            this.length = _length;
            this.kbps = _kbps;
        }

        public MP3Tag(MP3Tag mp3Tag)
        {
            this.album = mp3Tag.album;
            this.artist = mp3Tag.artist;
            this.comment = mp3Tag.comment;
            this.composer = mp3Tag.composer;
            this.date = mp3Tag.date;
            this.genre = mp3Tag.genre;
            this.publisher = mp3Tag.publisher;
            this.title = mp3Tag.title;
            this.trackNumber = mp3Tag.trackNumber;
            this.length = mp3Tag.length;
            this.kbps = mp3Tag.kbps;
        }
        #endregion

        #region 属性
        public string Album
        {
            set { this.album = value; }
            get { return this.album; }
        }

        public string Artist
        {
            set { this.album = value; }
            get { return this.album; }
        }

        public string Comment
        {
            set { this.comment = value; }
            get { return this.comment; }
        }

        public string Composer
        {
            set { this.composer = value; }
            get { return this.composer; }
        }

        public string Date
        {
            set { this.date = value; }
            get { return this.date; }
        }

        public string Genre
        {
            set { this.genre = value; }
            get { return this.genre; }
        }

        public string Publisher
        {
            set { this.publisher = value; }
            get { return this.publisher; }
        }

        public string Title
        {
            set { this.title = value; }
            get { return this.title; }
        }

        public string TrackNumber
        {
            set { this.trackNumber = value; }
            get { return this.trackNumber; }
        }

        public string Length
        {
            set { this.length = value; }
            get { return this.length; }
        }

        public string Kbps
        {
            set { this.kbps = value; }
            get { return this.kbps; }
        }
        #endregion
    }
    #endregion

    #region JPGExif
    /// <summary>
    /// 用于在JPGExifControl中显示的信息
    /// </summary>
    public struct JPGExif
    {
        public string Maker;//相机制造商
        public string Model;//相机型号
        public string ShootingTime;//拍摄时间
        public string F_Number;//光圈值
        public string ExposureTime;//曝光时间
        public string ISO;//ISO速度
        public string FlashLamp;//闪光灯
        public string FocalLength;//焦距
        public int Width;      //图像的宽  单位为像素
        public int Height;     //图像的高  单位为像素
        public Image PicPreview;//图片
    }
    #endregion
}

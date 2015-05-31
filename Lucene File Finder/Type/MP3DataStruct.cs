
namespace LuceneFileFinder.Type
{
    #region MP3FileData
    /// <summary>
    /// 扫描获取MP3文件时的文件属性
    /// </summary>
    public struct MP3FileData
    {
        public string name;
        public string length;
        public string directory;
        public string lastWriteTime;
    }
    #endregion

    #region MP3TagData
    /// <summary>
    /// 用于索引时存放读取MP3文件获取的MP3Tag数据
    /// </summary>
    public struct MP3TagData
    {
        public string album;//专辑名
        public string artist;//艺术家
        public string genre;//流派
        public string pubYear;//发布年份
        public string songName;//歌曲名
        public string name;//文件名
        public string length;//文件大小
        public string directory;//文件夹
        public string lastWriteTime;//上次修改时间
    }
    #endregion

    #region MP3ArtistInfo
    /// <summary>
    /// 艺术家曲目数
    /// </summary>
    public struct MP3ArtistInfo
    {
        /// <summary>
        /// 艺术家
        /// </summary>
        public string artist;
        /// <summary>
        /// 曲目数
        /// </summary>
        public int songNums;

        public MP3ArtistInfo(string _artist, int _songNums)
        {
            this.artist = _artist;
            this.songNums = _songNums;
        }
    }
    #endregion
}

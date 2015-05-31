using System.Collections.Generic;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Util
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class Static
    {
        /// <summary>
        /// 要索引的文件路径
        /// </summary>
        public static List<string> FileIndexPath = new List<string>();
        /// <summary>
        /// 要索引的MP3路径
        /// </summary>
        public static List<string> MP3IndexPath = new List<string>();
        /// <summary>
        /// 应用程序设置信息
        /// </summary>
        public static SettingsDB Settings = new SettingsDB();
        /// <summary>
        /// 文件索引存放的位置
        /// </summary>
        public static string FileIndexStorePath = "Index\\File\\";
        /// <summary>
        /// MP3索引存放位置
        /// </summary>
        public static string MP3IndexStorePath = "Index\\MP3\\";
        /// <summary>
        /// mp3索引中的艺术家列表
        /// </summary>
        public static List<string> MP3Artist = new List<string>();
        /// <summary>
        /// 数据库中的MP3曲目数
        /// </summary>
        public static int MP3SongNums = 0;
        /// <summary>
        /// mp3艺术家和曲目数信息，未获取数据时为空。
        /// </summary>
        public static List<MP3ArtistInfo> MArtistInfo = null;
        /// <summary>
        /// 搜索显示结果时一页显示的项数
        /// </summary>
        public const int PageItems = 100;
    }  
}

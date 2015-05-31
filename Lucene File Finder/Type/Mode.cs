using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneFileFinder.Type
{
    #region SearchMode
    /// <summary>
    /// 搜索的模式
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// MP3
        /// </summary>
        MP3
    }
    #endregion

    #region IndexMode
    /// <summary>
    /// 文件索引的模式
    /// </summary>
    public enum IndexMode
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// MP3
        /// </summary>
        MP3
    }
    #endregion

    #region MagicMode
    /// <summary>
    /// 在魔镜中显示的模式
    /// </summary>
    public enum MagicMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 普通文件
        /// </summary>
        File,
        /// <summary>
        /// MP3音乐
        /// </summary>
        MP3,
        /// <summary>
        /// JPG图片
        /// </summary>
        JPG
    }
    #endregion
}

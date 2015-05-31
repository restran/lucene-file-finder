using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneFileFinder.Type
{
    /// <summary>
    /// 搜索时用的关键字
    /// </summary>
    public struct KeywordStruct
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        public string Directory;
        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension;
        /// <summary>
        /// 艺术家，搜索MP3时使用。
        /// </summary>
        public string Artist;
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name;

        public void Reset()
        {
            this.Directory = null;
            this.Extension = null;
            this.Name = "";
            this.Artist = null;
        }
    }
}

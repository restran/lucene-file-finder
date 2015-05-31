using System;
using System.Collections.Generic;
using System.Text;

namespace LuceneFileFinder.Type
{
    #region FileData
    /// <summary>
    /// 用于索引时存储文件信息
    /// </summary>
    public struct FileData
    {
        #region 成员变量
        private string name;
        private string directory;
        /// <summary>
        /// -1表示文件夹
        /// </summary>
        private string length;
        private string lastWriteTime;
        /// <summary>
        /// 扩展名，文件夹为.f，无扩展名的文件为.e，其他为如txt
        /// </summary>
        private string extension;
        #endregion

        #region 构造函数
        public FileData(string _name, string _directory, string _length, string _lastWriteTime)
        {
            this.name = _name;
            this.directory = _directory;
            this.length = _length;
            this.lastWriteTime = _lastWriteTime;
            this.extension = "";
        }

        public FileData(FileData fileInfo)
        {
            this.name = fileInfo.name;
            this.directory = fileInfo.directory;
            this.length = fileInfo.length;
            this.lastWriteTime = fileInfo.lastWriteTime;
            this.extension = "";
        }
        #endregion

        #region 属性
        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        public string Directory
        {
            set { this.directory = value; }
            get { return this.directory; }
        }

        public string Length
        {
            set { this.length = value; }
            get { return this.length; }
        }

        public string LastWriteTime
        {
            set { this.lastWriteTime = value; }
            get { return this.lastWriteTime; }
        }

        public string Extension
        {
            set { this.extension = value; }
            get { return this.extension; }
        }
        #endregion
    }
    #endregion

    #region WIN32_FIND_DATA
    [Serializable,
    System.Runtime.InteropServices.StructLayout
        (System.Runtime.InteropServices.LayoutKind.Sequential,
        CharSet = System.Runtime.InteropServices.CharSet.Auto),
    System.Runtime.InteropServices.BestFitMapping(false)]
    public struct WIN32_FIND_DATA
    {
        public int dwFileAttributes;
        public int ftCreationTime_dwLowDateTime;
        public int ftCreationTime_dwHighDateTime;
        public int ftLastAccessTime_dwLowDateTime;
        public int ftLastAccessTime_dwHighDateTime;
        public int ftLastWriteTime_dwLowDateTime;
        public int ftLastWriteTime_dwHighDateTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 260)]
        public string cFileName;
        [System.Runtime.InteropServices.MarshalAs
            (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
            SizeConst = 14)]
        public string cAlternateFileName;
    }
    #endregion

    #region FindFileData
    /// <summary>
    /// 找到的文件或文件夹的数据
    /// </summary>
    public struct FindFileData
    {
        /// <summary>
        /// //文件名如abc.txt或abc(文件夹)
        /// </summary>
        public string name;
        /// <summary>
        /// //文件所在的目录如D:\
        /// </summary>
        public string directory;
        public int dwFileAttributes;
        public int ftLastWriteTime_dwLowDateTime;
        public int ftLastWriteTime_dwHighDateTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        /// <summary>
        /// //是否是文件
        /// </summary>
        public bool isFile;

        public FindFileData(WIN32_FIND_DATA win32_find_data, string _directory, bool _isFile)
        {
            this.name = win32_find_data.cFileName;
            this.directory = _directory;
            this.isFile = _isFile;
            this.dwFileAttributes = win32_find_data.dwFileAttributes;
            this.ftLastWriteTime_dwLowDateTime = win32_find_data.ftLastWriteTime_dwLowDateTime;
            this.ftLastWriteTime_dwHighDateTime = win32_find_data.ftLastWriteTime_dwHighDateTime;
            this.nFileSizeHigh = win32_find_data.nFileSizeHigh;
            this.nFileSizeLow = win32_find_data.nFileSizeLow;
        }
    }
    #endregion
}

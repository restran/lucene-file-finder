using System;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Util
{
    /// <summary>
    /// 文件和目录的遍历器
    /// 本对象为Win32API函数 FindFirstFile , FindNextFile的封装
    /// </summary>
    public class FileDirectoryFinder
    {
        private FindFileData currentFileData = new FindFileData();

        private bool bolIsFile = true;
        /// <summary>
        /// 当前对象是否为文件,若为true则当前对象为文件,否则为目录
        /// </summary>
        public bool IsFile
        {
            get { return this.bolIsFile; }
        }

        #region 控制对象特性的一些属性 ****************************************

        private string strSearchPath = null;
        /// <summary>
        /// 搜索的父目录,必须为绝对路径,不得有通配符,该目录必须存在
        /// </summary>
        public string SearchPath
        {
            get { return strSearchPath; }
            set { strSearchPath = value; }
        }
        #endregion

        /// <summary>
        /// 当前为文件夹时，返回文件夹全名
        /// </summary>
        public string FullName
        {
            get { return this.strSearchPath +  this.myData.cFileName + "\\"; }
        }
        /// <summary>
        /// 找到的文件的数据
        /// </summary>
        public FindFileData CurrentFileData
        {
            get { return this.currentFileData; }
        }

        /// <summary>
        /// 关闭对象,停止搜索
        /// </summary>
        public void Close()
        {
            this.CloseHandler();
        }

        /// <summary>
        /// 找到下一个文件或目录
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool MoveNext()
        {
            bool success = false;
            while (true)
            {
                if (this.bolStartSearchFlag)
                    success = this.SearchNext();
                else
                    success = this.StartSearch();
                if (success)
                {
                    if (this.UpdateCurrentObject())
                    {
                        //更新当前文件数据
                        this.currentFileData = new FindFileData(myData, this.strSearchPath, this.bolIsFile);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 重新设置对象
        /// </summary>
        public void Reset()
        {
            //if (this.strSearchPath == null)
            //    return;
                //throw new System.ArgumentNullException("SearchPath can not null");

            this.CloseHandler();
            this.bolStartSearchFlag = false;
        }

        #region 声明WIN32API函数

        [System.Runtime.InteropServices.DllImport
            ("kernel32.dll",
            CharSet = System.Runtime.InteropServices.CharSet.Auto,
            SetLastError = true)]
        private static extern IntPtr FindFirstFile(string pFileName, ref WIN32_FIND_DATA pFindFileData);

        [System.Runtime.InteropServices.DllImport
            ("kernel32.dll",
           CharSet = System.Runtime.InteropServices.CharSet.Auto,
            SetLastError = true)]
        private static extern bool FindNextFile(IntPtr hndFindFile, ref WIN32_FIND_DATA lpFindFileData);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindClose(IntPtr hndFindFile);

        #endregion

        #region 内部代码群

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        /// <summary>
        /// 查找处理的底层句柄
        /// </summary>
        private System.IntPtr intSearchHandler = INVALID_HANDLE_VALUE;

        private WIN32_FIND_DATA myData = new WIN32_FIND_DATA();
        /// <summary>
        /// 开始搜索标志
        /// </summary>
        private bool bolStartSearchFlag = false;
        /// <summary>
        /// 关闭内部句柄
        /// </summary>
        private void CloseHandler()
        {
            if (this.intSearchHandler != INVALID_HANDLE_VALUE)
            {
                FindClose(this.intSearchHandler);
                this.intSearchHandler = INVALID_HANDLE_VALUE;
            }
        }
        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <returns>操作是否成功</returns>
        private bool StartSearch()
        {
            bolStartSearchFlag = true;

            this.CloseHandler();
            intSearchHandler = FindFirstFile(this.strSearchPath + "*", ref myData);
            if (intSearchHandler == INVALID_HANDLE_VALUE)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 搜索下一个
        /// </summary>
        /// <returns>操作是否成功</returns>
        private bool SearchNext()
        {
            if (bolStartSearchFlag == false)
                return false;
            if (intSearchHandler == INVALID_HANDLE_VALUE)
                return false;
            try
            {
                if (FindNextFile(intSearchHandler, ref myData) == false)
                {
                    this.CloseHandler();
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }//private bool SearchNext()

        /// <summary>
        /// 更新当前对象
        /// </summary>
        /// <returns>操作是否成功</returns>
        private bool UpdateCurrentObject()
        {
            if (intSearchHandler == INVALID_HANDLE_VALUE)
                return false;
            else if (myData.cFileName == null)
                return false;
            else if ((myData.dwFileAttributes & 0x10) == 0)
            {
                // 当前对象为文件
                this.bolIsFile = true;
                return true;
            }
            else
            {
                // 当前对象为目录
                this.bolIsFile = false;
                if (myData.cFileName == "." || myData.cFileName == "..")
                    return false;
                else
                    return true;
            }
        }//private bool UpdateCurrentObject()

        #endregion
    }
}
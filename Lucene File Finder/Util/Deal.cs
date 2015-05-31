using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LuceneFileFinder.Type;

namespace LuceneFileFinder.Util
{
    /// <summary>
    /// 专门用来对一些数据进行处理
    /// </summary>
    public static class Deal
    {
        #region GetExtension

        private static string strTemp = "";
        private static int intTemp = 0;
        /// <summary>
        /// 根据文件名获取扩展名(必须是文件)
        /// <remarks>
        /// 无扩展名的文件为.e，其他为如txt。
        /// </remarks>
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>扩展名全小写格式</returns>
        public static string GetExtension(string name)
        {
            intTemp = name.LastIndexOf('.');
            if (intTemp > 0)
            {
                intTemp++;//不包含'.'
                strTemp = name.Substring(intTemp, name.Length - intTemp);
            }
            else
            {
                strTemp = ".u";//无扩展名
            }
            return strTemp.ToLower();
        }
        #endregion

        #region ToLong
        /// <summary>
        /// 对findFileData的length进行处理
        /// </summary>
        /// <param name="hight">high</param>
        /// <param name="low">low</param>
        /// <returns></returns>
        public static long ToLong(int high, int low)
        {
            long v = (uint)high;
            v = v << 0x20;
            v = v | ((uint)low);
            return v;
        }
        #endregion

        #region ToDateTimeString

        private static DateTime dtTemp;
        private static long lTemp;
        /// <summary>
        /// 对findFileData的time进行处理
        /// </summary>
        /// <param name="hight">hight</param>
        /// <param name="low">low</param>
        /// <returns></returns>
        public static string ToDateTimeString(int high, int low)
        {
            lTemp = Deal.ToLong(high, low);
            dtTemp = DateTime.FromFileTimeUtc(lTemp);
            return dtTemp.ToLocalTime().ToString();
        }
        #endregion

        #region CombineInOutPath

        #region 内部变量
        private static List<string> finPath = new List<string>();
        #endregion

        #region 内部方法
        private static void addSubdirectory(DirectoryInfo[] FSInfo, List<string> outPath)
        {
            if (FSInfo == null)
            {
                return;
            }
            foreach (DirectoryInfo i in FSInfo)
            {
                if (i.FullName.Contains("System Volume Information"))
                    continue;
                else
                {
                    finPath.Add(i.FullName + "\\");
                    foreach (string dir_exc in outPath)
                    {
                        if (dir_exc.Contains(i.FullName + "\\") && !dir_exc.Equals(i.FullName + "\\"))
                        {
                            addSubdirectory(i.GetDirectories(), outPath);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region CombineInOutPath
        /// <summary>
        /// 将包含路径和排除路径整合成索引路径
        /// </summary>
        /// <param name="inPath">包含路径</param>
        /// <param name="outPath">排除路径</param>
        /// <returns>索引路径</returns>
        public static List<string> CombineInOutPath(List<string> inPath, List<string> outPath)
        {
            if (outPath.Count == 0)
                return inPath;

            finPath.Clear();
            finPath.AddRange(inPath);
            foreach (string di in inPath)
            {
                foreach (string dir_exc in outPath)
                {
                    DirectoryInfo dir = new DirectoryInfo(di);
                    if (!dir.Exists)
                    {
                        finPath.Remove(di);
                        continue;
                    }
                    if (dir_exc.Contains(di) && !dir_exc.Equals(di))
                    {
                        DirectoryInfo[] diArr = dir.GetDirectories();
                        addSubdirectory(diArr, outPath);
                        break;
                    }
                }
            }
            List<string> temp = new List<string>();
            temp.AddRange(finPath);
            foreach (string dir_str in temp)
            {
                foreach (string dir_exc in outPath)
                {
                    if (dir_exc.Contains(dir_str))
                        finPath.Remove(dir_str);
                }
                if (dir_str.Contains("RECYCLER"))
                    finPath.Remove(dir_str);
                if (dir_str.Contains("$RECYCLE.BIN"))
                    finPath.Remove(dir_str);
            }
            return finPath;
        }
        #endregion

        #endregion

        #region ToEnglishNumString
        /// <summary>
        /// 将数字转换成英式的数字表示字符串
        /// 如1000，转换成"1,000"
        /// </summary>
        public static string ToEnglishNumString(long n)
        {
            string str = n.ToString();
            int i = str.Length;
            while (i > 3)
            {
                str = str.Insert(i - 3, ",");
                i -= 3;
            }
            return str;
        }
        #endregion

        #region FormatFileLength

        private static ulong len;
        private static string str;
        private static int i;
        /// <summary>
        /// 格式化文件大小表示
        /// 原本是字节如 1024 转换为1 KB
        /// </summary>
        /// <param name="length">字符串文件大小</param>
        /// <returns>如1,000 KB</returns>
        public static string FormatFileLength(string length)
        {
            len = Convert.ToUInt64(length);
            len = len >> 10;//右移10位，相当于除以1024

            str = len.ToString();
            i = str.Length;
            while (i > 3)
            {
                str = str.Insert(i - 3, ",");
                i -= 3;
            }
            return str + " KB";
        }

        /// <summary>
        /// 格式化文件大小表示
        /// 原本是字节如 1024 转换为1 KB
        /// </summary>
        /// <param name="length">long型文件大小</param>
        /// <returns>如1,000 KB</returns>
        public static string FormatFileLength(long length)
        {
            length = length >> 10;//右移10位，相当于除以1024

            str = length.ToString();
            i = str.Length;
            while (i > 3)
            {
                str = str.Insert(i - 3, ",");
                i -= 3;
            }
            return str + " KB";
        }
        #endregion

        #region GetKeyword

        private static KeywordStruct keyword = new KeywordStruct();
        /// <summary>
        /// 根据用户在cmbKeyword上输入的内容获取相应的关键字
        /// 如（"D:\\" ".mp3" 光良）或 ("artist: 光良")
        /// </summary>
        /// <param name="str">str已执行trim()</param>
        /// <returns></returns>
        public static KeywordStruct GetKeyword(string str)
        {
            keyword.Reset();
            string front;
            int k;
            k = str.LastIndexOf('\"');
            if (k == -1)
            {
                keyword.Name = str.Trim();
                return keyword;
            }
            else
            {
                if (str[0] == '\"')
                {
                    front = str.Substring(0, k++);
                    keyword.Name = str.Substring(k, str.Length - k).Trim();
                }
                else
                {
                    return keyword;
                }
            }

            char[] separator = { '\"' };
            string[] arr = front.Split(separator);
             
            foreach (string item in arr)
            {
                if (item.Contains(":\\"))
                    keyword.Directory = item.Trim();
                else if (item.Contains(":"))
                {
                    string[] arr2;
                    char[] separator2 = { ':' };
                    arr2 = item.Split(separator2);
                    if (arr2[0].ToLower().Trim() == "artist" && arr2.Length == 2)
                    {
                        keyword.Artist = arr2[1].Trim();
                    }
                }
                else
                {
                    //关键字一定要为小写,索引中存的是小写。
                    keyword.Extension = (item.Trim()).ToLower();
                }
            }

            return keyword;
        }
        #endregion

        #region ListStringClone
        /// <summary>
        /// 对List string做克隆
        /// </summary>
        /// <returns></returns>
        public static List<string> ListStringClone(List<string> source)
        {
            List<string> target = new List<string>();
            foreach (string item in source)
            {
                target.Add(item);
            }
            return target;
        }
        #endregion

        #region LimitStringLength
        private static Label lbl = new Label();
        private static Font tsslFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        /// <summary>
        /// 将字符串的像素长度限制在length内，超过的用...代替。
        /// </summary>
        public static string LimitStringLength(string str, int length)
        {
            if (str.Length <= 0)
                return str;

            int TextLength, Num;
            double FontWidth;
            TextLength = (int)lbl.CreateGraphics().MeasureString(str, tsslFont).Width;
            if (TextLength < length)
            {
                return str;
            }
            else
            {
                FontWidth = (TextLength * 1.0) / str.Length; //每个字符的宽度
                Num = (int)(length / FontWidth) - 3;
                Num = Num > 0 ? Num : 1;
                while (true)
                {
                    str = str.Substring(0, Num) + "...";
                    TextLength = (int)lbl.CreateGraphics().MeasureString(str, tsslFont).Width;
                    if (TextLength < length)
                        return str;
                    else
                    {
                        if (Num <= 3)
                            return str;
                        else
                            Num -= 3;
                    }
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace LuceneFileFinder.Type
{
    /// <summary>
    /// 索引数据库信息
    /// </summary>
    [Serializable]//可序列化
    public class IndexInfoDB
    {
        #region 成员变量

        public string FCreationTime = "";

        public string FUpdateTime = "";

        public int FFileNums = 0;

        public int FFolderNums = 0;

        public string MCreationTime = "";

        public string MUpdateTime = "";

        public int MFileNums = 0;

        public int MFolderNums = 0;

        public List<string> FileIndexPath = new List<string>();

        public List<string> MP3IndexPath = new List<string>();

        #region MP3索引数据库的艺术家和文件夹集合 
        public List<string> MP3Artist = new List<string>();
        public List<string> MP3Directorys = new List<string>();
        #endregion

        #endregion

        #region CopyTo
        /// <summary>
        /// 浅拷贝
        /// </summary>
        public void CopyTo(IndexInfoDB target)
        {
            target.FCreationTime = this.FCreationTime == null ? "" : this.FCreationTime;
            target.FUpdateTime = this.FUpdateTime == null ? "" : this.FUpdateTime;
            target.MCreationTime = this.MCreationTime == null ? "" : this.MCreationTime;
            target.MUpdateTime = this.MUpdateTime == null ? "" : this.MUpdateTime;
            target.FFileNums = this.FFileNums;
            target.FFolderNums = this.FFolderNums;
            target.MFileNums = this.MFileNums;
            target.MFolderNums = this.MFolderNums;
            target.FileIndexPath = this.FileIndexPath == null ? new List<string>() : this.FileIndexPath;
            target.MP3IndexPath = this.MP3IndexPath == null ? new List<string>() : this.MP3IndexPath;
            target.MP3Artist = this.MP3Artist == null ? new List<string>() : this.MP3Artist;
            target.MP3Directorys = this.MP3Directorys == null ? new List<string>() : this.MP3Directorys;
        }
        #endregion

        #region ReadIndexInfoDB
        /// <summary>
        /// 读索引信息
        /// </summary>
        public void ReadIndexInfoDB()
        {
            FileStream fs;
            BinaryFormatter bf = new BinaryFormatter();
            IndexInfoDB indexInfo = new IndexInfoDB();
         
            try
            {
                fs = new FileStream(Application.StartupPath + @"\IndexInfo.db", FileMode.OpenOrCreate);
                try
                {
                    indexInfo = (IndexInfoDB)bf.Deserialize(fs);
                }
                catch
                { }
                fs.Close();
            }
            catch
            { }

            indexInfo.CopyTo(this);
        }
        #endregion

        #region WriteIndexInfoDB
        /// <summary>
        /// 写索引信息
        /// </summary>
        public void WriteIndexInfoDB()
        {
            FileStream fs;
            BinaryFormatter bf = new BinaryFormatter();
            IndexInfoDB indexInfo = new IndexInfoDB();
            this.CopyTo(indexInfo);
            try
            {
                fs = new FileStream(Application.StartupPath + @"\IndexInfo.db", FileMode.Create);
                try
                {
                    bf.Serialize(fs, indexInfo);
                }
                catch
                { }
                fs.Close();
            }
            catch
            { }
        }
        #endregion
    }
}

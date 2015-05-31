using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace LuceneFileFinder.Type
{
    /// <summary>
    /// 设置文件的配置，供应用程序使用。
    /// </summary>
    [Serializable]//可序列化
    public class SettingsDB
    {
        #region 成员变量
        /// <summary>
        /// 文件索引包含路径
        /// </summary>
        public List<string> FileIndexInPath;

        /// <summary>
        /// 文件索引排除路径
        /// </summary>
        public List<string> FileIndexOutPath;

        /// <summary>
        /// MP3索引包含路径
        /// </summary>
        public List<string> MP3IndexInPath;

        /// <summary>
        /// MP3索引排除路径
        /// </summary>
        public List<string> MP3IndexOutPath;

        /// <summary>
        /// 要索引的文件路径
        /// </summary>
        public List<string> FileIndexPath;
        /// <summary>
        /// 要索引的MP3路径
        /// </summary>
        public List<string> MP3IndexPath;
        #endregion

        #region CopyTo
        private void CopyTo(SettingsDB target)
        {
            //直接引用，但此处不产生影响。
            target.FileIndexInPath = this.FileIndexInPath == null ? new List<string>() : this.FileIndexInPath;
            target.FileIndexOutPath = this.FileIndexOutPath == null ? new List<string>() : this.FileIndexOutPath;
            target.MP3IndexInPath = this.MP3IndexInPath == null ? new List<string>() : this.MP3IndexInPath;
            target.MP3IndexOutPath = this.MP3IndexOutPath == null ? new List<string>() : this.MP3IndexOutPath;
            target.FileIndexPath = this.FileIndexPath == null ? new List<string>() : this.FileIndexPath;
            target.MP3IndexPath = this.MP3IndexPath == null ? new List<string>() : this.MP3IndexPath;
        }
        #endregion

        #region ReadSettings
        /// <summary>
        /// 读设置
        /// </summary>
        public void ReadSettings()
        {
            FileStream fs;
            BinaryFormatter bf = new BinaryFormatter();

            SettingsDB settings = new SettingsDB();
            try
            {
                fs = new FileStream(Application.StartupPath + @"\Settings.db", FileMode.OpenOrCreate);
                try
                {
                    settings = (SettingsDB)bf.Deserialize(fs);
                }
                catch
                { }
                fs.Close();
            }
            catch
            { }

            settings.CopyTo(this);
        }
        #endregion

        #region WriteSettings
        /// <summary>
        /// 写设置
        /// </summary>
        public void WriteSettings()
        {
            FileStream fs;
            BinaryFormatter bf = new BinaryFormatter();
            SettingsDB settings = new SettingsDB();
            this.CopyTo(settings);
            try
            {
                fs = new FileStream(Application.StartupPath + @"\Settings.db", FileMode.Create);
                try
                {
                    bf.Serialize(fs, settings);
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

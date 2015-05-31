using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LuceneFileFinder.Type;
using System.Text;

namespace LuceneFileFinder.Util
{
    /// <summary>
    /// 根据扩展名获取文件属性，用于在魔镜中显示。
    /// </summary>
    public class GetProperty
    {
        #region GetFileProperty
        /// <summary>
        /// 获取文件属性
        /// </summary>
        /// <param name="fullName">文件全名</param>
        /// <returns>文件属性</returns>
        public static FileProperties GetFileProperty(string fullName)
        {
            FileProperties fpro = new FileProperties();
            try
            {
                FileInfo fileInfo = new FileInfo(fullName);
                fpro.Attributies = fileInfo.Attributes.ToString();
                fpro.CreationTime = fileInfo.CreationTime.ToString();
                fpro.Extension = fileInfo.Extension;
                fpro.LastAccessTime = fileInfo.LastAccessTime.ToString();
                fpro.LastWriteTime = fileInfo.LastWriteTime.ToString();
                fpro.Length = Deal.FormatFileLength(fileInfo.Length);
                fpro.Name = fileInfo.Name;
            }
            catch
            {
                fpro.Attributies = "";
                fpro.CreationTime = "";
                fpro.Extension = "";
                fpro.LastAccessTime = "";
                fpro.LastWriteTime = "";
                fpro.Length = "";
                fpro.Name = "";
            }
            return fpro;
        }
        #endregion

        #region GetDirectoryProperty
        /// <summary>
        /// 获取文件夹属性，信息还是FileProperties里的东西。
        /// </summary>
        /// <param name="fullName">文件全名</param>
        /// <returns>文件夹属性</returns>
        public static FileProperties GetDirectoryProperty(string fullName)
        {
            FileProperties fpro = new FileProperties();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullName);
                fpro.Attributies = dirInfo.Attributes.ToString();
                fpro.CreationTime = dirInfo.CreationTime.ToString();
                fpro.Extension = dirInfo.Extension;
                fpro.LastAccessTime = dirInfo.LastAccessTime.ToString();
                fpro.LastWriteTime = dirInfo.LastWriteTime.ToString();
                fpro.Length = "";
                fpro.Name = dirInfo.Name;
            }
            catch
            {
                fpro.Attributies = "";
                fpro.CreationTime = "";
                fpro.Extension = "";
                fpro.LastAccessTime = "";
                fpro.LastWriteTime = "";
                fpro.Length = "";
                fpro.Name = "";
            }
            return fpro;
        }
        #endregion

        #region GetMP3Tag
        /// <summary>
        /// 获取MP3Tag信息
        /// </summary>
        /// <param name="fullName">文件全名</param>
        /// <returns>MP3Tag</returns>
        public static MP3Tag GetMP3Tag(string fullName)
        {
            MP3Tag id3tag = new MP3Tag();
            try
            {
                TagLib.File file = TagLib.File.Create(fullName);
                id3tag.Album = file.Tag.Album;
                id3tag.Artist = file.Tag.FirstPerformer;
                id3tag.Comment = file.Tag.Comment;
                id3tag.Composer = file.Tag.FirstComposer;
                id3tag.Date = file.Tag.Year.ToString();
                id3tag.Genre = file.Tag.FirstGenre;
                id3tag.Title = file.Tag.Title;
                id3tag.TrackNumber = file.Tag.Track.ToString();
                id3tag.Length = file.Properties.Duration.Minutes.ToString() + ":" + 
                    file.Properties.Duration.Seconds.ToString();
                id3tag.Kbps = file.Properties.AudioBitrate.ToString() + "Kbps";
            }
            catch { }

            return id3tag;
        }
        #endregion

        #region GetJPGExif

        #region GetJPGExif
        /// <summary>
        /// 获取JPGExif信息
        /// </summary>
        /// <param name="fullName">文件全名</param>
        /// <returns>JPGExif</returns>
        public static JPGExif GetJPGExif(string fullName)
        {
            JPGExif info = new JPGExif();
            Image img;

            try
            {
                img = Image.FromFile(fullName); //建立新的图像
            }
            catch
            {
                img = null;
                return info;
            }
            PropertyItem[] pt = img.PropertyItems;//获取图片中的exif 信息 ，此时该信息是数组
            info.PicPreview = img;
            info.Width = img.Width;
            info.Height = img.Height;
            for (int i = 0; i < pt.Length; i++)
            {
                PropertyItem p = pt[i];
                switch (pt[i].Id)
                {
                    // 设备制造商  
                    case 0x010F:
                        info.Maker = System.Text.ASCIIEncoding.ASCII.GetString(p.Value);
                        break;
                    // 设备型号               
                    case 0x0110:
                        info.Model = GetValueOfType2(p.Value);
                        break;
                    // 拍照时间                
                    case 0x0132:
                        info.ShootingTime = GetValueOfType2(p.Value);
                        break;
                    // 曝光时间                
                    case 0x829A:
                        info.ExposureTime = GetValueOfType5(p.Value);
                        break;
                    // ISO                 
                    case 0x8827:
                        info.ISO = GetValueOfType3(p.Value);
                        break;
                    //相片的焦距
                    case 0x920a:
                        info.FocalLength = GetValueOfType5A(p.Value);
                        break;
                    //相片的光圈值
                    case 0x829D:
                        info.F_Number = GetValueOfType5A(p.Value);
                        break;
                    //闪光灯
                    case 0x9209:
                        info.FlashLamp = GetValueOfType3(p.Value);
                        break;
                    default:
                        break;
                }
            }
            return info;
        }
        #endregion

        #region 获取EXIF信息的函数
        /// <summary>
        /// 将exif信息转化为能够读懂的信息的转换函数
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static string GetValueOfType2(byte[] b)
        {
            return System.Text.Encoding.ASCII.GetString(b);
        }

        private static string GetValueOfType3(byte[] b)
        {
            if (b.Length != 2) return "  ";
            return Convert.ToUInt16(b[1] << 8 | b[0]).ToString();
        }

        private static string GetValueOfType4(byte[] b)
        {
            if (b.Length != 4) return "   ";
            return Convert.ToUInt32(b[3] << 24 | b[2] << 16 | b[1] << 8 | b[0]).ToString();
        }

        private static string GetValueOfType5(byte[] b)
        {
            if (b.Length != 8) return "  ";
            UInt32 fm, fz;
            fm = 0;
            fz = 0;
            fz = Convert.ToUInt32(b[7] << 24 | b[6] << 16 | b[5] << 8 | b[4]);
            fm = Convert.ToUInt32(b[3] << 24 | b[2] << 16 | b[1] << 8 | b[0]);
            return fm.ToString() + "/" + fz.ToString();
        }

        private static string GetValueOfType5A(byte[] b)//获取光圈的值
        {
            if (b.Length != 8) return " ";
            UInt32 fm, fz;
            fm = 0;
            fz = 0;
            fz = Convert.ToUInt32(b[7] << 24 | b[6] << 16 | b[5] << 8 | b[4]);
            fm = Convert.ToUInt32(b[3] << 24 | b[2] << 16 | b[1] << 8 | b[0]);
            double temp = (double)fm / fz;
            return (temp).ToString();
        }
        #endregion

        #endregion
    }
}

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LuceneFileFinder.Util
{
    /// <summary>
    /// 打开文件所在的文件夹，并选中该项。
    /// </summary>
    public class DirectoryOpener
    {
        #region API
        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern int SHOpenFolderAndSelectItems(
            IntPtr pidlFolder,
            uint cidl,
            [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
            uint dwFlags);

        [DllImport("ole32.dll", ExactSpelling = true)]
        public static extern int CoCreateInstance(
            [In] ref Guid rclsid,
            IntPtr pUnkOuter,
            CLSCTX dwClsContext,
            [In] ref Guid riid,
            [Out] out IntPtr ppv);

        public enum CLSCTX : uint
        {
            INPROC_SERVER = 0x1
        }

        static Guid CLSID_ShellLink = new Guid("00021401-0000-0000-C000-000000000046");
        static Guid IID_IShellLink = new Guid("000214F9-0000-0000-C000-000000000046");

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        public interface IShellLinkW
        {
            [PreserveSig]
            int GetPath(StringBuilder pszFile, int cch, [In, Out] ref WIN32_FIND_DATAW pfd, uint fFlags);

            [PreserveSig]
            int GetIDList([Out] out IntPtr ppidl);

            [PreserveSig]
            int SetIDList([In] ref IntPtr pidl);

            [PreserveSig]
            int GetDescription(StringBuilder pszName, int cch);

            [PreserveSig]
            int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [PreserveSig]
            int GetWorkingDirectory(StringBuilder pszDir, int cch);

            [PreserveSig]
            int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            [PreserveSig]
            int GetArguments(StringBuilder pszArgs, int cch);

            [PreserveSig]
            int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            [PreserveSig]
            int GetHotkey([Out] out ushort pwHotkey);

            [PreserveSig]
            int SetHotkey(ushort wHotkey);

            [PreserveSig]
            int GetShowCmd([Out] out int piShowCmd);

            [PreserveSig]
            int SetShowCmd(int iShowCmd);

            [PreserveSig]
            int GetIconLocation(StringBuilder pszIconPath, int cch, [Out] out int piIcon);

            [PreserveSig]
            int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            [PreserveSig]
            int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

            [PreserveSig]
            int Resolve(IntPtr hwnd, uint fFlags);

            [PreserveSig]
            int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), BestFitMapping(false)]
        public struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }
        #endregion

        #region OpenDirectory
        public static void OpenDirectory(string path)
        {
            IntPtr ppsl = IntPtr.Zero;
            int result = CoCreateInstance(
            ref CLSID_ShellLink,
            IntPtr.Zero,
            CLSCTX.INPROC_SERVER,
            ref IID_IShellLink,
            out ppsl);

            IShellLinkW psl = Marshal.GetObjectForIUnknown(ppsl) as IShellLinkW;
            psl.SetPath(path);

            IntPtr pidl = IntPtr.Zero;
            psl.GetIDList(out pidl);

            SHOpenFolderAndSelectItems(pidl, 0, null, 0);

            //清理内存和释放对象
            Marshal.FreeCoTaskMem(pidl);
            Marshal.Release(ppsl);
        }
        #endregion
    }
}

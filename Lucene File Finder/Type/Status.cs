
namespace LuceneFileFinder.Type
{
    #region ModeStatus
    public enum ModeStatus
    {
        /// <summary>
        /// 文件
        /// </summary>
        FILE,
        /// <summary>
        /// MP3音乐
        /// </summary>
        MP3,
        /// <summary>
        /// JPG图片
        /// </summary>
        JPG,
        /// <summary>
        /// 纯文本
        /// </summary>
        TXT
    }
    #endregion

    #region MagicMirrorStatus
    public enum MagicMirrorStatus
    {
        /// <summary>
        /// 魔镜处于关闭状态
        /// </summary>
        Close,
        /// <summary>
        /// 魔镜处于开启状态
        /// </summary>
        Open
    }
    #endregion

    #region ListViewStatus
    /// <summary>
    /// ListView列的显示的状态
    /// </summary>
    public enum ListViewStatus
    {
        /// <summary>
        /// 文件模式
        /// </summary>
        File,
        /// <summary>
        /// MP3模式
        /// </summary>
        MP3,
        /// <summary>
        /// 艺术家模式
        /// </summary>
        MP3Artist
    }
    #endregion

    #region KeyStatus
    /// <summary>
    /// 按键状态是否按下Ctrl，Alt。
    /// </summary>
    public enum KeyStatus
    {
        /// <summary>
        /// 按下了Ctrl键
        /// </summary>
        Ctrl,
        /// <summary>
        /// 按下了Alt键
        /// </summary>
        Alt,
        /// <summary>
        /// 没有按下Ctrl，Alt键
        /// </summary>
        None
    }
    #endregion

    //#region MagicPanelStatus
    ///// <summary>
    ///// 魔镜面板上控件的状态
    ///// </summary>
    //public enum MagicPanelStatus
    //{
    //    /// <summary>
    //    /// 无
    //    /// </summary>
    //    None,
    //    /// <summary>
    //    /// 文件
    //    /// </summary>
    //    File,
    //    /// <summary>
    //    /// JPG图片
    //    /// </summary>
    //    JPG,
    //    /// <summary>
    //    /// MP3音乐
    //    /// </summary>
    //    MP3
    //}
    //#endregion
}

using System.Windows.Forms;

namespace LuceneFileFinder.Control
{
    public class DoubleBufferListView : ListView
    {
        public DoubleBufferListView()
        {
            //启用双缓存,避免ListView控件加载数据时闪烁
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
    }
}

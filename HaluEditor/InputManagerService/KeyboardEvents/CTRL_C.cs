using System.Text;
using System.Windows;

namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_CTRL_C()
        {
            if (HasSelection)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = (int)SelectionFrom.Value.Y; i <= (int)SelectionTo.Value.Y; i++)
                {
                    int tmpMax = (SelectionFrom.Value.Y == SelectionTo.Value.Y || SelectionTo.Value.Y == i) ?
                            (int)SelectionTo.Value.X : Parent._Document[i].Count;
                    for (int j = ((int)SelectionFrom.Value.Y == i) ? (int)SelectionFrom.Value.X : 0;
                        j < tmpMax;
                        j++)
                    {
                        sb.Append(Parent._Document[i][j]);
                    }
                }

                Clipboard.SetText(sb.ToString(), TextDataFormat.UnicodeText);
            }
        }
    }
}
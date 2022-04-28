using System.Windows;

namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_CTRL_V()
        {
            DeleteSelectedText();

            string text = Clipboard.GetText(TextDataFormat.UnicodeText);
            InputTextInternal(text);
        }

        private void SplitRowAtCaretIfNecessary(char lastchar)
        {
            //TODO handle last chars \r\n
            if (Parent._Document[caretRenderer.Top].Count != Parent._Document[caretRenderer.Top].End && (lastchar.Equals('\r') || lastchar.Equals('\n')))
            {
                if (caretRenderer.Left == Parent._Document[caretRenderer.Top].End)
                {
                    Parent._Document.SplitRow(caretRenderer.Top, Parent._Document[caretRenderer.Top].Count);
                    caretRenderer.Top++;
                    caretRenderer.Left = 0;
                }
                else
                {
                    Parent._Document.SplitRow(caretRenderer.Top, caretRenderer.Left);
                    caretRenderer.Top++;
                    caretRenderer.Left = 0;
                }
            }
        }
    }
}
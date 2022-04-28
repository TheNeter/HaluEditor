namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Backspace()
        {
            if (!DeleteSelectedText())
            {
                if (caretRenderer.Left > 0)
                {
                    Parent._Document[caretRenderer.Top].RemoveLetter(caretRenderer.Left - 1);
                    caretRenderer.Left -= 1;
                }
                else
                {
                    if (caretRenderer.Top > 0)
                    {
                        if (Parent._Document[caretRenderer.Top].End == 0)
                        {
                            Parent._Document.RemoveRow(caretRenderer.Top);
                            caretRenderer.Top -= 1;
                            caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
                            if (Parent._Document[caretRenderer.Top + 1] == null)
                            {
                                Parent._Document[caretRenderer.Top].RemoveLineEnding();
                            }
                        }
                        else
                        {
                            int cursorLeft = Parent._Document[caretRenderer.Top - 1].End;
                            Parent._Document.MergeRows(caretRenderer.Top, caretRenderer.Top - 1);
                            caretRenderer.Top -= 1;
                            caretRenderer.Left = cursorLeft;
                        }
                    }
                    else if (!Parent._Document[caretRenderer.Top].RemoveLetter(caretRenderer.Left).Equals('\0'))
                    {
                        caretRenderer.Left -= 1;
                    }
                }
            }

            Parent.SetFocusToCursorPosition();
        }
    }
}
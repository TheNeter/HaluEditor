namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Delete()
        {
            if (!DeleteSelectedText())
            {
                if (Parent._Document[caretRenderer.Top].End > caretRenderer.Left)
                {
                    Parent._Document[caretRenderer.Top].RemoveLetter(caretRenderer.Left);
                }
                else
                {
                    Parent._Document.MergeRows(caretRenderer.Top + 1, caretRenderer.Top);
                }
            }

            Parent.SetFocusToCursorPosition();
        }
    }
}
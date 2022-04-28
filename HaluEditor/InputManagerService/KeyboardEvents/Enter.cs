namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Enter()
        {
            DeleteSelectedText();
            if (Parent._Document[caretRenderer.Top].Count == caretRenderer.Left)
            {
                Parent._Document[caretRenderer.Top].AddLetter('\r');
                Parent._Document.InsertRow(caretRenderer.Top + 1);
            }
            else
            {
                Parent._Document.SplitRow(caretRenderer.Top, caretRenderer.Left);
                Parent._Document[caretRenderer.Top].AddLetter('\r');
            }
            caretRenderer.Top += 1;
            caretRenderer.Left = 0;
            Parent.SetFocusToCursorPosition();
        }
    }
}
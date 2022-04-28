namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Tab()
        {
            DeleteSelectedText();
            Parent._Document[caretRenderer.Top].InsertLetter(caretRenderer.Left, '\t');
            caretRenderer.Left++;

            Parent.SetFocusToCursorPosition();
        }
    }
}
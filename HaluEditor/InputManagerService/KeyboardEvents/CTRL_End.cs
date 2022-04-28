namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_CTRL_End()
        {
            SetSelectionNull();
            caretRenderer.Top = Parent._Document.Count;
            caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
            Parent.SetFocusToCursorPosition();
        }
    }
}
namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_End()
        {
            SetSelectionNull();
            caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
            Parent.SetFocusToCursorPosition();
        }
    }
}
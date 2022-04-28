namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Position1()
        {
            SetSelectionNull();
            caretRenderer.Left = 0;
            Parent.SetFocusToCursorPosition();
        }
    }
}
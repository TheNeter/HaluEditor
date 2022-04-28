namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_CTRL_A()
        {
            caretRenderer.Top = 0;
            caretRenderer.Left = 0;
            SetSelectionFromCursorPos();

            caretRenderer.Top = Parent._Document.Count;
            caretRenderer.Left = Parent._Document[caretRenderer.Top].Count;
            SetSelectionToCursorPos();
        }
    }
}
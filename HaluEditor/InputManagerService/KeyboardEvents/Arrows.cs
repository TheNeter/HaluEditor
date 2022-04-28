namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Down()
        {
            SetSelectionNull();
            //TODO _rememberCaretLeft
            if (Parent._Document[caretRenderer.Top + 1] != null)
            {
                caretRenderer.Top += 1;
                if (caretRenderer.Left > Parent._Document[caretRenderer.Top].End)
                {
                    caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
                }
            }
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Left()
        {
            SetSelectionNull();
            if (caretRenderer.Left == 0)
            {
                if (caretRenderer.Top > 0)
                {
                    caretRenderer.Top -= 1;
                    caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
                }
            }
            else
            {
                caretRenderer.Left -= 1;
            }
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Right()
        {
            SetSelectionNull();
            if (caretRenderer.Left < Parent._Document[caretRenderer.Top].End)
            {
                caretRenderer.Left += 1;
            }
            else if (caretRenderer.Top < Parent._Document.Count)
            {
                caretRenderer.Top += 1;
                caretRenderer.Left = 0;
            }
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Up()
        {
            SetSelectionNull();
            if (caretRenderer.Top > 0)
            {
                caretRenderer.Top -= 1;
                if (caretRenderer.Left > Parent._Document[caretRenderer.Top].End)
                {
                    caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
                }
            }
            Parent.SetFocusToCursorPosition();
        }
    }
}
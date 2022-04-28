using System;

namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private int _rememberCaretLeft = -1;

        private void OnKeyDown_Shift_Down()
        {
            if (_rememberCaretLeft == -1)
            {
                _rememberCaretLeft = caretRenderer.Left;
                SetSelectionFromCursorPos();
            }
            if (caretRenderer.Top == Parent._Document.Count - 1)
            {
                caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
                SetSelectionToCursorPos();
                return;
            }

            caretRenderer.Top++;
            caretRenderer.Left = Math.Min(_rememberCaretLeft, Parent._Document[caretRenderer.Top].End);
            SetSelectionToCursorPos();
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Shift_Left()
        {
            if (_rememberCaretLeft == -1)
            {
                _rememberCaretLeft = caretRenderer.Left;
                SetSelectionFromCursorPos();
            }

            if (caretRenderer.Left == 0)
            {
                caretRenderer.Top--;
                caretRenderer.Left = Parent._Document[caretRenderer.Top].End;
            }
            else
            {
                caretRenderer.Left--;
            }
            SetSelectionToCursorPos();
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Shift_Right()
        {
            if (_rememberCaretLeft == -1)
            {
                _rememberCaretLeft = caretRenderer.Left;
                SetSelectionFromCursorPos();
            }

            if (caretRenderer.Left == Parent._Document[caretRenderer.Top].End)
            {
                caretRenderer.Top++;
                caretRenderer.Left = 0;
            }
            else
            {
                caretRenderer.Left++;
            }
            SetSelectionToCursorPos();
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_Shift_Up()
        {
            if (_rememberCaretLeft == -1)
            {
                _rememberCaretLeft = caretRenderer.Left;
                SetSelectionFromCursorPos();
            }
            if (caretRenderer.Top == 0)
            {
                caretRenderer.Left = 0;
                SetSelectionToCursorPos();
                return;
            }

            if (caretRenderer.Top > 0)
            {
                caretRenderer.Top--;
                caretRenderer.Left = Math.Min(_rememberCaretLeft, Parent._Document[caretRenderer.Top].End);
                SetSelectionToCursorPos();
            }
            Parent.SetFocusToCursorPosition();
        }
    }
}
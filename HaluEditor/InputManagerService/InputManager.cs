using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private CaretRenderer.CaretRenderer caretRenderer;
        private ConfigManager.ConfigManager config;
        private ConcurrentQueue<Action> inputqueue;
        private Dictionary<ModifierKeys, Dictionary<Key, Action>> keyDownHandlesSystem;
        private Point? selectionFrom;
        private Point? selectionTo;

        public InputManagerService()
        {
            inputqueue = new ConcurrentQueue<Action>();
            InitKeyHandles();
        }

        public bool HasSelection
        {
            get
            {
                return SelectionFrom != null && SelectionTo != null;
            }
        }

        public ServiceHost Host { get; set; }

        public bool IsSelecting { get; private set; }
        public HaluEditorControl Parent { get; set; }

        public bool SelectionDirectionIsReverse
        {
            get
            {
                if (selectionTo.HasValue && selectionFrom.HasValue)
                    return (selectionFrom.Value.X > selectionTo.Value.X && selectionFrom.Value.Y == selectionTo.Value.Y) ||
                        (selectionFrom.Value.Y > selectionTo.Value.Y);
                else
                    return false;
            }
        }

        public Point? SelectionFrom
        {
            get
            {
                if (selectionFrom == null)
                    return null;
                if (SelectionDirectionIsReverse)
                {
                    return selectionTo;
                }
                else
                {
                    return selectionFrom;
                }
            }
            set =>
                selectionFrom = value;
        }

        public Point? SelectionTo
        {
            get
            {
                if (selectionTo == null)
                    return null;
                if (SelectionDirectionIsReverse)
                {
                    return selectionFrom;
                }
                else
                {
                    return selectionTo;
                }
            }
            set => selectionTo = value;
        }

        public bool DeleteSelectedText()
        {
            bool retVal = false;
            if (HasSelection)
            {
                Point tmpSelectionFrom = SelectionFrom.Value;
                Point tmpSelectionTo = SelectionTo.Value;
                if (((int)tmpSelectionTo.Y - (int)tmpSelectionFrom.Y >= 2))
                {
                    Parent._Document.Rows.RemoveRange((int)tmpSelectionFrom.Y + 1, ((int)tmpSelectionTo.Y - (int)tmpSelectionFrom.Y) - 1);
                    tmpSelectionTo = new Point(tmpSelectionTo.X, tmpSelectionFrom.Y + 1);
                }
                if ((int)tmpSelectionTo.Y - (int)tmpSelectionFrom.Y == 1)
                {
                    int lengthFromRow = Parent._Document.Rows[(int)tmpSelectionFrom.Y].End;
                    int lngToRow = (int)tmpSelectionTo.X;
                    Parent._Document.MergeRows((int)tmpSelectionTo.Y, (int)tmpSelectionFrom.Y);
                    tmpSelectionTo = new Point(lengthFromRow + lngToRow, tmpSelectionFrom.Y);
                }
                if ((int)tmpSelectionTo.Y - (int)tmpSelectionFrom.Y == 0)
                {
                    if (Parent._Document.Rows[(int)tmpSelectionTo.Y].End <= (int)tmpSelectionTo.X)
                    {
                        Parent._Document.Rows.RemoveAt((int)tmpSelectionFrom.Y);
                    }
                    else
                    {
                        int delTo = (int)tmpSelectionTo.X + 1;
                        Parent._Document.Rows[(int)tmpSelectionFrom.Y].Letters.RemoveRange((int)tmpSelectionFrom.X, delTo - (int)tmpSelectionFrom.X);
                    }
                }
                caretRenderer.Top = (int)tmpSelectionFrom.Y;
                caretRenderer.Left = (int)tmpSelectionFrom.X;
                SetSelectionNull();
                retVal = true;
            }
            return retVal;
        }

        public void Enqueue(Action action)
        {
            inputqueue.Enqueue(action);
        }

        public void InputText(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.FirstOrDefault().Equals('\0'))
            {
                inputqueue.Enqueue(() =>
                {
                    DeleteSelectedText();
                    Parent._Document[caretRenderer.Top].InsertLetter(caretRenderer.Left, e.Text[0]);
                    caretRenderer.Left += 1;
                    Parent.SetFocusToCursorPosition();
                });
            }
        }

        public void InputText(string text)
        {
            Enqueue(() => InputTextInternal(text));
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = TryEnqueueKeyHandle(keyDownHandlesSystem, e);
        }

        public void LoadingDoneEvent()
        {
            caretRenderer = (CaretRenderer.CaretRenderer)Host.GetServiceFromType<CaretRenderer.CaretRenderer>();
            config = (ConfigManager.ConfigManager)Host.GetServiceFromType<ConfigManager.ConfigManager>();

            InitQueueHandler();
        }

        public void SetSelectionFromCursorPos()
        {
            SelectionFrom = new Point(caretRenderer.Left, caretRenderer.Top);
        }

        public void SetSelectionNull()
        {
            SelectionTo = null;
            SelectionFrom = null;
            _rememberCaretLeft = -1;
        }

        public void SetSelectionToCursorPos()
        {
            SelectionTo = new Point(caretRenderer.Left, caretRenderer.Top);
        }

        internal void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            inputqueue.Enqueue(() =>
            {
                Point rawClickPosition = e.GetPosition(Parent);
                if (e.ClickCount == 1)
                {
                    Parent.CalculateCaretPosition(rawClickPosition);
                    SetSelectionNull();
                    IsSelecting = true;
                    SetSelectionFromCursorPos();
                }
                if (e.ClickCount == 2)
                {
                    //Group words, whitespaces and special keys in separate selections
                    Regex r = new Regex(@"([\wÄÖÜäöüß-]+)|(\s+)|([\.;\?!§%&/()=#\[\]\\{}\u0022]+)|(\$)");
                    var matches = r.Matches(Parent._Document[caretRenderer.Top].Text);
                    foreach (Match match in matches)
                    {
                        var lx = match.Index;
                        var llength = match.Index + match.Value.Length - 1;
                        if (caretRenderer.Left >= lx && caretRenderer.Left <= llength)
                        {
                            SelectionFrom = new Point(lx, caretRenderer.Top);
                            caretRenderer.Left = llength;
                            SetSelectionToCursorPos();
                            break;
                        }
                    }
                }
                else if (e.ClickCount == 3)
                {
                    caretRenderer.Left = 0;
                    SetSelectionToCursorPos();
                    SelectionFrom = new Point(Parent._Document[caretRenderer.Top].Count, caretRenderer.Top);
                }
            });
        }

        internal void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            inputqueue.Enqueue(() =>
            {
                if (IsSelecting)
                {
                    Point rawClickPosition = e.GetPosition(Parent);
                    Parent.CalculateCaretPosition(rawClickPosition);
                    SetSelectionToCursorPos();
                    if (SelectionFrom.Value.X == SelectionTo.Value.X && SelectionFrom.Value.Y == SelectionTo.Value.Y)
                    {
                        SetSelectionNull();
                    }
                    IsSelecting = false;
                }
            });
        }

        /// <summary>
        /// line 1 line 2 line 3
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e">      </param>
        internal void MouseMove(object sender, MouseEventArgs e)
        {
            inputqueue.Enqueue(() =>
            {
                if (IsSelecting)
                {
                    Parent.CalculateCaretPosition(e.GetPosition((UIElement)sender));
                    SetSelectionToCursorPos();
                }
            });
        }

        private void AddRowToDocumentIfNecessary()
        {
            if (Parent._Document.AddRowToDocumentIfNecessary())
            {
                caretRenderer.Top++;
                caretRenderer.Left = 0;
            }
        }

        private void InitKeyHandles()
        {
            keyDownHandlesSystem = new Dictionary<ModifierKeys, Dictionary<Key, Action>>()
            {
                {
                    ModifierKeys.None, new Dictionary<Key, Action>()
                    {
                        { Key.Enter, OnKeyDown_Enter },
                        { Key.Back, OnKeyDown_Backspace},
                        { Key.Delete, OnKeyDown_Delete },
                        { Key.Left, OnKeyDown_Left },
                        { Key.Right, OnKeyDown_Right },
                        { Key.Up, OnKeyDown_Up },
                        { Key.Down, OnKeyDown_Down },
                        { Key.End, OnKeyDown_End },
                        { Key.Home, OnKeyDown_Position1 },
                        { Key.Tab, OnKeyDown_Tab },
                        { Key.Escape, OnKeyDown_Esc }
                    }
                },
                {
                    ModifierKeys.Shift, new Dictionary<Key, Action>()
                    {
                        { Key.Left, OnKeyDown_Shift_Left },
                        { Key.Right, OnKeyDown_Shift_Right },
                        { Key.Up, OnKeyDown_Shift_Up },
                        { Key.Down, OnKeyDown_Shift_Down },
                        { Key.Enter, OnKeyDown_Enter },
                    }
                },
                {
                    ModifierKeys.Control, new Dictionary<Key, Action>()
                    {
                        { Key.A, OnKeyDown_CTRL_A },
                        { Key.C, OnKeyDown_CTRL_C },
                        { Key.V, OnKeyDown_CTRL_V },
                        { Key.Home, OnKeyDown_CTRL_Position1 },
                        { Key.End, OnKeyDown_CTRL_End },
                        { Key.D0, OnKeyDown_CTRL_Zero },
                        { Key.Space, OnKeyDown_ShowAutocompleteWindow }
                    }
                }
            };
        }

        private void InitQueueHandler()
        {
            Task.Run(() => //TODO Exception beheben -> Beim schließen des Programms
            {
                bool inputHandled = false;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Thread.Sleep(1);
                    Parent.Dispatcher.Invoke(() =>
                    {
                        while (!inputqueue.IsEmpty)
                        {
                            inputqueue.TryDequeue(out Action action);
                            action.Invoke();
                            inputHandled = true;
                        }
                    }, System.Windows.Threading.DispatcherPriority.Render);

                    //render on change with 60 FPS
                    if (inputHandled && sw.ElapsedMilliseconds > 16)
                    {
                        Parent.Dispatcher.Invoke(() =>
                        {
                            if (inputHandled)
                            {
                                inputHandled = false;
                                Parent.InputHandledInfoForCleanUp = true;
                                Parent.InvalidateVisual();
                            }
                        }, System.Windows.Threading.DispatcherPriority.Render);
                        sw.Restart();
                    }
                }
            });
        }

        private void InputTextInternal(string text)
        {
            char lastchar = '\0';
            foreach (char c in text)
            {
                SplitRowAtCaretIfNecessary(lastchar);
                Parent._Document[caretRenderer.Top].InsertLetter(caretRenderer.Left, c);
                caretRenderer.Left++;

                lastchar = c;
            }
            SplitRowAtCaretIfNecessary(lastchar);
            AddRowToDocumentIfNecessary();
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_CTRL_Position1()
        {
            SetSelectionNull();
            caretRenderer.Top = 0;
            caretRenderer.Left = 0;
            Parent.SetFocusToCursorPosition();
        }

        private void OnKeyDown_CTRL_Zero()
        {
            Parent.SetScale(1);
        }

        private void OnKeyDown_ShowAutocompleteWindow()
        {
            Parent.SetAutocompleteVisibility(Visibility.Visible);
        }

        private bool TryEnqueueKeyHandle(Dictionary<ModifierKeys, Dictionary<Key, Action>> handles, KeyEventArgs e)
        {
            bool handled = false;
            if (handles.TryGetValue(e.KeyboardDevice.Modifiers, out Dictionary<Key, Action> subHandle) &&
                subHandle.TryGetValue(e.Key, out Action action))
            {
                inputqueue.Enqueue(action);
                handled = true;
            }

            return handled;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ngprojects.HaluEditor.Document;
using ngprojects.HaluEditor.Lexer;

namespace ngprojects.HaluEditor
{
    //FIXME es kann keine einzelne Selektion stattfinden. Es ist immer ein Multiselect
    //FIXME copy kopiert einen buchstaben zu wenig
    //TODO Popup-Window für Vervollständigung (von außen wie Lexer beeinflussbar)
    //FIXME Lexer mit aktueller Methodik wieder einbauen (onRender?)
    //TODO Version 2 Highlight für Cursor-Gegenpart (Klammern parsen) (tree-parsing)
    //TODO Version 3 Einrückung mit gestrichelter Linie darstellen (syntax-prüfung muss erfolgt sein)

    public delegate void LoadingDone();

    public delegate void RenderInvoked(DrawingContext dc);

    public partial class HaluEditorControl : Canvas
    {
        internal HaluDocument _Document;
        private const double maxScaleFactor = 4.0;
        private const double minScaleFactor = 0.25;
        private Point? _lexerErrorAsPositionPoint = null;
        private Point? _lexerErrorAsPositionPointRendered = null;
        private int _scrollOffsetX;
        private int _scrollOffsetY;
        private HaluEditorAutocomplete autocompleteWindow;
        private HaluEditorAutocompleteVM autocompleteWindowVM;
        private CaretRenderer.CaretRenderer caret;
        private ConfigManager.ConfigManager Config;
        private ServiceHost host;
        private ScaleTransform scaleTrans;

        public HaluEditorControl()
        {
            _dpi = VisualTreeHelper.GetDpi(this);
            this.Focusable = true;
            MouseDown += HaluEditorControl_MouseDown;
            host = new ServiceHost();
            host.Create(this);
            caret = (CaretRenderer.CaretRenderer)host.GetServiceFromType<CaretRenderer.CaretRenderer>();
            Input = (InputManagerService.InputManagerService)host.GetServiceFromType<InputManagerService.InputManagerService>();
            Config = (ConfigManager.ConfigManager)host.GetServiceFromType<ConfigManager.ConfigManager>();
            FontSize = Config.Configuration.FontSize;
            _Document = new HaluDocument();
            scaleTrans = new ScaleTransform();
            this.LayoutTransform = scaleTrans;
            this.Background = Config.Configuration.EditorColor;
            TextInput += HaluEditorControl_TextInput;
            KeyDown += HaluEditorControl_KeyDown;
            MouseLeftButtonDown += HaluEditorControl_MouseLeftButtonDown;
            MouseLeftButtonUp += HaluEditorControl_MouseLeftButtonUp;
            MouseWheel += HaluEditorControl_MouseWheel;
            MouseMove += HaluEditorControl_MouseMove;
            this.Cursor = Cursors.IBeam;
            InitDocumentCleanUpTask();
            autocompleteWindowVM = new HaluEditorAutocompleteVM()
            {
                ForegroundReturn = Config.Configuration.LineNumberTextColor
            };
            autocompleteWindow = new HaluEditorAutocomplete()
            {
                MaxHeight = 200,
                Background = Config.Configuration.EditorColor,
                Foreground = Config.Configuration.TextColor,
                Visibility = Visibility.Hidden,
                DataContext = autocompleteWindowVM
            };
            autocompleteWindowVM.ValueSelected += Autocomplete_ValueSelected;
            Children.Add(autocompleteWindow);
        }

        public Dictionary<TokenType, Func<string, SolidColorBrush>> HightlightMap { get; set; } = new Dictionary<TokenType, Func<string, SolidColorBrush>>();

        public InputManagerService.InputManagerService Input { get; private set; }

        public bool IsDebugging { get; set; }

        public Lexer.Lexer Lexer
        {
            get; set;
        }

        public int ScrollOffsetX
        {
            get => _scrollOffsetX;
            set
            {
                if (value >= 0)
                {
                    //TODO max scrollbar
                    _scrollOffsetX = value;
                }
                else
                {
                    _scrollOffsetX = 0;
                }
            }
        }

        public int ScrollOffsetY
        {
            get => _scrollOffsetY;
            set
            {
                if (value >= 0 && _Document[value] != null)
                {
                    _scrollOffsetY = value;
                }
                else if (value > 0)
                {
                    _scrollOffsetY = _Document.Count - 1;
                }
                else
                {
                    _scrollOffsetY = 0;
                }
            }
        }

        public string Text
        {
            get
            {
                return _Document.Text;
            }
            set
            {
                _Document.Text = value;
            }
        }

        public string TextVisual
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = FirstRenderedRow; i <= LastRenderedRow && i < _Document.Rows.Count; i++)
                {
                    sb.Append(_Document.Rows[i].Text);
                }

                return sb.ToString();
            }
        }

        internal bool InputHandledInfoForCleanUp { get; set; }

        private int RendertickCount { get; set; }

        public void ResetCursorPosition()
        {
            ScrollOffsetY = 0;
            ScrollOffsetX = 0;
            caret.Top = 0;
            caret.Left = 0;
            InvalidateVisual();
        }

        public void SetFocusToCursorPosition()
        {
            if (ScrollOffsetY >= _Document.Count)
            {
                ScrollOffsetY = _Document.Count;
                ScrollOffsetY -= (LastRenderedRow - FirstRenderedRow) / 4 * 3;
            }
            if (caret.Top < FirstRenderedRow || (RenderBreakOnLineHeight && caret.Top >= LastRenderedRow))
            {
                ScrollOffsetY = caret.Top - ((LastRenderedRow - FirstRenderedRow) / 4 * 3);
            }

            if (caret.Left < FirstRenderedColumn || caret.Left > LastRenderedColumn + 1)
            {
                ScrollOffsetX = caret.Left - Convert.ToInt32(ActualWidth / _defaultchar.Width) / 4 * 3;
            }
        }

        internal void MoveAutocompleteWindowToCursor()
        {
            Point cursorP = caret.CursorPos;
            Canvas.SetLeft(autocompleteWindow, cursorP.X);
            if (cursorP.Y + FontHeightFixed + autocompleteWindow.ActualHeight < ActualHeight)
            {
                Canvas.SetTop(autocompleteWindow, cursorP.Y + FontHeightFixed);
            }
            else
            {
                Canvas.SetTop(autocompleteWindow, cursorP.Y - autocompleteWindow.ActualHeight);
            }
        }

        internal void SetAutocompleteVisibility(Visibility vis)
        {
            autocompleteWindowVM.Filter = (DateTime.Now.Ticks % 2 == 0) ? "Tee" : "Test";
            autocompleteWindowVM.SelectedValue = null;
            MoveAutocompleteWindowToCursor();
            autocompleteWindow.Visibility = vis;
        }

        internal void SetScale(double scale)
        {
            if (scale < minScaleFactor)
            {
                scaleTrans.ScaleX = minScaleFactor;
                scaleTrans.ScaleY = minScaleFactor;
            }
            else if (scale > maxScaleFactor)
            {
                scaleTrans.ScaleX = maxScaleFactor;
                scaleTrans.ScaleY = maxScaleFactor;
            }
            else
            {
                scaleTrans.ScaleX = scale;
                scaleTrans.ScaleY = scale;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            Stopwatch watch = Stopwatch.StartNew();
            base.OnRender(dc);

            Render(dc);

            watch.Stop();
            RendertickCount++;
            FormattedText fpsText = Utils.FormatText($"SO: {ScrollOffsetY}, Renderticks: {RendertickCount}, FPS: {Stopwatch.Frequency / watch.ElapsedTicks}, ExecTime: {watch.ElapsedMilliseconds}ms", 20);
            dc.DrawText(fpsText, new Point(ActualWidth - fpsText.Width, 0));
        }

        private void Autocomplete_ValueSelected()
        {
            Input.InputText(autocompleteWindowVM.SelectedValue.Label);
            SetAutocompleteVisibility(Visibility.Hidden);
        }

        private void HaluEditorControl_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            Input.KeyDown(this, e);
        }

        private void HaluEditorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!HasEffectiveKeyboardFocus)
            {
                Focus();
            }
        }

        private void HaluEditorControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Input.MouseLeftButtonDown(this, e);
            InvalidateVisual();
        }

        private void HaluEditorControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Input.MouseLeftButtonUp(this, e);
            InvalidateVisual();
        }

        private void HaluEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            Input.MouseMove(this, e);
        }

        private void HaluEditorControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool ctrlKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool shiftKeyDown = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (ctrlKeyDown)
            {
                if (e.Delta > 0)
                {
                    SetScale(scaleTrans.ScaleX * 1.1);
                }
                else if (e.Delta < 0)
                {
                    SetScale(scaleTrans.ScaleX * 0.9);
                }
            }
            else if (shiftKeyDown)
            {
                if (e.Delta > 0)
                {
                    ScrollOffsetX -= 3;
                }
                else if (e.Delta < 0)
                {
                    ScrollOffsetX += 3;
                }
                InvalidateVisual();
            }
            else
            {
                if (e.Delta > 0)
                {
                    ScrollOffsetY -= 3;
                }
                else if (e.Delta < 0)
                {
                    ScrollOffsetY += 3;
                }
                InvalidateVisual();
            }
        }

        private void HaluEditorControl_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && e.Text[0] <= 31)
            {
                e.Handled = true;
                return;
            }
            Input.InputText(this, e);
        }

        private void InitDocumentCleanUpTask()
        {
            _ = Task.Run(() =>
              {
                  while (true)
                  {
                      Thread.Sleep(10000);
                      if (InputHandledInfoForCleanUp)
                      {
                          _Document.Rows.Capacity = _Document.Rows.Count;
                          for (int i = 0; i < _Document.Count; i++)
                          {
                              if (i != caret.Top)
                              {
                                  _Document.Rows[i].Letters.Capacity = _Document.Rows[i].Letters.Count;
                              }
                          }
                          GC.Collect();
                          InputHandledInfoForCleanUp = false;
                      }
                  }
              });
        }
    }
}
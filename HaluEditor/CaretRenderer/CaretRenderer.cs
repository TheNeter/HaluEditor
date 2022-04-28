using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ngprojects.HaluEditor.CaretRenderer
{
    public class CaretRenderer : IBaseServiceProvider
    {
        private readonly System.Windows.Shapes.Rectangle cursor;
        private ConfigManager.ConfigManager config;
        private InputManagerService.InputManagerService ims;
        private int left;
        private int top;

        public CaretRenderer()
        {
            cursor = new System.Windows.Shapes.Rectangle();
        }

        public Point CursorPos
        {
            get
            {
                return new Point(Canvas.GetLeft(cursor), Canvas.GetTop(cursor));
            }
        }

        public Brush Fill
        {
            get { return cursor.Fill; }
            set { cursor.Fill = value; }
        }

        public double Height
        {
            get { return cursor.Height; }
            set { cursor.Height = value; }
        }

        public ServiceHost Host { get; set; }

        public int Left
        {
            get => left;
            set { if (value >= 0) { left = Math.Min(value, Parent._Document[Top].End); } }
        }

        public HaluEditorControl Parent { get; set; }

        public int Top
        {
            get => top;
            set { if (value >= 0) { top = Math.Min(value, Parent._Document.Count - 1); } }
        }

        public double Width
        {
            get { return cursor.Width; }
            set { cursor.Width = value; }
        }

        public void DrawCursor(double x, double y)
        {
            Canvas.SetLeft(cursor, x);
            Canvas.SetTop(cursor, y);
        }

        public void DrawCursor(int xof, int yof, int rowEnd, double x, double y, double w)
        {
            if (top == yof)
            {
                if (left == xof && ims.HasSelection)
                {
                    Canvas.SetLeft(cursor, x + (ims.SelectionDirectionIsReverse ? 0 : w));
                }
                else if (left == xof)
                {
                    Canvas.SetLeft(cursor, x);
                }
                else if (left - 1 == xof && left == rowEnd)
                {
                    Canvas.SetLeft(cursor, x + w);
                }

                Canvas.SetTop(cursor, y);

                Parent.MoveAutocompleteWindowToCursor();
            }
        }

        public void LoadingDoneEvent()
        {
            ims = (InputManagerService.InputManagerService)Host.GetServiceFromType<InputManagerService.InputManagerService>();
            config = (ConfigManager.ConfigManager)Host.GetServiceFromType<ConfigManager.ConfigManager>();
            InitCursor();
        }

        private void InitCursor()
        {
            Fill = config.Configuration.CursorColor;
            Width = config.Configuration.CursorWidth;
            Height = Parent.FontSize;
            Parent.Children.Add(cursor);
            Parent.BeginStoryboard(Utils.CreateOpacityBlinkStoryboard(cursor, config.Configuration.CursorBlinkInMs));
        }
    }
}
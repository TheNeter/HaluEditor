using System.Windows.Media;

namespace ngprojects.HaluEditor.ConfigManager
{
    public class Configuration
    {
        private readonly ConfigFile configFile;
        private Brush _AutocompleteForegroundColor;
        private Brush _AutocompleteForegroundReturntypeColor;
        private Brush _AutocompleteWindowColor;
        private Brush _corsorColor;
        private Brush _EditorBrush;
        private Brush _LineNumberColor;
        private Brush _LineNumberTextColor;
        private Brush _RowHighlightColor;
        private Brush _RowHighlightOutlineColor;
        private Brush _SelectionColor;
        private Brush _SelectionOutlineColor;
        private Brush _TextColor;

        public Configuration(ConfigFile config)
        {
            configFile = config;
        }

        public Brush AutocompleteForegroundColor
        {
            get
            {
                if (_AutocompleteForegroundColor == null)
                {
                    configFile.AutocompleteForegroundColor ??= "#FF363636";
                    _AutocompleteForegroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.LineNumberTextColor));
                }

                return _AutocompleteForegroundColor;
            }
        }

        public Brush AutocompleteForegroundReturntypeColor
        {
            get
            {
                if (_AutocompleteForegroundReturntypeColor == null)
                {
                    configFile.AutocompleteForegroundReturntypeColor ??= "#FF99d8d0";
                    _AutocompleteForegroundReturntypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.LineNumberTextColor));
                }

                return _AutocompleteForegroundReturntypeColor;
            }
        }

        public Brush AutocompleteWindowColor
        {
            get
            {
                if (_AutocompleteWindowColor == null)
                {
                    configFile.AutocompleteWindowColor ??= "#FF204051";
                    _AutocompleteWindowColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.LineNumberTextColor));
                }

                return _AutocompleteWindowColor;
            }
        }

        public double CursorBlinkInMs
        {
            get
            {
                if (configFile.CursorBlinkInMs == 0)
                {
                    configFile.CursorBlinkInMs = 500;
                }
                return configFile.CursorBlinkInMs;
            }
        }

        public Brush CursorColor
        {
            get
            {
                if (_corsorColor == null)
                {
                    configFile.CursorColor ??= "#FFCCCCCC";
                    _corsorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.CursorColor));
                }

                return _corsorColor;
            }
        }

        public double CursorWidth
        {
            get
            {
                if (configFile.CursorWidth == 0)
                {
                    configFile.CursorWidth = 1;
                }
                return configFile.CursorWidth;
            }
        }

        public Brush EditorColor
        {
            get
            {
                if (_EditorBrush == null)
                {
                    configFile.EditorColor ??= "#FF204051";
                    _EditorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.EditorColor));
                }
                return _EditorBrush;
            }
        }

        public double FontSize
        {
            get
            {
                if (configFile.FontSize == 0)
                {
                    configFile.FontSize = 20;
                }
                return configFile.FontSize;
            }
        }

        public Brush LineNumberColor
        {
            get
            {
                if (_LineNumberColor == null)
                {
                    configFile.LineNumberColor ??= "#FF363636";
                    _LineNumberColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.LineNumberColor));
                }

                return _LineNumberColor;
            }
        }

        public Brush LineNumberTextColor
        {
            get
            {
                if (_LineNumberTextColor == null)
                {
                    configFile.LineNumberTextColor ??= "#FF99d8d0";
                    _LineNumberTextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.LineNumberTextColor));
                }

                return _LineNumberTextColor;
            }
        }

        public Brush RowHighlightColor
        {
            get
            {
                if (_RowHighlightColor == null)
                {
                    configFile.RowHighlightColor ??= "#00000000";
                    _RowHighlightColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.RowHighlightColor));
                }

                return _RowHighlightColor;
            }
        }

        public Brush RowHighlightOutlineColor
        {
            get
            {
                if (_RowHighlightOutlineColor == null)
                {
                    configFile.RowHighlightOutlineColor ??= "#00000000";
                    _RowHighlightOutlineColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.RowHighlightOutlineColor));
                }

                return _RowHighlightOutlineColor;
            }
        }

        public double RowHighlightOutlineWidth
        {
            get
            {
                return configFile.RowHighlightOutlineWidth;
            }
        }

        public Brush SelectionColor
        {
            get
            {
                if (_SelectionColor == null)
                {
                    configFile.SelectionColor ??= "#FF84A9AC";
                    _SelectionColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.SelectionColor));
                }

                return _SelectionColor;
            }
        }

        public Brush SelectionOutlineColor
        {
            get
            {
                if (_SelectionOutlineColor == null)
                {
                    configFile.SelectionOutlineColor ??= "#FF84A9AC";
                    _SelectionOutlineColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.SelectionOutlineColor));
                }

                return _SelectionOutlineColor;
            }
        }

        public double SelectionOutlineWidth
        {
            get
            {
                return configFile.SelectionOutlineWidth;
            }
        }

        public Brush TextColor
        {
            get
            {
                if (_TextColor == null)
                {
                    configFile.TextColor ??= "#FFE4E3E3";
                    _TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(configFile.TextColor));
                }

                return _TextColor;
            }
        }
    }
}
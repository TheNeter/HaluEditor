using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace ngprojects.HaluEditor.ConfigManager
{
    public class ConfigManager : IBaseServiceProvider
    {
        private string _defaultLineending;
        private Point _TextOffset;

        public ConfigManager()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + "/config.json";
            if (File.Exists(Path))
            {
                UserConfigurationObject = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(Path));
            }
            else
            {
                UserConfigurationObject = new ConfigFile();
            }

            Configuration = new Configuration(UserConfigurationObject);

            // lazy load for default values
            foreach (var prop in typeof(Configuration).GetProperties())
            {
                prop.GetValue(Configuration);
            }

            File.WriteAllText(Path + ".new", JsonConvert.SerializeObject(UserConfigurationObject));

            TextOffset = new Point(30, 5);
            DefaultLineending = "\r\n";
        }

        public delegate void NullEventHandler();

        public event NullEventHandler OnTextOffsetChanged;

        public Configuration Configuration
        {
            get;
            private set;
        }

        public string DefaultLineending
        {
            get => _defaultLineending;
            set
            {
                if (value.Equals("\r") || value.Equals("\n") || value.Equals("\r\n"))
                {
                    _defaultLineending = value;
                }
            }
        }

        public ServiceHost Host { get; set; }
        public HaluEditorControl Parent { get; set; }

        public Point TextOffset
        {
            get
            {
                return _TextOffset;
            }
            set
            {
                if (value.X != _TextOffset.X || value.Y != _TextOffset.Y)
                {
                    _TextOffset = value;
                    OnTextOffsetChanged?.Invoke();
                }
            }
        }

        public ConfigFile UserConfigurationObject { get; set; }
        public int ZValue => 0;

        public void LoadingDoneEvent()
        {
            Parent.Background = Configuration.EditorColor;
        }

        public void OnRender(DrawingContext dc)
        {
        }
    }
}
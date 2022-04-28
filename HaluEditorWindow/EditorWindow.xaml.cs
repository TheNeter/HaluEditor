using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using ngprojects.HaluEditor.Lexer;

namespace ngprojects.HaluEditorWindow
{
    public partial class EditorWindow : Window
    {
        private ICommand _openFileCommand;

        public EditorWindow()
        {
            InitializeComponent();

            HaluEditor.Lexer = new Lexer()
            {
                Definitions = {
                    new TokenDefinition($"(\"(.|\r|\n|(\r\n))*?\")", TokenType.Literal_String),
                    new TokenDefinition(@"IF|ELSE", TokenType.Identifier_Keyword),
                    new TokenDefinition(@"TestMe", TokenType.Identifier_Def),
                    new TokenDefinition(@"\(|\)|;", TokenType.Identifier_Def),
                    new TokenDefinition(@"(\d*\.)?\d+", TokenType.Literal_Number),
                    new TokenDefinition(@"\+|\*|-|\\|=|<|>|!|\?|~", TokenType.Operator),
                    new TokenDefinition(@"\||&|,|(\.\w*)|;|{|}|\[|\]|/|%|#", "special_chars"),
                    new TokenDefinition(@"(\w*)", "freedef")
                }
            };

            HaluEditor.HightlightMap.TryAdd(TokenType.Identifier_Def, (string identifier) => Brushes.Red);
            HaluEditor.HightlightMap.TryAdd(TokenType.Identifier_Keyword, (string identifier) => Brushes.Blue);
            HaluEditor.HightlightMap.TryAdd(TokenType.Operator, (string identifier) => Brushes.DarkSeaGreen);
            HaluEditor.HightlightMap.TryAdd(TokenType.Literal_Number, (string identifier) => Brushes.BlueViolet);
            HaluEditor.HightlightMap.TryAdd(TokenType.Literal_String, (string identifier) => Brushes.Orange);
            HaluEditor.HightlightMap.TryAdd(TokenType.NULL, (string identifier) =>
            {
                switch (identifier)
                {
                    case "special_chars":
                        return Brushes.Chocolate;

                    case "freedef":
                        return Brushes.DodgerBlue;

                    default:
                        //NTH log or error?
                        return Brushes.IndianRed;
                }
            });
            HaluEditor.Focus();
        }

        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                {
                    _openFileCommand = new RelayCommand(p => OpenFile(), p => true);
                }

                return _openFileCommand;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HaluEditor.IsDebugging = !HaluEditor.IsDebugging;
        }

        private void Grid_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = false;
        }

        private void OnKeyDown_CTRL_S()
        {
            throw new NotImplementedException();
        }

        private void OnKeyDown_CTRL_Shift_S()
        {
            throw new NotImplementedException();
        }

        private void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = DriveInfo.GetDrives().First().RootDirectory.FullName,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.UTF8, true))
                {
                    HaluEditor.Text = sr.ReadToEnd();
                }
                HaluEditor.ResetCursorPosition();
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            OnKeyDown_CTRL_S();
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            OnKeyDown_CTRL_Shift_S();
        }
    }
}
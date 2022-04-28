using System.Windows.Controls;

namespace ngprojects.HaluEditor
{
    /// <summary>
    /// Interaktionslogik für HaluEditorAutocomplete.xaml
    /// </summary>
    public partial class HaluEditorAutocomplete : UserControl
    {
        public HaluEditorAutocomplete()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is HaluEditorAutocompleteVM vm && vm.ValueSelectedCommand.CanExecute(null))
            {
                vm.ValueSelectedCommand.Execute(null);
            }
        }
    }
}
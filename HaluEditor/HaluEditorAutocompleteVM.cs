using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace ngprojects.HaluEditor
{
    public class HaluEditorAutocompleteVM : INotifyPropertyChanged
    {
        private string _filter;
        private AutocompleteItemVM selectedValue;
        private ICommand valueSelectedCommand;

        public HaluEditorAutocompleteVM()
        {
            Items = new ObservableCollection<AutocompleteItemVM>();
            for (int i = 0; i < 25; i++)
            {
                Items.Add(new AutocompleteItemVM(new AutocompleteItem()
                {
                    Label = "TestTestTestTestTestTestTestTestTestTestTestTestTestTest",
                    ReturnType = "string",
                    Information = @"(string Val1, int Val2)
Returns a string "
                }));
            }

            Items.Add(new AutocompleteItemVM(new AutocompleteItem { Label = "Tee", ReturnType = "bool", Information = @"(var B, var A)
Sets A to B" }));

            Items = new ObservableCollection<AutocompleteItemVM>(Items.OrderBy(item => item.Label));
        }

        public delegate void BasicEventHandler();

        public event PropertyChangedEventHandler PropertyChanged;

        internal event BasicEventHandler ValueSelected;

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (!Equals(_filter, value))
                {
                    _filter = value;
                    foreach (var item in Items)
                    {
                        item.IsVisible = string.IsNullOrWhiteSpace(_filter) || item.Label.StartsWith(_filter);
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
                }
            }
        }

        public Brush ForegroundReturn { get; set; }

        public ObservableCollection<AutocompleteItemVM> Items { get; set; }

        public AutocompleteItemVM SelectedValue
        {
            get
            {
                return selectedValue;
            }
            set
            {
                if (!Equals(selectedValue, value))
                {
                    selectedValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedValue)));
                }
            }
        }

        public ICommand ValueSelectedCommand
        {
            get
            {
                if (valueSelectedCommand == null)
                {
                    valueSelectedCommand = new RelayCommand(
                        p => true,
                        p => ValueSelectedMethod()
                        );
                }
                return valueSelectedCommand;
            }
        }

        private void ValueSelectedMethod()
        {
            ValueSelected?.Invoke();
        }
    }
}
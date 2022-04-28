using System;
using System.ComponentModel;

namespace ngprojects.HaluEditor
{
    public class AutocompleteItemVM : INotifyPropertyChanged
    {
        private readonly AutocompleteItem _model;
        private bool isVisible;

        public AutocompleteItemVM(AutocompleteItem model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            _model = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Information
        {
            get => _model.Information; set
            {
                if (_model.Information != value)
                {
                    _model.Information = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Information)));
                }
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        public string Label
        {
            get => _model.Label;
            set
            {
                if (_model.Label != value)
                {
                    _model.Label = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Label)));
                }
            }
        }

        public string ReturnType
        {
            get => _model.ReturnType;
            set
            {
                if (_model.ReturnType != value)
                {
                    _model.ReturnType = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ReturnType)));
                }
            }
        }
    }
}
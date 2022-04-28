namespace ngprojects.HaluEditor.InputManagerService
{
    public partial class InputManagerService : IBaseServiceProvider
    {
        private void OnKeyDown_Esc()
        {
            Parent.SetAutocompleteVisibility(System.Windows.Visibility.Hidden);
        }
    }
}
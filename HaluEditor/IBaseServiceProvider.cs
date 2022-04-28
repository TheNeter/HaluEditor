namespace ngprojects.HaluEditor
{
    public interface IBaseServiceProvider
    {
        ServiceHost Host { get; set; }

        HaluEditorControl Parent { get; set; }

        public void LoadingDoneEvent();
    }
}
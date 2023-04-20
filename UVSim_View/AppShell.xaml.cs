 namespace UVSim_View
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EditorPage), typeof(EditorPage));
            Routing.RegisterRoute(nameof(SimPage), typeof(SimPage));
        }
    }
}
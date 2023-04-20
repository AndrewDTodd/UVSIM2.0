namespace UVSim_View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            //double width = DeviceDisplay.Current.MainDisplayInfo.Width;
            //double height = DeviceDisplay.Current.MainDisplayInfo.Height;

            window.Width = 1800;
            window.Height = 1000;

            window.X = 50;
            window.Y = 10;

            //window.Title = "UVSim";

            return window;
        }
    }
}
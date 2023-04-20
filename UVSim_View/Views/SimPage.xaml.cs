using UVSim.ViewModel;

namespace UVSim_View
{
    public partial class SimPage : ContentPage
    {
        public SimPage(ArchitectureSimViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        private void DockOptionsButton_Tapped(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                RadioButton rbutton = button.Parent.Parent as RadioButton;
                rbutton.IsChecked = true;
            }
        }

        private async void EditModeButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
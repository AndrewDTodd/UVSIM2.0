using System.Windows.Input;
using UVSim.ViewModel;

namespace UVSim_View
{
    public partial class EditorPage : ContentPage
    {
        public EditorPage()
        {
            InitializeComponent();
        }

        private void DockOptionsButton_Tapped(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                RadioButton rbutton = button.Parent.Parent as RadioButton;
                rbutton.IsChecked = true;
            }
        }

        private async void SimModeButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SimPage));
        }
    }
}
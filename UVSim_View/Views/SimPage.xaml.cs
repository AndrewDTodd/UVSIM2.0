using UVSim.ViewModel;
using UVSim.ViewModel.Converters;

namespace UVSim_View
{
    public partial class SimPage : ContentPage
    {
        public SimPage(MasterViewModel viewModel, CollectionArrayToIntConverter converter)
        {
            InitializeComponent();

            BindingContext = viewModel;
            
            addressView.BindingContext = viewModel.ArchitectureSimViewModel;
            addressView.SetBinding(ItemsView.ItemsSourceProperty, new Binding("Memory", BindingMode.OneWay, converter, viewModel.ArchitectureSimViewModel.BytesPerWord));

            registersView.BindingContext = viewModel.ArchitectureSimViewModel;
            registersView.SetBinding(ItemsView.ItemsSourceProperty, new Binding("Registers", BindingMode.OneWay, converter, viewModel.ArchitectureSimViewModel.BytesPerWord));
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
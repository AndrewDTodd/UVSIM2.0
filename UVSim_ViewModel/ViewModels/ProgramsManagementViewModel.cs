using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim.ViewModel
{
    public partial class ProgramsManagementViewModel : BaseViewModel
    {
        #region FIELDS
        private readonly ArchitecturePackage_Interface _simPackage;

        //private readonly CancellationTokenSource _tokenSource = new();
        #endregion

        #region PROPERTIES
        public ObservableCollection<UVSim.Program> Programs { get => _simPackage.ProgramsManager.Programs; }

        public ObservableCollection<UVSim.Program> EditingPrograms { get; } = new();

        [ObservableProperty]
        private UVSim.Program _editingProgram;
        #endregion

        #region CONSTRUCTORS
        public ProgramsManagementViewModel(ArchitecturePackage_Interface simPackage)
        {
            _simPackage = simPackage;

            Initialize();
        }
        #endregion

        #region METHODS
        private void Initialize()
        {

        }

        public void TryAddSetEditingProgram(UVSim.Program program, CollectionView view)
        {
            if(EditingPrograms.Contains(program)) 
            {
                view.SelectedItem = program;
                EditingProgram = program;
            }
            else
            {
                EditingPrograms.Add(program);
                view.SelectedItem = program;
                EditingProgram = program;
            }
        }

        public async void TryRemoveEditingProgram(UVSim.Program program, CollectionView view)
        {
            try
            {
                if (EditingPrograms.Contains(program))
                {
                    if (view.SelectedItem == program)
                    {
                        view.SelectedItem = null;
                        EditingProgram = null;
                    }

                    EditingPrograms.Remove(program);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Could not Remove Program", ex.Message, "OK");
            }
        }
        #endregion
    }
}

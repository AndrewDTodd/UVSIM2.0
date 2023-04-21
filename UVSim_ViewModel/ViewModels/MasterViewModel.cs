using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim.ViewModel
{
    public partial class MasterViewModel : BaseViewModel
    {
        #region FIELDS
        private readonly ArchitectureSimViewModel _architectureSimViewModel;
        private readonly AssembliesManagementViewModel _assembliesManagementViewModel;
        private readonly ProgramsManagementViewModel _programsManagemetViewModel;
        #endregion

        #region PROPERTIES
        public ArchitectureSimViewModel ArchitectureSimViewModel { get => _architectureSimViewModel; }
        public AssembliesManagementViewModel AssembliesManagementViewModel { get => _assembliesManagementViewModel; }
        public ProgramsManagementViewModel ProgramsManagementViewModel { get => _programsManagemetViewModel; }
        #endregion

        #region CONSTRUCTORS
        public MasterViewModel(ArchitectureSimViewModel architectureSimViewModel,
            AssembliesManagementViewModel assembliesManagementViewModel,
            ProgramsManagementViewModel programsManagementViewModel)
        {
            _architectureSimViewModel = architectureSimViewModel;
            _assembliesManagementViewModel = assembliesManagementViewModel;
            _programsManagemetViewModel = programsManagementViewModel;

            Initialize();
        }
        #endregion

        #region METHODS
        private void Initialize()
        {

        }
        #endregion
    }
}

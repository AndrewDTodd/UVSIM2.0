using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim.ViewModel
{
    public partial class AssembliesManagementViewModel : BaseViewModel
    {
        #region FIELDS
        private readonly ArchitecturePackage_Interface _simPackage;

        //private readonly CancellationTokenSource _tokenSource = new();
        #endregion

        #region PROPERTIES
        public ObservableCollection<UVSim.Assembly_FixedSize> Assemblies { get => _simPackage.AssemblyManager.LoadedAssemblies; }
        #endregion

        #region CONSTRUCTORS
        public AssembliesManagementViewModel(ArchitecturePackage_Interface simPackage)
        {
            _simPackage = simPackage;

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

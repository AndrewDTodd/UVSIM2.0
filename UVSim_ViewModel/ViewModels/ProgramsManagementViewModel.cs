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
        private BasicMLPackage _simPackage;
        #endregion

        #region PROPERTIES
        public ObservableCollection<BasicMLProgram> Programs { get => _simPackage.ProgramsManager.Programs; }
        #endregion

        #region CONSTRUCTORS
        public ProgramsManagementViewModel(BasicMLPackage simPackage)
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

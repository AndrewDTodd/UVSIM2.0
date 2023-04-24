using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UVSim;

namespace UVSim.ViewModel
{
    public partial class ArchitectureSimViewModel : BaseViewModel
    {
        #region FIELDS
        private readonly ArchitecturePackage_Interface _simPackage;
        #endregion

        #region PROPERTIES
        public List<string> ColumnIndex { get; } = new() { "0x0", "0x1", "0x2", "0x3", "0x4", "0x5", "0x6", "0x7", "0x8", "0x9" };

        public List<string> RowIndex { get; } = new();

        public List<string> RegisterDescriptors { get; } = new();

        public ArchitectureSim_Interface ArchitectureSim { get => _simPackage.ArchitectureSim; }

        public ObservableCollection<byte[]> Registers { get => _simPackage.ArchitectureSim.Registers; }
        
        public ObservableCollection<byte[]> Memory { get =>  _simPackage.ArchitectureSim.Memory; }

        public int BytesPerWord { get => _simPackage.ArchitectureSim.BytesPerWord; }
        #endregion

        #region CONSTRUCTORS
        public ArchitectureSimViewModel(ArchitecturePackage_Interface package)
        {
            _simPackage = package;

            Initialize();
        }
        #endregion

        #region INITIALIZATION
        private void Initialize()
        {
#if DEBUG
            if (ArchitectureSim == null)
                throw new InvalidOperationException($"{nameof(ArchitectureSimViewModel)} cannot call Initialize before the _architectureSim field is set");
            /*else if(InstructionSet == null)
                throw new InvalidOperationException($"{nameof(ArchitectureSimViewModel)} cannot call Initialize before the _instructionSet field is set");*/
#endif

            int numRows = ArchitectureSim.Memory.Count % 10 == 0 ? ArchitectureSim.Memory.Count / ColumnIndex.Count : (ArchitectureSim.Memory.Count / ColumnIndex.Count) + 1;
            for (int i = 0; i < numRows; i++)
            {
                RowIndex.Add("0x" + Convert.ToString(i, 16));
            }

            int numRegisters = ArchitectureSim.Registers.Count;
            for(int register = 0; register < numRegisters; register++)
            {
                if (register >= ArchitectureSim.InstructionSet.GeneralPurposeRegistersIndexes.startIndex && register <= ArchitectureSim.InstructionSet.GeneralPurposeRegistersIndexes.endIndex)
                    RegisterDescriptors.Add($"R{register + 1} (General Purpose)");
                else if (register == ArchitectureSim.InstructionSet.ProgramCounterIndex)
                    RegisterDescriptors.Add($"R{register + 1} (Program Counter)");
                else if (register == ArchitectureSim.InstructionSet.CPSRIndex)
                    RegisterDescriptors.Add($"R{register + 1} (Current Program Status)");
                else if (register == ArchitectureSim.InstructionSet.LinkRegisterIndex)
                    RegisterDescriptors.Add($"R{register + 1} (Link Register)");
                else if (register == ArchitectureSim.InstructionSet.StackPointerIndex)
                    RegisterDescriptors.Add($"R{register + 1} (Stack Pointer)");
            }
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim
{
    /// <summary>
    /// Implements the <seealso cref="UVSim.Program{LinesContainer}"/> interface to facilatate BasicML syntax assembly programs
    /// </summary>
    internal class BasicMLProgram : Program<List<UVSim.LineData>>
    {
        #region CONSTRUCTORS
        public BasicMLProgram(string programName) : base(programName, "bml")
        { }
        #endregion

        #region PROPERTIES
        public override List<LineData> Lines { get => _lines; }
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="ProgramsManagement_Interface{ProgramsCollection, Program}"/> interface
    /// </summary>
    internal class UVSimProgramsManager : ProgramsManagement_Interface<Dictionary<string, BasicMLProgram>, BasicMLProgram>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim
{
    /// <summary>
    /// Implements the <seealso cref="UVSim.Program"/> interface to facilatate BasicML syntax assembly programs
    /// </summary>
    public class BasicMLProgram : Program
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Create a BasicML program object
        /// </summary>
        /// <param name="programName">Name given to the program file</param>
        public BasicMLProgram(string programName) : base(programName, "bml")
        { }
        #endregion

        #region PROPERTIES
        //public override List<LineData> Lines { get => _lines; }
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="ProgramsManagement_Interface"/> interface
    /// </summary>
    public class BasicMLProgramsManager : ProgramsManagement_Interface
    {
        ///<inheritdoc/>
        public override bool TryCreateNewProgram(string programName)
        {
            if (Activator.CreateInstance(typeof(BasicMLProgram), new { programName }) is BasicMLProgram newProgram)
            {
                Programs.Add(newProgram);

                return true;
            }
            else
                return false;
        }
    }
}

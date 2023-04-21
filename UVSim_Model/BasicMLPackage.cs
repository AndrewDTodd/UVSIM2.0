using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim
{
    /// <summary>
    /// UVSim BasicML package implementing the <seealso cref="ArchitecturePackage_Interface"/>
    /// </summary>
    public class BasicMLPackage : ArchitecturePackage_Interface
    {
        private static BasicMLInstructionSet basicML = new();

        /// <summary>
        /// Create a BasicML package instance
        /// </summary>
        public BasicMLPackage() : base(new BasicMLSim(basicML), new BasicMLProgramsManager(), new BasicMLAssemblyManager(basicML))
        { }
    }
}

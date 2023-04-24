using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class BasicMLPackage : ArchitecturePackage_Interface
    {
        private readonly static BasicMLInstructionSet basicML = new();

        /// <summary>
        /// Create a BasicML package instance
        /// </summary>
        public BasicMLPackage() : base(new BasicMLSim(basicML), new BasicMLProgramsManager(), new BasicMLAssemblyManager(basicML))
        {
        }
    }
}

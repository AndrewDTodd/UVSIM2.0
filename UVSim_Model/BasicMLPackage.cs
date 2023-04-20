using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVSim
{
    /// <summary>
    /// UVSim BasicML package implementing the <seealso cref="ArchitecturePackage_Interface{WordType, Assembly, Program}"/>
    /// </summary>
    public class BasicMLPackage : ArchitecturePackage_Interface<Int16, BasicMLAssembly, BasicMLProgram>
    {
        /// <summary>
        /// Create a BasicML package instance
        /// </summary>
        public BasicMLPackage() : base(new BasicMLSim(), new BasicMLAssemblyManager(), new BasicMLProgramsManager())
        {}
    }
}

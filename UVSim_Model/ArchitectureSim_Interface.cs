/*
ArchitectureSim_Interface
<summary>
This abstract class serves as the interface used to create any concrete class whos purpose
is to define and implement an Architecture for a machine language simulator. <seealso cref="{WordSize}"/>
<typeparam name="WordSize">Expected to be an unsigned integer type. Defines the register size in the system.</typeparam>
</summary>

Copyright 2023 Andrew Todd

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE
AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace UVSim
{
    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to define and implement an Architecture for a machine language simulator.
    /// </summary>
    /// <typeparam name="WordType">An integer type that specifies the word size used in the architecture</typeparam>
    /// <typeparam name="OPCodeWordType">An unsigned integer type that specifies the word size of the OP codes and operands used in the architecture. Also defines the addressable space.</typeparam>
    public abstract class ArchitectureSim_Interface<WordType, OPCodeWordType> where WordType : IBinaryInteger<WordType>, new()
        where OPCodeWordType : IUnsignedNumber<OPCodeWordType>, IBinaryInteger<OPCodeWordType>, new()
    {
        #region FIELDS
        protected WordType[] registers;
        protected WordType[] memory;

        //0 for false, 1 for true
        protected  int _programLoaded = 0;

        protected readonly InstructionSet_Interface<WordType, OPCodeWordType> _instructionSet;
        #endregion

        #region PROPERTIES
        public WordType[] Registers { get => registers; }
        public WordType[] Memory { get => memory; }
        public InstructionSet_Interface<WordType, OPCodeWordType> InstructionSet { get => _instructionSet; }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Constructor used by derived types to set up register and memory details in the simulator.
        /// </summary>
        /// <param name="numOfRegisters">Defines the number of registers to be used in the defined Architecture</param>
        /// <param name="memAddresses">Defines the number of addressable words in memory in the simulated system</param>
        protected ArchitectureSim_Interface(OPCodeWordType numOfRegisters, OPCodeWordType memAddresses, InstructionSet_Interface<WordType, OPCodeWordType> instructionSet)
        {
            registers = new WordType[Convert.ToUInt64(numOfRegisters)];
            memory = new WordType[Convert.ToInt64(memAddresses)];

            _instructionSet = instructionSet;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Allows a caller to send a set of instructions (program) to the simulator to be loaded into memory.
        /// </summary>
        /// <remarks>
        /// <para>Expects the program to contain words of a valid size as defined by <typeparamref name="WordType"/></para>
        /// <para>It is expected that the extending class will implement necessary set up with its registers as this class cannot have knowledge of the Architectures register design</para>
        /// </remarks>
        /// <param name="program"> (ref)erence to a collection containing the instructions that constitute the program</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual void LoadProgram(IList<WordType>? program)
        {
            if(program == null)
                throw new System.ArgumentNullException(nameof(program));

            if (program.Count > memory.Length)
                throw new System.ArgumentException($"The program is too big. This architecture only supports a memory of {memory.Length} addresses.");

            for(int i = 0; i < program.Count; i++)
            {
                memory[i] = program[i];
            }

            Interlocked.Exchange(ref _programLoaded, 1);
        }

        /// <summary>
        /// If called after a program is sucessfully loaded into simulator memory, will run the loaded program
        /// </summary>
        /// <remarks>
        /// <para>Will throw an <seealso cref="System.InvalidOperationException"/> if called before a program has been successfully loaded into memory</para>
        /// <para>Will check the input program against the Architectures instruction set throwing a <seealso cref="System.ArgumentException"/> if the program syntax is invalid</para>
        /// </remarks>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentException"/>
        public abstract void RunProgram();
        #endregion

        #region OPERATORS
        public override string ToString()
        {
            string output = $"---Simulator State---\n" +
                $"Registers: ";

            for(int i = 0; i < registers.Length; i++)
            {
                output += $"R{i}[{registers[i]}], ";
            }

            output += "\nMemory:\n";

            output += " \t0      1      2      3      4      5      6      7      8      9\n";

            for (int r = 0; r < 10; r++)
            {
                output += $"{r}\t";
                for (int c = 0; c < 10; c++)
                {
                    string? addressContents = memory[(r * 10) + c]?.ToString()?.PadLeft(4, '0');

                    addressContents ??= "****";

                    output += $"[{addressContents}] ";
                }
                output += "\n";
            }

            return output;
        }
        #endregion
    }
}
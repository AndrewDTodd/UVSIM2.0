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
using System.Collections;
using Microsoft.Win32;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace UVSim
{
    /**** Interface could not be used effectively. CONSIDER REMOVED ****
    /// <summary>
    /// Interface for Architecture Sims. Facilitates Substitutability for <seealso cref="ArchitectureSim_Interface{WordType}"/>
    /// </summary>
    public interface IArchitectureSim
    {
        /// <summary>
        /// Parameter for obtaining the simulators registers
        /// </summary>
        public IList Registers { get; }

        /// <summary>
        /// Parameter for obtaining the simulators system memory
        /// </summary>
        public IList Memory { get; }

        /// <summary>
        /// Parameter for obtaining the simulators instruction set
        /// </summary>
        public IInstructionSet InstructionSet {get;}

        /// <summary>
        /// Allows a caller to send a set of instructions (program) to the simulator to be loaded into memory.
        /// </summary>
        public abstract void LoadProgram(IList<ValueType> program);

        /// <summary>
        /// If called after a program is sucessfully loaded into simulator memory, will run the loaded program
        /// </summary>
        public abstract void RunProgram();
    }
    */

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to define and implement an Architecture for a machine language simulator.
    /// </summary>
    public abstract partial class ArchitectureSim_Interface : ObservableObject
    {
        #region FIELDS
        /// <summary>
        /// General purpose and control registers for the Architecture
        /// </summary>
        protected ObservableCollection<byte[]> registers;
        
        /// <summary>
        /// The avalable system memory for the Architecture
        /// </summary>
        protected ObservableCollection<byte[]> memory;

        /// <summary>
        /// Marks if a program is loaded into the simulator. 0 for false, 1 for true
        /// </summary>
        protected  int _programLoaded = 0;

        /// <summary>
        /// Represents the number or addressable words the system has in its memory
        /// </summary>
        protected int _memorySize;

        /// <summary>
        /// How many bytes are in an addressable word for this architecture, for instance in a 16bit architecture there are 2 bytes per word
        /// </summary>
        protected readonly int _bytesPerWord;

        /// <summary>
        /// Defines the instruction set supported by this architecture simulator <seealso cref="InstructionSet_Interface"/>
        /// </summary>
        protected readonly InstructionSet_Interface _instructionSet;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Property accessor for the simulators registers
        /// </summary>
        public ObservableCollection<byte[]> Registers { get => registers; }
        /// <summary>
        /// Property accessor for the simulators system memory
        /// </summary>
        public ObservableCollection<byte[]> Memory { get => memory; }
        
        /// <summary>
        /// Property accessor for the architectures instruction set
        /// </summary>
        public InstructionSet_Interface InstructionSet { get => _instructionSet; }

        /// <summary>
        /// Gets how many addressable words are in the systems memory
        /// </summary>
        public int MemorySize { get => _memorySize; }

        /// <summary>
        /// Gets the number of bytes that are in each addressable word in this architecture
        /// </summary>
        public int BytesPerWord { get => _bytesPerWord; }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Constructor used by derived types to set up register and memory details in the simulator.
        /// </summary>
        /// <param name="bytesPerWord">How many bytes are in each addressable word in this architecture, such as for a 16bit architecture the bytesPerWord would be 2. Value cannot be less than 1 or grater than 8</param>
        /// <param name="numOfRegisters">Defines the number of registers to be used in the defined Architecture</param>
        /// <param name="memAddresses">Defines the number of addressable words in memory in the simulated system</param>
        /// <param name="instructionSet">The <seealso cref="InstructionSet_Interface"/> opject that defines the instruction set supported by this architecture simulator</param>
        protected ArchitectureSim_Interface(int bytesPerWord, int numOfRegisters, int memAddresses, InstructionSet_Interface instructionSet)
        {
            if (bytesPerWord < 1 || bytesPerWord > 8)
                throw new ArgumentException($"Bytes Per Word cannot be less than 1 or greater than 8. Entered value was {bytesPerWord}");

            registers = new(new byte[numOfRegisters][]);
            Parallel.For(0, numOfRegisters, (regAddress) =>
            {
                registers[regAddress] = new byte[bytesPerWord];
            });

            memory = new(new byte[memAddresses][]);
            Parallel.For(0, memAddresses, (memAddress) =>
            {
                memory[memAddress] = new byte[bytesPerWord];
            });

            _memorySize = memAddresses;
            
            _bytesPerWord = bytesPerWord;

            _instructionSet = instructionSet;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Allows a caller to send a set of instructions (program) to the simulator to be loaded into memory. These are sent as a collection of bytes
        /// </summary>
        /// <remarks>
        /// <para>It is expected that the extending class will implement necessary set up with its registers as this class cannot have knowledge of the Architectures register design</para>
        /// </remarks>
        /// <param name="program"> (ref)erence to a collection containing the instructions that constitute the program</param>
        /// <exception cref="System.ArgumentNullException">Thrown is a null or empty program is passed in</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual void LoadProgram(IList<byte> program)
        {
            byte[] programCache = program.ToArray();

            if(program.Count == 0)
                throw new System.ArgumentException("The program passed in is empty");

            if (program.Count > _memorySize * _bytesPerWord)
                throw new System.ArgumentException($"The program is too big. This architecture only supports a memory of {_memorySize} addresses. This program would require {program.Count / _bytesPerWord} addresses");


            int memoryAddress = 0;
            for(int programIndex = 0; programIndex < programCache.Length; programIndex += _bytesPerWord)
            {
                Array.Copy(programCache, programIndex, memory[memoryAddress], 0, _bytesPerWord);
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
        /// <summary>
        /// String representation of the Architecure's resources
        /// </summary>
        /// <returns>String configured with the architectures resource states</returns>
        public override string ToString()
        {
            string output = $"---Simulator State---\n" +
                $"Registers: ";

            for(int i = 0; i < _memorySize; i++)
            {
                output += $"R{i}[{BitConverter.ToInt64(registers[i])}], ";
            }

            output += "\nMemory:\n";

            output += " \t0      1      2      3      4      5      6      7      8      9\n";

            for (int r = 0; r < 10; r++)
            {
                output += $"{r}\t";
                for (int c = 0; c < 10; c++)
                {
                    string? addressContents = BitConverter.ToInt64(memory[(r * 10) + c]).ToString()?.PadLeft(4, '0');

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
/*
<summary>
Implements the "ProgramsManagementFixedSize_Interface{ProgramsCollection, Program, WordType} interface (abstract generic class)
to fullfil the simulator requirnments of the UVSim BasicML Instruction set
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace UVSim
{
    /// <summary>
    /// Implements the Assembly interface define in <seealso cref="Assembly_FixedSize{WordType}"/> defining the assembly standard for the UVSim application
    /// </summary>
    public class BasicMLAssembly : Assembly_FixedSize<Int16>
    {
        #region FIELDS
        #endregion

        #region OPERATORS
        /// <summary>
        /// Get a reference to a word at a certain location
        /// </summary>
        /// <param name="index">The location of the word to retrieve</param>
        /// <returns></returns>
        public override Int16 this[int index]
        {
            get => Words[index];
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        public BasicMLAssembly(string assemblyName) : base(assemblyName, "bmlo", 100)
        {}
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name as well as an initial program
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="programText">The text to compile from</param>
        public BasicMLAssembly(string assemblyName, string[] programText) : base(assemblyName, "bmlo", programText, 100)
        {}
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name as well as an initial program copied from an existing collection of words
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="words">Collection of words to copy</param>
        public BasicMLAssembly(string assemblyName, Collection<Int16> words) : base(assemblyName, "bmlo", words, 100)
        {}
        #endregion

        #region METHODS
        ///<inheritdoc/>
        public override bool AssembleProgram(string assemblyName, string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Length > WordsCount)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {WordsCount} words per program, program contains {programText.Length}");

            for (int i = 0; i < programText.Length; i++)
            {
                string line = programText[i].Trim();

                if (line[0] != '#')
                {
                    if (char.IsSymbol(line[0]))
                        line = line[1..];

                    if (!byte.TryParse(line[..2], out byte opCode))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting first two characters of text that can be parsed to {typeof(byte)}");

                    if (!byte.TryParse(line[2..], out byte operand))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(byte)}");

                    this.Words[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(line[1..], out Int16 word))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                    this.Words[i] = word;
                }
            }

            return true;
        }

        ///<inheritdoc/>
        public override BasicMLAssembly ParseProgram(string assemblyName, string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Length > WordsCount)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {WordsCount} words per program, program contains {programText.Length}");

            BasicMLAssembly assembly = new(assemblyName);

            for (int i = 0; i < programText.Length; i++)
            {
                if (programText[i][0] != '#')
                {
                    if (char.IsSymbol(programText[i][0]))
                        programText[i] = programText[i][1..];

                    if (!byte.TryParse(programText[i][..2], out byte opCode))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting first two characters of text that can be parsed to {typeof(byte)}");

                    if (!byte.TryParse(programText[i][2..], out byte operand))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(byte)}");

                    assembly.Words[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(programText[i][1..], out Int16 word))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                    assembly.Words[i] = word;
                }
            }

            return assembly;
        }
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="AssembliesManagementFixedSize_Interface{FixedLengthAssembly, WordType}"/> interface (abstract generic class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    public class BasicMLAssemblyManager : AssembliesManagementFixedSize_Interface<BasicMLAssembly, Int16>
    {
        /// <summary>
        /// Construct and initialize this manager
        /// </summary>
        public BasicMLAssemblyManager() : base(100) { }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName)
        {
            LoadedAssemblies.Add(new(assemblyName));

            return true;
        }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName, string[] programText)
        {
            LoadedAssemblies.Add(new(assemblyName, programText));

            return true;
        }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName, Collection<Int16> words)
        {
            LoadedAssemblies.Add(new(assemblyName, words));

            return true;
        }
    }
}
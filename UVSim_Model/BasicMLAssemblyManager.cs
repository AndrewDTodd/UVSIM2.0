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
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;

namespace UVSim
{
    /// <summary>
    /// Implements the Assembly interface define in <seealso cref="Assembly_FixedSize"/> defining the assembly standard for the UVSim application
    /// </summary>
    public class BasicMLAssembly : Assembly_FixedSize
    {
        #region FIELDS
        #endregion

        #region OPERATORS
        /// <summary>
        /// Get a reference to a word at a certain location
        /// </summary>
        /// <param name="index">The location of the word to retrieve</param>
        /// <returns></returns>
        public override Int64 this[int index]
        {
            get => BitConverter.ToInt16(Words.ToArray(), index);
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public BasicMLAssembly(string assemblyName, InstructionSet_Interface instructionSet) : base(assemblyName, "bmlo", 100, 2, instructionSet)
        {}
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name as well as an initial program
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="programText">The text to compile from</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public BasicMLAssembly(string assemblyName, string[] programText, InstructionSet_Interface instructionSet) : base(assemblyName, "bmlo", programText, 100, 2, instructionSet)
        {}
        /// <summary>
        /// Construct and initialize this concrete Assembly type with an assembly name as well as an initial program copied from an existing collection of words
        /// </summary>
        /// <param name="assemblyName">The name of the assembly</param>
        /// <param name="words">Collection of words to copy</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public BasicMLAssembly(string assemblyName, Collection<byte> words, InstructionSet_Interface instructionSet) : base(assemblyName, "bmlo", words, 100, 2, instructionSet)
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

            Words.Clear();

            for (int lineNum = 0; lineNum < programText.Length; lineNum++)
            {
                string[] line = programText[lineNum].Trim().Split();

                if (line.Length > 1)
                {
                    if (!_instructionSet.TryQueryMnemonic(line[0], out int OpCode))
                        throw new ArgumentException($"The mnemonic at line {lineNum} of the program {AssemblyName} is not valid. Entered Mnemonic ({line[0]})");

                    Int16 instruction = (Int16)(OpCode << 11);

                    int bitsPerOperand = 12 / (line.Length - 1);

                    int bitsToShift = 16 - bitsPerOperand;

                    Int16 operand;

                    for (int wordNum = 1; wordNum < line.Length - 1; wordNum++)
                    {
                        if (!Int16.TryParse(line[wordNum], out operand))
                            throw new ArgumentException($"Word {wordNum} at line {lineNum} of the program {AssemblyName} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");
                    
                        operand <<= bitsToShift;
                        operand >>= (bitsPerOperand * wordNum);

                        instruction |= operand;
                    }

                    if (!Int16.TryParse(line.Last(), out operand))
                        throw new ArgumentException($"Word {line.Length - 1} at line {lineNum} of the program {AssemblyName} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                    operand <<= (bitsToShift + 1);
                    operand >>= (bitsPerOperand * (line.Length - 1) + 1);

                    instruction |= operand;

                    byte[] bytes = BitConverter.GetBytes(instruction);
                    this.Words.Add(bytes[0]);
                    this.Words.Add(bytes[1]);
                }
                else
                {
                    if (line[0][0] != '#')
                    {
                        if (char.IsSymbol(line[0][0]))
                            line[0] = line[0][1..];

                        string mnemonic = line[0][..2];

                        if (!_instructionSet.TryQueryMnemonic(mnemonic, out int OpCode))
                            throw new ArgumentException($"The mnemonic at line {lineNum} of the program {AssemblyName} is not valid. Entered Mnemonic ({mnemonic})");

                        if (!int.TryParse(line[0][2..], out int operand))
                            throw new ArgumentException($"Text of program {AssemblyName} on line {lineNum} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(int)}");

                        Int16 instruction = (Int16)((OpCode << 11) | operand);
                        byte[] bytes = BitConverter.GetBytes(instruction);
                        this.Words.Add(bytes[0]);
                        this.Words.Add(bytes[1]);
                    }
                    else
                    {
                        if (!Int16.TryParse(line[0][1..], out Int16 word))
                            throw new System.ArgumentException($"Text of program file ({AssemblyName}) on line {lineNum} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                        byte[] bytes = BitConverter.GetBytes(word);

                        this.Words.Add(bytes[0]);
                        this.Words.Add(bytes[1]);
                    }
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

            BasicMLAssembly assembly = new(assemblyName, this._instructionSet);

            assembly.Words.Clear();

            for (int lineNum = 0; lineNum < programText.Length; lineNum++)
            {
                string[] line = programText[lineNum].Trim().Split();

                if (line.Length > 1)
                {
                    if (!_instructionSet.TryQueryMnemonic(line[0], out int OpCode))
                        throw new ArgumentException($"The mnemonic at line {lineNum} of the program {AssemblyName} is not valid. Entered Mnemonic ({line[0]})");

                    Int16 instruction = (Int16)(OpCode << 11);

                    int bitsPerOperand = 12 / (line.Length - 1);

                    int bitsToShift = 16 - bitsPerOperand;

                    Int16 operand;

                    for (int wordNum = 1; wordNum < line.Length - 1; wordNum++)
                    {
                        if (!Int16.TryParse(line[wordNum], out operand))
                            throw new ArgumentException($"Word {wordNum} at line {lineNum} of the program {AssemblyName} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                        operand <<= bitsToShift;
                        operand >>= (bitsPerOperand * wordNum);

                        instruction |= operand;
                    }

                    if (!Int16.TryParse(line.Last(), out operand))
                        throw new ArgumentException($"Word {line.Length - 1} at line {lineNum} of the program {AssemblyName} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                    operand <<= (bitsToShift + 1);
                    operand >>= (bitsPerOperand * (line.Length - 1) + 1);

                    instruction |= operand;

                    byte[] bytes = BitConverter.GetBytes(instruction);
                    assembly.Words.Add(bytes[0]);
                    assembly.Words.Add(bytes[1]);
                }
                else
                {
                    if (line[0][0] != '#')
                    {
                        if (char.IsSymbol(line[0][0]))
                            line[0] = line[0][1..];

                        string mnemonic = line[0][..2];

                        if (!_instructionSet.TryQueryMnemonic(mnemonic, out int OpCode))
                            throw new ArgumentException($"The mnemonic at line {lineNum} of the program {AssemblyName} is not valid. Entered Mnemonic ({mnemonic})");

                        if (!int.TryParse(line[0][2..], out int operand))
                            throw new ArgumentException($"Text of program {AssemblyName} on line {lineNum} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(int)}");

                        Int16 instruction = (Int16)((OpCode << 11) | operand);
                        byte[] bytes = BitConverter.GetBytes(instruction);
                        assembly.Words.Add(bytes[0]);
                        assembly.Words.Add(bytes[1]);
                    }
                    else
                    {
                        if (!Int16.TryParse(line[0][1..], out Int16 word))
                            throw new System.ArgumentException($"Text of program file ({AssemblyName}) on line {lineNum} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");

                        byte[] bytes = BitConverter.GetBytes(word);

                        assembly.Words.Add(bytes[0]);
                        assembly.Words.Add(bytes[1]);
                    }
                }
            }

            return assembly;
        }
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="AssembliesManagementFixedSize_Interface"/> interface (abstract generic class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    public class BasicMLAssemblyManager : AssembliesManagementFixedSize_Interface
    {
        /// <summary>
        /// Construct and initialize this manager
        /// </summary>
        public BasicMLAssemblyManager(BasicMLInstructionSet basicML) : base(100, basicML) { }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName)
        {
            LoadedAssemblies.Add(new BasicMLAssembly(assemblyName, _instructionSet));

            return true;
        }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName, string[] programText)
        {
            LoadedAssemblies.Add(new BasicMLAssembly(assemblyName, programText, _instructionSet));

            return true;
        }

        ///<inheritdoc/>
        public override bool TryCreateAssembly(string assemblyName, Collection<byte> words)
        {
            LoadedAssemblies.Add(new BasicMLAssembly(assemblyName, words, _instructionSet));

            return true;
        }
    }
}
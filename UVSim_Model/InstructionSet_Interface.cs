/*
InstructionSet_Interface
<summary>
Abstract class that serves as the interface for defining an instructin set to be used by an <seealso cref="ArchitectureSim_Interface{WordType, OPCodeWordType}"/>
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
using System.Text.Json;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace UVSim
{
    /// <summary>
    /// Used to represent the range of registers that are general purpose in the subsequent property
    /// </summary>
    public struct IndexRange
    {
        public int startIndex;
        public int endIndex;
    }

    /// <summary>
    /// Used to store informations about which registers in a system are responsible for what role
    /// </summary>
    public readonly struct Registers
    {
        /// <summary>
        /// Index of register that is the program counter register
        /// </summary>
        public readonly int _programCounterIndex;

        /// <summary>
        /// Index of register that is the program counter register
        /// </summary>
        public readonly IndexRange _genPurposeRegisters;

        /// <summary>
        /// index of the register that is used to store the programs state
        /// </summary>
        public readonly int _cpsrIndex;

        /// <summary>
        /// index of register that is used to return control to stored address after subroutine return
        /// </summary>
        public readonly int? _linkRegisterIndex;

        /// <summary>
        /// register that stores the stack pointer
        /// </summary>
        public readonly int? _stackPointerIndex;

        public Registers(int pc, IndexRange gpr, int cpsr, int? lr = null, int? sp = null) =>
            (_programCounterIndex, _genPurposeRegisters, _cpsrIndex, _linkRegisterIndex, _stackPointerIndex) = (pc, gpr, cpsr, lr, sp);
    }
    /// <summary>
    /// Abstract class that serves as the interface for defining an instructin set to be used by an <seealso cref="ArchitectureSim_Interface{WordType, OPCodeWordType}"/>
    /// </summary>
    /// <typeparam name="WordType">An integer type that specifies the word size used in the architecture</typeparam>
    /// <typeparam name="OPCodeWordType">An unsigned integer type that specifies the word size of the OP codes and operands used in the architecture</typeparam>
    [Serializable]
    public abstract class InstructionSet_Interface<WordType, OPCodeWordType> where WordType : IBinaryInteger<WordType>, new()
        where OPCodeWordType : IUnsignedNumber<OPCodeWordType>, new()
    {
        #region FIELDS
        /// <remarks>
        /// Stores a set of key vallues that are OP codes(Keys) and delegates(values) to impementations that handle those operations
        /// </remarks>
        protected readonly Dictionary<WordType, OP>? _instructionSet;

        protected readonly Registers _registers;
        #endregion

        #region PROPERTIES
        /// <remarks>
        /// Program counter keeps track of the address of the next instruction to be executed.
        /// ***Required!!!***
        /// </remarks>
        public int ProgramCounterIndex { get => _registers._programCounterIndex; }
        
        /// <remarks>
        /// Contains the indexes of all general purpose registers supported by the architecture
        /// ***Required!!!***
        /// <seealso cref="IndexRange"/>
        /// </remarks>
        public IndexRange GeneralPurposeRegistersIndexes { get => _registers._genPurposeRegisters; }
        /// <summary>
        /// CPRS: Current program status register.
        /// Used to keep track of the state of the program
        /// </summary>
        public int CPSRIndex { get => _registers._cpsrIndex; }
        /// <summary>
        /// Used for control flow back to the caller after exiting a subroutine in supported architectures
        /// </summary>
        public int? LinkRegisterIndex { get => _registers._linkRegisterIndex; }
        /// <summary>
        /// Keeps track of the current next free address on stack memory in supported architectures
        /// </summary>
        public int? StackPointerIndex { get => _registers._stackPointerIndex; }

        /// <remarks>
        /// Used to initialize the <seealso cref="instructionSet"/> field
        /// </remarks>
        protected Dictionary<WordType, OP> InstructionSet { init => _instructionSet = value; }

        /// <summary>
        /// Can index into the object with the [] operator to see if the instruction set contains a certain OP code
        /// </summary>
        /// <param name="key">The OP code to query for</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual OP this[WordType key] 
        { 
            get
            {
                if (_instructionSet == null)
                    throw new System.InvalidOperationException("Instruction set has not been initialized. Can't query its Dictionary.");

                if (!_instructionSet.TryGetValue(key, out OP? value))
                    throw new System.ArgumentException($"The Instruction Set doesn't contain the entered OP code {key}");

                return value;
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Can index into the object with this method to see if the instruction set contains a certain OP code
        /// </summary>
        /// <param name="opCode"> the opCode to query for</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual OP QueryOP(WordType opCode)
        {
            if (_instructionSet == null)
                throw new System.InvalidOperationException("Instruction set has not been initialized. Can't query its Dictionary.");

            if (!_instructionSet.TryGetValue(opCode, out OP? value))
                throw new System.ArgumentException("The Instruction Set doesn't contain the entered OP code");

            return value;
        }
        #endregion

        #region CONSTRUCTORS
        protected InstructionSet_Interface(int PCIndex, IndexRange GeneralPurposeRegisters, int CPSRIndex,
            int? LinkRegisterIndex = null, int? StackPointerIndex = null) =>
            (this._registers) = 
            (new Registers(PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex));

        protected InstructionSet_Interface(Dictionary<WordType, OP> instructionSet,
            int PCIndex, IndexRange GeneralPurposeRegisters, int CPSRIndex,
            int? LinkRegisterIndex = null, int? StackPointerIndex = null) =>
            (this._instructionSet, this._registers) =
            (instructionSet, new Registers(PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex));
        #endregion

        #region OPS
        public delegate void OP(WordType[] memory, WordType[] registers, ref OPCodeWordType operand);
        #endregion
    }
}
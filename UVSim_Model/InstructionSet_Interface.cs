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
using System.Collections.ObjectModel;

namespace UVSim
{
    /// <summary>
    /// Used to represent the range of registers that are general purpose in the subsequent property
    /// </summary>
    public readonly struct IndexRange
    {
        /// <summary>
        /// The index that marks the start of the range
        /// </summary>
        public readonly int startIndex;
        /// <summary>
        /// The index that marks the end of the range
        /// </summary>
        public readonly int endIndex;
        /// <summary>
        /// Constructor to initialize and define the Index Range
        /// </summary>
        /// <param name="startIndex">The index number the range starts at</param>
        /// <param name="endIndex">The index number the range ends at</param>
        public IndexRange(int startIndex, int endIndex) =>
            (this.startIndex, this.endIndex) = (startIndex, endIndex);
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

        /// <summary>
        /// Constructor to initialize and define the instruction set
        /// </summary>
        /// <param name="pc">The index of the program counter register</param>
        /// <param name="gpr">The range of indexes in the set of registers that are general purpose registers</param>
        /// <param name="cpsr">The index of the Current Program Status Register</param>
        /// <param name="lr">The index of the link register</param>
        /// <param name="sp">The index of the stack pointer register</param>
        public Registers(int pc, IndexRange gpr, int cpsr, int? lr = null, int? sp = null) =>
            (_programCounterIndex, _genPurposeRegisters, _cpsrIndex, _linkRegisterIndex, _stackPointerIndex) = (pc, gpr, cpsr, lr, sp);
    }

    /// <summary>
    /// Interface to facilitate substitution for <seealso cref="InstructionSet_Interface{WordType}"/>
    /// </summary>
    public interface IInstructionSet
    {
        /// <summary>
        /// Parameter for optaining the register index of the architectures program counter
        /// </summary>
        public int ProgramCounterIndex { get; }

        /// <summary>
        /// Parameter for obtaining the indexes of the registers that are general purpose
        /// </summary>
        public IndexRange GeneralPurposeRegistersIndexes { get; }

        /// <summary>
        /// Parameter for obtaining the index of the curent program status register
        /// </summary>
        public int CPSRIndex { get; }

        /// <summary>
        /// Parameter for obtaining the index of the link register
        /// </summary>
        public int? LinkRegisterIndex { get; }
        
        /// <summary>
        /// Parameter for obtaining the index of the stack pointer
        /// </summary>
        public int? StackPointerIndex { get; }
    }

    /// <summary>
    /// Abstract class that serves as the interface for defining an instruction set to be used by an <seealso cref="ArchitectureSim_Interface{WordType}"/>
    /// </summary>
    /// <typeparam name="WordType">An integer type that specifies the word size used in the architecture</typeparam>
    //[Serializable]
    public abstract class InstructionSet_Interface<WordType> : IInstructionSet
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        /// <remarks>
        /// Stores a set of key vallues that are OP codes(Keys) and delegates(values) to impementations that handle those operations
        /// </remarks>
        protected readonly Dictionary<int, OP>? _instructionSet;

        /// <summary>
        /// The registers definition for this instruction set. See also <seealso cref="RegionInfo"/>
        /// </summary>
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
        /// Used to initialize the <see cref="_instructionSet"/> field
        /// </remarks>
        protected Dictionary<int, OP> InstructionSet { init => _instructionSet = value; }

        /// <summary>
        /// Can index into the object with the [] operator to see if the instruction set contains a certain OP code
        /// </summary>
        /// <param name="key">The OP code to query for</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public virtual OP this[int key] 
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
        public virtual OP QueryOP(int opCode)
        {
            if (_instructionSet == null)
                throw new System.InvalidOperationException("Instruction set has not been initialized. Can't query its Dictionary.");

            if (!_instructionSet.TryGetValue(opCode, out OP? value))
                throw new System.ArgumentException("The Instruction Set doesn't contain the entered OP code");

            return value;
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Create and initialize the instruction set. Called by type deriving from this
        /// </summary>
        /// <param name="PCIndex">The index of the program counter register</param>
        /// <param name="GeneralPurposeRegisters">The range of indexes that are general purpose registers</param>
        /// <param name="CPSRIndex">The index of the current program status register</param>
        /// <param name="LinkRegisterIndex">The index of the link register if one is supported. Default is null, unsupported</param>
        /// <param name="StackPointerIndex">The index of the stack pointer register if one is supported. Default is null, unsupported</param>
        protected InstructionSet_Interface(int PCIndex, IndexRange GeneralPurposeRegisters, int CPSRIndex,
            int? LinkRegisterIndex = null, int? StackPointerIndex = null) =>
            (this._registers) = 
            (new Registers(PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex));

        /// <summary>
        /// Create and initialize the instruction set. Called by type deriving from this
        /// </summary>
        /// <param name="instructionSet">A dictionary defining the instruction sets OpCodes and implementation</param>
        /// <param name="PCIndex">The index of the program counter register</param>
        /// <param name="GeneralPurposeRegisters">The range of indexes that are general purpose registers</param>
        /// <param name="CPSRIndex">The index of the current program status register</param>
        /// <param name="LinkRegisterIndex">The index of the link register if one is supported. Default is null, unsupported</param>
        /// <param name="StackPointerIndex">The index of the stack pointer register if one is supported. Default is null, unsupported</param>
        protected InstructionSet_Interface(Dictionary<int, OP> instructionSet,
            int PCIndex, IndexRange GeneralPurposeRegisters, int CPSRIndex,
            int? LinkRegisterIndex = null, int? StackPointerIndex = null) =>
            (this._instructionSet, this._registers) =
            (instructionSet, new Registers(PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex));
        #endregion

        #region OPS
        /// <summary>
        /// Delegate for OpCodes supported by the instruction set. Defines what information is available to any operation
        /// </summary>
        /// <param name="memory">Referance to the systems memory</param>
        /// <param name="registers">Referance to te systems registers</param>
        /// <param name="instruction">Referance to the instruction word to be operated on</param>
        public delegate void OP(ObservableCollection<WordType> memory, ObservableCollection<WordType> registers, ref WordType instruction);
        #endregion
    }
}
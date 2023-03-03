/*
InstructionSet_Interface

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
    /// Abstract class that serves as the interface for defining an instructin set to be used by an <seealso cref="ArchitectureSim_Interface{WordType}"/>
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
        protected readonly Dictionary<WordType, OP>? instructionSet;
        #endregion

        #region PROPERTIES
        /// <remarks>
        /// Program counter keeps track of the address of the next instruction to be executed.
        /// ***Required!!!***
        /// </remarks>
        public byte ProgramCounterIndex { get; protected init; }
        /// <summary>
        /// Used to represent the range of registers that are general purpose in the subsequent property
        /// </summary>
        public struct IndexRange
        {
            public byte startIndex;
            public byte endIndex;
        }
        /// <remarks>
        /// Contains the indexes of all general purpose registers supported by the architecture
        /// ***Required!!!***
        /// <seealso cref="IndexRange"/>
        /// </remarks>
        public IndexRange GeneralPurposeRegistersIndexes { get; protected init; }
        /// <summary>
        /// CPRS: Current program status register.
        /// Used to keep track of the state of the program
        /// </summary>
        public byte CPSRIndex { get; protected init; }
        /// <summary>
        /// Used for control flow back to the caller after exiting a subroutine in supported architectures
        /// </summary>
        public byte? LinkRegisterIndex { get; protected init; }
        /// <summary>
        /// Keeps track of the current next free address on stack memory in supported architectures
        /// </summary>
        public byte? StackPointerIndex { get; protected init; }

        /// <remarks>
        /// Used to initialize the <seealso cref="instructionSet"/> field
        /// </remarks>
        protected Dictionary<WordType, OP> InstructionSet { init => instructionSet = value; }

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
                if (instructionSet == null)
                    throw new System.InvalidOperationException("Instruction set has not been initialized. Can't query its Dictionary.");

                if (!instructionSet.TryGetValue(key, out OP? value))
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
            if (instructionSet == null)
                throw new System.InvalidOperationException("Instruction set has not been initialized. Can't query its Dictionary.");

            if (!instructionSet.TryGetValue(opCode, out OP? value))
                throw new System.ArgumentException("The Instruction Set doesn't contain the entered OP code");

            return value;
        }
        #endregion

        #region CONSTRUCTORS
        protected InstructionSet_Interface(byte PCIndex, IndexRange GeneralPurposeRegisters, byte CPSRIndex,
            byte? LinkRegisterIndex = null, byte? StackPointerIndex = null) =>
            (this.ProgramCounterIndex, this.GeneralPurposeRegistersIndexes, this.CPSRIndex, this.LinkRegisterIndex, this.StackPointerIndex) = 
            (PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex);

        protected InstructionSet_Interface(Dictionary<WordType, OP> instructionSet,
            byte PCIndex, IndexRange GeneralPurposeRegisters, byte CPSRIndex,
            byte? LinkRegisterIndex = null, byte? StackPointerIndex = null) =>
            (this.InstructionSet, this.ProgramCounterIndex, this.GeneralPurposeRegistersIndexes, this.CPSRIndex, this.LinkRegisterIndex, this.StackPointerIndex) =
            (instructionSet, PCIndex, GeneralPurposeRegisters, CPSRIndex, LinkRegisterIndex, StackPointerIndex);
        #endregion

        #region OPS
        public delegate void OP(WordType[] memory, WordType[] registers, ref OPCodeWordType operand);
        #endregion
    }
}
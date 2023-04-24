/*
BasicML
<summary>
Implements the <seealso cref="ArchitectureSim_Interface{WordSize}"/> interface (abstract class)
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

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace UVSim
{
    /// <summary>
    /// The supported set of instructions of the Basic Ml architecture
    /// </summary>
    public class BasicMLInstructionSet : InstructionSet_Interface
    {
        #region ADDITIONAL_OPS

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct the Basic ML instruction set, initializing its supported operations with assiciated OpCodes and its list of recognized mnemonics
        /// </summary>
        public BasicMLInstructionSet() : base(new Registers(1, new IndexRange(startIndex: 0, endIndex: 0), 2))
        {
            //Defines the OP codes and associated operations for the Basic ML Architecture used by programs on this system
            InstructionSet = new Dictionary<int, OP>()
            {
                //Read OP in BasicML (0)
                [0b00000] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    throw new NotImplementedException("Input is not supported");
                    /*
                    //This may need to be changed to have all IO happening at the driver, not in the logic of the program

                    //Depreciated
                    //Was used in version 1.0
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Task<string> task = Shell.Current.CurrentPage.DisplayPromptAsync("Input Request", "Enter a 4 digit number", "OK");

                    await task;

                    if (!Int16.TryParse(task.Result, out Int16 output))
                        throw new System.ArgumentException("The entered number is too large to store");

                    Array.Copy(BitConverter.GetBytes(output), memory[(int)operand], 2);
                    */
                }),
                //Write OP in BasicML (1)
                [0b00001] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    //This may need to be changed to have all IO happening at the driver, not in the logic of the program

                    //Depreciated
                    //Was used in version 1.0
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Trace.WriteLine(BitConverter.ToInt16(memory[(int)operand], 0).ToString().PadLeft(4, '0'));
                }),

                //Load OP in BasicML (2)
                [0b00010] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Array.Copy(memory[(int)operand], registers[GeneralPurposeRegistersIndexes.startIndex], 2);
                }),
                //Store OP in BasicML (3)
                [0b00011] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Array.Copy(registers[this.GeneralPurposeRegistersIndexes.startIndex], memory[(int)operand], 2);
                }),

                //ADD OP in BasicML (4)
                [0b00100] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 left = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);
                    Int16 right = BitConverter.ToInt16(memory[operand], 0);

                    Array.Copy(BitConverter.GetBytes((Int16)(left + right)), registers[GeneralPurposeRegistersIndexes.startIndex], 2);
                }),
                //SUB OP in BasicML (5)
                [0b00101] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 left = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);
                    Int16 right = BitConverter.ToInt16(memory[operand], 0);

                    Array.Copy(BitConverter.GetBytes((Int16)(left - right)), registers[GeneralPurposeRegistersIndexes.startIndex], 2);
                }),
                //DIV OP in BasicML (6)
                [0b00110] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 left = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);
                    Int16 right = BitConverter.ToInt16(memory[operand], 0);

                    Array.Copy(BitConverter.GetBytes((Int16)(left / right)), registers[GeneralPurposeRegistersIndexes.startIndex], 2);
                }),
                //MUL OP in BasicML (7)
                [0b00111] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 left = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);
                    Int16 right = BitConverter.ToInt16(memory[operand], 0);

                    Array.Copy(BitConverter.GetBytes((Int16)(left * right)), registers[GeneralPurposeRegistersIndexes.startIndex], 2);
                }),

                //Branch OP in BasicML (8)
                [0b01000] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Array.Copy(BitConverter.GetBytes(operand - 1), registers[ProgramCounterIndex], 2);
                }),
                //Branch if Neg OP in BasicML (9)
                [0b01001] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 rOne = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);

                    if (rOne < 0)
                        Array.Copy(BitConverter.GetBytes(operand - 1), registers[ProgramCounterIndex], 2);
                }),
                //Branch if Zero OP in BasicML (10)
                [0b01010] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Int16 operand = (Int16)(BitConverter.ToInt16(instruction, 0) & 0x7FF);

                    Int16 rOne = BitConverter.ToInt16(registers[GeneralPurposeRegistersIndexes.startIndex], 0);

                    if (rOne == 0)
                        Array.Copy(BitConverter.GetBytes(operand - 1), registers[ProgramCounterIndex], 2);
                }),
                //HLT (hault) OP in BasicML (11)
                [0b01011] = new OP((ObservableCollection<byte[]> memory, ObservableCollection<byte[]> registers, byte[] instruction) =>
                {
                    Array.Copy(BitConverter.GetBytes((Int16)43), registers[CPSRIndex], 2);
                })
            };

            Mnemonics = new Dictionary<string, int>()
            {
                //numerical mnemonics

                //Read (0)
                ["10"] = 0b00000,
                //Write (1)
                ["11"] = 0b00001,

                //Load (2)
                ["20"] = 0b00010,
                //store (3)
                ["21"] = 0b00011,

                //Add (4)
                ["30"] = 0b00100,
                //Substract (5)
                ["31"] = 0b00101,
                //Divide (6)
                ["32"] = 0b00110,
                //Multiply (7)
                ["33"] = 0b00111,

                //Branch (8)
                ["40"] = 0b01000,
                //Branch if Negative (9)
                ["41"] = 0b01001,
                //Branch if zero (10)
                ["42"] = 0b01010,

                //Halt (11)
                ["43"] = 0b01011
            };

        }
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="ArchitectureSim_Interface"/> interface (abstract class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    /// <inheritdoc/>
    public class BasicMLSim : ArchitectureSim_Interface
    {
        #region INTERNAL_HELPER_CLASSES
        #endregion

        #region FIELDS

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Calls the base class constructor with the register and memory specifications of the BasicML set
        /// </summary>
        /// <param name="basicML">A reference to an instance of the <seealso cref="BasicMLInstructionSet"/> which will be used by this simulator to actually run the simulation</param>
        /// <param name="numOfregisters">Defines the number of registers to be used in the defined Architecture</param>
        /// <param name="memAddresses">Defines the number of addressable words in memory in the simulated system</param>
        public BasicMLSim(BasicMLInstructionSet basicML, byte numOfregisters = 3, byte memAddresses = 100) : base(2, numOfregisters, memAddresses, basicML)
        {/*This space left intentionally blank ;)*/}
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Allows a caller to send a set of instructions (program) to the simulator to be loaded into memory.
        /// </summary>
        /// <param name="program">List of instructions and data with 16 bit words</param>
        public override void LoadProgram(IList<byte> program)
        {
            //call abstract base class' virtual method that implements basic functionality required by all ArchitectureSims
            base.LoadProgram(program);

            registers[0] = new byte[2];
            registers[1] = new byte[2];
            registers[2] = new byte[2];
        }

        /// <summary>
        /// If called after a program is sucessfully loaded into simulator memory, will run the loaded program
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the method is called with no program loaded</exception>
        public override void RunProgram()
        {
            if (0 == Interlocked.CompareExchange(ref _programLoaded, 0, 0))
                throw new System.InvalidOperationException("No program loaded into memory. Successfully call LoadProgram first.");

            do
            {
                Int16 programCounter = BitConverter.ToInt16(registers[InstructionSet.ProgramCounterIndex], 0);
                Int16 opCode = BitConverter.ToInt16(memory[programCounter], 0);
                opCode >>= 11;

                try
                {
                    InstructionSet_Interface.OP instruction = _instructionSet[opCode];

                    instruction.Invoke(memory, registers, memory[programCounter]);
                }
                finally
                {
                    programCounter = BitConverter.ToInt16(registers[InstructionSet.ProgramCounterIndex], 0);
                    programCounter++;
                    Array.Copy(BitConverter.GetBytes(programCounter), registers[InstructionSet.ProgramCounterIndex], 2);
                }
                
            } while (BitConverter.ToInt16(registers[InstructionSet.CPSRIndex], 0) != 43);
        }
        #endregion
    }
}
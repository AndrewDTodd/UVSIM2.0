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

namespace UVSim
{
    /// <summary>
    /// Implements the <seealso cref="ArchitectureSim_Interface{WordType}"/> interface (abstract class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    /// <inheritdoc/>
    public class BasicMLSim : ArchitectureSim_Interface<Int16>
    {
        #region INTERNAL_HELPER_CLASSES
        private class BasicMLInstructionSet : InstructionSet_Interface<Int16>
        {
            #region ADDITIONAL_OPS
            
            #endregion

            #region CONSTRUCTORS
            public BasicMLInstructionSet() : base(1, new IndexRange(startIndex:0, endIndex:0), 2)
            {
                //Defines the OP codes and associated operations for the Basic ML Architecture used by programs on this system
                InstructionSet = new Dictionary<int, OP>()
                {
                    //Read OP in BasicML
                    [10] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) => 
                    {
                        //This may need to be changed to have all IO happening at the driver, not in the logic of the program
                        
                        //Depreciated
                        //Was used in version 1.0
                        /*if (!Int16.TryParse(ConsoleUIManager.GetInput(), out memory[operand]))
                            throw new System.ArgumentException();*/
                    }),
                    //Write OP in BasicML
                    [11] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) => 
                    {
                        //This may need to be changed to have all IO happening at the driver, not in the logic of the program

                        //Depreciated
                        //Was used in version 1.0
                        /*ConsoleUIManager.Print(memory[operand].ToString().PadLeft(4,'0'));*/
                    }),

                    //Load OP in BasicML
                    [20] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) => 
                    {
                        registers[this.GeneralPurposeRegistersIndexes.startIndex] = memory[(int)operand];
                    }),
                    //Store OP in BasicML
                    [21] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        memory[(int)operand] = registers[this.GeneralPurposeRegistersIndexes.startIndex];
                    }),

                    //ADD OP in BasicML
                    [30] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.GeneralPurposeRegistersIndexes.startIndex] = (Int16)((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] + (int)memory[(int)operand]);
                    }),
                    //SUB OP in BasicML
                    [31] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.GeneralPurposeRegistersIndexes.startIndex] = (Int16)((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] - (int)memory[(int)operand]);
                    }),
                    //DIV OP in BasicML
                    [32] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.GeneralPurposeRegistersIndexes.startIndex] = (Int16)((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] / (int)memory[(int)operand]);
                    }),
                    //MUL OP in BasicML
                    [33] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.GeneralPurposeRegistersIndexes.startIndex] = (Int16)((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] * (int)memory[(int)operand]);
                    }),

                    //Branch OP in BasicML
                    [40] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.ProgramCounterIndex] = (Int16)((int)operand - 1);
                    }),
                    //Branch if Neg OP in BasicML
                    [41] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        if ((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] < 0)
                            registers[this.ProgramCounterIndex] = (Int16)((int)operand - 1);
                    }),
                    //Branch if Zero OP in BasicML
                    [42] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        if ((int)registers[this.GeneralPurposeRegistersIndexes.startIndex] == 0)
                            registers[this.ProgramCounterIndex] = (Int16)((int)operand - 1);
                    }),
                    //HLT (hault) OP in BasicML
                    [43] = new OP((ObservableCollection<Int16> memory, ObservableCollection<Int16> registers, ref Int16 operand) =>
                    {
                        registers[this.CPSRIndex] = 43;
                    })
                };
            }
            #endregion
        }
        #endregion

        #region FIELDS

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Calls the base class constructor with the register and memory specifications of the BasicML set
        /// </summary>
        /// <param name="numOfregisters">Defines the number of registers to be used in the defined Architecture</param>
        /// <param name="memAddresses">Defines the number of addressable words in memory in the simulated system</param>
        public BasicMLSim(byte numOfregisters = 3, byte memAddresses = 100) : base(numOfregisters, memAddresses, new BasicMLInstructionSet())
        {/*This space left intentionally blank ;)*/}
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Allows a caller to send a set of instructions (program) to the simulator to be loaded into memory.
        /// </summary>
        /// <param name="program">List of instructions and data with 16 bit words</param>
        public override void LoadProgram(IList<Int16> program)
        {
            //call abstract base class' virtual method that implements basic functionality required by all ArchitectureSims
            base.LoadProgram(program);

            registers[0] = 0;
            registers[1] = 0;
            registers[2] = 0;
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
                byte opCode = (byte)(memory[registers[this.InstructionSet.ProgramCounterIndex]] >> 8);
                Int16 operand = (byte)(memory[registers[this.InstructionSet.ProgramCounterIndex]] & 0xFF);

                try
                {
                    InstructionSet_Interface<Int16>.OP instruction = _instructionSet[opCode];

                    instruction.Invoke(memory, registers, ref operand);
                }
                finally
                {
                    registers[this.InstructionSet.ProgramCounterIndex]++;
                }
                
            } while (registers[this.InstructionSet.CPSRIndex] != 43);
        }
        #endregion
    }
}
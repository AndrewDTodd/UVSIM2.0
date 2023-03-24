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
using System.Linq;
using System.Text;

namespace UVSim
{
    /// <summary>
    /// Implements the Assembly interface define in <seealso cref="Assembly_FixedSize{WordCollection, WordType}"/> defining the assembly standard for the UVSim application
    /// </summary>
    public class BasicMLAssembly : Assembly_FixedSize<Int16[], Int16>
    {
        #region FIELDS
        private readonly Int16[] _words;
        #endregion

        #region PROPERTIES
        public override int Count { get =>  _words.Length; }

        public override Int16[] Words { get => _words; }
        #endregion

        #region OPERATORS
        public override ref Int16 this[int index]
        {
            get => ref _words[index];
        }
        #endregion

        #region CONSTRUCTORS
        public BasicMLAssembly() : base(100) =>
            _words = new Int16[AssemblyWords];
        #endregion

    }

    /// <summary>
    /// Implements the <seealso cref="AssembliesManagementFixedSize_Interface{AssembliesCollection, FixedLengthAssembly, WordType}"/> interface (abstract generic class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    public class UVSimAssemblyManager : AssembliesManagementFixedSize_Interface<List<BasicMLAssembly>, BasicMLAssembly, Int16>
    {
        public UVSimAssemblyManager() : base(100) { }

        public override bool CreateAssembly(string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Length > AssemblySize)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {AssemblySize} words per program, program contains {programText.Length}");

            loadedAssemblies.Add(new());

            BasicMLAssembly assembly = loadedAssemblies.Last();

            for(int i = 0; i < programText.Length; i++)
            {
                if (programText[i][0] != '#')
                {
                    if (char.IsSymbol(programText[i][0]))
                        programText[i] = programText[i][1..];

                    if (!byte.TryParse(programText[i][..2], out byte opCode))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting first two characters of text that can be parsed to {typeof(byte)}");

                    if (!byte.TryParse(programText[i][2..], out byte operand))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(byte)}");

                    assembly[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(programText[i][1..], out assembly[i]))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");
                }
            }

            return true;
        }

        public override BasicMLAssembly ParseProgram(string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Length > AssemblySize)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {AssemblySize} words per program, program contains {programText.Length}");

            BasicMLAssembly assembly = new();

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

                    assembly[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(programText[i][1..], out assembly[i]))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");
                }
            }

            return assembly;
        }

        public override void SerializeAssemblies(int[] indexes, string? directory, string[]? fileNames = null)
        {
            if (string.IsNullOrEmpty(directory))
                throw new System.ArgumentNullException(nameof(directory));

            if (loadedAssemblies.Count == 0)
                throw new System.InvalidOperationException("Program storage object is empty");

            for (int i = 0; i < indexes.Length; i++)
            {
                List<byte> buffer = new(capacity: (int)loadedAssemblies[i].Words.Length * BitConverter.GetBytes(loadedAssemblies[i][0]).Length);

                string fileName;

                if (fileNames != null && i < fileNames.Length)
                {
                    fileName = fileNames[i];

                    int index = fileName.IndexOf('.');
                    if (index != -1)
                        fileName = fileName[..index];
                }
                else
                    fileName = $"program{indexes[i]}";

                foreach (Int16 word in loadedAssemblies[indexes[i]].Words)
                {
                    buffer.AddRange(BitConverter.GetBytes(word));
                }

                using BinaryWriter writer = new(new FileStream(directory + "\\" + fileName + ".BML", FileMode.OpenOrCreate));
                writer.Write(buffer.ToArray());
            }
        }

        public override void DeserializeAssemblies(string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new System.ArgumentException($"File path cannot be empty!\nEntered file path {filePath}");

                byte[] bytes;

                using (BinaryReader reader = new(new FileStream(filePath, FileMode.Open)))
                {
                    bytes = new byte[reader.BaseStream.Length];

                    reader.Read(bytes);
                }

                loadedAssemblies.Add(new());

                BasicMLAssembly assembly = loadedAssemblies.Last();

                int bytesPerWord = BitConverter.GetBytes(Int16.MaxValue).Length;

                for(int i = 0; i < bytes.Length / bytesPerWord; i++)
                {
                    assembly[i] = BitConverter.ToInt16(bytes, bytesPerWord * i);
                }
            }
        }
    }
}
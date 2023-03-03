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
    /// Implements the <seealso cref="ProgramsManagementFixedSize_Interface{ProgramsCollection, Program, WordType}"/> interface (abstract generic class)
    /// to fullfil the simulator requirnments of the UVSim BasicML Instruction set
    /// </summary>
    /// <remarks>
    /// Utilizes the base classes default argument in its constructor to make the programs have a fixed length of 100 words
    /// </remarks>
    public class UVSimProgramManager : ProgramsManagementFixedSize_Interface<List<Int16[]>, Int16[], Int16>
    {
        public override bool CreateProgram(string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Count() > ProgramSize)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {ProgramSize} words per program, program contains {programText.Count()}");

            if (ProgramSize == null)
                throw new System.InvalidOperationException("This method needs to be overridden to for an implementation than doesn't require a defined program size. ProgramSize if null!");

            loadedPrograms.Add(new Int16[(int)ProgramSize]);

            Int16[] array = loadedPrograms.Last();

            for(int i = 0; i < programText.Count(); i++)
            {
                if (programText[i][0] != '#')
                {
                    if (char.IsSymbol(programText[i][0]))
                        programText[i] = programText[i][1..];

                    if (!byte.TryParse(programText[i][..2], out byte opCode))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting first two characters of text that can be parsed to {typeof(byte)}");

                    if (!byte.TryParse(programText[i][2..], out byte operand))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(byte)}");

                    array[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(programText[i][1..], out array[i]))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");
                }
            }

            return true;
        }

        public override Int16[] ParseProgram(string[] programText)
        {
            if (programText.Length == 0)
                throw new System.ArgumentException("Your program is empty. Canceling operation");

            if (programText.Count() > ProgramSize)
                throw new System.ArgumentException($"Program contains more words than system supports!\nSystem supports up to {ProgramSize} words per program, program contains {programText.Count()}");

            if (ProgramSize == null)
                throw new System.InvalidOperationException("This method needs to be overridden to for an implementation than doesn't require a defined program size. ProgramSize if null!");

            Int16[] array = new Int16[programText.Count()];

            for (int i = 0; i < programText.Count(); i++)
            {
                if (programText[i][0] != '#')
                {
                    if (char.IsSymbol(programText[i][0]))
                        programText[i] = programText[i][1..];

                    if (!byte.TryParse(programText[i][..2], out byte opCode))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting first two characters of text that can be parsed to {typeof(byte)}");

                    if (!byte.TryParse(programText[i][2..], out byte operand))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting last two characters of text that can be parsed to {typeof(byte)}");

                    array[i] = BitConverter.ToInt16(new byte[2] { operand, opCode });
                }
                else
                {
                    if (!Int16.TryParse(programText[i][1..], out array[i]))
                        throw new System.ArgumentException($"Text of program file on line (word number) {i} could not be parsed properly!\nExpecting text that can be parsed to {typeof(Int16)}");
                }
            }

            return array;
        }

        public override void SerializePrograms(int[] indexes, string? directory, string[]? fileNames = null)
        {
            if (string.IsNullOrEmpty(directory))
                throw new System.ArgumentNullException(nameof(directory));

            if (loadedPrograms.Count == 0)
                throw new System.InvalidOperationException("Program storage object is empty");

            if (ProgramSize == null)
                throw new System.InvalidOperationException("This method needs to be overridden to for an implementation than doesn't require a defined program size. ProgramSize if null!");


            for (int i = 0; i < indexes.Length; i++)
            {
                List<byte> buffer = new List<byte>(capacity: (int)loadedPrograms[i].Length * BitConverter.GetBytes(loadedPrograms[i][0]).Length);

                string fileName;

                if (fileNames != null && i < fileNames.Length)
                {
                    fileName = fileNames[i];

                    int index = fileName.IndexOf('.');
                    if (index != -1)
                        fileName = fileName.Substring(0, index);
                }
                else
                    fileName = $"program{indexes[i]}";

                foreach (Int16 word in loadedPrograms[indexes[i]])
                {
                    buffer.AddRange(BitConverter.GetBytes(word));
                }

                using (BinaryWriter writer = new(new FileStream(directory + "\\" + fileName + ".BML", FileMode.OpenOrCreate)))
                {
                    writer.Write(buffer.ToArray());
                }
            }
        }

        public override void DeserializePrograms(string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                if (ProgramSize == null)
                    throw new System.InvalidOperationException("This method needs to be overridden to for an implementation than doesn't require a defined program size. ProgramSize if null!");

                if (string.IsNullOrEmpty(filePath))
                    throw new System.ArgumentException($"File path cannot be empty!\nEntered file path {filePath}");

                byte[] bytes;

                using (BinaryReader reader = new(new FileStream(filePath, FileMode.Open)))
                {
                    bytes = new byte[reader.BaseStream.Length];

                    reader.Read(bytes);
                }

                loadedPrograms.Add(new Int16[(int)ProgramSize]);

                Int16[] array = loadedPrograms.Last();

                int bytesPerWord = BitConverter.GetBytes(Int16.MaxValue).Length;

                for(int i = 0; i < bytes.Length / bytesPerWord; i++)
                {
                    array[i] = BitConverter.ToInt16(bytes, bytesPerWord * i);
                }
            }
        }
    }
}
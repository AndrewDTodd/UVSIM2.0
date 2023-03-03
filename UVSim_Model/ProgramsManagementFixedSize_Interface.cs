/*
<summary>
This abstract class serves as the interface used to create any concrete class whos purpose
is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
an Architecture supported by a class derived from ArchitectureSim_Interface

Supports programs of a fixed maximum length
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
using System.Numerics;
using System.Text;
using System.IO;
using System.Reflection;

namespace UVSim
{
    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordType}"/>
    /// </summary>
    /// <remarks>
    /// ***Supports programs of a fixed maximum length!!!*** Programs can not exede the maximum size specified at initialization
    /// </remarks>
    /// <typeparamref name="ProgramsCollection"/> A collection that implements the <seealso cref="IList{T}"/> interface and also supports a public paramaterless constructor for the use of new()
    /// <typeparamref name="Program"/> A collection that implements the <seealso cref="IList{T}"/> interface but does not allow for parameterless construction
    /// <typeparamref name="WordType"/> An intager type that specifies the word size used in the architecture
    public abstract class ProgramsManagementFixedSize_Interface<ProgramsCollection, Program, WordType>
        where ProgramsCollection : IList<Program>, new() where Program : IList<WordType> where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        protected ProgramsCollection loadedPrograms;
        #endregion

        #region PROPERTIES
        protected byte? ProgramSize {get; init;}
        public int LoadedProgramsCount { get => loadedPrograms.Count; }

        public virtual Program this[int index]
        {
            get
            {
                if (loadedPrograms[index] == null)
                    throw new System.IndexOutOfRangeException("There is no program with that index");

                return loadedPrograms[index];
            }
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initialize the object
        /// </summary>
        protected ProgramsManagementFixedSize_Interface(byte programSize = 100)
        {
            ProgramSize = programSize;
            loadedPrograms = new();
        }
        #endregion

        #region METHODS
        /// <summary>
        /// <para>Method that will parse the input into the internal storage format for the Architectures programs and store the program in the program collection</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <param name="programText">String that contains the program in a supported syntax</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract bool CreateProgram(string[] programText);

        /// <summary>
        /// <para>Method that will parse the input into the internal storage format for the Architectures programs and return that program</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <remarks>Will not store the program in the program collection. Primarily for testing</remarks>
        /// <param name="programText">String that contains the program in a supported syntax</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract Program ParseProgram(string[] programText);

        /// <summary>
        /// Removes a program from the collection
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public virtual void DeleteProgram(byte index)
        {
            if (loadedPrograms.Count == 0)
                throw new System.InvalidOperationException("Program collection is null. Initialize the colletion first");

            if (index > loadedPrograms.Count)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            loadedPrograms.RemoveAt(index);
        }

        /*Should not be getting called. Use the SerializePrograms method instead
        /// <summary>
        /// Attempts to serialize a program in the collection at "index"
        /// </summary>
        /// <param name="index"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual async void SerializeProgram(int index, string? filePath, string? fileName)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new System.ArgumentNullException(nameof(filePath));

            if (loadedPrograms == null)
                throw new System.InvalidOperationException("Program storage object is null");

            if (string.IsNullOrEmpty(fileName))
                fileName = $"program{index}";

            if (ProgramSize == null)
                throw new System.InvalidOperationException("This method needs to be overridden to for an implementation than doesn't require a defined program size. ProgramSize if null!");

            List<string> lines = new List<string>(capacity: (int)ProgramSize);

            foreach(WordType word in loadedPrograms[index])
            {
                string? wordString = word.ToString();
                
                if(wordString != null)
                    lines.Add(wordString);
            }

            await File.WriteAllLinesAsync(filePath + "\\" + fileName + ".txt", lines.ToArray());
        }
        */

        /// <summary>
        /// Attempts to serialize the programs specified by indexes
        /// </summary>
        /// <param name="indexes"></param>
        /// <param name="filePath"></param>
        /// <exception cref="System.ArgumentNullException">Throws if filePath is null or empty</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public abstract void SerializePrograms(int[] indexes, string? directory, string[]? fileNames = null);

        /// <summary>
        /// Attempts to load a program(s) from the filePaths provided
        /// </summary>
        /// <param name="filePaths"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public abstract void DeserializePrograms(string[] filePaths);
        #endregion

        #region OPERATORS
        public override string ToString()
        {
            string output = $"---Program Collection---\nProgram max size: {ProgramSize}\n" +
                $"Programs in collection: {loadedPrograms.Count}\n" +
                "programs:\n[\n";

            int i = 0;

            foreach(Program program in loadedPrograms)
            {
                output += $"\t{i}.) [";

                for(int j = 0; j < (program.Count > 20 ? 20 : program.Count); j++)
                {
                    output += program[j].ToString() + ", ";
                }

                output += "]\n";

                i++;
            }

            output += "]";

            return output;
        }
        #endregion
    }
}
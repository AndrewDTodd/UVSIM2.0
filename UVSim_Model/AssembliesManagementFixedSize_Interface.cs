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
    public interface IAssemblyFixedLength<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        /// <summary>
        /// Is the assembly up to date, or have changes been made to its associated code file
        /// </summary>
        public bool UpToDate { get; }

        /// <summary>
        /// Gets a reference to the underlying collection
        /// </summary>
        public abstract IList<WordType>? Words { get; }

        /// <summary>
        /// Returns the number of elements in the underlying collection
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Allows indexing into the underlying collection
        /// </summary>
        /// <param name="index">The index number to retrive</param>
        /// <returns></returns>
        public abstract ref WordType this[int index] { get; }
        
        /// <summary>
        /// Retruns the maximum size of an assembly in the number of words it contains
        /// </summary>
        public int AssemblyWords { get; init; }
    }

    public abstract class Assembly_FixedSize<WordCollection, WordType> : IAssemblyFixedLength<WordType>
        where WordCollection : IList<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        protected bool _upToDate = true;
        #endregion

        #region PROPERTIES
        public bool UpToDate { get => _upToDate; }
        
        public abstract IList<WordType>? Words { get; }

        public abstract int Count { get; }

        public int AssemblyWords { get; init; }
        #endregion

        #region OPERATORS
        public abstract ref WordType this[int index] { get; }
        #endregion

        #region CONSTRUCTORS
        public Assembly_FixedSize(int assemblySize) =>
            (AssemblyWords) = (assemblySize);
        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordType, OPCodeWordType}"/>
    /// </summary>
    /// <remarks>
    /// ***Supports programs of a fixed maximum length!!!*** Programs can not exede the maximum size specified at initialization
    /// </remarks>
    /// <typeparamref name="AssembliesCollection"/> A collection that implements the <seealso cref="IList{T}"/> interface and also supports a public paramaterless constructor for the use of new()
    /// <typeparamref name="Assembly"/> A collection that implements the <seealso cref="IList{T}"/> interface but does not allow for parameterless construction
    /// <typeparamref name="WordType"/> An intager type that specifies the word size used in the architecture
    public abstract class AssembliesManagementFixedSize_Interface<AssembliesCollection, FixedLengthAssembly, WordType>
        where AssembliesCollection : IList<FixedLengthAssembly>, new()
        where FixedLengthAssembly : IAssemblyFixedLength<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        protected AssembliesCollection loadedAssemblies;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Returns the max size of an assembly in bytes, null if the architecture provides no limitation
        /// </summary>
        protected int? AssemblySize {get; init;}
        public int LoadedAssembliesCount { get => loadedAssemblies.Count; }
        #endregion

        #region OPERATORS
        public virtual FixedLengthAssembly this[int index]
        {
            get
            {
                if (loadedAssemblies[index] == null)
                    throw new System.IndexOutOfRangeException("There is no program with that index");

                return loadedAssemblies[index];
            }
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initialize the object
        /// </summary>
        protected AssembliesManagementFixedSize_Interface(int assemblySize)
        {
            AssemblySize = assemblySize;
            loadedAssemblies = new();
        }
        #endregion

        #region METHODS
        /// <summary>
        /// <para>Method that will parse the input into a binary assembly and store the assembly in the assemblies collection</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <param name="programText">Array of strings representing the lines of the program</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract bool CreateAssembly(string[] programText);

        /// <summary>
        /// <para>Method that will parse the input and return a binary assembly</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <remarks>Will not store the program in the program collection. Primarily for testing</remarks>
        /// <param name="programText">Array of strings representing the lines of the program</param>
        public abstract FixedLengthAssembly ParseProgram(string[] programText);

        /// <summary>
        /// Removes an assembly from the collection
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public virtual void DeleteProgram(byte index)
        {
            if (loadedAssemblies.Count == 0)
                throw new System.InvalidOperationException("Program collection is null. Initialize the colletion first");

            if (index > loadedAssemblies.Count)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            loadedAssemblies.RemoveAt(index);
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
        public abstract void SerializeAssemblies(int[] indexes, string? directory, string[]? fileNames = null);

        /// <summary>
        /// Attempts to load a program(s) from the filePaths provided
        /// </summary>
        /// <param name="filePaths"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public abstract void DeserializeAssemblies(string[] filePaths);
        #endregion

        #region OVERRIDES
        public override string ToString()
        {
            string output = $"---Assemblies Collection---\nAssembly max size: {AssemblySize}\n" +
                $"Programs in collection: {loadedAssemblies.Count}\n" +
                "programs:\n[\n";

            int i = 0;

            foreach(FixedLengthAssembly program in loadedAssemblies)
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
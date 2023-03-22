/*
<summary>
This abstract class serves as the interface used to create any concrete class whos purpose
is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
an Architecture supported by a class derived from ArchitectureSim_Interface

Extends ProgramManagerFixedSize_Interface to add support for programs of varying or dynamic length
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UVSim
{
    public interface IAssembly<WordType>
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
        /// Returns the name of the assembly, or sets it to a provided string
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Returns the extension used for the assembly
        /// </summary>
        public string AssemblyExtension { get; }

        /// <summary>
        /// Returns or sets the <seealso cref="System.IO.FileInfo"/> object that represents the path to the file used for storage on the system
        /// </summary>
        public FileInfo? FileInfo { get; set; }

        /// <summary>
        /// Allows indexing into the underlying collection
        /// </summary>
        /// <param name="index">The index number to retrive</param>
        /// <returns></returns>
        public abstract ref WordType this[int index] { get; }
    }

    public abstract class Assembly<WordCollection, WordType> : IAssembly<WordType>
        where WordCollection : IList<WordType>, new()
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        protected bool _upToDate = true;

        protected SerializationInfo _serializationInfo;
        #endregion

        #region PROPERTIES
        public bool UpToDate { get => _upToDate; }

        public abstract IList<WordType>? Words { get; }

        public abstract int Count { get; }

        public string AssemblyName { get => _serializationInfo._fileName; set => _serializationInfo._fileName = value; }

        public string AssemblyExtension { get => _serializationInfo._extension; }

        public FileInfo? FileInfo { get => _serializationInfo._fileInfo; set => _serializationInfo._fileInfo = value; }
        #endregion

        #region OPERATORS
        public abstract ref WordType this[int index] { get; }
        #endregion

        #region CONSTRUCTORS

        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordType, OPCodeWordType}"/>
    /// </summary>
    /// <remarks>
    /// Adds support for programs of a dynamic length with the use of generic containers as valid type paramaters
    /// </remarks>
    /// <typeparamref name="AssembliesCollection"/> A collection that implements the <seealso cref="IList{T}"/> interface and a public parameterless constructor
    /// <typeparamref name="Assembly"/> A type that derives implements the <seealso cref="IAssembly{WordType}"/> interface. Recommended that type is derived from <seealso cref="Assembly"/> for ease of use
    /// <typeparamref name="WordType"/> An intager type that specifies the word size used in the architecture
    public abstract class AssembliesManagement_Interface<AssembliesCollection, Assembly, WordType>
        where AssembliesCollection : IList<Assembly>, new()
        where Assembly : IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        protected AssembliesCollection loadedAssemblies = new();
        #endregion

        #region PROPERTIES
        public int LoadedAssembliesCount { get => loadedAssemblies.Count; }
        #endregion

        #region OPERATORS
        public virtual Assembly this[int index]
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
        public abstract Assembly ParseProgram(string[] programText);

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
            string output = $"---Assemblies Collection---\n" +
                $"Programs in collection: {loadedAssemblies.Count}\n" +
                "programs:\n[\n";

            int i = 0;

            foreach (Assembly program in loadedAssemblies)
            {
                output += $"\t{i}.) [";

                for (int j = 0; j < (program.Count > 20 ? 20 : program.Count); j++)
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
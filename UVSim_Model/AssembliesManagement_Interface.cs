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

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UVSim
{
    /// <summary>
    /// Interface that defines the common functionality expected of all Assemblies used in the application
    /// </summary>
    /// <typeparam name="WordType"></typeparam>
    public interface IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        /// <summary>
        /// Is the assembly up to date, or have changes been made to its associated code file
        /// </summary>
        public bool UpToDate { get; set; }

        /// <summary>
        /// Gets a reference to the underlying collection
        /// </summary>
        public ObservableCollection<WordType> Words { get; }

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
        public string Extension { get; }

        /// <summary>
        /// Returns or sets the <seealso cref="System.IO.FileInfo"/> object that represents the path to the file used for storage on the system
        /// </summary>
        public FileInfo? FileInfo { get; set; }

        /// <summary>
        /// Allows indexing into the underlying collection
        /// </summary>
        /// <param name="index">The index number to retrive</param>
        /// <returns></returns>
        public abstract WordType this[int index] { get; }

        /// <summary>
        /// <para>Method that will parse the input into a binary assembly and store the assembly in the assemblies collection</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program to be assembled</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract bool AssembleProgram(string assemblyName, string[] programText);

        /// <summary>
        /// <para>Method that will parse the input and return a binary assembly</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <remarks>Will not store the program in the program collection. Primarily for testing</remarks>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program to be assembled</param>
        public abstract Assembly<WordType> ParseProgram(string assemblyName, string[] programText);

        /// <summary>
        /// Serializes the assembly to disk
        /// </summary>
        /// <returns>Returns true if the operation succeeded, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown if the method is called when the assembly's FileInfo isn't set</exception>
        public Task<bool> SerializeAssembly();

        /// <summary>
        /// Deserializes the assembly from disk
        /// </summary>
        /// <param name="info">Info about the saved assembly to deserialize from disk</param>
        /// <returns>True if operation succeeds, false otherwise</returns>
        public Task<bool> DeserializeAssembly(FileInfo info);
    }

    /// <summary>
    /// Default implementations used to simplify concrete implementations of Assembly types
    /// </summary>
    /// <typeparam name="WordType">The kind of words used by the system</typeparam>
    public abstract partial class Assembly<WordType> : ObservableObject, IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        /// <summary>
        /// Marks if the assembly has been built to reflect changes in its associated <seealso cref="Program{WordType}"/>
        /// </summary>
        [ObservableProperty]
        protected bool _upToDate = true;

        /// <summary>
        /// Serialization info for saving assembly to disk
        /// </summary>
        protected SerializationInfo _serializationInfo;
        #endregion

        #region PROPERTIES
        //public bool UpToDate { get => _upToDate; }
        /// <summary>
        /// Collection of words that constitute the collection of machine instructions, or in other words the assembly
        /// </summary>
        public ObservableCollection<WordType> Words { get; init; } = new();

        /// <summary>
        /// The number of words in the assembly
        /// </summary>
        public int Count { get => Words.Count; }

        /// <summary>
        /// Name given to this Assembly's file
        /// </summary>
        public virtual string AssemblyName
        {
            get => _serializationInfo.FileName;
            set
            {
                if (!EqualityComparer<string>.Default.Equals(_serializationInfo.FileName, value))
                {
                    OnAssemblyNameChanging(value);
                    OnPropertyChanging(AssemblyNameChanging);
                    _serializationInfo.FileName = value;
                    OnAssemblyNameChanged(value);
                    OnPropertyChanged(AssemblyNameChanged);
                }
            }
        }
        /// <summary>Executes the logic for when _serializationInfo.FileName is changing.</summary>
        partial void OnAssemblyNameChanging(string value);
        /// <summary>Executes the logic for when _serializationInfo.FileName just changed.</summary>
        partial void OnAssemblyNameChanged(string value);
        /// <summary>
        /// Arguments for the changing event for the _serializationInfo.FileName property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs AssemblyNameChanging = new(nameof(AssemblyName));
        /// <summary>
        /// Arguments for the changed event for the _serializationInfo.FileName property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs AssemblyNameChanged = new(nameof(AssemblyName));

        /// <summary>
        /// <seealso cref="System.IO.FileInfo"/> for the Program's file 
        /// </summary>
        public virtual FileInfo? FileInfo
        {
            get => _serializationInfo.FileInfo;
            set
            {
                if (!EqualityComparer<FileInfo>.Default.Equals(_serializationInfo.FileInfo, value))
                {
                    OnAssemblyInfoChanging(value);
                    OnPropertyChanging(AssemblyInfoChanging);
                    _serializationInfo.FileInfo = value;
                    OnAssemblyInfoChanged(value);
                    OnPropertyChanged(AssemblyInfoChanged);
                }
            }
        }
        /// <summary>Executes the logic for when _serializationInfo.FileInfo is changing.</summary>
        partial void OnAssemblyInfoChanging(FileInfo? value);
        /// <summary>Executes the logic for when _serializationInfo.FileInfo just changed.</summary>
        partial void OnAssemblyInfoChanged(FileInfo? value);
        /// <summary>
        /// Arguments for the changing event for the _serializationInfo.FileInfo property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs AssemblyInfoChanging = new(nameof(Assembly<WordType>.FileInfo));
        /// <summary>
        /// Arguments for the changed event for the _serializationInfo.FileInfo property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs AssemblyInfoChanged = new(nameof(Assembly<WordType>.FileInfo));

        /// <summary>
        /// Extension of the File type for this Programs language
        /// </summary>
        public virtual string Extension { get => _serializationInfo.Extension; }
        #endregion

        #region OPERATORS
        /// <summary>
        /// Get a reference to a particular word at a given location in the Assembly
        /// </summary>
        /// <param name="index">The location of the word to access</param>
        /// <returns>A reference to the word at the index if one exists</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index entered is out of bounds for the Words collection</exception>
        public abstract WordType this[int index] { get; }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct and initialize an assembly with a file name and extension assigned to this assembly type
        /// </summary>
        /// <param name="assemblyName">The name of the assemly's file</param>
        /// <param name="assemblyExtension">The extension given to this assembly types files</param>
        public Assembly(string assemblyName, string assemblyExtension) =>
            _serializationInfo = new(assemblyName, assemblyExtension);
        /// <summary>
        /// Construct and initialize an assembly with its file name, the extension assigned to this assembly type, and build the assembly from a provided program text
        /// </summary>
        /// <param name="assemblyName">The name of the assemly's file</param>
        /// <param name="assemblyExtension">The extension given to this assembly types files</param>
        /// <param name="programText">Array of strings representing the lines of the program to be assembled</param>
        public Assembly(string assemblyName, string assemblyExtension, string[] programText)
        {
            _serializationInfo = new(assemblyName, assemblyExtension);

            this.AssembleProgram(assemblyName, programText);
        }
        /// <summary>
        /// Construct and initialize an assembly with its file name, the extension assigned to this assembly type, and with a collection of Words to copy
        /// </summary>
        /// <param name="assemblyName">The name of the assembly's file</param>
        /// <param name="assemblyExtension">The extension given to this assembly types files</param>
        /// <param name="words">A collection of <typeparamref name="WordType"/> to copy from another assembly</param>
        public Assembly(string assemblyName, string assemblyExtension, Collection<WordType> words) =>
            (_serializationInfo, Words) = (new(assemblyName, assemblyExtension), new(words));
        #endregion

        #region METHODS
        /// <summary>
        /// <para>Method that will parse the input into a binary assembly and store the assembly in the assemblies collection</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program to be assembled</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract bool AssembleProgram(string assemblyName, string[] programText);

        /// <summary>
        /// <para>Method that will parse the input and return a binary assembly</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <remarks>Will not store the program in the program collection. Primarily for testing</remarks>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program to be assembled</param>
        public abstract Assembly<WordType> ParseProgram(string assemblyName, string[] programText);

        /// <summary>
        /// Default implemenation for the <seealso cref="IAssembly{WordType}"/> interface's SerializeAssembly signature. Serializes the assembly to disk
        /// </summary>
        /// <returns>Returns true if the operation succeeds, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thown if the mothod is called when the assembly's FileInfo isn't set</exception>
        public virtual async Task<bool> SerializeAssembly()
        {
            if (FileInfo == null)
                throw new InvalidOperationException($"Cannot serialize assembly {AssemblyName} because it's FileInfo is not set");

            return await Task.Run<bool>(() =>
            {
                WordType wordType = new();

                byte[] wordBuffer = new byte[wordType.GetByteCount()];
                List<byte> buffer = new(capacity: Words.Count * wordBuffer.Length);

                foreach (WordType word in Words)
                {
                    word.WriteBigEndian(wordBuffer);
                    buffer.AddRange(wordBuffer);
                }

                using BinaryWriter writer = new(FileInfo.Open(FileMode.Create));
                writer.Write(buffer.ToArray());

                return true;
            });
        }

        /// <summary>
        /// Default implementation for the <seealso cref="IAssembly{WordType}"/> interface's DeserializeAssembly signature. Deserializes the assembly from disk
        /// </summary>
        /// <param name="info">Info about the saved assembly to deserialize from disk</param>
        /// <returns>True if operation succeeds, false otherwise</returns>
        public virtual async Task<bool> DeserializeAssembly(FileInfo info)
        {
            FileInfo ??= info;

            return await Task.Run<bool>(() =>
            {
                byte[] bytes;

                using (BinaryReader reader = new(FileInfo.Open(FileMode.Open)))
                {
                    bytes = new byte[reader.BaseStream.Length];

                    reader.Read(bytes);
                }

                AssemblyName = FileInfo.Name;

                WordType wordType = new();

                for (int b = 0; b < bytes.Length; b += wordType.GetByteCount())
                {
                    Words[b] = WordType.ReadBigEndian(bytes, b, false);
                }

                return true;
            });
        }
        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordType}"/>
    /// </summary>
    /// <remarks>
    /// Adds support for programs of a dynamic length with the use of generic containers as valid type paramaters
    /// </remarks>
    /// <typeparam name="Assembly">A type that derives implements the <seealso cref="IAssembly{WordType}"/> interface. Recommended that type is derived from <seealso cref="UVSim.Assembly{WordType}"/> for ease of use</typeparam>
    /// <typeparam name="WordType">An integer type that specifies the word size used in the architecture</typeparam>
    public abstract class AssembliesManagement_Interface<Assembly, WordType>
        where Assembly : IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region FIELDS
        #endregion

        #region PROPERTIES
        /// <summary>
        /// The <seealso cref="Assembly{WordType}"/>s in the manager's collection
        /// </summary>
        public ObservableCollection<Assembly> LoadedAssemblies { get; } = new();

        /// <summary>
        /// The number of <seealso cref="Assembly{WordType}"/>s in the manager's collection
        /// </summary>
        public int LoadedAssembliesCount { get => LoadedAssemblies.Count; }

        /// <summary>
        /// Gets the assembly at the end of the collection
        /// </summary>
        public Assembly Last { get => LoadedAssemblies.Last(); }
        #endregion

        #region OPERATORS
        /// <summary>
        /// Retrieve a particular <seealso cref="Assembly{WordType}"/> at a given index
        /// </summary>
        /// <param name="index">The location (index) of the <seealso cref="Assembly{WordType}"/> to retrieve</param>
        /// <returns>The retieved <seealso cref="Assembly{WordType}"/> if one was obtained</returns>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if the index entered is out of bounds of the manager's collection</exception>
        public virtual Assembly this[int index]
        {
            get
            {
                if (LoadedAssemblies[index] == null)
                    throw new System.IndexOutOfRangeException("There is no program with that index");

                return LoadedAssemblies[index];
            }
        }
        #endregion

        #region CONSTRUCTORS

        #endregion

        #region METHODS
        /* Moved these into Assembly itself
        /// <summary>
        /// <para>Method that will parse the input into a binary assembly and store the assembly in the assemblies collection</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program</param>
        /// <returns>true if operation succeeds, false otherwise</returns>
        public abstract bool CreateAssembly(string assemblyName, string[] programText);

        /// <summary>
        /// <para>Method that will parse the input and return a binary assembly</para>
        /// <para>Expectes the data words to be proceeded by a # character. Opcodes to be entered without sign or the # character</para>
        /// </summary>
        /// <remarks>Will not store the program in the program collection. Primarily for testing</remarks>
        /// <param name="assemblyName">The name of the assembly being created</param>
        /// <param name="programText">Array of strings representing the lines of the program to assemble</param>
        public abstract Assembly<WordType> ParseProgram(string assemblyName, string[] programText);
        */

        /// <summary>
        /// Adds an assembly to the collection
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to add</param>
        public abstract bool TryCreateAssembly(string assemblyName);

        /// <summary>
        /// Adds an assembly to the collection and initializes it with a program
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to add</param>
        /// <param name="programText">Array of strings representing the lines of the program to assemble</param>
        /// <returns></returns>
        public abstract bool TryCreateAssembly(string assemblyName, string[] programText);

        /// <summary>
        /// Adds an assembly to the collection and initializes it by copying the words from another assembly
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to add</param>
        /// <param name="words">The collection of words to copy</param>
        /// <returns></returns>
        public abstract bool TryCreateAssembly(string assemblyName, Collection<WordType> words);

        /// <summary>
        /// Removes an assembly from the collection
        /// </summary>
        /// <param name="index">The index of the <seealso cref="Assembly{WordType}"/> to remove</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the method is called when there are no <seealso cref="Assembly{WordType}"/>s in the manager's collection</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index entered is out of bounds of the manager's collection</exception>
        public virtual void DeleteAssembly(int index)
        {
            if (LoadedAssemblies.Count == 0)
                throw new System.InvalidOperationException("Program collection is null. Initialize the colletion first");

            if (index > LoadedAssemblies.Count)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            LoadedAssemblies.RemoveAt(index);
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
        /// Saves a binary copy of the assembly(s) to disk
        /// </summary>
        /// <param name="indexes">Specify the index(s) of the assemblie(s) that are to be serialized</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of assemblies that failed to serialize</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the direcotry provided is null or empty</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if method is called when the assemblies collection is empty, there is nothing to serialize</exception>
        public virtual async Task<List<Assembly>> SerializeAssemblies(int[] indexes, CancellationToken token)
        {
            List<Assembly> failedAssemblies = new();

            await Parallel.ForEachAsync(indexes, token, async (index, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = LoadedAssemblies[index].SerializeAssembly();

                await task;

                if (task.Result)
                    failedAssemblies.Add(LoadedAssemblies[index]);
            });

            return failedAssemblies;
        }

        /// <summary>
        /// Saves all of te assemblies in the collection
        /// </summary>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the assemblies that failed to serialize</returns>
        public virtual async Task<List<Assembly>> SerializeAll(CancellationToken token)
        {
            List<Assembly> failedAssemblies = new();

            await Parallel.ForEachAsync(LoadedAssemblies, token, async (assembly, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = assembly.SerializeAssembly();

                await task;

                if (task.Result)
                    failedAssemblies.Add(assembly);
            });

            return failedAssemblies;
        }

        /// <summary>
        /// Attempts to load a program(s) from the filePaths provided
        /// </summary>
        /// <param name="infos">An array of FileInfo objects containing the file paths and file names of the assemblies to be loaded</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the FileInfos that failed to properly deserialize</returns>
        public virtual async Task<List<FileInfo>> DeserializeAssemblies(FileInfo[] infos, CancellationToken token)
        {
            List<FileInfo> failedFiles = new();

            await Parallel.ForEachAsync(infos, token, async (info, token) =>
            {
                token.ThrowIfCancellationRequested();

                TryCreateAssembly(info.Name);

                Task<bool> task = LoadedAssemblies.Last().DeserializeAssembly(info);

                await task;

                if (!task.Result)
                {
                    LoadedAssemblies.RemoveAt(LoadedAssembliesCount - 1);
                    failedFiles.Add(info);
                }
            });

            return failedFiles;
        }
        #endregion

        #region OVERRIDES
        /// <summary>
        /// Provides a string representation of the objects state for output
        /// </summary>
        /// <returns>string formatted to provide information about the object's state</returns>
        public override string ToString()
        {
            string output = $"---Assemblies Collection---\n" +
                $"Programs in collection: {LoadedAssemblies.Count}\n" +
                "programs:\n[\n";

            int i = 0;

            foreach (Assembly program in LoadedAssemblies)
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
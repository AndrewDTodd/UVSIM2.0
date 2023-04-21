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
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UVSim
{
    /**** Use abstract class instead ****
    ///<summary>Interface that defines the common functionality expected of all Assemblies used by the application that have a fixed size constraint</summary>
    public interface IAssemblyFixedLength<WordType> : IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        /// <summary>
        /// Retruns the maximum size of an assembly in terms of the number of words it can contain
        /// </summary>
        public abstract int WordsCount { get; init; }
    }
    */
    /// <remarks>*** Used for assemblies that have a fixed size constraint</remarks>
    /// <inheritdoc/>
    public abstract partial class Assembly_FixedSize : UVSim.Assembly
    {
        /* These are now inherited directly from Assembly
        #region FIELDS
        [ObservableProperty]
        protected bool _upToDate = true;

        protected SerializationInfo _serializationInfo;

        //protected WordCollection? _words;
        #endregion

        #region PROPERTIES
        public bool UpToDate { get => _upToDate; }

        public ObservableCollection<WordType>? Words { get; } = new();

        public abstract int Count { get; }

        public string AssemblyName { get => _serializationInfo.FileName; set => _serializationInfo.FileName = value; }

        public string AssemblyExtension { get => _serializationInfo.Extension; }

        public FileInfo? FileInfo { get => _serializationInfo.FileInfo; set => _serializationInfo.FileInfo = value; }

        public int WordsCount { get; init; }
        #endregion

        #region OPERATORS
        public abstract ref WordType this[int index] { get; }
        #endregion
        */
        #region PROPERTIES
        /// <summary>
        /// The fixed number of words that can be in this type of assembly
        /// </summary>
        public int WordsCount { get; init; }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct and initialize an assembly with a file name and extension assigned to this assembly type as well as the limit to how many words this type of assembly supports
        /// </summary>
        /// <param name="assemblyName">The name of the assemly's file</param>
        /// <param name="assemblyExtension">The extension given to this assemly types files</param>
        /// <param name="assemblyCapacity">The maximum number of words supported by this assembly type</param>
        /// <param name="bytesPerWord">How many bytes are in an addressable word in this architecture. For example for a 16bit architecture this is 2</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public Assembly_FixedSize(string assemblyName, string assemblyExtension, int assemblyCapacity, int bytesPerWord, InstructionSet_Interface instructionSet) : 
            base(assemblyName, assemblyExtension, bytesPerWord, instructionSet) =>
            (WordsCount) = (assemblyCapacity);

        /// <summary>
        /// Construct and initialize an assembly with its file name, the extension assigned to this assembly type as well as the limit to how many words this type of assembly supoorts, and build the assembly from a provided program text
        /// </summary>
        /// <param name="assemblyName">The name of the assemly's file</param>
        /// <param name="assemblyExtension">The extension given to this assemly types files</param>
        /// <param name="programText">Array of strings representing the program to be assembled</param>
        /// <param name="assemblyCapacity">The maximum number of words supported by this assembly type</param>
        /// <param name="bytesPerWord">How many bytes are in an addressable word in this architecture. For example for a 16bit architecture this is 2</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public Assembly_FixedSize(string assemblyName, string assemblyExtension, string[] programText, int assemblyCapacity, int bytesPerWord, InstructionSet_Interface instructionSet) : 
            base(assemblyName, assemblyExtension, programText, bytesPerWord, instructionSet) =>
            (WordsCount) = (assemblyCapacity);

        /// <summary>
        /// Construct and initialize an assembly with its file name, the extension assigned to this assembly type, and with a collection of Words to copy
        /// </summary>
        /// <param name="assemblyName">The name of the assembly's file</param>
        /// <param name="assemblyExtension">The extension given to this assembly types files</param>
        /// <param name="words">A collection of bytes to copy from another assembly</param>
        /// <param name="assemblyCapacity">The maximum number of words supported by this assembly type</param>
        /// <param name="bytesPerWord">How many bytes are in an addressable word in this architecture. For example for a 16bit architecture this is 2</param>
        /// <param name="instructionSet">The instruction set this assembly is to build in</param>
        public Assembly_FixedSize(string assemblyName, string assemblyExtension, Collection<byte> words, int assemblyCapacity, int bytesPerWord, InstructionSet_Interface instructionSet) : 
            base(assemblyName, assemblyExtension, words, bytesPerWord, instructionSet) =>
            (WordsCount) = (assemblyCapacity);
        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface"/>
    /// </summary>
    /// <remarks>
    /// ***Supports programs of a fixed maximum length!!!*** Programs can not exede the maximum size specified at initialization
    /// </remarks>
    public abstract class AssembliesManagementFixedSize_Interface : AssembliesManagement_Interface
    {
        #region PROPERTIES
        /// <summary>
        /// Returns the max size of an assembly in bytes
        /// </summary>
        protected int AssemblySize {get; init;}

        /// <summary>
        /// The <seealso cref="Assembly_FixedSize"/>s in the manager's collection
        /// </summary>
        public new ObservableCollection<Assembly_FixedSize> LoadedAssemblies { get; } = new();
        #endregion

        #region OPERATORS
        /// <summary>
        /// Retrieve a particular <seealso cref="Assembly_FixedSize"/> at a given index
        /// </summary>
        /// <param name="index">The location (index) of the <seealso cref="Assembly_FixedSize"/> to retrieve</param>
        /// <returns>The retieved <seealso cref="Assembly_FixedSize"/> if one was obtained</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index entered is out of bounds of the manager's collection</exception>
        public new virtual Assembly_FixedSize this[int index]
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
        /// <summary>
        /// Initialize the object
        /// </summary>
        /// <param name="assemblySize">The maximum size of the assembly in bytes</param>
        protected AssembliesManagementFixedSize_Interface(int assemblySize, InstructionSet_Interface instructionSet) : base(instructionSet) =>
            AssemblySize = assemblySize;
        #endregion

        #region OVERRIDES
        /// <summary>
        /// Output the state of the collection in a formated string
        /// </summary>
        /// <returns>string formatted with collection's state</returns>
        public override string ToString()
        {
            string output = $"---Assemblies Collection---\nAssembly max size: {AssemblySize}\n" +
                $"Programs in collection: {LoadedAssemblies.Count}\n" +
                "programs:\n[\n";

            int i = 0;

            foreach(Assembly_FixedSize program in LoadedAssemblies)
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
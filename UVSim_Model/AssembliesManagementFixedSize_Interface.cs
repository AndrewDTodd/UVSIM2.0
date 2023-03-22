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
    public interface IAssemblyFixedLength<WordType> : IAssembly<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        /// <summary>
        /// Retruns the maximum size of an assembly in the number of words it contains
        /// </summary>
        public abstract int AssemblyWords { get; init; }
    }

    public abstract class Assembly_FixedSize<WordCollection, WordType> : IAssemblyFixedLength<WordType>
        where WordCollection : IList<WordType>
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
    public abstract class AssembliesManagementFixedSize_Interface<AssembliesCollection, FixedLengthAssembly, WordType> : AssembliesManagement_Interface<AssembliesCollection, FixedLengthAssembly, WordType>
        where AssembliesCollection : IList<FixedLengthAssembly>, new()
        where FixedLengthAssembly : IAssemblyFixedLength<WordType>
        where WordType : IBinaryInteger<WordType>, new()
    {
        #region PROPERTIES
        /// <summary>
        /// Returns the max size of an assembly in bytes, null if the architecture provides no limitation
        /// </summary>
        protected int? AssemblySize {get; init;}
        #endregion

        #region OPERATORS
        public new virtual FixedLengthAssembly this[int index]
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
        protected AssembliesManagementFixedSize_Interface(int assemblySize) =>
            AssemblySize = assemblySize;
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
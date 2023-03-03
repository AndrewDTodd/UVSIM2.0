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
    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordSize}"/>
    /// </summary>
    /// <remarks>
    /// Adds support for programs of a dynamic length with the use of generic containers as valid type paramaters
    /// </remarks>
    /// <typeparamref name="ProgramsCollection"/> A collection that implements the <seealso cref="IList{T}"/> interface and a public parameterless constructor
    /// <typeparamref name="Program"/> A collection that implements the <seealso cref="IList{T}"/> interface and a public parameterless constructor
    /// <typeparamref name="WordType"/> An intager type that specifies the word size used in the architecture
    public abstract class ProgramsManagement_Interface<ProgramsCollection, Program, WordType> : ProgramsManagementFixedSize_Interface<ProgramsCollection, Program, WordType>
        where ProgramsCollection : IList<Program>, new() where Program : IList<WordType>, new() where WordType : IBinaryInteger<WordType>, new()
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Initialize the object
        /// </summary>
        protected ProgramsManagement_Interface()
        {
            ProgramSize = null;
            loadedPrograms = new();
        }
        #endregion
    }
}
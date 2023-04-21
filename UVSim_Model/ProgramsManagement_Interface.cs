/*
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
//using Microsoft.IdentityServer.Management.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UVSim
{
    /// <summary>
    /// Simple container for file info needed for GUI display as well as serialization
    /// </summary>
    public partial class SerializationInfo : ObservableObject
    {
        /// <summary>
        /// Name given to the file object. Will be used to save file to disk, or will be set when opening file from disk
        /// </summary>
        [ObservableProperty]
        private string _fileName;
        /// <summary>
        /// Extension given to the file type
        /// </summary>
        [ObservableProperty]
        private string _extension;
        /// <summary>
        /// <seealso cref="FileInfo"/> object used for serialization of the file. Primarily used for the FullName property contained within 
        /// </summary>
        [ObservableProperty]
        private FileInfo? _fileInfo;

        /// <summary>
        /// Construct and initialize the serialization info structure
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="extension">The extension given to the file for serialization to disk</param>
        public SerializationInfo(string fileName, string extension) =>
            (_fileName, _extension) = (fileName, extension);
    }

    /// <summary>
    /// Contains information the application tracks on a per line basis in a human readable text assemly language program
    /// </summary>
    public partial class LineData : ObservableObject
    {
        #region FIELDS
        /// <summary>
        /// Has the line in question been changed since last save
        /// </summary>
        [ObservableProperty]
        protected bool _changed = false;
        /// <summary>
        /// The actual text in the line of the text file
        /// </summary>
        [ObservableProperty]
        protected string _text;
        #endregion
        //old property accessors from before ObservableProperty attribute use
        /*
         **** Not needed after fields marked as Obserbable Properties ****
        #region PROPERTIES
        /// <summary>
        /// Property to retrieve the value of the _changed attribute
        /// </summary>
        public bool Changed { get => _changed; }
        /// <summary>
        /// Property to retrive the text of the line
        /// </summary>
        public string Text { get => _text; set => (_text, _changed) = (value, true); }
        #endregion
        */

        #region CONSTRUCTORS
        /// <summary>
        /// Builds the <seealso cref="LineData"/> object with a defined text content
        /// </summary>
        /// <param name="text">A string containing the text for initializing the line with</param>
        public LineData(string text) =>
            _text = text;
        #endregion
    }

    /**** Switching to using abstract class as interface alone ****
    /// <summary>
    /// Interface that defines the unified functionality expected of every human readable syntactic programing language used in the application
    /// </summary>
    public interface IProgram
    {
        #region PROPERTIES
        /// <summary>
        /// Property to check whether or not the file is marked as needing to be saved
        /// </summary>
        public bool NeedsSave { get; set; }
        /// <summary>
        /// Property to obtain the lines collection of the <seealso cref="IProgram{WordType}"/>
        /// </summary>
        public ObservableCollection<LineData> Lines { get; set; }
        /// <summary>
        /// Gets the program at the end of the collection
        /// </summary>
        public IAssembly<WordType>? Assembly { get; set; }
        /// <summary>
        /// Property to obtain the text of the <seealso cref="IProgram{WordType}"/>
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Property to get or change the name of the file
        /// </summary>
        public string ProgramName { get; set; }
        /// <summary>
        /// Property to obtain the extension given to the program language file type
        /// </summary>
        public string Extension { get;}
        /// <summary>
        /// Property to get or change the stored <seealso cref="System.IO.FileInfo"/> object
        /// </summary>
        public FileInfo? FileInfo { get; set; }
        #endregion

        #region OPERATORS
        /// <summary>
        /// Operator for indexing into the programs <seealso cref="LineData"/> collection
        /// </summary>
        /// <param name="index">The 0 based index of the <seealso cref="LineData"/> to obtain</param>
        /// <returns></returns>
        public LineData this[int index]
        {
            get;
        }
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Signature for a method that will attempt to add a new <seealso cref="LineData"/> object to the underlying collection to the program
        /// </summary>
        /// <param name="text">The text to initialize the new <seealso cref="LineData"/> object</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public bool TryAddLine(string text);
        /// <summary>
        /// Signature for a method to remove a <seealso cref="LineData"/> object from the program's underlying collection
        /// </summary>
        /// <param name="index">The 0 based index of the <seealso cref="LineData"/> object in the collection to remove</param>
        public void RemoveLine(int index);
        /// <summary>
        /// Signature for a method to chnage the text of a <seealso cref="LineData"/> object in the program's collection
        /// </summary>
        /// <param name="index">The 0 based index of the <seealso cref="LineData"/> object to change</param>
        /// <param name="newText">The new text for the line</param>
        public void SetLine(int index, string newText);
        /// <summary>
        /// Signature for a method that attempts to change the entire collection of <seealso cref="LineData"/> in the program
        /// </summary>
        /// <param name="newLines">Array containing the new lines of text for the program</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public bool TrySetContent(string[] newLines);
        /// <summary>
        /// Signature for method that attempts to change to text of the program and set the <seealso cref="LineData"/> collection relative to the new text provided
        /// </summary>
        /// <param name="newText">String that represents the whole program text</param>
        /// <returns></returns>
        public bool TrySetContent(string newText);
        #endregion

        #region SERIALIZATION
        /// <summary>
        /// Implementation to serialize the program to disk
        /// </summary>
        /// <returns>True if serialized successfully, false otherwise</returns>
        public Task<bool> SerializeProgram();
        /// <summary>
        /// Implementation to deserialize the program from disk
        /// </summary>
        /// <param name="info"><seealso cref="System.IO.FileInfo"/> object containing the file's full path and other info. Will be ignored if the program already contains a <seealso cref="System.IO.FileInfo"/></param>
        /// <returns>True if deserialized successfully, false otherwise</returns>
        public Task<bool> DeserializeProgram(FileInfo info);
        #endregion
    }
    */

    /// <summary>
    /// Defines an abstract class that concrete types will derive from to define program types digestable by the application
    /// </summary>
    public abstract partial class Program : ObservableObject
    {
        #region FIELDS
        /// <summary>
        /// Attribute representing whether or not the <seealso cref="Program"/> has been changed since the last serialization
        /// </summary>
        [ObservableProperty]
        protected bool _needsSave = true;
        /// <summary>
        /// String that contains the entire text of the program
        /// </summary>
        [ObservableProperty]
        protected string _text = "";
        /// <summary>
        /// The <seealso cref="SerializationInfo"/> structure aggregated in this object that contains this Program's file info for serialization and deserialization
        /// </summary>
        protected SerializationInfo _serializationInfo;
        /// <summary>
        /// The <seealso cref="UVSim.Assembly"/> this <seealso cref="Program"/> is associated with
        /// </summary>
        [ObservableProperty]
        protected UVSim.Assembly? _assembly;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// The collection of <seealso cref="LineData"/> that represent the lines of the Program
        /// </summary>
        public ObservableCollection<LineData> Lines { get; set; } = new();

        //Previouse implementation of Lines property
        /*
         **** Obsolete as now using an ObservableCollection ****
        /// <inheritdoc cref="_lines"/>
        public IList<LineData> Lines
        {
            get => _lines;
            set
            {
                if (!EqualityComparer<IList<LineData>>.Default.Equals(_lines, value))
                {
                    OnLinesChanging(value);
                    OnPropertyChanging(LinesChanging);
                    _lines = (LinesContainer)value;
                    OnLinesChanged(value);
                    OnPropertyChanged(LinesChanged);
                }
            }
        }
        /// <summary>Executes the logic for when <see cref="Lines"/> is changing.</summary>
        partial void OnLinesChanging(IList<LineData> value);
        /// <summary>Executes the logic for when <see cref="Lines"/> just changed.</summary>
        partial void OnLinesChanged(IList<LineData> value);
        /// <summary>
        /// Arguments for the changing event for the <seealso cref="Lines"/> property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs LinesChanging = new (nameof(Lines));
        /// <summary>
        /// Arguments for the changed event for the <seealso cref="Lines"/> property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs LinesChanged = new (nameof(Lines));
        */

        /// <summary>
        /// Name given to this Program's file
        /// </summary>
        public virtual string ProgramName
        {
            get => _serializationInfo.FileName;
            set
            {
                if (!EqualityComparer<string>.Default.Equals(_serializationInfo.FileName, value))
                {
                    OnProgramNameChanging(value);
                    OnPropertyChanging(ProgramNameChanging);
                    _serializationInfo.FileName = value;
                    OnProgramNameChanged(value);
                    OnPropertyChanged(ProgramNameChanged);
                }
            }
        }
        /// <summary>Executes the logic for when _serializationInfo.FileName is changing.</summary>
        partial void OnProgramNameChanging(string value);
        /// <summary>Executes the logic for when _serializationInfo.FileName just changed.</summary>
        partial void OnProgramNameChanged(string value);
        /// <summary>
        /// Arguments for the changing event for the _serializationInfo.FileName property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs ProgramNameChanging = new (nameof(ProgramName));
        /// <summary>
        /// Arguments for the changed event for the _serializationInfo.FileName property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs ProgramNameChanged = new (nameof(ProgramName));

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
                    OnFileInfoChanging(value);
                    OnPropertyChanging(FileInfoChanging);
                    _serializationInfo.FileInfo = value;
                    OnFileInfoChanged(value);
                    OnPropertyChanged(FileInfoChanged);
                }
            }
        }
        /// <summary>Executes the logic for when _serializationInfo.FileInfo is changing.</summary>
        partial void OnFileInfoChanging(FileInfo? value);
        /// <summary>Executes the logic for when _serializationInfo.FileInfo just changed.</summary>
        partial void OnFileInfoChanged(FileInfo? value);
        /// <summary>
        /// Arguments for the changing event for the _serializationInfo.FileInfo property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs FileInfoChanging = new (nameof(FileInfo));
        /// <summary>
        /// Arguments for the changed event for the _serializationInfo.FileInfo property
        /// </summary>
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs FileInfoChanged = new (nameof(FileInfo));

        /// <summary>
        /// Extension of the File type for this Programs language
        /// </summary>
        public virtual string Extension { get => _serializationInfo.Extension; }

        /**** No longer needed or sufficient after marking fields as ObservableProperty needing that functionality for those that are manually implemented ****
        /// <summary>
        /// Property accessor to check if the program is in need of saving
        /// </summary>
        public virtual bool NeedsSave { get => _needsSave; }
        /// <summary>
        /// The container of <seealso cref="LineData"/>
        /// </summary>
        public abstract IList<LineData> Lines { get; }
        /// <summary>
        /// The programs text
        /// </summary>
        public string Text { get => _text; }
        /// <summary>
        /// Name given to this Program's file
        /// </summary>
        public virtual string FileName { get => _serializationInfo._fileName; set => _serializationInfo._fileName = value; }
        /// <summary>
        /// <seealso cref="System.IO.FileInfo"/> for the Program's file 
        /// </summary>
        public virtual FileInfo? FileInfo { get => _serializationInfo._fileInfo; set => _serializationInfo._fileInfo = value; }
        /// <summary>
        /// Extension of the File type for this Programs language
        /// </summary>
        public virtual string Extension { get => _serializationInfo._extension; }
        */
        #endregion

        #region OPERATORS
        /// <summary>
        /// Operator to index into the Program's collection of lines
        /// </summary>
        /// <param name="index">The 0 based index of the <seealso cref="LineData"/> to obtain</param>
        /// <returns>The <seealso cref="LineData"/> object at the entered index if one exists</returns>
        /// <exeption cref="ArgumentOutOfRangeException">Thrown if the index entered is out of bounds for the Program's <seealso cref="LineData"/> collection</exeption>
        public virtual LineData this[int index]
        {
            get => Lines[index];
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initialize the <seealso cref="Program"/> with a file name and an extension given to this type of program
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="ext">Extension used for this Program's language file type</param>
        public Program(string fileName, string ext) =>
            _serializationInfo = new(fileName, ext);
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Default implementation for the <seealso cref="Program"/> interface's TryAddLine signature. Adds a line of text to the program
        /// </summary>
        /// <param name="text">Text to add to program's <seealso cref="LineData"/> collection</param>
        /// <returns>True if the operation is successfull, false otherwise</returns>
        public virtual bool TryAddLine(string text)
        {
            if (Activator.CreateInstance(typeof(LineData), new { text }) is LineData line)
            {
                Lines.Add(line);
                NeedsSave = true;

                Text = string.Join(Text, "\n" + text);

                if(Assembly != null)
                    Assembly.UpToDate = false;

                return true;
            }
            return false;
        }
        /// <summary>
        /// Default implementation for the <seealso cref="Program"/> interface's RemoveLine signature. Removes a <seealso cref="LineData"/> object from the collection
        /// </summary>
        /// <param name="index">The 0 based index to try and remove the <seealso cref="LineData"/> from</param>
        /// <exeption cref="ArgumentOutOfRangeException">Thrown if the index entered is out of bounds for the collection</exeption>
        public virtual void RemoveLine(int index)
        {
            Lines.RemoveAt(index);
            NeedsSave = true;

            var programText = new StringBuilder();

            foreach(var line in Lines)
            {
                programText.Append(line.Text + "\n");
            }

            programText.Remove(programText.Length - 1, 1);
            Text = programText.ToString();

            if(Assembly != null)
                Assembly.UpToDate = false;
        }
        /// <summary>
        /// Default imlementation for the <seealso cref="Program"/> interface's SetLine signature. Changes the text at a specified line
        /// </summary>
        /// <param name="index">The 0 based index of the <seealso cref="LineData"/> whose text is to be changed</param>
        /// <param name="newText">The text to replace the specified lines text with</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified index if out of bounds for the collection</exception>
        public virtual void SetLine(int index, string newText)
        {
            Lines[index].Text = newText;

            var programText = new StringBuilder();

            foreach (var line in Lines)
            {
                programText.Append(line.Text + "\n");
            }

            programText.Remove(programText.Length - 1, 1);
            Text = programText.ToString();

            if (Assembly != null)
                Assembly.UpToDate = false;
        }
        /// <summary>
        /// Default implementation for the <seealso cref="Program"/> interface's TrySetContent signature. Sets the lines of the program's text
        /// </summary>
        /// <param name="newLines">Array of new lines of text</param>
        /// <returns>True if operation suceeds, false otherwise</returns>
        public virtual bool TrySetContent(string[] newLines)
        {
            ObservableCollection<LineData> newCont = new();

            foreach(string text in newLines)
            {
                if (Activator.CreateInstance(typeof(LineData), new { text }) is LineData line)
                {
                    newCont.Add(line);
                }
                else
                    return false;
            }

            Lines = newCont;
            NeedsSave = true;

            var programText = new StringBuilder();

            foreach (var line in Lines)
            {
                programText.Append(line.Text + "\n");
            }

            programText.Remove(programText.Length - 1, 1);
            Text = programText.ToString();

            if(Assembly != null)
                Assembly.UpToDate = false;
            return true;
        }
        /// <summary>
        /// Default implementation for the <seealso cref="Program"/> interface's TrySetContent signature. Sets the lines of the program's text
        /// </summary>
        /// <param name="newText">Program's text</param>
        /// <returns>True if operation suceeds, false otherwise</returns>
        public virtual bool TrySetContent(string newText)
        {
            return TrySetContent(MyRegex().Split(newText));
        }
        #endregion

        #region SERIEALIZATION
        /// <summary>
        /// Default implementation for the <seealso cref="Program"/> interface's SerializeProgram signature. Serializes the program to disk
        /// </summary>
        /// <returns>True if operation succeeded, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <seealso cref="System.IO.FileInfo"/> for this program is null</exception>
        public virtual async Task<bool> SerializeProgram()
        {
            if (FileInfo == null)
                throw new InvalidOperationException("No file path specified for the program trying to save");

            using StreamWriter writer = new(new FileStream(FileInfo.FullName, FileMode.Create));

            writer.AutoFlush = false;

            foreach(LineData line in Lines)
            {
                await writer.WriteLineAsync(line.Text);
            }

            await writer.FlushAsync();

            NeedsSave = false;

            return true;
        }
        /// <summary>
        /// Default implementation of the <seealso cref="Program"/> interface's DeserializeProgram signature. Deserializes a program from disk
        /// </summary>
        /// <param name="info"><seealso cref="System.IO.FileInfo"/> object pointing to the program on disk</param>
        /// <returns>True if operation succeeded, false otherwise</returns>
        /// <exception cref="InvalidDataException">Thrown if any line from the program on disk could not be deserialized and added to the collection</exception>
        public virtual async Task<bool> DeserializeProgram(FileInfo info)
        {
            FileInfo ??= info;

            using StreamReader reader = new(new FileStream(FileInfo.FullName, FileMode.Open));

            Task<string> result = reader.ReadToEndAsync();

            Lines.Clear();

            await result;

            foreach(string line in result.Result.Split('\n'))
            {
                if(!TryAddLine(line))
                    throw new InvalidDataException($"Was unable to add line ({line}) to the programs line collection");
            }

            NeedsSave = false;

            ProgramName = FileInfo.Name;

            return true;
        }

        [GeneratedRegex("(?<=[\\n])")]
        private static partial Regex MyRegex();
        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface"/>
    /// </summary>
    public abstract partial class ProgramsManagement_Interface : ObservableObject
    {
        #region FIELDS
        /**** Made obsolete by addition of ObservableCollection in properties
        /// <summary>
        /// Dictionary of objects implementing the <seealso cref="IProgram"/> interface
        /// </summary>
        [ObservableProperty]
        protected ProgramsCollection _programs = new();
        */
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Collection of objects implementing the <seealso cref="Program"/> interface
        /// </summary>
        public ObservableCollection<Program> Programs { get; } = new();

        /// <summary>
        /// Gets the number of programs in the collecton
        /// </summary>
        public int ProgramsCount { get=> Programs.Count; }

        /// <summary>
        /// Gets the program at the end of the collection
        /// </summary>
        public Program Last { get => Programs.Last(); }
        #endregion

        #region OPERATORS
        /// <summary>
        /// Operator to index into the manager's collection of prgrams
        /// </summary>
        /// <param name="index">The index that relates to the program to obtain</param>
        /// <returns>The Program in the Dictionary if one exists under the specified key</returns>
        /// <exception cref="ArgumentNullException">Thrown if the key entered is null</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the Manager's Dictionary of Programs doesn't contain the entered key</exception>
        public virtual Program this[int index]
        {
            get => Programs[index];
        }
        /**** Using indexed collection now, not dictionary ****
        /// <summary>
        /// Operator to index into the manager's collection of prgrams
        /// </summary>
        /// <param name="index">The 0 based index that relates to the <seealso cref="KeyValuePair"/> to obtain</param>
        /// <returns>The <seealso cref="KeyValuePair"/> in the Dictionary if one exists under the specified index</returns>
        /// <exception cref="ArgumentNullException">Thrown if the index entered is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index entered is out of bounds for the Manager's Dictionary of Programs</exception>
        public virtual KeyValuePair<string, Program> this[int index]
        {
            get => Programs.ElementAt(index);
        }
        */
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Attempts to create a new instace of <seealso cref="Program"/> specified for the manager and add it to the manager's dictionary of programs
        /// </summary>
        /// <param name="programName">A name to be given to this new program</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public abstract bool TryCreateNewProgram(string programName);

        /// <summary>
        /// Delete a program from the collection at the specified index
        /// </summary>
        /// <param name="index">The index of the program to delete</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the method is called when the collection is empty</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the specified index is not in the range of the collection</exception>
        public virtual void DeleteProgram(int index)
        {
            if (Programs.Count == 0)
                throw new System.InvalidOperationException("Program collection is null. Initialize the colletion first");

            if (index > Programs.Count)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            Programs.RemoveAt(index);
        }

        /// <summary>
        /// Serialize a selection of the programs in the collection
        /// </summary>
        /// <param name="indexes">The indexes of the programs to be saved</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that wern't serialized</returns>
        public virtual async Task<List<Program>> SerializePrograms(int[] indexes, CancellationToken token)
        {
            List<Program> failedPrograms = new();

            await Parallel.ForEachAsync(indexes, token, async (index, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = Programs[index].SerializeProgram();

                await task;

                if (!task.Result)
                    failedPrograms.Add(Programs[index]);
            });

            return failedPrograms;
        }

        /// <summary>
        /// Serializes all of the programs in the collection
        /// </summary>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that failed to serialize</returns>
        public virtual async Task<List<Program>> SerializeAll(CancellationToken token)
        {
            List<Program> failedPrograms = new();

            await Parallel.ForEachAsync(Programs, token, async (program, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = program.SerializeProgram();

                await task;

                if (task.Result)
                    failedPrograms.Add(program);
            });

            return failedPrograms;
        }

        /// <summary>
        /// Deserializes a set of programs from the FileInfo objects provided
        /// </summary>
        /// <param name="infos">Array of FileInfo pertaining to the programs to load</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the FileInfo objects that failed to deserialize</returns>
        public virtual async Task<List<FileInfo>> DeserializePrograms(FileInfo[] infos, CancellationToken token)
        {
            List<FileInfo> failedFiles = new();

            await Parallel.ForEachAsync(infos, token, async (info, token) =>
            {
                token.ThrowIfCancellationRequested();

                TryCreateNewProgram(info.Name);

                Task<bool> task = Programs.Last().DeserializeProgram(info);

                await task;

                if (!task.Result)
                {
                    Programs.RemoveAt(ProgramsCount - 1);
                    failedFiles.Add(info);
                }
            });

            return failedFiles;
        }
        #endregion
    }
}

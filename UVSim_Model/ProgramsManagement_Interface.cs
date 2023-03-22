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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UVSim
{
    /// <summary>
    /// Simple container for file info needed for GUI display as well as serialization
    /// </summary>
    public struct SerializationInfo
    {
        public string _fileName;
        public string _extension;
        public FileInfo? _fileInfo;
    }

    /// <summary>
    /// Contains information the application tracks on a per line basis in a text sytax assemly language program
    /// </summary>
    public class LineData
    {
        #region FIELDS
        protected bool _changed = false;
        
        protected string _text;
        #endregion

        #region PROPERTIES
        public bool Changed { get => _changed; }

        public string Text { get => _text; set => (_text, _changed) = (value, true); }
        #endregion

        #region CONSTRUCTORS
        public LineData(string text) =>
            _text = text;
        #endregion
    }

    /// <summary>
    /// Interface that defines the unified functionality expected of every human readable syntactic programing language used in the application
    /// </summary>
    public interface IProgram
    {
        #region PROPERTIES
        public bool NeedsSave { get; }
        public abstract IList<UVSim.LineData> Lines { get; }
        public string FileName { get; set; }
        public string Extension { get; }
        public FileInfo? FileInfo { get; set; }
        #endregion

        #region OPERATORS
        public LineData this[int index]
        {
            get;
        }
        #endregion

        #region PUBLIC_METHODS
        public bool TryAddLine(string text);

        public void RemoveLine(int index);

        public void SetLine(int index, string newText);

        public bool TrySetContent(string[] newLines);
        #endregion

        #region SERIALIZATION
        public Task<bool> SerializeProgram();
        public Task<bool> DeserializeProgram(FileInfo info);
        #endregion
    }

    /// <summary>
    /// Implements the <seealso cref="IProgram"/> interface and defines an abstract class that concrete types will derive from to define program types digestable by the application
    /// </summary>
    /// <typeparam name="LinesContainer"></typeparam>
    public abstract class Program<LinesContainer> : IProgram
        where LinesContainer : IList<UVSim.LineData>, new()
    {
        #region FIELDS
        protected bool _needsSave = true;

        protected LinesContainer _lines = new();

        protected SerializationInfo _serializationInfo;
        #endregion

        #region PROPERTIES
        public virtual bool NeedsSave { get => _needsSave; }
        public abstract IList<UVSim.LineData> Lines { get; }
        public virtual string FileName { get => _serializationInfo._fileName; set => _serializationInfo._fileName = value; }
        public virtual FileInfo? FileInfo { get => _serializationInfo._fileInfo; set => _serializationInfo._fileInfo = value; }
        public virtual string Extension { get => _serializationInfo._extension; }
        #endregion

        #region OPERATORS
        public virtual UVSim.LineData this[int index]
        {
            get => _lines[index];
        }
        #endregion

        #region CONSTRUCTORS
        public Program(string fileName, string ext) =>
            (_serializationInfo._fileName, _serializationInfo._extension) = (fileName, ext);
        #endregion

        #region PUBLIC_METHODS
        public virtual bool TryAddLine(string text)
        {
            if (Activator.CreateInstance(typeof(LineData), new { text }) is LineData line)
            {
                _lines.Add(line);
                _needsSave = true;
                return true;
            }
            return false;
        }

        public virtual void RemoveLine(int index)
        {
            _lines.RemoveAt(index);
            _needsSave = true;
        }

        public virtual void SetLine(int index, string newText)
        {
            _lines[index].Text = newText;
            _needsSave = true;
        }

        public virtual bool TrySetContent(string[] newLines)
        {
            LinesContainer newCont = new();

            foreach(string text in newLines)
            {
                if (Activator.CreateInstance(typeof(LineData), new { text }) is LineData line)
                {
                    newCont.Add(line);
                }
                else
                    return false;
            }

            _lines = newCont;
            _needsSave = true;
            return true;
        }
        #endregion

        #region SERIEALIZATION
        public virtual async Task<bool> SerializeProgram()
        {
            if (_serializationInfo._fileInfo == null || string.IsNullOrEmpty(_serializationInfo._fileInfo.FullName))
                throw new InvalidOperationException("No file path specified for the program trying to save");

            using StreamWriter writer = new(new FileStream(_serializationInfo._fileInfo.FullName, FileMode.Create));

            writer.AutoFlush = false;

            foreach(UVSim.LineData line in _lines)
            {
                await writer.WriteLineAsync(line.Text);
            }

            await writer.FlushAsync();

            _needsSave = false;

            return true;
        }

        public virtual async Task<bool> DeserializeProgram(FileInfo info)
        {
            _serializationInfo._fileInfo ??= info;

            using StreamReader reader = new(new FileStream(_serializationInfo._fileInfo.FullName, FileMode.Open));

            Task<string> result = reader.ReadToEndAsync();

            _lines.Clear();

            await result;

            foreach(string line in result.Result.Split('\n'))
            {
                if(!TryAddLine(line))
                    throw new InvalidDataException($"Was unable to add line ({line}) to the programs line collection");
            }

            _needsSave = false;

            return true;
        }
        #endregion
    }

    /// <summary>
    /// This abstract class serves as the interface used to create any concrete class whos purpose
    /// is to manage the creation, storage, serialization and valadation of any generic set of instructions known as a program for
    /// an Architecture supported by a class derived from <seealso cref="ArchitectureSim_Interface{WordType, OPCodeWordType}"/>
    /// </summary>
    /// <typeparam name="ProgramsCollection">A collection that will store all the currently loaded programs. Type must implement the IDictionary interface and have a public parameterless constructor</typeparam>
    /// <typeparam name="Program">Type that implements the <seealso cref="IProgram"/> interface. Recommended that type serives from <seealso cref="Program"/> for ease of use</typeparam>
    public abstract class ProgramsManagement_Interface<ProgramsCollection, Program>
        where ProgramsCollection : IDictionary<string, Program>, new()
        where Program : IProgram
    {
        #region FIELDS
        protected ProgramsCollection _programs = new();
        #endregion

        #region PROPERTIES
        public virtual ProgramsCollection Programs { get => _programs; }
        #endregion

        #region OPERATORS
        public virtual Program this[string key]
        {
            get => _programs[key];
        }

        public virtual KeyValuePair<string, Program> this[int index]
        {
            get => _programs.ElementAt(index);
        }
        #endregion

        #region PUBLIC_METHODS
        public virtual bool TryCreateNewProgram(string programName)
        {
            if (Activator.CreateInstance(typeof(Program), new { programName }) is Program newProgram)
            {
                _programs.Add(new KeyValuePair<string, Program>(programName, newProgram));

                return true;
            }
            else
                return false;
        }
        #endregion
    }
}

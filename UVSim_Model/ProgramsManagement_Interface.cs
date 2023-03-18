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

    public interface IProgram
    {
        #region PROPERTIES
        public bool NeedsSave { get; }
        public IList<UVSim.LineData> Lines { get; }

        public string Extension { get; set; }
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

    public abstract class Program<LinesContainer> : IProgram
        where LinesContainer : IList<UVSim.LineData>, new()
    {
        #region FIELDS
        protected bool _needsSave = true;

        protected LinesContainer _lines = new();

        protected string _fileName;

        protected string _extension;

        protected FileInfo? _fileInfo;
        #endregion

        #region PROPERTIES
        public bool NeedsSave { get => _needsSave; }
        public IList<UVSim.LineData> Lines { get => _lines; }
        public string FileName { get => _fileName; set => _fileName = value; }
        public FileInfo? FileInfo { get => _fileInfo; set => _fileInfo = value; }
        public string Extension { get => _extension; set => _extension = value; }
        #endregion

        #region OPERATORS
        public virtual UVSim.LineData this[int index]
        {
            get => _lines[index];
        }
        #endregion

        #region CONSTRUCTORS
        public Program(string fileName, string ext) =>
            (_fileName, _extension) = (fileName, ext);
        #endregion

        #region PUBLIC_METHODS
        public bool TryAddLine(string text)
        {
            if (Activator.CreateInstance(typeof(LineData), new { text }) is LineData line)
            {
                _lines.Add(line);
                _needsSave = true;
                return true;
            }
            return false;
        }

        public void RemoveLine(int index)
        {
            _lines.RemoveAt(index);
            _needsSave = true;
        }

        public void SetLine(int index, string newText)
        {
            _lines[index].Text = newText;
            _needsSave = true;
        }

        public bool TrySetContent(string[] newLines)
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
        public async Task<bool> SerializeProgram()
        {
            if (_fileInfo == null || string.IsNullOrEmpty(_fileInfo.FullName))
                throw new InvalidOperationException("No file path specified for the program trying to save");

            using StreamWriter writer = new(new FileStream(_fileInfo.FullName, FileMode.Create));

            writer.AutoFlush = false;

            foreach(UVSim.LineData line in _lines)
            {
                await writer.WriteLineAsync(line.Text);
            }

            await writer.FlushAsync();

            _needsSave = false;

            return true;
        }

        public async Task<bool> DeserializeProgram(FileInfo info)
        {
            _fileInfo ??= info;

            using StreamReader reader = new(new FileStream(_fileInfo.FullName, FileMode.Open));

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

    public abstract class ProgramsManagement_Interface<ProgramsCollection, Program>
        where ProgramsCollection : IDictionary<string, Program>, new()
        where Program : IProgram
    {
        #region FIELDS
        protected ProgramsCollection _programs = new();
        #endregion

        #region PROPERTIES
        public ProgramsCollection Programs { get => _programs; }
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
        public bool TryCreateNewProgram(string programName)
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

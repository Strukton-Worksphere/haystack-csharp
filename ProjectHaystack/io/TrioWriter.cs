﻿using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ProjectHaystack.io
{
    public class TrioWriter : IDisposable
    {
        // Strings can be left unquoted if they begin with any of these "safe" chars:
        // Any non-ASCII Unicode character, A-Z, a-z, underbar, dash, or space.
        private Regex _safeCharsRegex = new Regex(@"^[^\x00-\x7F]|^[A-Za-z0-9_\ ]+$", RegexOptions.Compiled);
        private readonly TextWriter _trioWriter;
        private bool isFirst = true;

        public TrioWriter(TextWriter writer)
        {
            _trioWriter = writer;
        }

        public TrioWriter(Stream trioStream)
        {
            _trioWriter = new StreamWriter(trioStream, null, 1024, true);
        }

        public void WriteGrid(HaystackGrid grid)
        {
            foreach (var row in grid.Rows)
                WriteEntity(row);
        }


        public void WriteEntity(HaystackDictionary entity)
        {
            if (isFirst)
                isFirst = false;
            else
                _trioWriter.WriteLine("---");

            foreach (var kv in entity)
            {
                if (kv.Value == null)
                    continue;
                _trioWriter.Write(kv.Key);
                if (kv.Value is HaystackMarker)
                {
                    _trioWriter.WriteLine();
                    continue;
                }
                _trioWriter.Write(":");
                if (kv.Value is HaystackGrid)
                {
                    _trioWriter.Write("Zinc:");
                    _trioWriter.WriteLine();
                    var val = ZincWriter.ToZinc(kv.Value);
                    foreach (var line in val.TrimEnd().Split(new[] { "\n" }, StringSplitOptions.None))
                    {
                        _trioWriter.Write("  ");
                        _trioWriter.WriteLine(line.TrimEnd());
                    }
                }
                else if (kv.Value is HaystackString stringValue && stringValue.Value.Contains("\n"))
                {
                    var val = stringValue.Value;
                    _trioWriter.WriteLine();
                    foreach (var line in val.TrimEnd().Split(new[] { "\n" }, StringSplitOptions.None))
                    {
                        _trioWriter.Write("  ");
                        _trioWriter.WriteLine(line.TrimEnd());
                    }
                }
                else
                {
                    var val = ZincWriter.ToZinc(kv.Value);
                    if (val.Contains("\\n"))
                        val = val.Replace("\\n", "\n");
                    if (val.StartsWith("\"") && val.EndsWith("\"") && _safeCharsRegex.IsMatch(val.Substring(1, val.Length - 2)))
                        val = val.Substring(1, val.Length - 2);
                    _trioWriter.WriteLine(val.TrimEnd());
                }
            }
        }

        public void WriteComment(string comment)
        {
            foreach (var line in comment.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                _trioWriter.Write("// ");
                _trioWriter.WriteLine(line.TrimEnd());
            }
        }

        public void Dispose()
        {
            _trioWriter.Dispose();
        }
    }
}
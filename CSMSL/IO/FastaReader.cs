﻿///////////////////////////////////////////////////////////////////////////
//  FastaReader.cs - Reads a text-based file using the fasta format       /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace CSMSL.IO
{
    public class FastaReader : IDisposable
    {
        private readonly StreamReader _reader;
     
        public char Delimiter { get; private set; }
        
        public FastaReader(string fileName, char delimiter = '>')
        {
            FileName = fileName;
            Delimiter = delimiter;
            _reader = new StreamReader(fileName);
        }

        public string FileName { get; set; }

        public void Close()
        {
            _reader.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public IEnumerable<Fasta> ReadNextFasta()
        {
            StringBuilder sequenceSb = new StringBuilder(500);
            StringBuilder headerSb = new StringBuilder(80);
            
            while (!_reader.EndOfStream)
            {
                string line = _reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line[0] == Delimiter)
                {
                    if (sequenceSb.Length > 0)
                    {
                        yield return new Fasta(sequenceSb.ToString(), headerSb.ToString());
                        sequenceSb.Clear();
                        headerSb.Clear();
                    }
                    headerSb.Append(line.TrimStart(Delimiter));
                }
                else
                {
                    sequenceSb.Append(line);
                }
            }
            if (sequenceSb.Length > 0)
                yield return new Fasta(sequenceSb.ToString(), headerSb.ToString());
        }
      
        public IEnumerable<Protein> ReadNextProtein()
        {
            return ReadNextFasta().Select(f => new Protein(f.Sequence, f.Description));
        }

        public override string ToString()
        {
            return FileName;
        }

        public static int NumberOfEntries(string filePath, char delimiter = '>')
        {
            int entries = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] == delimiter)
                        entries++;
                }
            }
            return entries;
        }
    }
}
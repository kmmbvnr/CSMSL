﻿///////////////////////////////////////////////////////////////////////////
//  FastaWriter.cs - Writes a text-based file using the fasta format      /
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

namespace CSMSL.IO
{
    public class FastaWriter : IDisposable
    {
        public int CharactersPerLine { get; private set; }
        public char Delimiter { get; private set; }
        public string FilePath { get; private set; }
        private readonly StreamWriter _writer;

        public FastaWriter(string filename, char delimiter = '>', int charperline = 80)
        {
            FilePath = filename;
            Delimiter = delimiter;
            CharactersPerLine = charperline;
            _writer = new StreamWriter(filename);
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }

        void IDisposable.Dispose()
        {
            Close();
        }
        
        public void Write(IEnumerable<Protein> proteins)
        {
            foreach (Protein protein in proteins)
            {
                Write(protein);
            }
        }

        public void Write(Protein protein)
        {
            Write(protein.Sequence, protein.Description);
        }

        public void Write(IEnumerable<Fasta> fastas)
        {
            foreach (Fasta fasta in fastas)
            {
                Write(fasta.Sequence, fasta.Description);
            }
        }

        public void Write(Fasta fasta)
        {
            Write(fasta.Sequence, fasta.Description);
        }

        public void Write(string sequence, string description)
        {
            _writer.WriteLine("{0}{1}", Delimiter, description);
            for (int i = 0; i < sequence.Length; i += CharactersPerLine)
            {
                _writer.WriteLine(sequence.Substring(i, (i + CharactersPerLine < sequence.Length) ?
                    CharactersPerLine :
                    sequence.Length - i));
            }
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }

      
    }
}
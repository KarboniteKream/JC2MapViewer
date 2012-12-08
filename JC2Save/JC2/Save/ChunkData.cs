/*
 JC2MapViewer
 Copyright 2010 - DerPlaya78
 
 this program is free software: you can redistribute it and/or modify
 it under the terms of the GNU Lesser General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Lesser General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

namespace JC2.Save
{
    public class ChunkData
    {
        public ChunkHeader Header { get; private set; }
        
        private readonly List<ChunkDataEntry> _dataEntries = new List<ChunkDataEntry>();
        public ReadOnlyCollection<ChunkDataEntry> DataEntries
        {
            get { return _dataEntries.AsReadOnly(); }
        }
        
        public ChunkDataEntry this[short index]
        {
            get {
                if (_dataEntries == null) return null;
                return _dataEntries.SingleOrDefault(c => c.Index == index);
            }
        }

        public void Read(BinaryReader r, ChunkHeader header, int startOffset)
        {
            Header = header;
            r.BaseStream.Seek(startOffset + Header.EntryListOffset, SeekOrigin.Begin);
            int read = 0;
            _dataEntries.Clear();
            while (read < Header.EntryListSize)
            {
                ChunkDataEntry entry = new ChunkDataEntry(r);
                read += entry.Length;
                _dataEntries.Add(entry);
            }
        }

        public override string ToString()
        {
            if (Header != null)
            {
                //string oname = Header.ObjectName != null ? "\"" + Header.ObjectName + "\"" : Header.ObjectID.ToString("X8");
                //string iname = Header.InstanceName != null ? "\"" + Header.InstanceName + "\"" : Header.InstanceID.ToString("X8");
                //return string.Format("Object: {0} Instance: {1}", oname, iname);
                return Header.ToString() + " Size: " +  Header.EntryListSize.ToString();
            }
            return base.ToString();
        }
    }
}

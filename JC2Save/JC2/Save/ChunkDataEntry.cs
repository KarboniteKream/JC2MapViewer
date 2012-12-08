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
using System.IO;
using System;

namespace JC2.Save
{
    public class ChunkDataEntry
    {
        public short Index { get; private set; }
        public byte[] Data { get; private set; }

        public int Length { get { return 6 + (Data != null ? Data.Length : 0); } }

        // temp
        public long _Offset;

        public ChunkDataEntry(BinaryReader r)
        {
            Read(r);
        }

        public void Read(BinaryReader r)
        {
            _Offset = r.BaseStream.Position;
            
            Index = r.ReadInt16();
            int dataSize = r.ReadInt32();
            Data = r.ReadBytes(dataSize);
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Index, Length - 6);
        }
    }
}

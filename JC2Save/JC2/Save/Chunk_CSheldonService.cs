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
using System;

namespace JC2.Save
{
    [KnownChunk("CSheldonService")]
    public class Chunk_CSheldonService : IParsedChunk
    {
        public string Name { 
            get 
            {
                return "CSheldonService"; 
            } 
        }

        public byte Unknown1 { get; private set; }
        public byte Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }

        public void Parse(ChunkData data)
        {
            Unknown1 = data[0].Data[0];
            Unknown2 = data[1].Data[0];
            Unknown3 = EndianessSwitchableBitConverter.ToInt32(data[2].Data, 0);
        }
    }
}

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
    [KnownChunk("HeatLevels")]
    public class Chunk_HeatLevels : IParsedChunk
    {
        public string Name { 
            get 
            {
                return "HeatLevels"; 
            } 
        }

        public int MaxLevel { get; private set; }

        public void Parse(ChunkData data)
        {
            MaxLevel = EndianessSwitchableBitConverter.ToInt32(data[0].Data, 0);
        }
    }
}

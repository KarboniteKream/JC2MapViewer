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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JC2.Save
{
    [KnownChunk("HeatSystem")]
    public class Chunk_HeatSystem : IParsedChunk
    {
        public string Name { 
            get 
            {
                return "HeatSystem"; 
            } 
        }

        public class SavedObject1
        {
            public uint Unknown1 { get; private set; }
            public byte Unknown2 { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                Unknown1 = EndianessSwitchableBitConverter.ToUInt32(data[listOffset++].Data, 0);
                Unknown2 = data[listOffset++].Data[0];
            }

            public override string ToString()
            {
                return string.Format("Unknown1: {0} Unknown2: {1}", Unknown1, Unknown2);
            }
        }

        public int Unknown1 { get; private set; }

        private readonly List<SavedObject1> _objects = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            short listOffset = 0;

            Unknown1 = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);

            for (int i = 0; i < 12; i++)
            { 
                SavedObject1 o = new SavedObject1();
                o.Parse(data, ref listOffset);
                
                _objects.Add(o);
            }
        }
    }
}

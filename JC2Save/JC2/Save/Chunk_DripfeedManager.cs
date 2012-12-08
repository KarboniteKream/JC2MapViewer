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
    [KnownChunk("DripfeedManager")]
    public class Chunk_DripfeedManager : IParsedChunk
    {
        public string Name { get { return "DripfeedManager"; } }

        public class SavedObject1
        {
            public uint Hash { get; private set; }
            public string Name
            {
                get { return NameLookup.GetName(Hash); }
            }

            public byte Unknown1 { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                Hash = EndianessSwitchableBitConverter.ToUInt32(data.DataEntries[listOffset++].Data, 0);
                Unknown1 = data.DataEntries[listOffset++].Data[0];

            }

            public override string ToString()
            {
                string oname = Name != null ? "\"" + Name + "\"" : Hash.ToString("X8");
                return string.Format("Name: {0} Unknown1: {1}", oname, Unknown1);
            }
        }

        private readonly List<SavedObject1> _objects = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        public byte Unknown1 { get; private set; }

        public void Parse(ChunkData data)
        {
            short listOffset = 0;
            int count = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);
            for (int i = 0; i < count; i++)
            {
                SavedObject1 o = new SavedObject1();
                o.Parse(data, ref listOffset);
                _objects.Add(o);
            }
            Unknown1 = data[500].Data[0];
        }
    }
}

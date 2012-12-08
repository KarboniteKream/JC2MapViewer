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
    [KnownChunk(0x72DE987B)]
    public class Chunk_Strongholds : IParsedChunk, IContainsSettlementInfo
    {
        public string Name { get { return "Strongholds"; } }

        public class SavedObject1
        {
            public uint NameHash { get; private set; }
            public string Name
            {
                get { return NameLookup.GetName(NameHash); }
            }

            public int Count { get; private set; }
            public byte HasBeenDiscovered { get; private set; }
            public byte HasBeenOvertaken { get; private set; }
            public byte Unknown4 { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                NameHash = EndianessSwitchableBitConverter.ToUInt32(data[listOffset++].Data, 0);
                Count = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);
                HasBeenDiscovered = data[listOffset++].Data[0];
                HasBeenOvertaken = data[listOffset++].Data[0];
                Unknown4 = data[listOffset++].Data[0];
            }

            public override string ToString()
            {
                string oname1 = Name != null ? "\"" + Name + "\"" : NameHash.ToString("X8");
                return string.Format("Mission: {0} Count: {1} Unknown2: {2} Unknown3: {3} Unknown4: {4}", oname1, Count, HasBeenDiscovered, HasBeenOvertaken, Unknown4);
            }
        }

        private readonly List<SavedObject1> _objects = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            short listOffset = 0;
            int count = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);
            _objects.Clear();
            for (int i = 0; i < count; i++)
            {
                SavedObject1 o = new SavedObject1();
                o.Parse(data, ref listOffset);
                _objects.Add(o);
            }
        }

        public Dictionary<string, SavedSettlementInfo> GetSettlementsInfo()
        {
            Dictionary<string, SavedSettlementInfo> returnValue = new Dictionary<string, SavedSettlementInfo>();
            foreach (SavedObject1 o in _objects)
            {
                if (SettlementInfoLookup.SettlementList.ContainsKey(o.NameHash))
                {
                    SettlementInfo i = SettlementInfoLookup.SettlementList[o.NameHash];
                    SavedSettlementInfo i2 = new SavedSettlementInfo(i, o.Count, o.HasBeenDiscovered == 1, o.HasBeenOvertaken == 1);
                    returnValue.Add(i.ID, i2);                
                }
            }
            return returnValue;
        }    
    }
}

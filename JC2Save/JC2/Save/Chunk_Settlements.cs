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
    [KnownChunk(0x56E9FCEC)]
    public class Chunk_Settlements : IParsedChunk, IContainsSettlementInfo
    {
        public string Name { get { return "Settlements"; } }

        public class SavedObject1
        {
            public uint Index { get; private set; }
            public int Count { get; private set; }
            public byte HasBeenDiscovered { get; private set; }
            public byte Unknown4 { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                Index = EndianessSwitchableBitConverter.ToUInt32(data[listOffset++].Data, 0);
                Count = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);
                HasBeenDiscovered = data[listOffset++].Data[0];
                Unknown4 = data[listOffset++].Data[0];
            }

            public override string ToString()
            {
                return string.Format("Index: {0} Count: {1} HasBeenDiscovered: {2} Unknown4: {3}", Index, Count, HasBeenDiscovered, Unknown4);
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
                SavedObject1 so = new SavedObject1();
                so.Parse(data, ref listOffset);
                _objects.Add(so);
            }
        }

        public Dictionary<string, SavedSettlementInfo> GetSettlementsInfo()
        {
            Dictionary<string, SavedSettlementInfo> returnValue = new Dictionary<string, SavedSettlementInfo>();
            foreach (SavedObject1 o in _objects)
            {
                if (SettlementInfoLookup.SettlementList.ContainsKey(o.Index))
                {
                    SettlementInfo i = SettlementInfoLookup.SettlementList[o.Index];
                    SavedSettlementInfo i2 = new SavedSettlementInfo(i, o.Count, o.HasBeenDiscovered == 1, false);
                    returnValue.Add(i.ID, i2);
                }
            }
            return returnValue;
        }
    }
}

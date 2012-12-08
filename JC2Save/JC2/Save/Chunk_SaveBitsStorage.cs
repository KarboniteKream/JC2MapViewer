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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JC2.Save
{
    [KnownChunk("SaveBitStorage")]
    public class Chunk_SaveBitStorage : IParsedChunk, IContainsSavedObjectInfo
    {
        public string Name { get { return "SaveBitStorage"; } }

        public int Capacity { get; private set; }
        public int UsedCount { get; private set; }
        private readonly List<ObjectReference> _objects = new List<ObjectReference>();
        public ReadOnlyCollection<ObjectReference> Objects
        {
            get { return _objects.AsReadOnly(); }
        }
        
        public BitArray Bits { get; private set; }

        public void Parse(ChunkData data)
        {
            Capacity = EndianessSwitchableBitConverter.ToInt32(data[0].Data, 0);
            UsedCount = EndianessSwitchableBitConverter.ToInt32(data[1].Data, 0);

            _objects.Clear();
            for(int i = 0; i < Capacity * 8; i+= 8)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data[9].Data, i);
                _objects.Add(obj);
            }

            Bits = new BitArray(data[10].Data);
            Bits.Length = Capacity;
        }

        public List<SavedObjectInfo> GetSavedObjectInfo(Dictionary<string, int> countsbycategory)
        {
            if (!SavedObjectInfoLookup.Items.ContainsKey(Name))
            {
                return null;
            }
            
            List<ulong> exclude = new List<ulong>();
            List<ulong> all = new List<ulong>();
            
            for (int i = 0; i < _objects.Count; i++)
            {
                ulong key = _objects[i].ObjectID;
                key <<= 32;
                key |= _objects[i].InstanceID;

                if (Bits[i])
                {
                    exclude.Add(key);
                }
                all.Add(key);
            } 

            List<SavedObjectInfo> returnList = new List<SavedObjectInfo>();

            foreach (var i in SavedObjectInfoLookup.Items[Name].Items)
            {
                if (i.Value.IsBitSet)
                {
                    if (exclude.Contains(i.Key) || !all.Contains(i.Key))
                    {
                        if (countsbycategory != null) countsbycategory[i.Value.Category] = countsbycategory[i.Value.Category] - 1;
                        returnList.Add(i.Value);
                    }
                }
                else {
                    if (!exclude.Contains(i.Key))
                    {
                        if (countsbycategory != null) countsbycategory[i.Value.Category] = countsbycategory[i.Value.Category] - 1;
                        returnList.Add(i.Value);
                    }
                }
            }
            return returnList;
        }
    }
}

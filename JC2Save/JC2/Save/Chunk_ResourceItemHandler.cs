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
    [KnownChunk("ResourceItemHandler")]
    public class Chunk_ResourceItemHandler : IParsedChunk, IContainsSavedObjectInfo
    {
        public string Name { get { return "ResourceItemHandler"; } }

        private readonly List<ObjectReference> _objects = new List<ObjectReference>();
        public ReadOnlyCollection<ObjectReference> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            int savedCount1 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[0].Data, 0);

            int expectedEntryCount = savedCount1 + 1;
            
            /*
            if (data.DataEntries.Count != expectedEntryCount)
            {
                Debugger.Break();
            }
            */

            _objects.Clear();
            int listOffset = 1;
            for (int i = 0; i < savedCount1; i++)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data.DataEntries[listOffset++].Data);
                _objects.Add(obj);
            }
        }

        public List<SavedObjectInfo> GetSavedObjectInfo(Dictionary<string, int> countsbycategory)
        {
            if (!SavedObjectInfoLookup.Items.ContainsKey(Name))
            {
                return null;
            }
            
            List<ulong> exclude = new List<ulong>();
            for (int i = 0; i < _objects.Count; i++)
            {
                ulong key = _objects[i].ObjectID;
                key <<= 32;
                key |= _objects[i].InstanceID;

                exclude.Add(key);
            }

            List<SavedObjectInfo> returnList = new List<SavedObjectInfo>();
            foreach (var i in SavedObjectInfoLookup.Items[Name].Items)
            {
                if (!exclude.Contains(i.Key))
                {
                    if(countsbycategory != null) countsbycategory[i.Value.Category] = countsbycategory[i.Value.Category] - 1;
                    returnList.Add(i.Value);
                }
            }
            return returnList;
        }
    }
}

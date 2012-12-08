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

namespace JC2.Save
{
    [KnownChunk("TriggerSaveHandler")]
    public class Chunk_TriggerSaveHandler : IParsedChunk, IContainsSavedObjectInfo
    {
        public string Name { get { return "TriggerSaveHandler"; } }

        public class SavedObject1
        {
            public ObjectReference Object { get; private set; }

            public byte Unknown1 { get; private set; }
            public int Unknown2 { get; private set; }

            public void Parse(ChunkData data, ref int listOffset)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data.DataEntries[listOffset++].Data);
                Object = obj;

                Unknown1 = data.DataEntries[listOffset++].Data[0];
                Unknown2 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[listOffset++].Data, 0);
            }

            public override string ToString()
            {
                return string.Format("{0} U1: {1} U2: {2}", Object, Unknown1, Unknown2);
            }
        }

        public class SavedObject2
        {
            public ObjectReference Object { get; private set; }
            
            public byte Unknown1 { get; private set; }
            public byte Unknown2 { get; private set; }
            public int Unknown3 { get; private set; }
            public float Unknown4 { get; private set; }

            public void Parse(ChunkData data, ref int listOffset)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data.DataEntries[listOffset++].Data);
                Object = obj;

                Unknown1 = data.DataEntries[listOffset++].Data[0];
                Unknown2 = data.DataEntries[listOffset++].Data[0];
                Unknown3 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[listOffset++].Data, 0);
                Unknown4 = EndianessSwitchableBitConverter.ToSingle(data.DataEntries[listOffset++].Data, 0);
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0} U1: {1} U2: {2} U3: {3} U5: {4:0.000000}",
                    Object,
                    Unknown1,
                    Unknown2,
                    Unknown3,
                    Unknown4
                );
            }
        }

        public class SavedObject3
        {
            public ObjectReference Object { get; private set; }

            public byte Unknown1 { get; private set; }
            public byte Unknown2 { get; private set; }
            public byte Unknown3 { get; private set; }
            public byte Unknown4 { get; private set; }

            public void Parse(ChunkData data, ref int listOffset)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data.DataEntries[listOffset++].Data);
                Object = obj;

                Unknown1 = data.DataEntries[listOffset++].Data[0];
                Unknown2 = data.DataEntries[listOffset++].Data[0];
                Unknown3 = data.DataEntries[listOffset++].Data[0];
                Unknown4 = data.DataEntries[listOffset++].Data[0];
            }

            public override string ToString()
            {
                return string.Format("{0} U1: {1} U2: {2} U3: {3} U4: {4}", Object, Unknown1, Unknown2, Unknown3, Unknown4);
            }
        }

        public class SavedObject4
        {
            public ObjectReference Object { get; private set; }

            public byte Unknown1 { get; private set; }
            public byte Unknown2 { get; private set; }

            public void Parse(ChunkData data, ref int listOffset)
            {
                ObjectReference obj = new ObjectReference();
                obj.Parse(data.DataEntries[listOffset++].Data);
                Object = obj;

                Unknown1 = data.DataEntries[listOffset++].Data[0];
                Unknown2 = data.DataEntries[listOffset++].Data[0];
            }

            public override string ToString()
            {
                return string.Format("{0} U1: {1} U2: {2}", Object, Unknown1, Unknown2);
            }
        }

        private readonly List<SavedObject1> _objects1 = new List<SavedObject1>();
        private readonly List<SavedObject2> _objects2 = new List<SavedObject2>();
        private readonly List<SavedObject3> _objects3 = new List<SavedObject3>();
        private readonly List<SavedObject4> _objects4 = new List<SavedObject4>();

        public void Parse(ChunkData data)
        {
            int savedCount1 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[0].Data, 0);
            int savedCount2 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[1].Data, 0);
            int savedCount3 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[2].Data, 0);
            int savedCount4 = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[3].Data, 0);

            int expectedEntryCount = 3 * savedCount1 + 5 * savedCount2 + 5 * savedCount3 + 3 * savedCount4 + 4;
            
            /*
            if (data.DataEntries.Count != expectedEntryCount)
            {
                Debugger.Break();
            }
            */

            _objects1.Clear();
            _objects2.Clear();
            _objects3.Clear();
            _objects4.Clear();

            int listOffset = 4;
            for (int i = 0; i < savedCount1; i++)
            { 
                SavedObject1 o = new SavedObject1();
                o.Parse(data, ref listOffset);
                _objects1.Add(o);
            }

            for (int i = 0; i < savedCount2; i++)
            {
                SavedObject2 o = new SavedObject2();
                o.Parse(data, ref listOffset);
                _objects2.Add(o);
            }

            for (int i = 0; i < savedCount3; i++)
            {
                SavedObject3 o = new SavedObject3();
                o.Parse(data, ref listOffset);
                _objects3.Add(o);
            }

            for (int i = 0; i < savedCount4; i++)
            {
                SavedObject4 o = new SavedObject4();
                o.Parse(data, ref listOffset);
                _objects4.Add(o);
            }
        }

        public List<SavedObjectInfo> GetSavedObjectInfo(Dictionary<string, int> countsbycategory)
        {
            if (!SavedObjectInfoLookup.Items.ContainsKey(Name))
            {
                return null;
            }

            List<ulong> exclude = new List<ulong>();
            for (int i = 0; i < _objects4.Count; i++)
            {
                ulong key = _objects4[i].Object.ObjectID;
                key <<= 32;
                key |= _objects4[i].Object.InstanceID;
                if (_objects4[i].Unknown2 == 1)
                {
                    exclude.Add(key);
                }
            }

            List<SavedObjectInfo> returnList = new List<SavedObjectInfo>();
            foreach (var i in SavedObjectInfoLookup.Items[Name].Items)
            {
                if (!exclude.Contains(i.Key))
                {
                    if (countsbycategory != null) countsbycategory[i.Value.Category] = countsbycategory[i.Value.Category] - 1;
                    returnList.Add(i.Value);
                }
            }
            return returnList;
        }
    }
}

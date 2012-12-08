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
    [KnownChunk(0x0d7295f3)]
    public class Chunk_PDADatabase : IParsedChunk
    {
        public string Name { get { return "PDADatabase"; } }

        public class SavedObject1
        {
            public uint NameHash { get; private set; }

            public string Name
            {
                get { return NameLookup.GetName(NameHash); }
            }

            public short Unknown1 { get; private set; }
            public short Unknown2 { get; private set; }

            public void Parse(byte[] data, int offset)
            {
                NameHash = EndianessSwitchableBitConverter.ToUInt32(data, offset);
                Unknown1 = EndianessSwitchableBitConverter.ToInt16(data, offset + 4);
                Unknown2 = EndianessSwitchableBitConverter.ToInt16(data, offset + 6);
            }

            public override string ToString()
            {
                string oname = Name != null ? "\"" + Name + "\"" : NameHash.ToString("X8");
                return string.Format("Name: {0} U1: {1} U2: {2}", oname, Unknown1, Unknown2);
            }
        }

        public class PdaDbEntryVehicle
        {
            public uint NameHash { get; private set; }

            public string Name
            {
                get { return NameLookup.GetName(NameHash); }
            }

            public void Parse(byte[] data, int offset)
            {
                NameHash = EndianessSwitchableBitConverter.ToUInt32(data, offset);
            }

            public override string ToString()
            {
                string oname = Name != null ? "\"" + Name + "\"" : NameHash.ToString("X8");
                return string.Format("Object: {0}", oname);
            }
        }

        private readonly List<SavedObject1> _objects = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        private readonly List<SavedObject1> _objects2 = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects2
        {
            get { return _objects2.AsReadOnly(); }
        }

        private readonly List<SavedObject1> _objects3 = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects3
        {
            get { return _objects3.AsReadOnly(); }
        }

        private readonly List<PdaDbEntryVehicle> _vehicles = new List<PdaDbEntryVehicle>();
        public ReadOnlyCollection<PdaDbEntryVehicle> Vehicles
        {
            get { return _vehicles.AsReadOnly(); }
        }

        private readonly List<SavedObject1> _objects5 = new List<SavedObject1>();
        public ReadOnlyCollection<SavedObject1> Objects5
        {
            get { return _objects5.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            int count = EndianessSwitchableBitConverter.ToInt32(data[0].Data, 0);
            _objects.Clear();
            for (int i = 0; i < count; i++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data[1].Data, i * 8);
                _objects.Add(so);
            }

            count = EndianessSwitchableBitConverter.ToInt32(data[2].Data, 0);
            _objects2.Clear();
            for (int i = 0; i < count; i++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data[3].Data, i * 8);
                _objects2.Add(so);
            }

            count = EndianessSwitchableBitConverter.ToInt32(data[4].Data, 0);
            _objects3.Clear();
            for (int i = 0; i < count; i++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data[5].Data, i * 8);
                _objects3.Add(so);
            }

            count = EndianessSwitchableBitConverter.ToInt32(data[6].Data, 0);
            _vehicles.Clear();
            for (int i = 0; i < count; i++)
            {
                PdaDbEntryVehicle so = new PdaDbEntryVehicle();
                so.Parse(data[7].Data, i * 4);
                _vehicles.Add(so);
            }

            count = EndianessSwitchableBitConverter.ToInt32(data[8].Data, 0);
            _objects5.Clear();
            for (int i = 0; i < count; i++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data[9].Data, i * 8);
                _objects5.Add(so);
            }
        }
    }
}

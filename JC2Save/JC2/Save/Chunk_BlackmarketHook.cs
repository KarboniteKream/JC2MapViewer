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
    [KnownChunk("BlackmarketHook")]
    public class Chunk_BlackmarketHook : IParsedChunk
    {
        public class SavedObject1
        {
            public uint NameHash { get; private set; }
            public string Name
            {
                get { return NameLookup.GetName(NameHash); }
            }

            public byte Level { get; private set; }
            public byte Unknown1 { get; private set; }
            public byte IsLocked { get; private set; }

            public void Parse(ChunkData data, ref int listOffset)
            {
                NameHash = EndianessSwitchableBitConverter.ToUInt32(data.DataEntries[listOffset++].Data, 0);
                Level = data.DataEntries[listOffset++].Data[0];
                Unknown1 = data.DataEntries[listOffset++].Data[0];
                IsLocked = data.DataEntries[listOffset++].Data[0];
            }

            public override string ToString()
            {
                string oname = Name != null ? "\"" + Name + "\"" : NameHash.ToString("X8");
                return string.Format("Object: {0} Level: {1} Unknown: {2} Locked: {3}", oname, Level, Unknown1, IsLocked);
            }
        }

        public string Name { get { return "BlackmarketHook"; } }

        private int NumWeaponParts;
        private int NumArmorParts;
        private int NumVehicleParts;

        private int Money;
        private int Chaos;

        private readonly List<SavedObject1> weapons = new List<SavedObject1>();
        private readonly List<SavedObject1> vehicles = new List<SavedObject1>();

        public void Parse(ChunkData data)
        {
            if (data.DataEntries.Count < 7)
            {
                System.Diagnostics.Debugger.Break();
            }

            NumWeaponParts = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[0].Data, 0);
            NumArmorParts = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[1].Data, 0);
            NumVehicleParts = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[2].Data, 0);
            Money = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[3].Data, 0);
            Chaos = EndianessSwitchableBitConverter.ToInt32(data.DataEntries[4].Data, 0);
            short numWeapons = EndianessSwitchableBitConverter.ToInt16(data.DataEntries[5].Data, 0);
            short numVehicles = EndianessSwitchableBitConverter.ToInt16(data.DataEntries[6].Data, 0);

            int expectedEntryCount = (numVehicles + numWeapons) * 4;
            if (expectedEntryCount != data.DataEntries.Count - 7)
            {
                System.Diagnostics.Debugger.Break();
            }

            weapons.Clear();
            int listOffset = 7;
            for (int i = 0; i < numWeapons; i ++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data, ref listOffset);
                weapons.Add(so);
            }

            vehicles.Clear();
            for (int i = 0; i < numVehicles; i++)
            {
                SavedObject1 so = new SavedObject1();
                so.Parse(data, ref listOffset);
                vehicles.Add(so);
            }
        }
    }
}

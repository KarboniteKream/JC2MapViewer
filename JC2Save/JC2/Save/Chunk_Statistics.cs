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
using System.Text;

namespace JC2.Save
{
    [KnownChunk(0x65C2DA11)]
    public class Chunk_Statistics : IParsedChunk
    {
        public string Name { get { return "Statistics"; } }

        public class StatisticEntry
        {
            public enum ValueType
            { 
                TimeSpan,
                Bool,
                Int,
                Float,
                HashedStringList,
            }

            public short Index { get; private set; }
            public string Name { get; private set; }
            public ValueType Type { get; private set; }
            public object Value { get; private set; }

            public StatisticEntry(short index, string name, ChunkData data, ValueType type)
            {
                ChunkDataEntry ce = data[index];
                if (ce == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                
                Index = index;
                Name = name;
                Type = type;
               
                switch (type)
                {
                    case ValueType.Bool:
                        {
                            if (ce.Length != 1 + 6)
                            {
                                throw new Exception("not a Bool");
                            }
                            Value = data[index].Data[0] != 0;
                            break;
                        }
                    case ValueType.Float:
                        {
                            if (ce.Length != 4 + 6)
                            {
                                throw new Exception("not a Float");
                            }
                            Value = EndianessSwitchableBitConverter.ToSingle(data[index].Data, 0);
                            break;
                        }
                    case ValueType.Int:
                        {
                            if (ce.Length != 4 + 6)
                            {
                                throw new Exception("not an Int");
                            }
                            Value = EndianessSwitchableBitConverter.ToInt32(data[index].Data, 0);
                            break;
                        }
                    case ValueType.TimeSpan:
                        {
                            if (ce.Length != 8 + 6)
                            {
                                throw new Exception("not a TimeSpan");
                            }
                            Value = TimeSpan.FromSeconds(EndianessSwitchableBitConverter.ToInt64(data[index].Data, 0));
                            break;
                        }
                    case ValueType.HashedStringList:
                        {
                            try
                            {
                                int count = EndianessSwitchableBitConverter.ToInt32(data[index++].Data, 0);
                                List<HashedString> val = new List<HashedString>();
                                for (int i = 0; i < count; i++)
                                {
                                    val.Add(new HashedString(data[index++].Data));
                                }
                                Value = val;
                            }
                            catch {
                                throw new Exception("not a list of hashed strings");
                            }
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException("StatisticEntry: unsupported type");
                        }
                }
            }

            public override string ToString()
            {
                string valString = "";
                if (Type == ValueType.HashedStringList)
                {
                    List<string> tmp = new List<string>();
                    foreach (var o in ((List<HashedString>)Value))
                    {
                        tmp.Add(o.ToString());
                    }
                    valString = "[\"" + string.Join("\",\"", tmp.ToArray()) + "\"]";
                }
                else {
                    valString = Value.ToString();
                }
                
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("[{0}] {1} = {2}", Index, Name, valString);
                return sb.ToString();
            }
        }

        private readonly List<StatisticEntry> _entries = new List<StatisticEntry>();

        public ReadOnlyCollection<StatisticEntry> Entries
        {
            get { return _entries.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            _entries.Clear();
            _entries.Add(new StatisticEntry(0, "Game Time", data, StatisticEntry.ValueType.TimeSpan));
            // ???
            _entries.Add(new StatisticEntry(2, "Total kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(3, "Fall kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(4, "Drag kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(5, "Pinata kills", data, StatisticEntry.ValueType.Int));
            //_entries.Add(new StatisticEntry(6, "Wrecking Ball", data, StatisticEntry.ValueType.Int)); // perfect kill combo ?
            _entries.Add(new StatisticEntry(7, "Melee kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(8, "Hang kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(9, "Road kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(10, "Stunt driver points", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(11, "Highest parachute climb (m)", data, StatisticEntry.ValueType.Float));
            _entries.Add(new StatisticEntry(12, "Fully upgraded Items", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(13, "Stunt jump combo", data, StatisticEntry.ValueType.Int));
            
            _entries.Add(new StatisticEntry(163, "Unique vehicles driven", data, StatisticEntry.ValueType.HashedStringList));
            
            _entries.Add(new StatisticEntry(313, "Hijackings", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(314, "Highest Base jump (m)", data, StatisticEntry.ValueType.Float));
            // ???
            _entries.Add(new StatisticEntry(316, "Locations discovered", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(317, "Headshots", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(318, "Locations completed", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(319, "Total chaos", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(320, "Game Time (h)", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(321, "Total earnings ($)", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(322, "Agency missions completed", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(323, "Faction missions completed", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(324, "Stronghold takeovers completed", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(325, "Black market orders", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(326, "Weapons parts", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(327, "Vehicle parts", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(328, "Armor parts", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(329, "Cash stashes", data, StatisticEntry.ValueType.Int));
            // ???
            _entries.Add(new StatisticEntry(331, "Resource Items", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(332, "Sabotages comleted", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(333, "Skulls collected", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(334, "Black boxed collected", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(335, "Drug drops collected", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(336, "Faction items collected", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(337, "Challenges completed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(339, "Wrecking Ball", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(341, "Juggle kills", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(343, "Kilometers driven", data, StatisticEntry.ValueType.Float));
            _entries.Add(new StatisticEntry(344, "Bridge limbo", data, StatisticEntry.ValueType.Int));
            _entries.Add(new StatisticEntry(345, "Low flyer record (s)", data, StatisticEntry.ValueType.Float));
            // ...???
            _entries.Add(new StatisticEntry(352, "Fuel depots destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(355, "Mobile radars destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(358, "Baby panay statues destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(361, "Comm. stations destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(364, "Fuel shafts destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(367, "Propaganda trailers destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(370, "Faction items collected", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(373, "Colonels assassinated", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(376, "Pipelines destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(379, "Satellite dishes destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(382, "Gas pumps destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(385, "Gas holders destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(391, "Water towers destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(394, "Radars destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(397, "Wind turbines destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(400, "Generators destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(403, "Transformers destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(406, "Broadcast towers destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(409, "Radio masts destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(415, "Sams destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(418, "Military vehicles destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(421, "Demolition Officers killed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(424, "Elites killed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(427, "Ninjas killed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(433, "Offshore rigs destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(436, "Factory chimneys destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(439, "Cranes destroyed", data, StatisticEntry.ValueType.Int));
            // ...???
            _entries.Add(new StatisticEntry(442, "Silos destroyed", data, StatisticEntry.ValueType.Int));
        }
    }
}

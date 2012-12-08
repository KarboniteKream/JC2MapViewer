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
    [KnownChunk("MissionSystem")]
    public class Chunk_MissionSystem : IParsedChunk
    {
        public string Name { get { return "MissionSystem"; } }

        public class SavedMissionInfo
        {
            [Flags]
            public enum MissionFlag : byte
            { 
                NONE = 0x00,
                f01 = 0x01,
                f02 = 0x02,
                f04 = 0x04,
                f10 = 0x10,
                f20 = 0x20
            }
            
            public uint Hash { get; private set; }

            public string Name { get { return NameLookup.GetName(Hash); } }

            public MissionFlag Flags { get; private set; }

            private readonly List<SavedMissionObjectiveInfo> _missionObjectives = new List<SavedMissionObjectiveInfo>();
            public ReadOnlyCollection<SavedMissionObjectiveInfo> MissionObjectives
            {
                get { return _missionObjectives.AsReadOnly(); }
            }

            public void Parse(ChunkData data, ref short listOffset)
            {
                Hash = EndianessSwitchableBitConverter.ToUInt32(data[listOffset++].Data, 0);
                Flags = (MissionFlag)data[listOffset++].Data[0];
                int count = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SavedMissionObjectiveInfo o = new SavedMissionObjectiveInfo();
                        o.Parse(data, ref listOffset);
                        _missionObjectives.Add(o);
                    }
                }
            }

            public override string ToString()
            {
                string subs = null;
                if (_missionObjectives.Count != 0)
                {
                    List<string> tmp = new List<string>();
                    foreach (var o in _missionObjectives)
                    {
                        tmp.Add(o.ToString());
                    }
                    subs = string.Join(", ", tmp.ToArray());
                }

                string n = "0x" + Hash.ToString("X8");
                if (Name != null)
                {
                    n = Name;
                }
                if (subs != null)
                {
                    return string.Format("Mission name: {0} Flags: {1} Objectives: [{2}]", n, Flags, subs);
                }
                else
                {
                    return string.Format("Mission name: {0} Flags: {1}", n, Flags);
                }
            }
        }

        public class SavedMissionObjectiveInfo
        {
            [Flags]
            public enum MissionStatus : byte
            {
                NONE = 00,
                Activated = 02, // not sure...
                Completed = 04, // not sure...
                //Unknown4 = 06
            };
            
            public uint Hash { get; private set; }
            public string Name { get { return NameLookup.GetName(Hash); } }

            public MissionStatus Status { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                Hash = EndianessSwitchableBitConverter.ToUInt32(data[listOffset++].Data, 0);
                Status = (MissionStatus)data[listOffset++].Data[0];
            }

            public override string ToString()
            {
                string n = "0x" + Hash.ToString("X8");
                if (Name != null)
                {
                    n = Name;
                }
                
                return string.Format("Name: {0} Status: {1}", n, Status);
            }        
        }

        public class SavedChallengeInfo
        {
            public ObjectReference ChallengeObject { get; private set; }
            public float Time { get; private set; }

            public void Parse(ChunkData data, ref short listOffset)
            {
                ChallengeObject = new ObjectReference();
                ChallengeObject.Parse(data[listOffset++].Data);
                Time = EndianessSwitchableBitConverter.ToSingle(data[listOffset++].Data, 0); 
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "Object: {0} Time: {1}", ChallengeObject, Time);
            }
        }

        private readonly List<SavedMissionInfo> _objects = new List<SavedMissionInfo>();
        public ReadOnlyCollection<SavedMissionInfo> Objects
        {
            get { return _objects.AsReadOnly(); }
        }

        private readonly List<SavedChallengeInfo> _challenges = new List<SavedChallengeInfo>();

        public ReadOnlyCollection<SavedChallengeInfo> Challenges
        {
            get { return _challenges.AsReadOnly(); }
        }

        public void Parse(ChunkData data)
        {
            int numObjects1 = EndianessSwitchableBitConverter.ToInt32(data[1].Data, 0);
            
            // Missions
            short listOffset = 100;
            for(int i = 0; i < numObjects1; i++)
            {
                SavedMissionInfo o = new SavedMissionInfo();
                o.Parse(data, ref listOffset);
                _objects.Add(o);
            }

            // Challenges...
            listOffset = 2402;
            int numChallenges = EndianessSwitchableBitConverter.ToInt32(data[listOffset++].Data, 0);
            for (int i = 0; i < numChallenges; i++)
            {
                SavedChallengeInfo o = new SavedChallengeInfo();
                o.Parse(data, ref listOffset);
                _challenges.Add(o);
            }
            // end
        }
    }
}

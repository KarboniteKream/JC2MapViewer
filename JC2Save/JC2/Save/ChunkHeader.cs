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
using System.IO;

namespace JC2.Save
{
    public class ChunkHeader
    {
        private ulong _id;
        
        public uint InstanceID 
        {
            get
            {
                return (uint)(_id >> 32);
            }
        }
        public string ObjectName { get; private set; }

        public uint ObjectID
        {
            get
            {
                return (uint)(_id & 0xffffffff);
            }
        }
        public string InstanceName { get; private set; }

        public int EntryListOffset { get; private set; }
        public int EntryListSize { get; private set; }
        
        public ChunkHeader(BinaryReader r)
        {
            Read(r);
        }

        public void Read(BinaryReader r)
        {
            _id = r.ReadUInt64();

            EntryListOffset = r.ReadInt32();
            EntryListSize = r.ReadInt32();

            ObjectName = NameLookup.GetName(ObjectID);
            InstanceName = NameLookup.GetName(InstanceID);
        }

        public override string ToString()
        {
            if (ObjectName == null)
            {
                ObjectReference or = new ObjectReference();
                or.Parse(_id);
                return or.ToString();
            }
            else {
                string iname = InstanceName != null ? "\"" + InstanceName + "\"" : InstanceID.ToString("X8");            
                return string.Format("Object: {0} Instance: {1}", ObjectName, iname);
            }
        }
    }
}

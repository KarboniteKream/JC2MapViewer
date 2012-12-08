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

namespace JC2.Save
{
    public class ObjectReference
    {
        private ulong _id;

        public uint InstanceID
        {
            get
            {
                return (uint)(_id >> 32);
            }
        }

        public string ClassName
        {
            get { return ClassLookup.GetClassName(ObjectID); }
        }

        public uint ObjectID
        {
            get
            {
                return (uint)(_id & 0xffffffff);
            }
        }

        public void Parse(byte[] data)
        {
            Parse(data, 0);
        }

        public void Parse(byte[] data, int offset)
        {
            _id = EndianessSwitchableBitConverter.ToUInt64(data, offset);
            //ObjectID = EndianessSwitchableBitConverter.ToUInt32(data, offset);
            //InstanceID = EndianessSwitchableBitConverter.ToUInt32(data, offset + 4);
        }

        public void Parse(ulong id)
        {
            _id = id;
        }

        public override string ToString()
        {
            string oname = ClassName != null ? "\"" + ClassName + "\"" : ObjectID.ToString("X8");

            string name = NameLookup.GetName(ObjectID);
            if (ObjectID == 0xffffffff && InstanceID == 0xffffffff)
            {
                return "NULL";
            } 
            else 
            {
                if (name != null)
                {
                    return string.Format("Class: {0} ObjectID: {1:X8}(\"{2}\") InstanceID: {3:X8}", oname, ObjectID, name, InstanceID);
                }
                else
                {
                    return string.Format("Class: {0} ObjectID: {1:X8} InstanceID: {2:X8}", oname, ObjectID, InstanceID);
                }
            }
        }
    }
}

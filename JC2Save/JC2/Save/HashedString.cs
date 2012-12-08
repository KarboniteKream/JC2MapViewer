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
    public class HashedString
    {
        public uint Hash { get; private set; }
        public string Name { get { return NameLookup.GetName(Hash); } }

        public HashedString(uint hash)
        {
            Hash = hash;
        }

        public HashedString(byte[] data)
        {
            Hash = EndianessSwitchableBitConverter.ToUInt32(data, 0);
        }

        public override string ToString()
        {
            string name = Name != null ? Name : Hash.ToString("X8");
            return name;
        }
    }
}

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
using System.IO;

namespace JC2.Save
{
    public class SaveFileHeader
    {
        int unk1;
        int unk2;
        public int TotalChaos { get; private set; }
        public int TotalCash { get; private set; }
        public int PercentDone { get; private set; }

        int unk6;
        public TimeSpan PlayTime { get; private set; }
        public DateTime TimeSaved { get; private set; }

        int unk8;
        short unk9_1;
        short unk9_2;
        int unk10;
        short unk_11_1;
        short unk_11_2;

        public void Read(BinaryReader r)
        {
            unk1 = r.ReadInt32();
            unk2 = r.ReadInt32();
            TotalChaos = r.ReadInt32();
            TotalCash = r.ReadInt32();
            PercentDone = r.ReadInt32();
            unk6 = r.ReadInt32();

            PlayTime = TimeSpan.FromSeconds(r.ReadUInt64());
            TimeSaved = new DateTime(1970, 1, 1).AddSeconds(r.ReadUInt64());

            unk8 = r.ReadInt32();
            
            unk9_1 = r.ReadInt16();
            unk9_2 = r.ReadInt16();
            
            unk10 = r.ReadInt32();

            unk_11_1 = r.ReadInt16();
            unk_11_2 = r.ReadInt16();
        }
    }
}

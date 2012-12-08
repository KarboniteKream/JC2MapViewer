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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JC2.Save
{
    public static class NameLookup
    {
        static Dictionary<uint, string> _lookup = null;
        static List<uint> missedHashes = new List<uint>();

        public static string GetName(uint hash)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<uint, string>();

                try
                {
                    using (StreamReader reader = new StreamReader("Resources\\stringstohash.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            string item = reader.ReadLine();
                            if (item.Length > 0)
                            {
                                uint newhash = GetHash(item);
                                if (!_lookup.ContainsKey(newhash))
                                {
                                    _lookup.Add(newhash, item);
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            if (_lookup.ContainsKey(hash))
            {
                return _lookup[hash];
            }
            else
            {
                if (!missedHashes.Contains(hash)) missedHashes.Add(hash);
            }
            return null;
        }

        public static uint GetHash(string name)
        {
            byte[] data = Encoding.ASCII.GetBytes(name);
            return HashJenkins(data, data.Length, 0);
        }

        private unsafe static uint HashJenkins(byte[] data, int length, uint seed)
        {
            uint a, b, c;
            a = b = c = 0xDEADBEEF + (uint)length + seed;

            fixed (byte* ptrData = data)
            {
                uint* k = (uint*)ptrData;
                while (length > 12)
                {
                    a += k[0];
                    b += k[1];
                    c += k[2];

                    // mix
                    a -= c; a ^= (c << 4) | (c >> (32 - 4)); c += b;
                    b -= a; b ^= (a << 6) | (a >> (32 - 6)); a += c;
                    c -= b; c ^= (b << 8) | (b >> (32 - 8)); b += a;
                    a -= c; a ^= (c << 16) | (c >> (32 - 16)); c += b;
                    b -= a; b ^= (a << 19) | (a >> (32 - 19)); a += c;
                    c -= b; c ^= (b << 4) | (b >> (32 - 4)); b += a;

                    length -= 12;
                    k += 3;
                }

                switch (length)
                {
                    case 12: c += k[2]; b += k[1]; a += k[0]; break;
                    case 11: c += k[2] & 0xffffff; b += k[1]; a += k[0]; break;
                    case 10: c += k[2] & 0xffff; b += k[1]; a += k[0]; break;
                    case 9: c += k[2] & 0xff; b += k[1]; a += k[0]; break;
                    case 8: b += k[1]; a += k[0]; break;
                    case 7: b += k[1] & 0xffffff; a += k[0]; break;
                    case 6: b += k[1] & 0xffff; a += k[0]; break;
                    case 5: b += k[1] & 0xff; a += k[0]; break;
                    case 4: a += k[0]; break;
                    case 3: a += k[0] & 0xffffff; break;
                    case 2: a += k[0] & 0xffff; break;
                    case 1: a += k[0] & 0xff; break;
                    case 0: return c;              /* zero length strings require no mixing */
                }

                // final
                c ^= b; c -= (b << 14) | (b >> (32 - 14));
                a ^= c; a -= (c << 11) | (c >> (32 - 11));
                b ^= a; b -= (a << 25) | (a >> (32 - 25));
                c ^= b; c -= (b << 16) | (b >> (32 - 16));
                a ^= c; a -= (c << 4) | (c >> (32 - 4));
                b ^= a; b -= (a << 14) | (a >> (32 - 14));
                c ^= b; c -= (b << 24) | (b >> (32 - 24));
            }
            return c;
        }        
    }
}

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
using System.Reflection;

namespace JC2.Save
{
    internal static class ParsedChunkTypeHelper
    {
        private static Dictionary<uint, Type> _lookup = null;

        private static void BuildLookup()
        {
            _lookup = new Dictionary<uint, Type>();

            foreach (Type type in Assembly.GetAssembly(typeof(ParsedChunkTypeHelper)).GetTypes())
            {
                object[] attributes = type.GetCustomAttributes(typeof(KnownChunkAttribute), false);
                if (attributes.Length == 1)
                {
                    KnownChunkAttribute attribute = (KnownChunkAttribute)attributes[0];
                    _lookup.Add(attribute.Hash, type);
                }
            }
        }

        public static Type GetType(uint hash)
        {
            if (_lookup == null)
            {
                BuildLookup();
            }

            if (_lookup.ContainsKey(hash))
            {
                return _lookup[hash];
            }

            return null;
        }

        public static IParsedChunk TryParse(ChunkData data)
        {
            IParsedChunk o = null;

            Type type = GetType(data.Header.ObjectID);

            if (type != null)
            {
                try
                {
                    o = (IParsedChunk)System.Activator.CreateInstance(type);
                    o.Parse(data);
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }

            return o;
        }
    }
}

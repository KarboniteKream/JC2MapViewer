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
using System.IO;
using System.Xml;

namespace JC2.Save
{
    public class SettlementInfoLookup
    {
        private static readonly Dictionary<uint, SettlementInfo> _settlementList;
        public static ReadOnlyDictionary<uint, SettlementInfo> SettlementList
        {
            get { return new ReadOnlyDictionary<uint, SettlementInfo>(_settlementList); }
        }

        private static readonly Dictionary<uint, string> _itemsToSettlementList;
        public static ReadOnlyDictionary<uint, string> ItemsToSettlementList
        {
            get { return new ReadOnlyDictionary<uint, string>(_itemsToSettlementList); }
        }

        static SettlementInfoLookup()
        {
            _settlementList = new Dictionary<uint, SettlementInfo>();
            _itemsToSettlementList = new Dictionary<uint, string>();

            try
            {
                string appdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                XmlDocument doc = new XmlDocument();
                doc.Load(File.OpenRead(Path.Combine(Path.Combine(appdir, "SettlementsInfo"), "Settlements.xml")));

                XmlNodeList itemNodes = doc.GetElementsByTagName("settlementitems");
                foreach (XmlNode n in itemNodes)
                {
                    string id = n.Attributes["id"].Value;
                    foreach (XmlNode c in n.ChildNodes)
                    {
                        string siid = c.Attributes["iid"].Value;
                        uint iid = uint.Parse(siid, System.Globalization.NumberStyles.HexNumber, null);
                        _itemsToSettlementList.Add(iid, id);
                    }
                }            
                
                itemNodes = doc.GetElementsByTagName("settlement");
                foreach (XmlNode n in itemNodes)
                {
                    SettlementInfo i = new SettlementInfo(n);
                    uint index;
                    if (n.Attributes["index"] == null)
                    {
                        if (n.Attributes["name"] == null) throw new Exception("need name or index...");
                        index = NameLookup.GetHash(n.Attributes["name"].Value);
                    }
                    else
                    {
                        index = uint.Parse(n.Attributes["index"].Value);
                    }
                    _settlementList.Add(index, i);
                }
            }
            catch { }
        }


        

    }
}

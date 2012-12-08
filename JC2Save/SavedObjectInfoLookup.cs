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
using System.IO;
using System.Xml;

namespace JC2.Save
{
    public static class SavedObjectInfoLookup
    {
        private static readonly List<string> _categories = new List<string>();
        public static ReadOnlyCollection<string> Categories
        {
            get { return _categories.AsReadOnly(); }
        }

        private static readonly Dictionary<string, int> _totalCountByCategory = new Dictionary<string, int>();
        public static ReadOnlyDictionary<string,int> TotalCountByCategory
        {
            get { return new ReadOnlyDictionary<string, int>(_totalCountByCategory); }
        }

        private static readonly Dictionary<string, SavedObjectInfoCollection> _items = new Dictionary<string, SavedObjectInfoCollection>();
        public static ReadOnlyDictionary<string, SavedObjectInfoCollection> Items
        {
            get { return new ReadOnlyDictionary<string, SavedObjectInfoCollection>(_items); }
        }

        public class SavedObjectInfoCollection
        {
            private readonly Dictionary<ulong, SavedObjectInfo> _items = new Dictionary<ulong, SavedObjectInfo>();
            public ReadOnlyDictionary<ulong, SavedObjectInfo> Items
            {
                get { return new ReadOnlyDictionary<ulong, SavedObjectInfo>(_items); }
            }

            public SavedObjectInfoCollection(string name)
            {
                loadList(name);
            }

            private static string getName(string n, XmlNode node)
            {
                if (node != null && node.Attributes != null && node.Attributes["name"] != null)
                {
                    if (n == null) n = node.Attributes["name"].Value;
                    else n = node.Attributes["name"].Value + "\\" + n;
                }
                
                if (node == null) return n;

                return getName(n, node.ParentNode);
            }

            private static Dictionary<string, string> getAttributes(string[] attributeNames, Dictionary<string, string> source, XmlNode node)
            {
                if (node == null)
                {
                    throw new Exception("missing attributes...");
                }
                foreach (string s in attributeNames)
                {
                    if (source.ContainsKey(s) && source[s] != null) continue;
                    if (node.Attributes != null && node.Attributes[s] != null)
                    {
                        source[s] = node.Attributes[s].Value;
                    }
                }
                if (attributeNames.Length != source.Values.Count)
                {
                    source = getAttributes(attributeNames, source, node.ParentNode);
                }
                return source;
            }

            private void loadList(string listName)
            {
                try
                {
                    string appdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(File.OpenRead(Path.Combine(Path.Combine(appdir, "SavedObjectInfo"), Path.ChangeExtension(listName, ".xml"))));

                    XmlNodeList itemNodes = doc.GetElementsByTagName("item");

                    string[] attributeNames = new string[] {
                        "oid",
                        "iid",
                        "x",
                        "y",
                        "itemname",
                        "icon"
                    };

                    foreach (XmlNode n in itemNodes)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();

                        dict = getAttributes(attributeNames, dict, n);

                        string name = dict["itemname"];
                        string iconName = dict["icon"];
                        uint oid = uint.Parse(dict["oid"], System.Globalization.NumberStyles.HexNumber, null);
                        uint iid = uint.Parse(dict["iid"], System.Globalization.NumberStyles.HexNumber, null);

                        // test stuff not part of a settlement
                        /*
                        if (SettlementInfoLookup.SettlementToItemList.ContainsKey(iid))
                        {
                            continue;
                        }
                        */
                        
                        string category = getName(null, n);
                        if (!_categories.Contains(category)) _categories.Add(category);

                        if (_totalCountByCategory.ContainsKey(category)) _totalCountByCategory[category] = _totalCountByCategory[category] + 1;
                        else _totalCountByCategory.Add(category, 1);

                        double x = double.Parse(dict["x"], System.Globalization.CultureInfo.InvariantCulture);
                        double y = double.Parse(dict["y"], System.Globalization.CultureInfo.InvariantCulture);
                        bool bitSet = false;
                        if (n.Attributes["set"] != null)
                        {
                            if(n.Attributes["set"].Value.ToLowerInvariant() == "true" || n.Attributes["set"].Value == "1") bitSet = true;
                        }

                        ulong id = oid;
                        id <<= 32;
                        id |= iid;

                        SavedObjectInfo soi = new SavedObjectInfo(oid, iid, x, y, category, name, iconName, bitSet);
                        
                        _items.Add(id, soi);
                    }
                }
                catch {
                    throw;
                }
            }        
        }

        static SavedObjectInfoLookup()
        {
            _items.Add("ResourceItemHandler", new SavedObjectInfoCollection("ResourceItemHandler"));
            _items.Add("TriggerSaveHandler", new SavedObjectInfoCollection("TriggerSaveHandler"));
            _items.Add("SaveBitStorage", new SavedObjectInfoCollection("SaveBitStorage"));
        }    
    }
}

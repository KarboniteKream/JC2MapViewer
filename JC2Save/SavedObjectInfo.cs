using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC2.Save
{
    public class SavedObjectInfo
    {
        public uint ObjectID { get; private set; }
        public uint InstanceID { get; private set; }

        public string IconName { get; private set; }

        public string Category { get; private set; }
        public string Name { get; private set; }

        public double PosX { get; private set; }
        public double PosY { get; private set; }

        public string SettlementID
        {
            get
            {
                if (SettlementInfoLookup.ItemsToSettlementList.ContainsKey(InstanceID)) return SettlementInfoLookup.ItemsToSettlementList[InstanceID];
                return null;
            }
        }

        public bool IsBitSet { get; private set; }

        public SavedObjectInfo(uint objid, uint instid, double x, double y, string category, string name, string iconName, bool isBitSet)
        {
            ObjectID = objid;
            InstanceID = instid;
            IsBitSet = isBitSet;
            PosX = x;
            PosY = y;
            Category = category;
            Name = name;
            IconName = iconName;
        }

        public override string ToString()
        {
            return string.Format("{0} X:{1} Y:{2}", Name, Math.Round(PosX), Math.Round(PosY));
        }
    }
}

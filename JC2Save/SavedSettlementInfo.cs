using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC2.Save
{
    public class SavedSettlementInfo
    {
        public string ID { get; private set; }
        public SettlementType Type { get; private set; }
        public string Text { get; private set; }

        public int Count { get; private set; }
        public FactionType Faction { get; private set; }
        public bool Discovered { get; private set; }

        public int TotalCount { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public bool Completed
        {
            get { return Discovered && (Count == TotalCount); }
        }

        public int PercentDone
        {
            get { return (int)(100.0 / TotalCount * Count); }
        }

        public SavedSettlementInfo(SettlementInfo baseInfo, int count, bool discovered, bool strongholdOvertaken)
        {
            ID = baseInfo.ID;
            Type = baseInfo.Type;
            Text = baseInfo.Text;
            TotalCount = baseInfo.TotalCount;
            X = baseInfo.X;
            Y = baseInfo.Y;

            if (!strongholdOvertaken) Faction = FactionType.military;

            Count = count;
            Discovered = discovered;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "Discovered:{0} Completed:{1} Name:\"{2}\" Percent:{3}% Faction:{4} Type:{5} X:{6} Y:{7}", Discovered, Completed, Text, PercentDone, Faction, Type, X, Y);
        }
    }
}

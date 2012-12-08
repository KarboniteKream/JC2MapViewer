using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JC2.Save
{
    public enum FactionType
    {
        military,
        ular_boys,
        roaches,
        reapers
    }

    public enum SettlementType
    {
        military_base,
        airport,
        seaport,
        comm_station,
        oilrig,
        commercial,
        civilian,
        stronghold
    }
    
    public class SettlementInfo
    {
        public string ID { get; private set; }
        public SettlementType Type { get; private set; }
        public string Text { get; private set; }

        public FactionType Faction { get; private set; }
        public int TotalCount { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }

        public SettlementInfo(XmlNode node)
        {
            ID = node.Attributes["id"].Value;
            X = float.Parse(node.Attributes["x"].Value, System.Globalization.CultureInfo.InvariantCulture);
            Y = float.Parse(node.Attributes["y"].Value, System.Globalization.CultureInfo.InvariantCulture);
            Text = node.Attributes["text"].Value;
            Type = (SettlementType)Enum.Parse(typeof(SettlementType), node.Attributes["type"].Value);
            TotalCount = int.Parse(node.Attributes["totalcount"].Value);
            if (node.Attributes["faction"] != null)
            {
                Faction = (FactionType)Enum.Parse(typeof(FactionType), node.Attributes["faction"].Value);
            }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "Name:\"{0}\" Faction:{1} Type:{2} X:{3} Y:{4}", Text, Faction, Type, X, Y);
        }
    }
}

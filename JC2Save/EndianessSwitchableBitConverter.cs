using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC2.Save
{
    internal static class EndianessSwitchableBitConverter
    {
        public enum Endianess
        { 
            LITTLE,
            BIG
        }

        public static Endianess Mode { get; set; }

        public static float ToSingle(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToSingle(value, startIndex);
            }
            else
            {
                return BitConverter.ToSingle(swap(value, startIndex, 4), 0);
            }
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToUInt32(value, startIndex);
            }
            else
            {
                return BitConverter.ToUInt32(swap(value, startIndex, 4), 0);
            }
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToInt32(value, startIndex);
            }
            else
            {
                return BitConverter.ToInt32(swap(value, startIndex, 4), 0);
            }
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToInt16(value, startIndex);
            }
            else
            {
                return BitConverter.ToInt16(swap(value, startIndex, 2), 0);
            }
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToInt64(value, startIndex);
            }
            else
            {
                return BitConverter.ToInt64(swap(value, startIndex, 8), 0);
            }
        }

        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            if (Mode == Endianess.LITTLE)
            {
                return BitConverter.ToUInt64(value, startIndex);
            }
            else
            {
                return BitConverter.ToUInt64(swap(value, startIndex, 8), 0);
            }
        }

        private static byte[] swap(byte[] data, int startIndex, int len)
        {
            byte[] returnValue = new byte[len];
            for (int i = 0; i < len; i++) returnValue[len - i - 1] = data[startIndex + i];
            return returnValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Utlilities
{
    public class NumbersUtil
    {
        public static byte RandomByte(byte Max)
        {
            return (byte)(new Random().Next() % Max);
        }
        public static bool IsPair(byte num)
        {
            return num % 2 == 0;
        }
    }
}

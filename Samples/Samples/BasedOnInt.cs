using Liversage.Primitives;
using System;
using System.Globalization;

namespace Samples
{
    [Primitive]
    public readonly partial struct BasedOnInt
    {
        readonly int id;

        public static bool TryParse(string @string, NumberStyles numberStyles, IFormatProvider formatProvider, out BasedOnInt value)
        {
            if (!int.TryParse(@string, numberStyles, formatProvider, out int result))
            {
                value = default;
                return false;
            }
            value = new BasedOnInt(result);
            return true;
        }
    }
}
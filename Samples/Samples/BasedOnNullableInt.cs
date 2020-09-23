using Liversage.Primitives;
using System;
using System.Globalization;

namespace Samples
{
    [Primitive]
    public readonly partial struct BasedOnNullableInt
    {
        readonly int? id;

        public static bool TryParse(string @string, IFormatProvider formatProvider, out BasedOnInt value)
        {
            if (string.IsNullOrEmpty(@string))
            {
                value = default;
                return true;
            }
            if (!int.TryParse(@string, NumberStyles.Integer, formatProvider, out int result))
            {
                value = default;
                return false;
            }
            value = new BasedOnInt(result);
            return true;
        }
    }
}
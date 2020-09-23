using Liversage.Primitives;
using System;
using System.Globalization;

namespace Samples
{
    [Primitive(Features.Default | Features.Formattable)]
    public readonly partial struct BasedOnDateTime
    {
        readonly DateTime timestamp;

        public static BasedOnDateTime UtcNow => FromDateTime(DateTime.UtcNow);

        public static bool TryParse(string s, string format, IFormatProvider formatProvider, DateTimeStyles styles, out BasedOnDateTime value)
        {
            if (!DateTime.TryParseExact(s, format, formatProvider, styles, out var result))
            {
                value = default;
                return false;
            }
            value = new BasedOnDateTime(result);
            return true;
        }
    }
}

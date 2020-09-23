using Liversage.Primitives;
using System;
using System.Linq;

namespace Samples
{
    [Primitive(StringComparison = StringComparison.OrdinalIgnoreCase)]
    public readonly partial struct Currency
    {
        readonly string currency;

        public Currency(string currency)
        {
            if (!IsValid(currency))
                throw new ArgumentException("Invalid currency.", nameof(currency));
            this.currency = currency;
        }

        public override string ToString() => currency.ToUpperInvariant();

        public static bool IsValid(string currency) => currency?.Length == 3 && currency.All(char.IsLetter);
    }
}

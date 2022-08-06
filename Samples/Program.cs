using System;
using System.Globalization;
using System.Linq;
using static System.FormattableString;

namespace Samples;

public static class Program
{
    public static void Main()
    {
        BasedOnIntTest();
        BasedOnNullableIntTest();
        ComparableTest();
        BasedOnGuidTest();
        FormattableTest();
        ParseableTest();
        ConvertibleTest();
        BasedOnDateTimeTest();
        BasedOnNullableStringTest();
        BasedOnNonNullableStringTest();
        CurrencyTest();
        BasedOnCustomTest();
        BasedOnTupleTest();
    }

    static void BasedOnIntTest()
    {
        Console.WriteLine("BasedOnInt");
        Console.WriteLine("==========");

        var id1 = new BasedOnInt(123);
        // Use explicit cast to convert to inner primitive type.
        ToExternal((int) id1);

        static void ToExternal(int id) => Console.WriteLine($"ID: {id}");

        // Inner primitive type is implicitly cast to generated type.
        var id2 = FromExternal(456);
        Console.WriteLine($"ID: {id2}");

        static BasedOnInt FromExternal(int id) => id;

        Console.WriteLine($"{id1} == {id2}: {id1 == id2}");

        Console.WriteLine();
    }

    static void BasedOnNullableIntTest()
    {
        Console.WriteLine("BasedOnNullableInt");
        Console.WriteLine("==================");

        var value1 = (BasedOnNullableInt) 123;
        var value2 = (BasedOnNullableInt) null;
        var value3 = new BasedOnNullableInt();

        Console.WriteLine($"'{value1}' == '{value2}': {value1 == value2}");
        Console.WriteLine(
            $"{(((int?) value2).HasValue ? value2.ToString() : "null")} == {(((int?) value3).HasValue ? value2.ToString() : "null")}: {value2 == value3}");

        Console.WriteLine();
    }

    static void ComparableTest()
    {
        Console.WriteLine("Comparable");
        Console.WriteLine("==========");

        var values = Enumerable.Range(0, 10).Select(_ => (Comparable) Random.Shared.Next(-100, 100)).ToList();

        Console.WriteLine(string.Join(", ", values.OrderBy(comparable => comparable)));
        Console.WriteLine(string.Join(", ", values.Select(comparable => $"{comparable} > 0 = {comparable > Comparable.Zero}")));

        Console.WriteLine();
    }

    static void BasedOnGuidTest()
    {
        Console.WriteLine("BasedOnGuid");
        Console.WriteLine("===========");

        var id1 = BasedOnGuid.CreateNew();
        // Use explicit cast to convert to inner primitive type.
        ToExternal((Guid) id1);

        static void ToExternal(Guid id) => Console.WriteLine($"ID: {id}");

        // Inner primitive type is implicitly cast to generated type.
        var id2 = FromExternal(Guid.NewGuid());
        Console.WriteLine($"ID: {id2}");

        static BasedOnGuid FromExternal(Guid id) => id;

        Console.WriteLine($"{id1} == {id2}: {id1 == id2}");

        Console.WriteLine();
    }

    static void FormattableTest()
    {
        Console.WriteLine("Formattable");
        Console.WriteLine("==========");

        var formattable = (Formattable) 1234.56789M;

        Console.WriteLine(CurrentCulture($"{formattable:N3}"));
        Console.WriteLine(Invariant($"{formattable:N3}"));

        Console.WriteLine();
    }

    static void ParseableTest()
    {
        Console.WriteLine("Parseable");
        Console.WriteLine("==========");

        var parseable1 = new Parseable(-1163220307);
        var @string = parseable1.ToString("X8", CultureInfo.InvariantCulture);
        var result = Parseable.TryParse(@string, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parseable2);
        Console.WriteLine($"'{@string}': {result} => {parseable2}");

        Console.WriteLine();
    }

    static void ConvertibleTest()
    {
        Console.WriteLine("Convertible");
        Console.WriteLine("===========");

        var convertible = (Convertible) 1234.56789F;

        Console.WriteLine(Convert.ToInt32(convertible, CultureInfo.InvariantCulture));
        Console.WriteLine(Convert.ToString(convertible, CultureInfo.InvariantCulture));
        Console.WriteLine(Convert.ToDecimal(convertible, CultureInfo.InvariantCulture));

        Console.WriteLine();
    }

    static void BasedOnDateTimeTest()
    {
        Console.WriteLine("BasedOnDateTime");
        Console.WriteLine("===============");

        var now = BasedOnDateTime.UtcNow;
        var @string = now.ToString("o", CultureInfo.InvariantCulture);
        var result = BasedOnDateTime.TryParse(@string, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var roundtrip);
        Console.WriteLine($"{@string}: {result} => {roundtrip} {roundtrip.ToDateTime().Kind}");

        Console.WriteLine();
    }

    static void BasedOnNullableStringTest()
    {
        Console.WriteLine("BasedOnNullableString");
        Console.WriteLine("=====================");

        var value1 = (BasedOnNullableString) "Hello world";
        var value2 = (BasedOnNullableString) null;
        var value3 = new BasedOnNullableString();

        Console.WriteLine($"'{value1}' == '{value2}': {value1 == value2}");
        // The Roslyn analyzer believes that casting the value to string? by
        // invoking the custom explicit cast operator cannot give a null
        // result which is wrong so the warning is turned off.
#pragma warning disable CA1508 // Avoid dead conditional code
        Console.WriteLine($"{(string?) value2 ?? "null"} == {(string?) value3 ?? "null"}: {value2 == value3}");
#pragma warning restore CA1508 // Avoid dead conditional code

        Console.WriteLine();
    }

    static void BasedOnNonNullableStringTest()
    {
        Console.WriteLine("BasedOnNonNullableString");
        Console.WriteLine("========================");

        try
        {
            var _ = (BasedOnNonNullableString) null;
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }
        var value1 = (BasedOnNonNullableString) "Hello world";
        // Unfortunately, a default constructed value will have a null string
        // breaking the "contract" of this primitive. This is the nature of a
        // value type. If you could override the default constructor for a value
        // type it would become very expensive to create a big array of this
        // type. Instead of just zeroing out the array every element would have
        // to be initialized with a call to the custom constructor.
        var value2 = new BasedOnNonNullableString();

        Console.WriteLine($"'{value1}' == '{value2}': {value1 == value2}");

        Console.WriteLine();
    }

    static void CurrencyTest()
    {
        Console.WriteLine("Currency");
        Console.WriteLine("========");

        try
        {
            var _ = new Currency("invalid");
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }

        var currency1 = Currency.FromString("eur");
        var currency2 = Currency.FromString("EUR");
        Console.WriteLine($"{currency1} == {currency2}: {currency1 == currency2}");
        Console.WriteLine($"{currency1.GetHashCode()} == {currency2.GetHashCode()}: {currency1.GetHashCode() == currency2.GetHashCode()}");

        Console.WriteLine();
    }

    static void BasedOnCustomTest()
    {
        Console.WriteLine("BasedOnCustom");
        Console.WriteLine("==========");

        var value1 = new BasedOnCustom(new FixedPoint(123.456D));
        // Use explicit cast to convert to inner primitive type.
        ToExternal((FixedPoint) value1);

        static void ToExternal(FixedPoint value) => Console.WriteLine($"Value: {value}");

        // Inner primitive type is implicitly cast to generated type.
        var value2 = FromExternal(new FixedPoint(456.789D));
        Console.WriteLine($"Value: {value2}");

        static BasedOnCustom FromExternal(FixedPoint value) => value;

        Console.WriteLine($"{value1} == {value2}: {value1 == value2}");

        Console.WriteLine();
    }

    static void BasedOnTupleTest()
    {
        Console.WriteLine("BasedOnTuple");
        Console.WriteLine("==========");

        var value1 = new BasedOnTuple((Direction.Left, 123));
        // Use explicit cast to convert to inner primitive type.
        ToExternal(((Direction, int)) value1);

        static void ToExternal((Direction, int) tuple) => Console.WriteLine($"Tuple: {tuple}");

        // Inner primitive type is implicitly cast to generated type.
        var value2 = FromExternal((Direction.Right, 456));
        Console.WriteLine($"Tuple: {value2}");

        static BasedOnTuple FromExternal((Direction, int) tuple) => tuple;

        Console.WriteLine($"{value1} == {value2}: {value1 == value2}");

        Console.WriteLine();
    }
}

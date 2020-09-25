# Combatting primitive obsession

This library allows you to generate your own primitives with very little overhead. The code generation integrates with the build pipeline. You create a `partial struct`, decorate it with an attribute and the code generator takes care of the rest.

# Motivation and example

Consider this ficticious order entity:

```csharp
class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public IEnumerable<int> ItemIds { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public OrderStatus Status { get; set; }
}
```

Here all the IDs are integers. This is an example of **primitive obsession** where the same primitive type (`int`) is used to represent values that have different types. Order IDs should not be mixed with customer IDs and none of these should be mixed with order item IDs.

Instead you can introduce distinct types:

```csharp
class Order
{
    public OrderId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public IEnumerable<OrderItemId> ItemIds { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public OrderStatus Status { get; set; }
}
```

The IDs are very simple. This is the `OrderId`:

```csharp
readonly struct OrderId : IEquatable<OrderId>
{
    readonly int value;

    public OrderId(int value) => this.value = value;

    public bool Equals(OrderId other) => Equals(value, other.value);

    public override bool Equals(object obj) => obj is OrderId orderId && Equals(orderId);

    public override int GetHashCode() => value.GetHashCode();
}
```

The other IDs use the same template.

These primitives behave the same way as the built-in primtives of C# like `int`, `long`, `Guid` and `string`. In principle the JITed code for an `int` ID and an ID that wraps an `int` in a `readonly struct` should be the same but in practice the `struct` may have a slight overhead. However, in most cases this overhead shouldn't matter.

# Usage

Add a reference to [`Liversage.Primitives`](https://www.nuget.org/packages/Liversage.Primitives/) (this is a .NET Standard 2.0 NuGet package). Then create your primitive type as a `readonly partial struct` with a field:

```csharp
[Primitive]
readonly partial struct OrderId
{
    readonly int id;
}
```

Adding the `[Primitive]` attribute generates a source file in the `obj` folder of your project. Normally you should not care about this file that is automatically included in your build but to better understand the code generated it's instructive to look at it:

```csharp
readonly partial struct OrderId : IEquatable<OrderId>
{
    public OrderId(int id) => this.id = id;
    public static OrderId FromInt32(int id) => new OrderId(id);
    public static implicit operator OrderId(int id) => FromInt32(id);
    public int ToInt32() => id;
    public static explicit operator int (OrderId id) => id.ToInt32();
    public override string ToString() => id.ToString();
    public bool Equals(OrderId other) => this.id == other.id;
    public override bool Equals(object obj) => obj is OrderId value && Equals(value);
    public override int GetHashCode() => id.GetHashCode();
    public static bool operator ==(OrderId value1, OrderId value2) => value1.Equals(value2);
    public static bool operator !=(OrderId value1, OrderId value2) => !(value1 == value2);
}
```

The code generator adds members to the `struct` so you can use it just like you would use an `int`. It creates an implicit cast so you can use an `int` where an `OrderId` is required:

```csharp
Order GetOrderById(OrderId id) { ... }

// The int 123 is implicitly cast to an OrderId.
var order = GetOrderById(123);
```

You have to use an explicit cast to do it the other way:

```csharp
void UpdateOrderExternal(int id) { ... }

// Use explicit cast to convert OrderId to int.
UpdateOrderExternal((int) orderId);
// Or use the To... method.
UpdateOrderExternal(orderId.ToInt32());
```

Want to use a `long` instead of an `int`? Modify the partial `struct`:

```csharp
[Primitive]
readonly partial struct OrderId
{
    readonly long id;
}
```

The code generator will update the generated methods to match the new type of the field. Instead of an integral type like `int` or `long` you can use `string`:

```csharp
[Primitive]
readonly partial struct OrderId
{
    readonly string id;
}
```

The generated code becomes slightly different because `string` is a reference type which might be `null`.

## Types supported

The code generator supports the following _inner_ types:

- `sbyte`
- `byte`
- `short`
- `ushort`
- `int`
- `uint`
- `long`
- `ulong`
- `decimal`
- `float`
- `double`
- `char`
- `DateTime`
- `DateTimeOffset`
- `TimeSpan`
- `Guid`
- Most immutable `struct`s
- `Nullable<T>` where `T` is supported
- `string`

## Customizing the generated code

The `[Primitive]` attribute has an optional `Features` parameter:

### `Features.None`

This is the baseline used by the code generator. The following members will be generated:

- A constructor that constructs a primitve from an instance of the inner type.
- A static `From...` method (e.g. `FromInt32`) that converts an instance of the inner type to a primitive.
- An implicit cast that casts an instance of the inner type to a primitive.
- A `To...` method (e.g. `ToInt32`) that converts a primtive to an instance of the inner type.
- An explicit cast that casts a primtive to an instance of the inner type.
- A `ToString` method that delegates to the same method of the inner type.

The `From...` and `To...` methods will be named so they match the inner type. However, C# has the concept of _type keywords_ where `int` can be used instead of `System.Int32`. Unfortunately, using the type keyword to create a method name doesn't work so well so instead the name of the type is used. This means that the method names will be `FromInt32` and `ToInt32` and not `Fromint` and `Toint` when the inner type is `int`. For `DateTime` the names will unsuprisingly be `FromDateTime` and `ToDateTime` etc.

### `Features.Equatable`

This is the default and extends the members generated by `Features.None` by implementing `IEquatable<T>`:

- The `IEquatable<T>` interface is implemented by using the `==` operator of the inner type.
- `object.Equals` is overriden based on `IEquatable<T>.Equals`.
- `object.GetHashCode` is overriden and delegates to `GetHashCode` of the inner type.
- Operators `==` and `!=` are created based on `IEquatable<T>`.

### `Features.Formattable`

This provides supports for string formatting:

- The `IFormattable` interface is implemented by delegating `ToString(string format, CultureInfo cultureInfo)` to the inner type.

### `Features.Parsable`

This provides support for parsing strings:

- Add static method `TryParse` that parses a `string` by delegating to the inner type.
- Add static method `TryParse` that parses a `ReadOnlySpan<char>` by delegating to the inner type.

Only the following inner types supports `Features.Parsable`:

- `sbyte`
- `byte`
- `short`
- `ushort`
- `int`
- `uint`
- `long`
- `ulong`
- `decimal`
- `float`
- `double`
- `DateTime`
- `DateTimeOffset`
- `TimeSpan`

The `TryParse` methods for `DateTime` and `DateTimeOffset` delegate to `TryParseExact` with a single format string.

### `Features.Convertible`

This provides support for converting to other types using the static `Convert` class:

- The `IConvertible` interface is implemented by delegating to the inner type.

## Other customizations

### Specifying `StringComparison`

When the inner type is `string` values are by default compared using `StringComparison.Ordinal`. However, another `StringComparison` can be spcified in the `[Primitive]` attribute:

```csharp
[Primitive(StringComparison = StringComparison.OrdinalIgnoreCase)]
readonly partial struct Keyword
{
    readonly string keyword;
}
```

### Providing a constructor

If you provide a constructor in the partial `struct` no constructor will be generated. The same applies to the `ToString` method. You can use that to provide validation:

```csharp
[Primitive(StringComparison = StringComparison.OrdinalIgnoreCase)]
readonly partial struct Currency
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
```

You can use `Currency` as a primitive type. The following expression is `true`:

```
Currency.FromString("eur") == Currency.FromString("EUR")
```

Notice that a `struct` always has a default constructor that will initialize the field to its default value (0, `null` etc.). When this constructor is used (e.g when creating arrays) no validation is performed. Even if you disallow the field to have the default value you should be prepared to handle this value in case the default constructor is used.

# Serialization

A lot of processing in software systems happens at the edge where domain types are serialized to formats like JSON or storage like a relational database. JSON serializers and OR frameworks understand types like `int` and `string` but don't understand the primitive `OrderId`. If you are using DTOs at the edge you will often use an object mapper to convert between domain models and DTOs and chances are that this mapper doesn't understand primitives like `OrderId`.

Fortunately many serializers, OR frameworks and object mappers are extensible and allow custom converters to be used but unfortunately you will have to create these converters yourself. One might argue that since this library already uses code generation it should also code generate relevant converters. This is true but the scope of doing this is very wide and is not include (yet?). In the samples there are some examples that demonstrates how to integrate with Entity Framework Core and `System.Text.Json`.

# Limitations

The code generator has certain expectations about the `struct` that `[Primitive]` is attached to. If these expectations are not upheld it will provide some diagnostic output describing the problem or in many cases just crash. Either way you get compiler warnings or errors but the errors with long stack traces are not so easy to understand compared to the diagnostics messages so there is room for improvement.

# C# 9 source generators

The soon to be released (at this time of writing) C# 9 source generators will provide a language integrated way to generate code. However, this package is built without preview software and instead generates code with the `CodeGeneration.Roslyn` package that today provides features similar to C# 9 source generators.

# Acknowledgements

This project would not be possible without [`CodeGeneration.Roslyn`](https://github.com/aarnott/CodeGeneration.Roslyn).
using System;

namespace Liversage.Primitives;

[Flags]
public enum Features
{
    /// <summary>
    ///   Implements constructor, ToString() method and conversions to and
    ///   from the inner type.
    /// </summary>
    None,

    /// <summary>
    ///   Implements IEquatable&lt;T&gt; using the == operator for the inner
    ///   type and overrides Equals() and GetHashCode() and implements ==
    ///   and != operators. 
    /// </summary>
    Equatable,

    /// <summary>
    ///   Implements IComparable&lt;T&gt; delegating to the inner type and
    ///   implements &lt;, &lt;=, &gt; and &gt;= operators.
    /// </summary>
    Comparable = Equatable << 1,

    /// <summary>
    ///   Implements IFormattable by delegating to the inner type.
    /// </summary>
    Formattable = Comparable << 1,

    /// <summary>
    ///    Implements TryParse() methods by delegating to the inner type.
    /// </summary>
    Parseable = Formattable << 1,

    /// <summary>
    ///   Implements IConvertible by delegating to the inner type.
    /// </summary>
    Convertible = Parseable << 1,

    Default = Equatable
}

﻿using Liversage.Primitives;
using System;

namespace Samples
{
    [Primitive(Features.Default | Features.Formattable | Features.Parseable)]
    public readonly partial struct BasedOnTimeSpan
    {
        readonly TimeSpan duration;
    }
}

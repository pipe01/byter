﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Byter
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Byter
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}

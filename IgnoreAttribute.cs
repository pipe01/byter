using System;

namespace Byter
{
    [Flags]
    public enum Method
    {
        None = 0,
        Serialize = 1,
        Deserialize = 2,
        All = Serialize | Deserialize
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        public Method OnMethod { get; set; } = Method.Serialize | Method.Deserialize;
    }
}

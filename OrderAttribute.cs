using System;
using System.Collections.Generic;
using System.Text;

namespace Byter
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}

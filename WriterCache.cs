using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Writer = System.Action<object, Byter.IByteWriter>;
using static System.Linq.Expressions.Expression;
using System.Linq;

namespace Byter
{
    internal class WriterCache
    {
        private readonly IDictionary<Type, Writer> Writers = new Dictionary<Type, Writer>();

        public Writer GetWriter(Type type)
        {
            if (!Writers.TryGetValue(type, out var writer))
            {
                Writers[type] = writer = MakeWriter(type);
            }

            return writer;
        }

        private Writer MakeWriter(Type type)
        {
            var objParam = Parameter(typeof(object), "obj");
            var writerParam = Parameter(typeof(IByteWriter), "writer");
            var exprs = new List<Expression>();

            var convVar = Variable(type, "converted");
            exprs.Add(Assign(convVar, Convert(objParam, type)));

            var writeMethod = typeof(IByteWriter).GetMethod(nameof(IByteWriter.Write));

            foreach (var (member, memType) in GetSerializedFields(type))
            {
                exprs.Add(Call(writerParam, writeMethod, Convert(MakeMemberAccess(convVar, member), typeof(object)), Constant(memType)));
            }

            return Lambda<Writer>(Block(new[] { convVar }, exprs), objParam, writerParam).Compile();
        }

        private IEnumerable<(MemberInfo, Type)> GetSerializedFields(Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(o => ((MemberInfo)o, o.FieldType))
                .Concat(
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(o => ((MemberInfo)o, o.PropertyType)))
                .Where(o => o.Item1.GetCustomAttribute<IgnoreAttribute>() == null)
                .OrderBy(o => o.Item1.GetCustomAttribute<OrderAttribute>()?.Order ?? 0);
        }
    }
}

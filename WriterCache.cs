using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Writer = System.Action<object, Byter.IByteWriter>;
using static System.Linq.Expressions.Expression;

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

            foreach (var field in GetSerializedFields(type))
            {
                exprs.Add(Call(writerParam, writeMethod, Convert(MakeMemberAccess(convVar, field), typeof(object)), Constant(field.FieldType)));
            }

            return Lambda<Writer>(Block(new[] { convVar }, exprs), objParam, writerParam).Compile();
        }

        private IEnumerable<FieldInfo> GetSerializedFields(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                yield return field;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Reader = System.Func<Byter.IByteReader, object>;
using static System.Linq.Expressions.Expression;
using System.Reflection;

namespace Byter
{
    public class ReaderCache
    {
        private readonly IDictionary<Type, Reader> Readers = new Dictionary<Type, Reader>();

        public Reader GetReader(Type type)
        {
            if (!Readers.TryGetValue(type, out var writer))
            {
                Readers[type] = writer = MakeReader(type);
            }

            return writer;
        }

        private Reader MakeReader(Type type)
        {
            var readMethod = typeof(IByteReader).GetMethod(nameof(IByteReader.Read));

            var readerParam = Parameter(typeof(IByteReader), "reader");

            var exprs = new List<Expression>();

            var objVar = Variable(type, "obj");
            exprs.Add(Assign(objVar, New(type)));

            foreach (var (member, memberType) in Serializer.GetSerializedFields(type, Method.None))
            {
                var attr = member.GetCustomAttribute<IgnoreAttribute>();
                var ignoreSer = attr != null && (attr.OnMethod & Method.Serialize) != 0;
                var ignoreDes = attr != null && (attr.OnMethod & Method.Deserialize) != 0;

                var readerCallExpr = Call(readerParam, readMethod, Constant(memberType));

                if (ignoreDes && !ignoreSer)
                {
                    exprs.Add(readerCallExpr);
                }
                else
                {
                    exprs.Add(Assign(
                       PropertyOrField(objVar, member.Name),
                       Convert(readerCallExpr, memberType)));
                }
            }

            exprs.Add(Convert(objVar, typeof(object)));

            return Lambda<Reader>(Block(typeof(object), new[] { objVar }, exprs), readerParam).Compile();
        }
    }
}

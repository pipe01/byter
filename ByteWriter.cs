using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Byter
{
    public interface IByteWriter
    {
        bool Write(object obj, Type type);
    }

    public class ByteWriter : IByteWriter
    {
        private readonly Stream Stream;
        private readonly BinaryWriter BinWriter;

        public ByteWriter(Stream stream)
        {
            this.Stream = stream;
            this.BinWriter = new BinaryWriter(stream);
        }

        public bool Write(object obj, Type type)
        {
            if (type == typeof(ulong))
                BinWriter.Write((ulong)obj);
            else if (type == typeof(uint))
                BinWriter.Write((uint)obj);
            else if (type == typeof(ushort))
                BinWriter.Write((ushort)obj);
            else if (type == typeof(string))
                BinWriter.Write((string)obj);
            else if (type == typeof(float))
                BinWriter.Write((float)obj);
            else if (type == typeof(sbyte))
                BinWriter.Write((sbyte)obj);
            else if (type == typeof(long))
                BinWriter.Write((long)obj);
            else if (type == typeof(int))
                BinWriter.Write((int)obj);
            else if (type == typeof(short))
                BinWriter.Write((short)obj);
            else if (type == typeof(decimal))
                BinWriter.Write((decimal)obj);
            else if (type == typeof(char[]))
                BinWriter.Write((char[])obj);
            else if (type == typeof(byte[]))
                BinWriter.Write((byte[])obj);
            else if (type == typeof(byte))
                BinWriter.Write((byte)obj);
            else if (type == typeof(bool))
                BinWriter.Write((bool)obj);
            else if (type == typeof(double))
                BinWriter.Write((double)obj);
            else if (type == typeof(char))
                BinWriter.Write((char)obj);
            else if (type.IsEnum)
                Write(obj, type.GetEnumUnderlyingType());
            else
                return false;

            return true;
        }
    }

    public class ExtendedByteWriter : IByteWriter
    {
        private readonly IByteWriter Base;
        private readonly Stream Stream;

        public IDictionary<Type, Action<object, Stream>> CustomWriters { get; } = new Dictionary<Type, Action<object, Stream>>();

        public ExtendedByteWriter(Stream stream, IByteWriter @base)
        {
            this.Base = @base;
            this.Stream = stream;
        }

        public ExtendedByteWriter(Stream stream) : this(stream, new ByteWriter(stream))
        {
        }

        public bool Write(object obj, Type type)
        {
            if (!Base.Write(obj, type))
            {
                if (CustomWriters.TryGetValue(type, out var writer))
                {
                    writer(obj, Stream);
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}

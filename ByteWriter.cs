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
            switch (obj)
            {
                case ulong d:
                    BinWriter.Write(d);
                    break;
                case uint d:
                    BinWriter.Write(d);
                    break;
                case ushort d:
                    BinWriter.Write(d);
                    break;
                case string d:
                    BinWriter.Write(d);
                    break;
                case float d:
                    BinWriter.Write(d);
                    break;
                case sbyte d:
                    BinWriter.Write(d);
                    break;
                case long d:
                    BinWriter.Write(d);
                    break;
                case int d:
                    BinWriter.Write(d);
                    break;
                case short d:
                    BinWriter.Write(d);
                    break;
                case decimal d:
                    BinWriter.Write(d);
                    break;
                case char[] d:
                    BinWriter.Write(d);
                    break;
                case byte[] d:
                    BinWriter.Write(d);
                    break;
                case byte d:
                    BinWriter.Write(d);
                    break;
                case bool d:
                    BinWriter.Write(d);
                    break;
                case double d:
                    BinWriter.Write(d);
                    break;
                case char d:
                    BinWriter.Write(d);
                    break;
                default:
                    return false;
            }

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Byter
{
    public interface IByteReader
    {
        object Read(Type type);
    }

    public class ByteReader : IByteReader
    {
        private readonly BinaryReader BinReader;

        public ByteReader(Stream stream)
        {
            this.BinReader = new BinaryReader(stream);
        }

        public object Read(Type type)
        {
            if (type == typeof(ulong))
                return BinReader.ReadUInt64();
            else if (type == typeof(uint))
                return BinReader.ReadUInt32();
            else if (type == typeof(ushort))
                return BinReader.ReadUInt16();
            else if (type == typeof(string))
                return BinReader.ReadString();
            else if (type == typeof(float))
                return BinReader.ReadSingle();
            else if (type == typeof(sbyte))
                return BinReader.ReadSByte();
            else if (type == typeof(long))
                return BinReader.ReadInt64();
            else if (type == typeof(int))
                return BinReader.ReadInt32();
            else if (type == typeof(short))
                return BinReader.ReadInt16();
            else if (type == typeof(decimal))
                return BinReader.ReadDecimal();
            else if (type == typeof(byte))
                return BinReader.ReadByte();
            else if (type == typeof(bool))
                return BinReader.ReadBoolean();
            else if (type == typeof(double))
                return BinReader.ReadDouble();
            else if (type == typeof(char))
                return BinReader.ReadChar();
            else if (type.IsEnum)
                return Read(type.GetEnumUnderlyingType());

            return null;
        }
    }
}

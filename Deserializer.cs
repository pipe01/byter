using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Byter
{
    public class Deserializer
    {
        public Stream BaseStream { get; set; }

        private readonly bool OwnStream;
        private readonly ReaderCache Cache = new ReaderCache();

        public IByteReader ByteReader { get; set; }
        public bool EOF => BaseStream.Position == BaseStream.Length;

        public Deserializer(Stream stream, bool own = false)
        {
            this.BaseStream = stream;
            this.OwnStream = own;

            this.ByteReader = new ByteReader(stream);
        }

        public Deserializer() : this(new MemoryStream(), true)
        {
        }

        public T Read<T>() => (T)Read(typeof(T));

        public object Read(Type type) => Cache.GetReader(type)(ByteReader);
    }
}

﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("Tester")]

namespace Byter
{
    public class Serializer : IDisposable
    {
        private readonly Stream Stream;
        private readonly bool OwnStream;
        private readonly WriterCache Cache = new WriterCache();

        public IByteWriter ByteWriter { get; set; }

        public Serializer(Stream stream, bool own = false)
        {
            this.Stream = stream;
            this.OwnStream = own;

            this.ByteWriter = new ByteWriter(stream);
        }

        public Serializer() : this(new MemoryStream(), true)
        {
        }

        public void Write<T>(T obj)
        {
            Write(obj, typeof(T));
        }

        public void Write(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "To serialize a null object you must specify its type");

            Write(obj, obj.GetType());
        }

        public void Write(object obj, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Cache.GetWriter(type)(obj, ByteWriter);
        }

        public void WriteStruct<T>(T obj) where T : struct
        {
            int length = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            byte[] buffer = new byte[length];

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            Stream.Write(buffer, 0, length);
        }

        public void Dispose()
        {
            if (OwnStream)
                Stream.Dispose();
        }
    }
}

#region License
//Copyright(c) 2016 Dmytro Mukalov

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

namespace ObjectPort.Formatters
{
    using System;
    using System.IO;
    using System.Text;

    public sealed class Writer : BinaryWriter, IFormatter<Writer>
    {
        private ValueStruct _primitiveBuffer;
        private byte[] _stringByteBuffer;
        private char[] _stringCharBuffer;

        public Stream Stream { get; set; }

        public Encoding Encoding { get; set; }

        public Writer Next { get; set; }
#if !NETCORE
        public bool FromAffinityCache { get; set; }
#endif

        public Writer() : this(Stream.Null, Encoding.Default)
        {
        }

        public Writer(Stream stream, Encoding encoding) : base(stream, encoding)
        {
            Stream = stream;
            Encoding = encoding;
            _primitiveBuffer.Bytes = new byte[Formatter.BytesBufferSize];
            _stringByteBuffer = new byte[Formatter.StringBufferSize];
            _stringCharBuffer = new char[Formatter.StringBufferSize];
        }

        public override void Write(bool value)
        {
            _primitiveBuffer.BoolVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfBool);
        }

        public override void Write(byte value)
        {
            Stream.WriteByte(value);
        }

        public override void Write(char value)
        {
            _primitiveBuffer.CharVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfChar);
        }

        public override void Write(decimal value)
        {
            _primitiveBuffer.DecimalVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfDecimal);
        }

        public override void Write(double value)
        {
            _primitiveBuffer.DoubleVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfDouble);
        }

        public override void Write(float value)
        {
            _primitiveBuffer.FloatVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfFloat);
        }

        public override void Write(int value)
        {
            _primitiveBuffer.IntVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfInt);
        }

        public override void Write(long value)
        {
            _primitiveBuffer.LongVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfLong);
        }

        public override void Write(sbyte value)
        {
            Stream.WriteByte((byte)value);
        }

        public override void Write(short value)
        {
            _primitiveBuffer.ShortVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfShort);
        }

        public override void Write(string value)
        {
            var count = 0;
            try
            {
                count = Encoding.GetBytes(value, 0, value.Length, _stringByteBuffer, 0);
            }
            catch (ArgumentException)
            {
                var newStrBufferLength = Encoding.GetByteCount(value);
                if (newStrBufferLength > _stringByteBuffer.Length)
                    _stringByteBuffer = new byte[newStrBufferLength];
                count = Encoding.GetBytes(value, 0, value.Length, _stringByteBuffer, 0);
            }
            Write((ushort)count);
            Stream.Write(_stringByteBuffer, 0, count);
        }

        public override void Write(uint value)
        {
            _primitiveBuffer.UIntVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfUInt);
        }

        public override void Write(ulong value)
        {
            _primitiveBuffer.ULongVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfULong);
        }

        public override void Write(ushort value)
        {
            _primitiveBuffer.UShortVal[0] = value;
            Stream.Write(_primitiveBuffer.Bytes, 0, Formatter.SizeOfUShort);
        }

        public override void Write(byte[] value)
        {
            Stream.Write(value, 0, value.Length);
        }
    }
}

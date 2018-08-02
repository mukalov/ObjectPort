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

    public sealed class Reader : BinaryReader, IFormatter<Reader>
    {
        private ValueStruct _primitiveBuffer;
        private byte[] _stringByteBuffer;
        private char[] _stringCharBuffer;

        public Stream Stream { get; set; }

        public Encoding Encoding { get; set; }

        public Reader Next { get; set; }
#if !NETCORE
        public bool FromAffinityCache { get; set; }
#endif
        public Reader() : this(Stream.Null, Encoding.Default)
        {
        }

        public Reader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
            Stream = stream;
            Encoding = encoding;
            _primitiveBuffer.Bytes = new byte[Formatter.BytesBufferSize];
            _stringByteBuffer = new byte[Formatter.StringBufferSize];
            _stringCharBuffer = new char[Formatter.StringBufferSize];
        }

        public override bool ReadBoolean()
        {
            _primitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return _primitiveBuffer.BoolVal[0];
        }

        public override byte ReadByte()
        {
            _primitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return _primitiveBuffer.Bytes[0];
        }

        public override char ReadChar()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfChar);
            return _primitiveBuffer.CharVal[0];
        }

        public override decimal ReadDecimal()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfDecimal);
            return _primitiveBuffer.DecimalVal[0];
        }

        public override double ReadDouble()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfDouble);
            return _primitiveBuffer.DoubleVal[0];
        }

        public override float ReadSingle()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfFloat);
            return _primitiveBuffer.FloatVal[0];
        }

        public Guid ReadGuid()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfGuid);
            return new Guid(_primitiveBuffer.Bytes);
        }

        public override int ReadInt32()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfInt);
            return _primitiveBuffer.IntVal[0];
        }

        public override long ReadInt64()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfLong);
            return _primitiveBuffer.LongVal[0];
        }

        public override sbyte ReadSByte()
        {
            _primitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return (sbyte)_primitiveBuffer.Bytes[0];
        }

        public override short ReadInt16()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfShort);
            return _primitiveBuffer.ShortVal[0];
        }

        public override string ReadString()
        {
            var length = ReadInt16();
            if (_stringByteBuffer.Length < length)
            {
                _stringByteBuffer = new byte[length];
                if (_stringCharBuffer.Length < length)
                    _stringCharBuffer = new char[length];
            }
            Stream.Read(_stringByteBuffer, 0, length);
            var chars = Encoding.GetChars(_stringByteBuffer, 0, length, _stringCharBuffer, 0);
            return new string(_stringCharBuffer, 0, chars);
        }

        public override uint ReadUInt32()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfUInt);
            return _primitiveBuffer.UIntVal[0];
        }

        public override ulong ReadUInt64()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfULong);
            return _primitiveBuffer.ULongVal[0];
        }

        public override ushort ReadUInt16()
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, Formatter.SizeOfUShort);
            return _primitiveBuffer.UShortVal[0];
        }

        public override byte[] ReadBytes(int count)
        {
            Stream.Read(_primitiveBuffer.Bytes, 0, count);
            return _primitiveBuffer.Bytes;
        }
    }
}

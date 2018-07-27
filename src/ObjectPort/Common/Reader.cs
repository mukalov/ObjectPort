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

namespace ObjectPort.Common
{
    using System;

    public sealed class Reader : Formatter<Reader>
    {
        public bool ReadBool()
        {
            PrimitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return PrimitiveBuffer.BoolVal[0];
        }

        public byte ReadByte()
        {
            PrimitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return PrimitiveBuffer.Bytes[0];
        }

        public char ReadChar()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfChar);
            return PrimitiveBuffer.CharVal[0];
        }

        public decimal ReadDecimal()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfDecimal);
            return PrimitiveBuffer.DecimalVal[0];
        }

        public double ReadDouble()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfDouble);
            return PrimitiveBuffer.DoubleVal[0];
        }

        public float ReadFloat()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfFloat);
            return PrimitiveBuffer.FloatVal[0];
        }

        public Guid ReadGuid()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfGuid);
            return new Guid(PrimitiveBuffer.Bytes);
        }

        public int ReadInt()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfInt);
            return PrimitiveBuffer.IntVal[0];
        }

        public long ReadLong()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfLong);
            return PrimitiveBuffer.LongVal[0];
        }

        public sbyte ReadSByte()
        {
            PrimitiveBuffer.Bytes[0] = (byte)Stream.ReadByte();
            return (sbyte)PrimitiveBuffer.Bytes[0];
        }

        public short ReadShort()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfShort);
            return PrimitiveBuffer.ShortVal[0];
        }

        public string ReadString()
        {
            var length = ReadShort();
            if (StringByteBuffer.Length < length)
                StringByteBuffer = new byte[length];
            Read(StringByteBuffer, 0, length);
            var chars = Encoding.GetChars(StringByteBuffer, 0, length, StringCharBuffer, 0);
            return new string(StringCharBuffer, 0, chars);
        }

        public uint ReadUInt()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfUInt);
            return PrimitiveBuffer.UIntVal[0];
        }

        public ulong ReadULong()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfULong);
            return PrimitiveBuffer.ULongVal[0];
        }

        public ushort ReadUShort()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfUShort);
            return PrimitiveBuffer.UShortVal[0];
        }

        public byte[] ReadBytes()
        {
            var length = ReadInt();
            var buffer = new byte[length];
            Read(buffer, 0, length);
            return buffer;
        }

        public DateTime ReadDateTime()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfLong);
            return DateTime.FromBinary(PrimitiveBuffer.LongVal[0]);
        }

        public TimeSpan ReadTimeSpan()
        {
            Read(PrimitiveBuffer.Bytes, 0, SizeOfLong);
            return TimeSpan.FromTicks(PrimitiveBuffer.LongVal[0]);
        }

        private void Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            while (read < count)
                read += Stream.Read(buffer, offset + read, count - read);
        }
    }
}

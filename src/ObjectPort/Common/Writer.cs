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
    using System.IO;
    using System.Text;

    public sealed class Writer : Formatter<Writer>
    {
        public void Write(bool value)
        {
            PrimitiveBuffer.BoolVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfBool);
        }

        public void Write(byte value)
        {
            Stream.WriteByte(value);
        }

        public void Write(char value)
        {
            PrimitiveBuffer.CharVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfChar);
        }

        public void Write(decimal value)
        {
            PrimitiveBuffer.DecimalVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfDecimal);
        }

        public void Write(double value)
        {
            PrimitiveBuffer.DoubleVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfDouble);
        }

        public void Write(float value)
        {
            PrimitiveBuffer.FloatVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfFloat);
        }

        public void Write(Guid value)
        {
            var bytes = value.ToByteArray();
            Stream.Write(bytes, 0, bytes.Length);
        }

        public void Write(int value)
        {
            PrimitiveBuffer.IntVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfInt);
        }

        public void Write(long value)
        {
            PrimitiveBuffer.LongVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfLong);
        }

        public void Write(sbyte value)
        {
            Stream.WriteByte((byte)value);
        }

        public void Write(short value)
        {
            PrimitiveBuffer.ShortVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfShort);
        }

        public void Write(string value)
        {
            var count = 0;
            try
            {
                count = Encoding.GetBytes(value, 0, value.Length, StringByteBuffer, 0);
            }
            catch (ArgumentException)
            {
                var newStrBufferLength = Encoding.GetByteCount(value);
                if (newStrBufferLength > StringByteBuffer.Length)
                    StringByteBuffer = new byte[newStrBufferLength];
                count = Encoding.GetBytes(value, 0, value.Length, StringByteBuffer, 0);
            }
            Write((ushort)count);
            Stream.Write(StringByteBuffer, 0, count);
        }

        public void Write(uint value)
        {
            PrimitiveBuffer.UIntVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfUInt);
        }

        public void Write(ulong value)
        {
            PrimitiveBuffer.ULongVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfULong);
        }

        public void Write(ushort value)
        {
            PrimitiveBuffer.UShortVal[0] = value;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfUShort);
        }

        public void Write(byte[] value)
        {
            Stream.Write(value, 0, value.Length);
        }

        public void Write(DateTime value)
        {
            PrimitiveBuffer.LongVal[0] = value.ToBinary();
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfLong);
        }

        public void Write(TimeSpan value)
        {
            PrimitiveBuffer.LongVal[0] = value.Ticks;
            Stream.Write(PrimitiveBuffer.Bytes, 0, SizeOfLong);
        }
    }
}

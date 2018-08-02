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
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal struct ValueStruct
    {
        [FieldOffset(0)]
        internal byte[] Bytes;
        [FieldOffset(0)]
        internal bool[] BoolVal;
        [FieldOffset(0)]
        internal char[] CharVal;
        [FieldOffset(0)]
        internal decimal[] DecimalVal;
        [FieldOffset(0)]
        internal double[] DoubleVal;
        [FieldOffset(0)]
        internal float[] FloatVal;
        [FieldOffset(0)]
        internal int[] IntVal;
        [FieldOffset(0)]
        internal long[] LongVal;
        [FieldOffset(0)]
        internal sbyte[] SByteVal;
        [FieldOffset(0)]
        internal short[] ShortVal;
        [FieldOffset(0)]
        internal uint[] UIntVal;
        [FieldOffset(0)]
        internal ulong[] ULongVal;
        [FieldOffset(0)]
        internal ushort[] UShortVal;
    }
}

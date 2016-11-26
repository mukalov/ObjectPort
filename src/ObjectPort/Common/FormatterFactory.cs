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
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System;
    using Common;

    internal class FormatterFactory<T>
        where T : Formatter<T>, new()
    {
        private const int IncrementalCapacity = 512;
        private const int ShortcutsCapacity = 512;

        private static T _first;
        private static T _last;
        private static T[] _affinityCache = new T[ShortcutsCapacity];
        private static object _locker = new object();

        static FormatterFactory()
        {
            _first = new T();
            InitFormattersPool(_first);
        }

        private static void InitFormattersPool(T from)
        {
            var currentFormater = from;
            for (var i = 0; i < IncrementalCapacity - 1; i++)
            {
                currentFormater.Next = new T();
                currentFormater = currentFormater.Next;
            }
            _last = currentFormater;
        }

        internal static T GetFormatter(Stream stream, Encoding encoding)
        {
            var formatter = default(T);

#if !NETCORE
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (threadId < _affinityCache.Length)
                formatter = _affinityCache[threadId];
            if (formatter == default(T))
#endif
            {
                formatter = Interlocked.Exchange<T>(ref _first, _first.Next);
                if (formatter == null)
                {
                    lock (_locker)
                    {
                        _first = _last;
                        InitFormattersPool(_first);
                    }
                    formatter = Interlocked.Exchange<T>(ref _first, _first.Next);
                    if (formatter == null)
                        formatter.OutOfFormattersPoolCapacity();
                }

#if !NETCORE
                if (threadId < _affinityCache.Length)
                {
                    _affinityCache[threadId] = formatter;
                    formatter.FromAffinityCache = true;
                }
#endif
            }

            formatter.Stream = stream;
            formatter.Encoding = encoding;
            formatter.Next = null;
            return formatter;
        }

        internal static void ReleaseFormatter(T formatter)
        {
#if !NETCORE
            if (!formatter.FromAffinityCache)
#endif
            {
                var oldLast = Interlocked.Exchange(ref _last, formatter);
                oldLast.Next = formatter;
            }
        }
    }
}

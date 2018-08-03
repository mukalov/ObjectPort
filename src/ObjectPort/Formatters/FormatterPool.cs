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
    using System.IO;
    using System.Text;
    using System.Threading;

    internal class FormatterPool<T>
        where T : class, IFormatter<T>, new()
    {
        internal struct FormatterHolder
        {
            public T Formatter;
            private long alignment1;
            private long alignment2;
            private long alignment3;
            private long alignment4;
            private long alignment5;
            private long alignment6;
            private long alignment7;
            private long alignment8;
        }

        private const int PoolInitialCapacity = 512;
        private const int PoolIncrementalCapacity = 64;
        private const int ShortcutsCapacity = 512;

        private static T _first;
        private static T _last;
        private static FormatterHolder[] _affinityCache = new FormatterHolder[ShortcutsCapacity];
        private static object _locker = new object();

        static FormatterPool()
        {
            _first = new T();
            InitFormattersPool(_first);
        }

        private static void InitFormattersPool(T from)
        {
            var currentFormater = from;
            for (var i = 0; i < PoolInitialCapacity - 1; i++)
            {
                currentFormater.Next = new T();
                currentFormater = currentFormater.Next;
            }
            _last = currentFormater;
            _last.Next = _last;
        }

        internal static T GetFormatter(Stream stream, Encoding encoding)
        {
            var formatter = default(T);

#if !NETCORE
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (threadId < ShortcutsCapacity)
                formatter = _affinityCache[threadId].Formatter;
            if (formatter == default(T))
#endif
            {
                for (;;)
                {
                    T first = default(T);
                    do
                    {
                        first = _first;
                        formatter = Interlocked.CompareExchange(ref _first, first.Next, first);
                    } while (formatter != first);

                    if (formatter.Next == formatter)
                    {
                        lock (_locker)
                        {
                            if (formatter.Next == formatter)
                            {
                                var newFirst = new T();
                                var currentFormater = newFirst;
                                for (var i = 0; i < PoolIncrementalCapacity - 1; i++)
                                {
                                    currentFormater.Next = new T();
                                    currentFormater = currentFormater.Next;
                                }
                                currentFormater.Next = formatter;
                                Interlocked.Exchange(ref _first, newFirst);
                            }
                        }
                    }
                    else
                        break;
                }
#if !NETCORE
                if (threadId < _affinityCache.Length)
                {
                    _affinityCache[threadId].Formatter = formatter;
                    formatter.FromAffinityCache = true;
                }
#endif
            }

            formatter.Stream = stream;
            formatter.Encoding = encoding;
            return formatter;
        }

        internal static void ReleaseFormatter(T formatter)
        {
#if !NETCORE
            if (!formatter.FromAffinityCache)
#endif
            {
                formatter.Next = formatter;
                var oldLast = Interlocked.Exchange(ref _last, formatter);
                oldLast.Next = formatter;
            }
        }
    }
}

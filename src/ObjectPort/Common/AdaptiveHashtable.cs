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
    using System.Collections.Generic;
    using System.Linq;

    internal class AdaptiveHashtable<T>
    {
        internal struct Item
        {
            public byte Loaded;
            public uint Key;
            public T Value;
        }


        private const double MinimalLoadFactor = 0.0000001;
        private const double LoadFactorStep = 0.1;
        private const uint DefaultCapacity = 512;
        private const uint DefaultDepth = 1;
        private const uint LengthTreshold = 0xffff;

        private Item[,] _values;
        private uint _length;
        private uint _depth;
        private uint _loaded;
        private List<uint> _loadedIndexes;

        public AdaptiveHashtable() : this(DefaultCapacity, DefaultDepth)
        {
        }

        public AdaptiveHashtable(uint length, uint depth)
        {
            _length = length;
            _depth = depth;
            _values = new Item[_length, _depth];
            _loaded = 0;
            _loadedIndexes = new List<uint>();
        }

        public T TryGetValue(uint key)
        {
            var index = key % _length;
            var item = _values[index, 0];
            for (var i = 0; i < item.Loaded; i++)
            {
                item = _values[index, i];
                if (item.Key == key)
                    return item.Value;
            }
            return default(T);
        }

        public T GetValue(uint key)
        {
            var index = key % _length;
            var item = _values[index, 0];
            for (var i = 0; i < item.Loaded; i++)
            {
                item = _values[index, i];
                if (item.Key == key)
                    return item.Value;
            }
            throw new ArgumentException($"Key not found {key}");
        }

        public void AddValue(uint key, T value)
        {
            Action<uint> registerIndexHandler = (i) =>
            {
                _loadedIndexes.Add(i);
                _loaded++;
            };


            var index = AddValue(_values, _length, _depth, key, value);
            if (index != uint.MaxValue)
            {
                registerIndexHandler(index);
                return;
            }

            var loadFactor = (double)_loaded / (_length * _depth);
            var loadFactorStep = LoadFactorStep;
            var newLength = _length;
            Item[,] newValues;
            var newIndexes = new List<uint>();
            bool rebuilt;
            do
            {
                if (loadFactor < MinimalLoadFactor)
                    throw new InvalidOperationException("Could not rebuild the values");

                while (loadFactor <= loadFactorStep)
                    loadFactorStep *= loadFactorStep;
                loadFactor -= loadFactorStep;
                if (newLength < LengthTreshold)
                    newLength = Convert.ToUInt32(_loaded / loadFactor / _depth);
                else
                    _depth++;
                newValues = new Item[newLength, _depth];
                newIndexes.Clear();
                rebuilt = Rebuild(_values, newValues, newLength, _depth, _loadedIndexes, newIndexes);
                if (rebuilt)
                    index = AddValue(newValues, newLength, _depth, key, value);
            } while (index == uint.MaxValue || !rebuilt);

            _values = newValues;
            _loadedIndexes = newIndexes;
            _length = newLength;
            registerIndexHandler(index);
        }

        public AdaptiveHashtable<T> Clone()
        {
            var copy = new AdaptiveHashtable<T>(_length, _depth);
            Array.Copy(_values, 0, copy._values, 0, _length);
            copy._loaded = _loaded;
            copy._loadedIndexes = _loadedIndexes.ToList();
            return copy;
        }

        private static uint AddValue(Item[,] values, uint length, uint depth, uint key, T value)
        {
            var index = key % length;
            var loaded = values[index, 0].Loaded;

            if (loaded >= depth)
                return uint.MaxValue;

            values[index, loaded].Key = key;
            values[index, loaded].Value = value;
            values[index, 0].Loaded++;
            return index;
        }

        private static bool Rebuild(Item[,] oldValues, Item[,] newValues, uint newLength, uint newDepth, IList<uint> oldIndexes, IList<uint> newIndexes)
        {
            foreach (var index in oldIndexes)
            {
                var loaded = oldValues[index, 0].Loaded;
                for (var i = 0; i < loaded; i++)
                {
                    var newInd = AddValue(newValues, newLength, newDepth, oldValues[index, i].Key, oldValues[index, i].Value);
                    if (newInd == uint.MaxValue)
                        return false;
                    newIndexes.Add(newInd);
                }
            }
            return true;
        }
    }
}

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

namespace ObjectPort.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

#if !NET40
    [Collection("ObjectPort")]
#endif
    public class OtherEnumerableMembersTests : TestsBase
    {
        protected static readonly int[] TestIntArray = new[] { -345, 54, -456456, 3453455 };
        protected static readonly string[] TestStringArray = new[] { "Test string 1", null, "Test string 3", "Test string 4" };
        protected static readonly TestCustomStruct[] TestArray = new[]
        {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
        };

        private static readonly TestCustomStruct[][] TestArrayOfArray = new TestCustomStruct[][]
        {
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            },
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            },
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            }
        };

        private static readonly List<TestCustomStruct>[] TestArrayOfList = new List<TestCustomStruct>[]
        {
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            }.ToList(),
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            }.ToList(),
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            }.ToList()
        };

        private static readonly Dictionary<int, TestCustomStruct>[] TestArrayOfDictionary = new Dictionary<int, TestCustomStruct>[]
        {
            new Dictionary<int, TestCustomStruct>
            {
                [1] = new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                [2] = new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                [3] = new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                [4] = new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            },
            new Dictionary<int, TestCustomStruct>
            {
                [1] = new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                [2] = new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                [3] = new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            },
            new Dictionary<int, TestCustomStruct>
            {
                [1] = new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                [2] = new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                [3] = new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                [4] = new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            }
        };


        private static readonly HashSet<TestCustomStruct> TestHashSet = new HashSet<TestCustomStruct>(TestArray);
        private static readonly IEnumerable<TestCustomStruct> TestHashSetFromEnumerable = new HashSet<TestCustomStruct>(TestArray);
        private static readonly LinkedList<TestCustomStruct> TestLinkedList = new LinkedList<TestCustomStruct>(TestArray);
        private static readonly IEnumerable<TestCustomStruct> TestLinkedListFromEnumerable = new LinkedList<TestCustomStruct>(TestArray);

        private static readonly Queue<TestCustomStruct> TestQueue = new Queue<TestCustomStruct>(TestArray);
        private static readonly IEnumerable<TestCustomStruct> TestQueueFromEnumerable = new Queue<TestCustomStruct>(TestArray);
        private static readonly SortedSet<TestCustomStruct> TestSortedSet = new SortedSet<TestCustomStruct>(TestArray);
        private static readonly IEnumerable<TestCustomStruct> TestSortedSetFromEnumerable = new SortedSet<TestCustomStruct>(TestArray);
        private static readonly Stack<TestCustomStruct> TestStack = new Stack<TestCustomStruct>(TestArray);
        private static readonly IEnumerable<TestCustomStruct> TestStackFromEnumerable = new Stack<TestCustomStruct>(TestArray);



        [Fact]
        public void Should_Serialize_Array_Of_Array()
        {
            TestStructField(TestArrayOfArray);
            TestStructField(TestArrayOfArray);
            TestStructProperty(TestArrayOfArray);
            TestStructProperty(TestArrayOfArray);
            TestClassField(TestArrayOfArray);
            TestClassField(TestArrayOfArray);
            TestClassProperty(TestArrayOfArray);
            TestClassProperty(TestArrayOfArray);
        }

        [Fact]
        public void Should_Serialize_Array_Of_List()
        {
            TestStructField(TestArrayOfList);
            TestStructField(TestArrayOfList);
            TestStructProperty(TestArrayOfList);
            TestStructProperty(TestArrayOfList);
            TestClassField(TestArrayOfList);
            TestClassField(TestArrayOfList);
            TestClassProperty(TestArrayOfList);
            TestClassProperty(TestArrayOfList);
        }

        [Fact]
        public void Should_Serialize_Array_Of_Dictionary()
        {
            TestStructField(TestArrayOfDictionary);
            TestStructField(TestArrayOfDictionary);
            TestStructProperty(TestArrayOfDictionary);
            TestStructProperty(TestArrayOfDictionary);
            TestClassField(TestArrayOfDictionary);
            TestClassField(TestArrayOfDictionary);
            TestClassProperty(TestArrayOfDictionary);
            TestClassProperty(TestArrayOfDictionary);
        }

        [Fact]
        public void Should_Serialize_List_Of_Array()
        {
            TestStructField(TestArrayOfArray.ToList());
            TestStructField(TestArrayOfArray.ToList());
            TestStructProperty(TestArrayOfArray.ToList());
            TestStructProperty(TestArrayOfArray.ToList());
            TestClassField(TestArrayOfArray.ToList());
            TestClassField(TestArrayOfArray.ToList());
            TestClassProperty(TestArrayOfArray.ToList());
            TestClassProperty(TestArrayOfArray.ToList());
        }

        [Fact]
        public void Should_Serialize_List_Of_List()
        {
            TestStructField(TestArrayOfList.ToList());
            TestStructField(TestArrayOfList.ToList());
            TestStructProperty(TestArrayOfList.ToList());
            TestStructProperty(TestArrayOfList.ToList());
            TestClassField(TestArrayOfList.ToList());
            TestClassField(TestArrayOfList.ToList());
            TestClassProperty(TestArrayOfList.ToList());
            TestClassProperty(TestArrayOfList.ToList());
        }

        [Fact]
        public void Should_Serialize_List_Of_Dictionary()
        {
            TestStructField(TestArrayOfDictionary.ToList());
            TestStructField(TestArrayOfDictionary.ToList());
            TestStructProperty(TestArrayOfDictionary.ToList());
            TestStructProperty(TestArrayOfDictionary.ToList());
            TestClassField(TestArrayOfDictionary.ToList());
            TestClassField(TestArrayOfDictionary.ToList());
            TestClassProperty(TestArrayOfDictionary.ToList());
            TestClassProperty(TestArrayOfDictionary.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_Of_Array_From_Array()
        {
            TestStructField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestStructField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestStructProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestStructProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestClassField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestClassField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestClassProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
            TestClassProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_Of_Array_From_List()
        {
            TestStructField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestStructField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestStructProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestStructProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestClassField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestClassField<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestClassProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
            TestClassProperty<IEnumerable<TestCustomStruct[]>>(TestArrayOfArray.ToList());
        }

        [Fact]
        public void Should_Serialize_HashSet()
        {
            TestStructField(TestHashSet);
            TestStructField(TestHashSet);
            TestStructProperty(TestHashSet);
            TestStructProperty(TestHashSet);
            TestClassField(TestHashSet);
            TestClassField(TestHashSet);
            TestClassProperty(TestHashSet);
            TestClassProperty(TestHashSet);
        }

        [Fact]
        public void Should_Serialize_HashSet_From_Enumerable()
        {
            TestStructField(TestHashSetFromEnumerable);
            TestStructField(TestHashSetFromEnumerable);
            TestStructProperty(TestHashSetFromEnumerable);
            TestStructProperty(TestHashSetFromEnumerable);
            TestClassField(TestHashSetFromEnumerable);
            TestClassField(TestHashSetFromEnumerable);
            TestClassProperty(TestHashSetFromEnumerable);
            TestClassProperty(TestHashSetFromEnumerable);
        }

        [Fact]
        public void Should_Serialize_LinkedList()
        {
            TestStructField(TestLinkedList);
            TestStructField(TestLinkedList);
            TestStructProperty(TestLinkedList);
            TestStructProperty(TestLinkedList);
            TestClassField(TestLinkedList);
            TestClassField(TestLinkedList);
            TestClassProperty(TestLinkedList);
            TestClassProperty(TestLinkedList);
        }

        [Fact]
        public void Should_Serialize_LinkedList_From_Enumerable()
        {
            TestStructField(TestLinkedListFromEnumerable);
            TestStructField(TestLinkedListFromEnumerable);
            TestStructProperty(TestLinkedListFromEnumerable);
            TestStructProperty(TestLinkedListFromEnumerable);
            TestClassField(TestLinkedListFromEnumerable);
            TestClassField(TestLinkedListFromEnumerable);
            TestClassProperty(TestLinkedListFromEnumerable);
            TestClassProperty(TestLinkedListFromEnumerable);
        }

        [Fact]
        public void Should_Serialize_Queue()
        {
            TestStructField(TestQueue);
            TestStructField(TestQueue);
            TestStructProperty(TestQueue);
            TestStructProperty(TestQueue);
            TestClassField(TestQueue);
            TestClassField(TestQueue);
            TestClassProperty(TestQueue);
            TestClassProperty(TestQueue);
        }

        [Fact]
        public void Should_Serialize_Queue_From_Enumerable()
        {
            TestStructField(TestQueueFromEnumerable);
            TestStructField(TestQueueFromEnumerable);
            TestStructProperty(TestQueueFromEnumerable);
            TestStructProperty(TestQueueFromEnumerable);
            TestClassField(TestQueueFromEnumerable);
            TestClassField(TestQueueFromEnumerable);
            TestClassProperty(TestQueueFromEnumerable);
            TestClassProperty(TestQueueFromEnumerable);
        }

        [Fact]
        public void Should_Serialize_SortedSet()
        {
            TestStructField(TestSortedSet);
            TestStructField(TestSortedSet);
            TestStructProperty(TestSortedSet);
            TestStructProperty(TestSortedSet);
            TestClassField(TestSortedSet);
            TestClassField(TestSortedSet);
            TestClassProperty(TestSortedSet);
            TestClassProperty(TestSortedSet);
        }

        [Fact]
        public void Should_Serialize_SortedSet_From_Enumerable()
        {
            TestStructField(TestSortedSetFromEnumerable);
            TestStructField(TestSortedSetFromEnumerable);
            TestStructProperty(TestSortedSetFromEnumerable);
            TestStructProperty(TestSortedSetFromEnumerable);
            TestClassField(TestSortedSetFromEnumerable);
            TestClassField(TestSortedSetFromEnumerable);
            TestClassProperty(TestSortedSetFromEnumerable);
            TestClassProperty(TestSortedSetFromEnumerable);
        }

        [Fact]
        public void Should_Serialize_Stack()
        {
            TestStructField(TestStack);
            TestStructField(TestStack);
            TestStructProperty(TestStack);
            TestStructProperty(TestStack);
            TestClassField(TestStack);
            TestClassField(TestStack);
            TestClassProperty(TestStack);
            TestClassProperty(TestStack);
        }
    }
}

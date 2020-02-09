using System;
using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using NUnit.Framework;
using Unity.Collections;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests.Collections
{
    public class TestNativeBinaryHeap
    {
        private static NativeBinaryHeap<int> CreateEmptyHeap()
        {
            return new NativeBinaryHeap<int>(0, Allocator.TempJob);
        }
        
        private static void AssertRequiresReadOrWriteAccess(
            NativeBinaryHeap<int> heap,
            Action action)
        {
            heap.TestUseOnlySetAllowReadAndWriteAccess(false);
            try
            {
                Assert.That(
                    () => action(),
                    Throws.TypeOf<InvalidOperationException>());
            }
            finally
            {
                heap.TestUseOnlySetAllowReadAndWriteAccess(true);
            }
        }
        
        [Test]
        public void ConstructorCreatesEmptySet()
        {
            using(NativeBinaryHeap<int> heap = new NativeBinaryHeap<int>(1, Allocator.Temp))
            {
                Assert.That(heap.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void ConstructorClampsToMinimumCapacity()
        {
            using(NativeBinaryHeap<int> heap = new NativeBinaryHeap<int>(1, Allocator.Temp))
            {
                Assert.That(heap.Capacity, Is.GreaterThan(1));
            }
        }

        [Test]
        [TestCase(3, 16)]
        [TestCase(7, 16)]
        [TestCase(15, 16)]
        [TestCase(31, 32)]
        [TestCase(63, 64)]
        public void ConstructorChoosesCapacityClosestToNextPowerOfTwo(int capacity, int expected)
        {
            using(NativeBinaryHeap<int> heap = new NativeBinaryHeap<int>(capacity, Allocator.Temp))
            {
                Assert.That(heap.Capacity, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ConstructorRequiresValidAllocator()
        {
            Assert.That(() => new NativeBinaryHeap<int>(1, default),
                Throws.Exception);
        }

        [Test]
        public void PeakEmptyHeapReturnsNull()
        {
            using(NativeBinaryHeap<int> heap = CreateEmptyHeap())
            {
                Assert.That(heap.Peak(), Is.Null);
            }
        }

        [Test]
        public void PushAndPopFunctionsCorrectly()
        {
            using(NativeBinaryHeap<int> heap = CreateEmptyHeap())
            {
                heap.Push(1);
                heap.Push(5);
                heap.Push(2);

                Assert.That(heap.Peak(), Is.EqualTo(5));
                Assert.That(heap.Length, Is.EqualTo(3));

                Assert.That(heap.Pop(), Is.EqualTo(5));
                Assert.That(heap.Pop(), Is.EqualTo(2));
                Assert.That(heap.Pop(), Is.EqualTo(1));
                Assert.That(heap.Pop(), Is.Null);
            }
        }

        [Test]
        public void GetLengthRequiresReadAccess()
        {
            using(NativeBinaryHeap<int> heap = CreateEmptyHeap())
            {
                int len;
                AssertRequiresReadOrWriteAccess(
                    heap,
                    () => len = heap.Length);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
using static Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Collections
{
    [NativeContainer]
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeBinaryHeapDebugView<>))]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeBinaryHeap<T>
        : IEnumerable<T>, IDisposable
        where T: unmanaged, IComparable<T>
    {
        
        [NativeDisableUnsafePtrRestriction]
        private UnsafeBinaryHeap* binaryHeapData;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle safety;

        [NativeSetClassTypeToNullOnSchedule]
        DisposeSentinel disposeSentinel;
#endif
        public NativeBinaryHeap(Allocator allocator)
            : this(0, allocator) { }
        
        public NativeBinaryHeap(int initialCapacity, Allocator allocator)
        {
            // Insist on a minimum capacity
            initialCapacity = ceilpow2(initialCapacity);
            initialCapacity = max(initialCapacity, 4);

            var sizeOf = SizeOf<T>();
            var totalSize = sizeOf * (long)initialCapacity;
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(allocator <= Allocator.None)
            {
                throw new ArgumentException(
                    "Allocator must be Temp, TempJob or Persistent",
                    nameof(allocator));
            }

            if(totalSize > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), $"Capacity * sizeof(T) cannot exceed {int.MaxValue} bytes");
            }
            DisposeSentinel.Create(out this.safety, out this.disposeSentinel, 0, allocator);
#endif
            
            var alignOf = AlignOf<T>();
            
            this.binaryHeapData = UnsafeBinaryHeap.Create(sizeOf, alignOf, initialCapacity, allocator);
        }

        public int Capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(this.safety);
#endif

                return this.binaryHeapData->Capacity;
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(this.safety);
                if(value < this.binaryHeapData->Length)
                {
                    throw new ArgumentException("Capacity must be larger than the length of the NativeBinaryTree.");
                }
#endif
                this.binaryHeapData->SetCapacity<T>(value);
            }
        }

        public int Length
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(this.safety);
#endif

                return this.binaryHeapData->Length;
            }
        }

        public bool IsEmpty
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(this.safety);
#endif

                return this.binaryHeapData->IsEmpty;
            }
        }

        [WriteAccessRequired]
        public void Push(T value)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(this.safety);
#endif
            
            this.binaryHeapData->Push(value);
        }

        public Nullable<T> Peak()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(this.safety);
#endif

            return this.binaryHeapData->Peak<T>();
        }

        [WriteAccessRequired]
        public Nullable<T> Pop()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(this.safety);
#endif

            return this.binaryHeapData->Pop<T>();
        }

        private T this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(this.safety);
#endif
                return ReadArrayElement<T>(this.binaryHeapData->Ptr, index);
            }
        }

        void Deallocate()
        {
            UnsafeBinaryHeap.Destroy(this.binaryHeapData);
            this.binaryHeapData = null;
        }

        [WriteAccessRequired]
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref this.safety, ref this.disposeSentinel);
#endif
            
            this.Deallocate();
        }

        public NativeArray<T> AsArray()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(this.safety);
            var arraySafety = this.safety;
            AtomicSafetyHandle.UseSecondaryVersion(ref arraySafety);
#endif

            var array = ConvertExistingDataToNativeArray<T>(
                this.binaryHeapData->Ptr,
                this.binaryHeapData->Length,
                Allocator.Invalid);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            SetAtomicSafetyHandle(ref array, arraySafety);
#endif
            return array;
        }

        public T[] ToArray()
        {
            return this.AsArray().ToArray();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void TestUseOnlySetAllowReadAndWriteAccess(bool allowReadOrWriteAccess)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.SetAllowReadOrWriteAccess(
                this.safety,
                allowReadOrWriteAccess);
#endif
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private NativeBinaryHeap<T> heap;
            private int index;

            public Enumerator(ref NativeBinaryHeap<T> heap)
            {
                this.heap = heap;
                this.index = -1;
            }

            public void Dispose(){ }

            public bool MoveNext()
            {
                ++this.index;
                return this.index < this.heap.Length;
            }

            public void Reset()
            {
                this.index = -1;
            }

            public T Current => this.heap[this.index];

            object IEnumerator.Current => this.Current;
        }
    }

    internal sealed class NativeBinaryHeapDebugView<T>
        where T : unmanaged, IComparable<T>
    {
        private NativeBinaryHeap<T> heap;

        public NativeBinaryHeapDebugView(NativeBinaryHeap<T> heap)
        {
            this.heap = heap;
        }

        public T[] Items => this.heap.ToArray();
    }
}

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Collections {
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    internal unsafe struct UnsafeBinaryHeap: IDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        internal void* Ptr;
        internal int Length;
        internal int Capacity;
        internal Allocator Allocator;

        public unsafe UnsafeBinaryHeap(int sizeOf, int alignOf, int initialCapacity, Allocator allocator)
        {
            this.Allocator = allocator;
            this.Ptr = null;
            this.Length = 0;
            this.Capacity = 0;

            if(initialCapacity != 0)
            {
                SetCapacity(sizeOf, alignOf, initialCapacity);
            }
        }
   
        void Realloc(int sizeOf, int alignOf, int capacity)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.Allocator == Allocator.Invalid)
            {
                throw new Exception("UnsafeBinaryHeap is not initialized, it must be initialized with allocator before use.");
            }
#endif

            void* newPointer = null;

            if(capacity > 0)
            {
                var bytesToMalloc = sizeOf * capacity;
                newPointer = Malloc(bytesToMalloc, alignOf, this.Allocator);

                if(this.Capacity > 0)
                {
                    var itemsToCopy = math.min(capacity, this.Capacity);
                    var bytesToCopy = itemsToCopy * sizeOf;
                    MemCpy(newPointer, this.Ptr, bytesToCopy);
                }
            }

            Free(this.Ptr, this.Allocator);

            this.Ptr = newPointer;
            this.Capacity = capacity;
            this.Length = math.min(this.Length, capacity);
        }

        public void SetCapacity(int sizeOf, int alignOf, int capacity)
        {
            var newCapacity = math.max(capacity, 64 / sizeOf);
            newCapacity = math.ceilpow2(newCapacity);

            if(newCapacity == this.Capacity)
            {
                return;
            }

            this.Realloc(sizeOf, alignOf, newCapacity);
        }

        public void SetCapacity<T>(int capacity) where T : unmanaged
        {
            SetCapacity(SizeOf<T>(), AlignOf<T>(), capacity);
        }

        public void Resize(int sizeOf, int alignOf, int length)
        {
            if(length > this.Capacity)
            {
                SetCapacity(sizeOf, alignOf, length);
            }

            this.Length = length;
        }

        public void Resize<T>(int length) where T : struct
        {
            Resize(SizeOf<T>(), AlignOf<T>(), length);
        }

        public void Push<T>(T value)
            where T : unmanaged, IComparable<T>
        {
            var idx = this.Length;

            if(this.Length + 1 > this.Capacity)
            {
                Resize<T>(idx + 1);
            }
            else
            {
                this.Length += 1;
            }

            WriteArrayElement(this.Ptr, idx, value);

            var isValidMaxHeap = false;

            int heapUpCurrentIndex = idx;
            int parentIndex;
            T parent;
            
            while(!isValidMaxHeap && heapUpCurrentIndex > 0)
            {
                parentIndex = (heapUpCurrentIndex - 1) / 2;;
                parent = ReadArrayElement<T>(this.Ptr, parentIndex);

                if(value.CompareTo(parent) > 0)
                {
                    WriteArrayElement(this.Ptr, heapUpCurrentIndex, parent);
                    WriteArrayElement(this.Ptr, parentIndex, value);
                    heapUpCurrentIndex = parentIndex;
                }
                else
                {
                    isValidMaxHeap = true;
                }
            }
        }

        public Nullable<T> Peak<T>() where T : unmanaged
        {
            if(this.Length == 0)
            {
                return null;
            }

            return ReadArrayElement<T>(this.Ptr, 0);
        }

        public Nullable<T> Pop<T>()
            where T : unmanaged, IComparable<T>
        {
            if(this.Length == 0)
            {
                return null;
            }

            var rootValue = ReadArrayElement<T>(this.Ptr, 0);

            var lastItem = ReadArrayElement<T>(this.Ptr, --this.Length);

            WriteArrayElement(this.Ptr, 0, lastItem);

            var i = 0;
            while(true)
            {
                var leftIndex = i * 2 + 1;
                var rightIndex = i * 2 + 2;
                var largestIndex = i;

                var current = ReadArrayElement<T>(this.Ptr, i);

                var largest = current;

                if(leftIndex < this.Length)
                {
                    var left = ReadArrayElement<T>(this.Ptr, leftIndex);

                    if(left.CompareTo(largest) > 0)
                    {
                        largestIndex = leftIndex;
                        largest = left;
                    }
                }

                if(rightIndex < this.Length)
                {
                    var right = ReadArrayElement<T>(this.Ptr, rightIndex);

                    if(right.CompareTo(largest) > 0)
                    {
                        largestIndex = rightIndex;
                        largest = right;
                    }
                }

                if(largestIndex != i)
                {
                    WriteArrayElement(this.Ptr, i, largest);
                    WriteArrayElement(this.Ptr, largestIndex, current);
                    i = largestIndex;
                    continue;
                }

                break;
            }

            return rootValue;
        }

        public static UnsafeBinaryHeap* Create(int sizeOf, int alignOf, int initialCapacity, Allocator allocator)
        {
            var binaryHeapDataSize = SizeOf<UnsafeBinaryHeap>();
            UnsafeBinaryHeap* binaryHeapData = (UnsafeBinaryHeap*)Malloc(binaryHeapDataSize, AlignOf<UnsafeBinaryHeap>(), allocator);
            MemClear(binaryHeapData, binaryHeapDataSize);

            binaryHeapData->Allocator = allocator;

            if(initialCapacity != 0)
            {
                binaryHeapData->SetCapacity(sizeOf, alignOf, initialCapacity);
            }

            return binaryHeapData;
        }

        public static void Destroy(UnsafeBinaryHeap* binaryHeapData)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(binaryHeapData == null)
            {
                throw new Exception("UnsafeBinaryHeap has yet to be created or has been destroyed!");
            }
#endif
            var allocator = binaryHeapData->Allocator;
            binaryHeapData->Dispose();
            Free(binaryHeapData, allocator);
        }

        public void Dispose()
        {
            if(this.Allocator != Allocator.Invalid)
            {
                Free(this.Ptr, this.Allocator);
                this.Allocator = Allocator.Invalid;
            }

            this.Ptr = null;
            this.Length = 0;
            this.Capacity = 0;
        }

        public bool IsCreated => this.Ptr != null;

        public bool IsEmpty => this.Length == 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Geometry
{
    public partial struct VectorLine
    {
        private int2 start;
        private int2 end;
    }

    // Impl
    public partial struct VectorLine
    {
        public static VectorLine New(int2 start, int2 end)
        {
            return new VectorLine
            {
                start = start,
                end = end
            };
        }
    }

    // Iterator
    public partial struct VectorLine
    {
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        #region Nested type: Enumerator

        public struct Enumerator : IEnumerator<int2>, IEnumerator, IDisposable
        {
            private readonly int2 end;
            private readonly float2 slope;
            private float2 currentPos;
            private bool finished;
            private bool reallyFinished;
            private int2? current;

            object IEnumerator.Current => this.Current;

            public int2 Current => this.current.Value;

            public Enumerator(ref VectorLine vectorLine)
            {
                var currentPos = new float2(vectorLine.start) + new float2(0.5f);
                var destination = new float2(vectorLine.end) + new float2(0.5f);
                var slope = normalize(destination - currentPos);

                this.end = vectorLine.end;
                this.currentPos = currentPos;
                this.slope = slope;
                this.finished = false;
                this.reallyFinished = false;
                this.current = null;
            }

            public bool MoveNext()
            {
                if(this.finished)
                {
                    if(!this.reallyFinished)
                    {
                        this.reallyFinished = true;
                        this.current = new int2(this.currentPos);
                        return true;
                    }
                    else
                    {
                        this.current = null;
                        return false;
                    }
                }
                else
                {
                    var currentPoint = new int2(this.currentPos);
                    this.currentPos += this.slope;
                    var newPoint = new int2(this.currentPos);

                    if(newPoint.x == this.end.x && newPoint.y == this.end.y)
                    {
                        this.finished = true;
                    }

                    this.current = currentPoint;
                    return true;
                }
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose() { }
        }

        #endregion
    }
}

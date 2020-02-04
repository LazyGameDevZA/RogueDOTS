using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Geometry
{
    public partial struct BresenhamCircleNoDiag
    {
        private int2 center;
        private int radius;
    }

    // Impl
    public partial struct BresenhamCircleNoDiag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BresenhamCircleNoDiag(int2 center, int radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }

    // Iterator
    public partial struct BresenhamCircleNoDiag
    {
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        #region Nested type: Enumerator

        public struct Enumerator : IEnumerator<int2>, IEnumerator, IDisposable
        {
            private readonly int2 center;
            private int x, y;
            private int error;
            private byte quadrant;
            private int2? current;

            object IEnumerator.Current => this.Current;

            public int2 Current => this.current.Value;

            public Enumerator(ref BresenhamCircleNoDiag bresenhamCircle)
            {
                this.center = bresenhamCircle.center;
                this.x = -bresenhamCircle.radius;
                this.y = 0;
                this.error = 0;
                this.quadrant = 1;
                this.current = null;
            }

            public bool MoveNext()
            {
                if(this.x < 0)
                {
                    var point = int2.zero;

                    switch(this.quadrant)
                    {
                        case 1:
                            point = new int2(this.center.x - this.x, this.center.y + this.y);
                            break;
                        case 2:
                            point = new int2(this.center.x - this.y, this.center.y - this.x);
                            break;
                        case 3:
                            point = new int2(this.center.x + this.x, this.center.y - this.y);
                            break;
                        case 4:
                            point = new int2(this.center.x + this.y, this.center.y + this.x);
                            break;
                        default:
                            Assert.IsTrue(false, "quadrant value was out of range");
                            break;
                    }

                    if(this.quadrant == 4)
                    {
                        // This version moves in x or in y - not both - depending on the error.
                        if(abs(this.error + 2 * this.x + 1) <= abs(this.error + 2 * this.y + 1))
                        {
                            this.error += this.x * 2 + 1;
                            this.x += 1;
                        }
                        else
                        {
                            this.error += this.y * 2 + 1;
                            this.y += 1;
                        }
                    }

                    this.quadrant = (byte)(this.quadrant % 4 + 1);

                    this.current = point;
                    return true;
                }
                else
                {
                    this.current = null;
                    return false;
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

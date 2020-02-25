using System;
using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using Unity.Collections;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Pathfinding
{
    public static class AStar
    {
        private const int MaxAStarSteps = 65536; 
        
        public static NavigationPath AStarSearch<TMap>(int start, int end, in TMap map)
            where TMap : IBaseMap
        {
            return new AStarCalc(start, end).Search(map);
        }

        public partial struct NavigationPath
        {
            public int Destination;
            public bool Success;
            public NativeList<int> Steps;
        }

        private partial struct Node
        {
            public int Idx;
            public float F;
            public float G;
            public float H;
        }
    
        private partial struct Node: IComparable<Node>
        {
            public int CompareTo(Node other)
            {
                // This is inverted!!!
                return other.F.CompareTo(this.F);
            }
        }
    
        public partial struct NavigationPath
        {
            public static NavigationPath New(int stepCount = 0)
            {
                return new NavigationPath
                {
                    Destination = 0,
                    Success = false,
                    Steps = new NativeList<int>(stepCount, Allocator.Temp)
                };
            }
        }
    
        private partial struct AStarCalc
        {
            private int start;
            private int end;
            private NativeBinaryHeap<Node> openList;
            private NativeHashMap<int, float> closedList;
            private NativeHashMap<int, int> parents;
            private int stepCounter;
        }

        private partial struct AStarCalc
        {
            public AStarCalc(int start, int end)
            {
                var openList = new NativeBinaryHeap<Node>(Allocator.Temp);
                
                openList.Push(new Node
                {
                    Idx = start,
                    F = 0f,
                    G = 0f,
                    H = 0f
                });

                this.start = start;
                this.end = end;
                this.openList = openList;
                this.closedList = new NativeHashMap<int, float>(openList.Capacity, Allocator.Temp);
                this.parents = new NativeHashMap<int, int>(openList.Capacity, Allocator.Temp);
                this.stepCounter = 0;
            }

            private float DistanceToEnd<TMap>(int idx, in TMap map)
                where TMap : IBaseMap
            {
                return map.GetPathingDistance(idx, this.end);
            }

            bool AddSuccessor<TMap>(Node q, int idx, float cost, in TMap map)
                where TMap : IBaseMap
            {
                if(idx == this.end)
                {
                    this.parents.Add(idx, q.Idx);
                    return true;
                }
                else
                {
                    var distance = this.DistanceToEnd(idx, in map);
                    var s = new Node
                    {
                        Idx = idx,
                        F = distance + cost,
                        G = cost,
                        H = distance
                    };

                    var shouldAdd = true;

                    foreach(var e in this.openList)
                    {
                        if(e.F < s.F && e.Idx == idx)
                        {
                            shouldAdd = false;
                        }
                    }

                    if(shouldAdd && this.closedList.ContainsKey(idx) && this.closedList[idx] < s.F)
                    {
                        shouldAdd = false;
                    }

                    if(shouldAdd)
                    {
                        this.openList.Push(s);
                        // Update parent
                        if(!this.parents.TryAdd(idx, q.Idx))
                        {
                            this.parents[idx] = q.Idx;
                        }
                    }

                    return false;
                }
            }

            private NavigationPath FoundIt
            {
                get
                {
                    var reverseSteps = new NativeList<int>(Allocator.Temp);

                    reverseSteps.Add(this.end);
                    var current = this.end;
                    while(current != this.start)
                    {
                        var parent = this.parents[current];
                        reverseSteps.Add(parent);
                        current = parent;
                    }
                    
                    var result = NavigationPath.New(reverseSteps.Length);
                    result.Success = true;
                    result.Destination = this.end;

                    for(int i = reverseSteps.Length - 1; i >= 0; i--)
                    {
                        result.Steps.Add(reverseSteps[i]);
                    }

                    return result;
                }
            }

            public NavigationPath Search<TMap>(in TMap map)
                where TMap : IBaseMap
            {
                var result = NavigationPath.New();
                var successors = new NativeList<Exit>(8, Allocator.Temp);

                while(!this.openList.IsEmpty && this.stepCounter < MaxAStarSteps)
                {
                    this.stepCounter += 1;

                    // ReSharper disable once PossibleInvalidOperationException
                    var q = this.openList.Pop().Value;

                    successors = map.GetAvailableExits(q.Idx, successors);

                    for(int i = 0; i < successors.Length; i++)
                    {
                        var (index, cost) = successors[i];
                        if(this.AddSuccessor(q, index, cost + q.F, in map))
                        {
                            var success = this.FoundIt;
                            return success;
                        }
                    }

                    if(this.closedList.ContainsKey(q.Idx))
                    {
                        this.closedList.Remove(q.Idx);
                    }

                    this.closedList.Add(q.Idx, q.F);
                }

                return result;
            }
        }
    }
}

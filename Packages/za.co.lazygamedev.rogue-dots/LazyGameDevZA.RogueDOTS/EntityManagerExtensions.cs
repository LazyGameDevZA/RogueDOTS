using Unity.Collections;
using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS
{
    public static class EntityManagerExtensions
    {
        public static void Insert<T>(this EntityManager entityManager, NativeArray<T> array, string name) where T : struct, IBufferElementData
        {
            var entity = entityManager.CreateEntity(typeof(T));
            entityManager.SetName(entity, name);
            var buffer = entityManager.GetBuffer<T>(entity);
            buffer.Capacity = array.Length;

            foreach(var t in array)
            {
                buffer.Add(t);
            }
        }
    }
}

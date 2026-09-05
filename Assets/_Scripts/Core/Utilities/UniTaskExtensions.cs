using Core.Pooling;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Core.Utilities
{
    public static class UniTaskExtensions
    {
        public static async UniTask DespawnAfter<T>(this T obj, float delay, ObjectPool<T> pool) where T : MonoBehaviour
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            if (pool != null)
            {
                pool.Despawn(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace InspectorLock.Internal
{
    internal class CachableAssetReference<Class> where Class : class, new()
    {
        private static Dictionary<string, Object> CachedInstances = new Dictionary<string, Object>();

        private static Class instance;
        public static Class Instance => instance ??= new Class();

        /// <summary>
        /// Attempts to find the specified asset in AssetDatabase. If successful, result is cached in this class for faster access later.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="assetPath"></param>
        /// <returns>The Object as Type T. Default if not found.</returns>
        protected T GetAsset<T>(string assetName, string assetPath) where T : Object =>
            IsAssetCached(assetName) ? ReturnCachedAsset<T>(assetName)
                                     : FindAndCacheAsset<T>(assetPath, assetName);


        protected bool IsAssetCached(string assetName) => CachedInstances.ContainsKey(assetName);

        /// <summary>
        /// Uses AssetDatabase to search for an asset of Type T at the supplied path. Caches the result if successful to avoid searching again.
        /// </summary>
        /// <typeparam name="T">Type T to find.</typeparam>
        /// <param name="assetPath">The file path to search.</param>
        /// <param name="assetName">The string Key to store the cached instance as.</param>
        /// <returns>The Type T instance. Default if not found.</returns>
        protected T FindAndCacheAsset<T>(string assetPath, string assetName) where T : Object
        {
            var assetInstance = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (assetInstance == null)
            {
                Debug.LogWarning($"Asset in path: '{assetPath}' could not be found and was not cached.");
                return default;
            }

            CachedInstances.Add(assetName, assetInstance);
            return ReturnCachedAsset<T>(assetName);
        }

        protected T ReturnCachedAsset<T>(string assetName) where T : Object
        {
            if (!CachedInstances.TryGetValue(assetName, out var instance))
            {
                Debug.LogWarning($"Could not return cached asset with name {assetName}.");
                return default;
            }

            if (instance is not T)
            {
                Debug.LogWarning($"Cached asset '{assetName}' is not of type {typeof(T)}.");
                return default;
            }

            return (T)instance;
        }
    }
}


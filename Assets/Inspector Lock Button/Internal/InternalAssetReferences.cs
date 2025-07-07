using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace EditorLock
{
    public class InternalAssetReferences
    {
        private record CachedAsset        
        {
            public string AssetName;
            public Object CachedInstance;
        }

        private static Dictionary<string, Object> CachedInstances = new Dictionary<string, Object>();

        // Is this allowed singleton pattern?
        private static InternalAssetReferences instance;
        public static InternalAssetReferences Instance => instance == null ? instance = new InternalAssetReferences()
                                                                           : instance;

        public readonly string uxmlLockableObjectTemplatePath = "Assets/Inspector Lock Button/UI/UXML/LockableUXMLTemplate.uxml";
        public readonly string uxmlEditorLockButtonPath = "Assets/Inspector Lock Button/UI/UXML/EditorLockButton.uxml";
        public readonly string uxmlFolderPathSelectionPath = "Assets/Inspector Lock Button/UI/UXML/FolderPathSelection.uxml";
        
        public readonly string ussLockedButtonPath = "Assets/Inspector Lock Button/UI/USS/LockedButton.uss";
        public readonly string ussUnlockedButtonPath = "Assets/Inspector Lock Button/UI/USS/UnlockedButton.uss";
        public readonly string ussEditorLockButtonPath = "Assets/Inspector Lock Button/UI/USS/EditorLockButtonStyle.uss";

        public readonly string texture2DLockedIcon = "Assets/Inspector Lock Button/UI/Icons/locked.png";
        public readonly string texture2DUnlockedIcon = "Assets/Inspector Lock Button/UI/Icons/unlocked.png";

        public VisualTreeAsset UxmlLockableObjTemplateTree => GetAsset<VisualTreeAsset>(nameof(UxmlLockableObjTemplateTree), uxmlLockableObjectTemplatePath);
        public VisualTreeAsset UxmlEditorLockButtonTree => GetAsset<VisualTreeAsset>(nameof(UxmlEditorLockButtonTree), uxmlEditorLockButtonPath);
        public VisualTreeAsset UxmlFolderPathSelectionTree => GetAsset<VisualTreeAsset>(nameof(UxmlFolderPathSelectionTree), uxmlFolderPathSelectionPath);

        public StyleSheet UssLockedButtonSheet => GetAsset<StyleSheet>(nameof(UssLockedButtonSheet), ussLockedButtonPath);
        public StyleSheet UssUnlockedButtonSheet => GetAsset<StyleSheet>(nameof(UssUnlockedButtonSheet), ussUnlockedButtonPath);

        public Texture2D tex2DLockedIcon => GetAsset<Texture2D>(nameof(tex2DLockedIcon), texture2DLockedIcon);
        public Texture2D text2DUnlockedIcon => GetAsset<Texture2D>(nameof(text2DUnlockedIcon), texture2DUnlockedIcon);



        /// <summary>
        /// Attempts to find the specified asset in AssetDatabase. If successful, result is cached in this class for faster access later.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="assetPath"></param>
        /// <returns>The Object as Type T. Default if not found.</returns>
        private T GetAsset<T>(string assetName, string assetPath) where T : Object =>
            IsAssetCached(assetName) ? ReturnCachedAsset<T>(assetName)
                                     : FindAndCacheAsset<T>(assetPath, assetName); 
  

        private bool IsAssetCached(string assetName) => CachedInstances.ContainsKey(assetName);
        
        /// <summary>
        /// Uses AssetDatabase to search for an asset of Type T at the supplied path. Caches the result if successful to avoid searching again.
        /// </summary>
        /// <typeparam name="T">Type T to find.</typeparam>
        /// <param name="assetPath">The file path to search.</param>
        /// <param name="assetName">The string Key to store the cached instance as.</param>
        /// <returns>The Type T instance. Default if not found.</returns>
        private T FindAndCacheAsset<T>(string assetPath, string assetName) where T: Object
        {
            var assetInstance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            
            if(assetInstance == null)
            {
                Debug.LogWarning($"Asset in path: '{assetPath}' could not be found and was not cached.");
                return default;
            }

           CachedInstances.Add(assetName, assetInstance);
           return ReturnCachedAsset<T>(assetName);
        }

        private T ReturnCachedAsset<T>(string assetName) where T : Object
        {
            if (!CachedInstances.TryGetValue(assetName, out var instance))
            {
                Debug.LogWarning($"Could not return cached asset with name {assetName}.");
                return default;
            }

            if(instance is not T)
            {
                Debug.LogWarning($"Cached asset '{assetName}' is not of type {typeof(T)}.");
                return default;
            }

            return (T) instance;
        }
    } 
}


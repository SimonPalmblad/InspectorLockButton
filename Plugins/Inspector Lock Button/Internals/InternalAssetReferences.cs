using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Runtime.CompilerServices;
using InspectorLock.Internal;

namespace InspectorLock.Internals
{
    internal class InternalAssetReferences : CachableAssetReference<InternalAssetReferences>
    {
        public readonly string uxmlLockableObjectTemplatePath = "Assets/Plugins/Inspector Lock Button/UI/UXML/LockableUXMLTemplate.uxml";
        public readonly string uxmlInspectorLockButtonPath = "Assets/Plugins/Inspector Lock Button/UI/UXML/InspectorLockButton.uxml";
        public readonly string uxmlFolderPathSelectionPath = "Assets/Plugins/Inspector Lock Button/UI/UXML/FolderPathSelection.uxml";

        public readonly string ussLockedButtonPath = "Assets/Plugins/Inspector Lock Button/UI/USS/LockedButton.uss";
        public readonly string ussUnlockedButtonPath = "Assets/Plugins/Inspector Lock Button/UI/USS/UnlockedButton.uss";
        public readonly string ussInspectorLockButtonPath = "Assets/Plugins/Inspector Lock Button/UI/USS/InspectorLockButtonStyle.uss";

        public readonly string texture2DLockedIcon = "Assets/Plugins/Inspector Lock Button/UI/Icons/LockedIcon.png";
        public readonly string texture2DUnlockedIcon = "Assets/Plugins/Inspector Lock Button/UI/Icons/UnlockedIcon.png";

        public readonly string globalLockSettingsPath = "Assets/Plugins/Inspector Lock Button/Editor Lock Settings.asset";

        public VisualTreeAsset UxmlLockableObjTemplateTree => GetAsset<VisualTreeAsset>(nameof(UxmlLockableObjTemplateTree), uxmlLockableObjectTemplatePath);
        public VisualTreeAsset UxmlInspectorLockButtonTree => GetAsset<VisualTreeAsset>(nameof(UxmlInspectorLockButtonTree), uxmlInspectorLockButtonPath);
        public VisualTreeAsset UxmlFolderPathSelectionTree => GetAsset<VisualTreeAsset>(nameof(UxmlFolderPathSelectionTree), uxmlFolderPathSelectionPath);

        public StyleSheet UssLockedButtonSheet => GetAsset<StyleSheet>(nameof(UssLockedButtonSheet), ussLockedButtonPath);
        public StyleSheet UssUnlockedButtonSheet => GetAsset<StyleSheet>(nameof(UssUnlockedButtonSheet), ussUnlockedButtonPath);

        public Texture2D tex2DLockedIcon => GetAsset<Texture2D>(nameof(tex2DLockedIcon), texture2DLockedIcon);
        public Texture2D text2DUnlockedIcon => GetAsset<Texture2D>(nameof(text2DUnlockedIcon), texture2DUnlockedIcon);
    }
}


using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#pragma warning disable 0414
#pragma warning disable IDE0051 // Remove unused private members

namespace Voltrig.VoltSpriter
{
    public partial class VSSettings : ScriptableObject //VSSettingsProvider
    {
        internal const string settingsPath = "Voltrig/Volt Spriter";

        private class VSSettingsProvider : SettingsProvider
        {
            public const string vsSettingsFolderGUID = "0bd5c037aa5d3be48b5729b2d207d5c0";
            public const string vsSettingsName = "VSSettings.asset";

            private SerializedObject vsSettings;

            private Editor VSSettingsEditor;

            public VSSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
                : base(path, scope)
            {

            }

            public static bool IsSettingsAvailable()
            {
                string folderPath = AssetDatabase.GUIDToAssetPath(vsSettingsFolderGUID);

                if (string.IsNullOrEmpty(folderPath))
                {
                    return false;
                }

                string path = Path.Combine(folderPath, vsSettingsName);

                return File.Exists(path);
            }

            public override void OnActivate(string searchContext, VisualElement rootElement)
            {
                // This function is called when the user clicks on the VSSettings element in the Settings window.
                if (VSSettingsEditor != null)
                {
                    return;
                }

                VSSettingsEditor = Editor.CreateEditor(VSSettings.GetOrCreateSettings());
            }

            public override void OnDeactivate()
            {
                VSSettingsEditor = null;
            }

            public override void OnGUI(string searchContext)
            {
                VSSettingsEditor.OnInspectorGUI();
            }

            [SettingsProvider]
            private static SettingsProvider CreateVSSettingsProvider()
            {
                VSSettingsProvider provider = new VSSettingsProvider(VSSettings.settingsPath, SettingsScope.Project);

                // Automatically extract all keywords from the Styles.
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<VSSettingsProvider>();
                return provider;
            }
        }
    }
}
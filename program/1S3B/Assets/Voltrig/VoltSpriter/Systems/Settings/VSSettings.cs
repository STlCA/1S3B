using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#pragma warning disable 0414

namespace Voltrig.VoltSpriter
{
    [CreateAssetMenu(fileName = nameof(VSSettings), menuName = nameof(Voltrig) + "/" + nameof(VoltSpriter) + "/" + nameof(VSSettings), order = 1)]
    public partial class VSSettings : ScriptableObject
    {
        [Header("Edit mode")]
        [Tooltip("If set to TRUE sprite pivots will be editable in the edit mode in the sprite editor.")]
        public bool editPivot = true;

        [Tooltip("If set to TRUE sprite borders will be editable in the edit mode in the sprite editor.")]
        public bool editBorders = true;

        [Tooltip("If set to TRUE sprite rects will be editable in the edit mode in the sprite editor.")]
        public bool editRect = true;

        [Header("Sprite Editor")]

        [Tooltip("Size of the mode and hover texts inside the sprite editor.")]
        public int hoverTextSize = 12;

        [Tooltip("Font style of the mode and hover texts inside the sprite editor.")]
        public FontStyle hoverFontStyle = FontStyle.Bold;

        [Header("VS Window")]
        public VSSpritePanelSettings spritePanelSettings;

        internal event System.Action OnValidateE;

        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            VSSettings vsSettings = EditorUtility.InstanceIDToObject(instanceID) as VSSettings;

            if (vsSettings != null)
            {
                Selection.activeObject = null;
                SettingsService.OpenProjectSettings(settingsPath);
                return true;
            }

            return false;
        }

        private void OnValidate()
        {
            OnValidateE?.Invoke();
        }

        internal static VSSettings GetOrCreateSettings()
        {
            string folderPath = AssetDatabase.GUIDToAssetPath(VSSettingsProvider.vsSettingsFolderGUID);
            string path;
            VSSettings settings;

            if (string.IsNullOrEmpty(folderPath))
            {
                VSConsole.LogError(typeof(VSSettings), "Failed to find Data folder. Please reinstall.");
                return null;
            }

            path = Path.Combine(folderPath, VSSettingsProvider.vsSettingsName);

            settings = AssetDatabase.LoadAssetAtPath<VSSettings>(path);

            if (settings == null)
            {
                VSConsole.LogWarning(typeof(VSSettings), "Failed to load VSSettings. Attempting to recreate.");

                settings = ScriptableObject.CreateInstance<VSSettings>();
                settings.ResetToDefault();

                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
            }

            if (settings == null)
            {
                VSConsole.LogError(typeof(VSSettings), "Failed to recreate VSSettings. Please reinstall the Volt Spriter.");
                return null;
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        [ContextMenu("Reset to default")]
        private void ResetToDefault()
        {
            editPivot = true;
            editBorders = true;
            editRect = true;

            hoverTextSize = 12;
            hoverFontStyle = FontStyle.Bold;

            spritePanelSettings.ResetToDefault();
        }
    }
}
using UnityEditor;

namespace Voltrig.VoltSpriter
{
    internal class VSGUIDAsset<T> where T : UnityEngine.Object
    {
        public string defaultGUID = string.Empty;
        public string currentGUID = string.Empty;
        public string key;
        private T _asset;

        public T asset
        {
            get => _asset;
            set
            {
                if (_asset == value)
                {
                    return;
                }

                _asset = value;
                Save();
            }
        }

        public VSGUIDAsset(string key, string defaultGUID)
        {
            this.key = key;
            this.defaultGUID = defaultGUID;

            Load();
        }

        public void Save()
        {
            string path = AssetDatabase.GetAssetPath(_asset);
            if (string.IsNullOrEmpty(path))
            {
                VSConsole.LogError(this, VSErrors.WRONG_ASSET_PATH);
                currentGUID = "";
                return;
            }

            currentGUID = AssetDatabase.AssetPathToGUID(path);
            EditorPrefs.SetString(key, currentGUID);
        }

        public void Load()
        {
            currentGUID = EditorPrefs.GetString(key, defaultGUID);
            string path = AssetDatabase.GUIDToAssetPath(currentGUID);

            if (string.IsNullOrEmpty(path))
            {
                path = AssetDatabase.GUIDToAssetPath(defaultGUID);
                currentGUID = defaultGUID;
            }

            if (string.IsNullOrEmpty(path))
            {
                VSConsole.LogError(this, VSErrors.INSTALLATION_ERROR);
                VSConsole.LogError(this, $"GUIDAsset load failed :{key}");
                _asset = null;
            }
            else
            {
                _asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }
    }
}

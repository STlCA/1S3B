#if COM_UNITY_2D_SPRITE
//#define VS_DEBUG_MODE

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    /// <summary>
    /// Patches the Sprite 2D editor by adding missing features, that make VoltSpriter more enjoyable.
    /// </summary>
    [InitializeOnLoad]
    internal class VSSpriteEditorPatcher
    {
        private const string dataToPackageLibrary = "../Library/PackageCache/";
        private const string dataToLocalPackages = "../Packages/";
        private const string packageName = "com.unity.2d.sprite";

        private const string spriteEditorWindowPath = "Editor/SpriteEditor/SpriteEditorWindow.cs";

        static VSSpriteEditorPatcher()
        {
            VSSpriteEditorPatcher patcher = new VSSpriteEditorPatcher();
            VSConsole.Log(patcher, "Initialized Sprite Editor!");
            patcher.PatchSpriteEditorIfRequired();
        }

        private void PatchSpriteEditorIfRequired()
        {
            if (!PreparePackageForPatching(out DirectoryInfo packagePath))
            {
                Debug.LogError("Failed to prepare Sprite 2D Editor for patching.");
                return;
            }

            if (!ApplyPatchIfNeeded(packagePath))
            {
                Debug.LogError("Failed to patch Sprite 2D Editor.");
                return;
            }
        }

        private bool PreparePackageForPatching(out DirectoryInfo packagePath)
        {
            packagePath = null;

            try
            {
                if (!GetPackagePath(dataToLocalPackages, packageName, out packagePath))
                {
                    //Package doesn't exist in local package folder.
                    VSConsole.Log(this, $"package {packageName} in path {dataToLocalPackages} doesn't exist!");
                }
                else
                {
                    //Package exists in local package folder.
                    VSConsole.Log(this, $"package {packageName} in path {dataToLocalPackages} was found.");
                    return true;
                }

                if (!GetPackagePath(dataToPackageLibrary, packageName, out packagePath))
                {
                    //Package doesn't exist in library package folder.
                    VSConsole.LogWarning(this, $"package {packageName} in path {dataToPackageLibrary} doesn't exist! Can't patch");
                    return false;
                }
                else
                {
                    //Package exists in library package folder.
                    VSConsole.Log(this, $"package {packageName} in path {dataToPackageLibrary} was found.");
                    return true;
                }
            }
            catch (IOException e)
            {
                Debug.Log($"Failed to prepare/find the {packageName} package for patching. {e}");
            }

            return false;
        }

        /// <summary>
        /// Check for patch and applies it if needed. Returns TRUE if patch is applied. FALSE otherwise.
        /// </summary>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        private bool ApplyPatchIfNeeded(DirectoryInfo packagePath)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(packagePath.FullName, spriteEditorWindowPath));

            VSScriptPatcher patcher = new VSScriptPatcher(fileInfo);

            return patcher.CheckOrApplyPatch();
        }

        private void DebugLogFileContents(DirectoryInfo directoryPath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Path:");
            sb.AppendLine(directoryPath.FullName);

            sb.AppendLine("Files:");

            FileInfo[] files = directoryPath.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                sb.AppendLine(files[i].FullName);
            }

            sb.AppendLine("Directories:");

            DirectoryInfo[] directories = directoryPath.GetDirectories();

            for (int i = 0; i < directories.Length; i++)
            {
                sb.AppendLine(directories[i].FullName);
            }

            Debug.Log(sb.ToString());
        }

        private bool GetPackagePath(string relativePath, string packageName, out DirectoryInfo path)
        {
            DirectoryInfo absoultePath = new DirectoryInfo(Path.GetFullPath(Path.Combine(Application.dataPath, relativePath)));

            path = null;

            if (!absoultePath.Exists)
            {
                // "Directory doesnt' exist"
                return false;
            }

            //Find the package and its version.
            DirectoryInfo[] directories = absoultePath.GetDirectories();

            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].Name.StartsWith(packageName))
                {
                    path = directories[i];
                    return true;
                }
            }

            // "Failed to find the package"
            return false;
        }
    }
}
#endif
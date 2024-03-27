#if !COM_UNITY_2D_SPRITE

using UnityEditor;

namespace Voltrig.VoltSpriter
{
    internal static class VSSpriteEditorChecker
    {
        [InitializeOnLoadMethod]
        private static void PrintError()
        {
            VSConsole.LogError(typeof(VSSpriteEditorChecker), VSErrors.SPRITE_EDITOR_MISSING);
        }
    }
}

#endif

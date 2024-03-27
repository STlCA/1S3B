#if UNITY_EDITOR
using UnityEngine;

#pragma warning disable 0649

namespace Voltrig.VoltSpriter
{
    [System.Serializable]
    internal class VSSpritePanelColumn
    {
        internal VSSpritePanelColumnSettings settings;

        public readonly GUIContent gcShort;
        public readonly GUIContent gc;

        public float currentColumnWidth;
        public int id = -1;

        public VSSpritePanelColumn(string tooltip, int id, VSSpritePanelColumnSettings settings)
        {
            if (settings.useIcon)
            {
                gcShort = new GUIContent(settings.icon, tooltip);
            }
            else
            {
                gcShort = new GUIContent(settings.shortName, tooltip);
            }
            gc = new GUIContent(settings.longName, tooltip);

            this.id = id;
            this.settings = settings;
        }
    }
}
#endif

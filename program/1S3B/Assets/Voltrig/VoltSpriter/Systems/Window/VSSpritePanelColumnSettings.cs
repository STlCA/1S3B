#if UNITY_EDITOR

#pragma warning disable 0649

using UnityEngine;

namespace Voltrig.VoltSpriter
{
    [System.Serializable]
    public partial class VSSpritePanelColumnSettings
    {
        public bool isVisible = true;
        public float minColumnWidth;
        public bool isElastic = false;
        public bool useIcon = false;
        public string shortName = string.Empty;
        public string longName = string.Empty;
        public Texture icon = null;

        public VSSpritePanelColumnSettings(string shortName,
                                           string longName,
                                           float minColumnWidth,
                                           bool isElastic,
                                           bool useIcon,
                                           bool isVisible,
                                           Texture icon )
        {
            this.shortName = shortName;
            this.longName = longName;
            this.isElastic = isElastic;
            this.useIcon = useIcon;
            this.minColumnWidth = minColumnWidth;
            this.isVisible = isVisible;
            this.icon = icon;
        }
    }
}
#endif

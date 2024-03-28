#if UNITY_EDITOR

#pragma warning disable 0649

using UnityEngine;

namespace Voltrig.VoltSpriter
{
    [System.Serializable]
    public partial class VSSpritePanelPivotSettings
    {
        public Texture topLeft = null;
        public Texture topCenter = null;
        public Texture topRight = null;
        public Texture leftCenter = null;
        public Texture center = null;
        public Texture rightCenter = null;
        public Texture bottomLeft = null;
        public Texture bottomCenter = null;
        public Texture bottomRight = null;
        public Texture custom = null;
        
        internal void ResetToDefault()
        {
            VSGUIDAsset<Texture> topLeftIcon      = new VSGUIDAsset<Texture>("VS_pivotTopLeft"      , "8db1d3422a03f654d99f2cbcf1a04e6b");
            VSGUIDAsset<Texture> topCenterIcon    = new VSGUIDAsset<Texture>("VS_pivotTopCenter"    , "a2ae3b490ef3d3645a718226b94bc672");
            VSGUIDAsset<Texture> topRightIcon     = new VSGUIDAsset<Texture>("VS_pivotTopRight"     , "b3cca1912447b7840aee6fc984c272b7");
            VSGUIDAsset<Texture> leftCenterIcon   = new VSGUIDAsset<Texture>("VS_pivotleftCenter"   , "57e384e3bd630864e8db7488a2167db8");
            VSGUIDAsset<Texture> centerIcon       = new VSGUIDAsset<Texture>("VS_pivotCenter"       , "023b5bda643e87d48ae72433ac665664");
            VSGUIDAsset<Texture> rightCenterIcon  = new VSGUIDAsset<Texture>("VS_pivotRightCenter"  , "084c6055bec9f0d419399e562dadbac0");
            VSGUIDAsset<Texture> bottomLeftIcon   = new VSGUIDAsset<Texture>("VS_pivotBottomLeft"   , "e41c4bde755a1d947a740faab9131b08");
            VSGUIDAsset<Texture> bottomCenterIcon = new VSGUIDAsset<Texture>("VS_pivotBottomCenter" , "e6b9bde4a64341646a8b0f848fddce22");
            VSGUIDAsset<Texture> bottomRightIcon  = new VSGUIDAsset<Texture>("VS_pivotBottomRight"  , "90a87a3a19a790f469b119e0a430c25e");
            VSGUIDAsset<Texture> customIcon       = new VSGUIDAsset<Texture>("VS_pivotCustom"       , "df8d6cbf8e6d94e42b498566ea373a02");

            topLeft      = topLeftIcon.asset;
            topCenter    = topCenterIcon.asset;
            topRight     = topRightIcon.asset;
            leftCenter   = leftCenterIcon.asset;
            center       = centerIcon.asset;
            rightCenter  = rightCenterIcon.asset;
            bottomLeft   = bottomLeftIcon.asset;
            bottomCenter = bottomCenterIcon.asset;
            bottomRight  = bottomRightIcon.asset;
            custom       = customIcon.asset;
        }
    }
}
#endif

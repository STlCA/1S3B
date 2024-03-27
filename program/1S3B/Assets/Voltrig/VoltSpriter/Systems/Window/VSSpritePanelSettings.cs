#if UNITY_EDITOR

#pragma warning disable 0649

using UnityEngine;

namespace Voltrig.VoltSpriter
{
    [System.Serializable]
    public partial class VSSpritePanelSettings
    {
        public VSSpritePanelColumnSettings idColumn;
        public VSSpritePanelColumnSettings nameColumn;
        public VSSpritePanelColumnSettings xColumn;
        public VSSpritePanelColumnSettings yColumn;
        public VSSpritePanelColumnSettings widthColumn;
        public VSSpritePanelColumnSettings heightColumn;
        public VSSpritePanelColumnSettings borderLColumn;
        public VSSpritePanelColumnSettings borderTColumn;
        public VSSpritePanelColumnSettings borderRColumn;
        public VSSpritePanelColumnSettings borderBColumn;
        public VSSpritePanelColumnSettings pivotColumn;
        public VSSpritePanelColumnSettings pivotXColumn;
        public VSSpritePanelColumnSettings pivotYColumn;

        public VSSpritePanelPivotSettings pivotSettings;

        internal void ResetToDefault()
        {
            float XXL = 150.0f;
            //float XL = 120.0f;
            //float M = 90.0f;
            float S = 44.0f; //5 digits
            float XS = 36.0f; //4 digits
            float XXS = 28.0f; //3 digits

            VSGUIDAsset<Texture> idIcon      = new VSGUIDAsset<Texture>("VS_idIcon"     , "b1076b5f757981b4ab179f8e531fc199");
            VSGUIDAsset<Texture> nameIcon    = new VSGUIDAsset<Texture>("VS_nameIcon"   , "e22202fe22fdbb244b7bd56f2d6fa56c");
            VSGUIDAsset<Texture> xIcon       = new VSGUIDAsset<Texture>("VS_xIcon"      , "34649710421e2a84baea4afa8559076b");
            VSGUIDAsset<Texture> yIcon       = new VSGUIDAsset<Texture>("VS_yIcon"      , "891cf6fc10ecfa644a93cb1974f6663e");
            VSGUIDAsset<Texture> heightIcon  = new VSGUIDAsset<Texture>("VS_heightIcon" , "f387419fee99ae344b34bfe0699ea7a6");
            VSGUIDAsset<Texture> widthIcon   = new VSGUIDAsset<Texture>("VS_widthIcon"  , "4c8fa45efc82616439f6412878b8ae4c");
            VSGUIDAsset<Texture> borderLIcon = new VSGUIDAsset<Texture>("VS_borderLIcon", "4756345ff7d69554592b683b9bda290b");
            VSGUIDAsset<Texture> borderTIcon = new VSGUIDAsset<Texture>("VS_borderTIcon", "d2ec3179a095fb84c90c9d69bdc7200f");
            VSGUIDAsset<Texture> borderRIcon = new VSGUIDAsset<Texture>("VS_borderRIcon", "634f9908583c26941ab3e3ccb47db328");
            VSGUIDAsset<Texture> borderBIcon = new VSGUIDAsset<Texture>("VS_borderBIcon", "c32e8de7e95ccc241b6b2525146b99b0");
            VSGUIDAsset<Texture> pivotIcon   = new VSGUIDAsset<Texture>("VS_nameIcon"   , "023b5bda643e87d48ae72433ac665664");
            VSGUIDAsset<Texture> pivotXIcon  = new VSGUIDAsset<Texture>("VS_nameIcon"   , "8e913a5fa65a1564cbb406a6f2f79724");
            VSGUIDAsset<Texture> pivotYIcon  = new VSGUIDAsset<Texture>("VS_nameIcon"   , "9dcfe3f53d7e6fe4a97ac24e12b9ceac");

            idColumn = new VSSpritePanelColumnSettings      ("ID"   ,"ID"       , XS,  false, false, true, idIcon.asset);
            nameColumn = new VSSpritePanelColumnSettings    ("Name" ,"Name"     , XXL, true,  false, true, nameIcon.asset);
            xColumn = new VSSpritePanelColumnSettings       ("X"    ,"X"        , XS,  false, false, true, xIcon.asset);
            yColumn = new VSSpritePanelColumnSettings       ("Y"    ,"Y"        , XS,  false, false, true, yIcon.asset);
            widthColumn = new VSSpritePanelColumnSettings   ("Wid"  ,"Width"    , XXS, false, true,  true, widthIcon.asset);
            heightColumn = new VSSpritePanelColumnSettings  ("Hei"  ,"Height"   , XXS, false, true,  true, heightIcon.asset);
            borderLColumn = new VSSpritePanelColumnSettings ("B L"  ,"Border L" , XXS, false, true,  true, borderLIcon.asset);
            borderTColumn = new VSSpritePanelColumnSettings ("B T"  ,"Border T" , XXS, false, true,  true, borderTIcon.asset);
            borderRColumn = new VSSpritePanelColumnSettings ("B R"  ,"Border R" , XXS, false, true,  true, borderRIcon.asset);
            borderBColumn = new VSSpritePanelColumnSettings ("B B"  ,"Border B" , XXS, false, true,  true, borderBIcon.asset);
            pivotColumn = new VSSpritePanelColumnSettings   ("Pivot","Pivot"    , XS,  false, true,  true, pivotIcon.asset);
            pivotXColumn = new VSSpritePanelColumnSettings  ("P X"  ,"Pivot X"  , S,   false, true,  true, pivotXIcon.asset);
            pivotYColumn = new VSSpritePanelColumnSettings  ("P Y"  ,"Pivot Y"  , S,   false, true,  true, pivotYIcon.asset);

            pivotSettings.ResetToDefault();
        }
    }
}
#endif

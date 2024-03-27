#if UNITY_EDITOR
using UnityEngine;
using System;

namespace Voltrig.VoltSpriter
{
    [Serializable]
    public class SliceSettings : ScriptableObject
    {
        public Vector2 gridCellCount = new Vector2(1, 1);

        public Vector2 gridSpriteSize = new Vector2(64, 64);

        public Vector2 gridSpriteOffset = new Vector2(0, 0);

        public Vector2 gridSpritePadding = new Vector2(0, 0);

        public Vector2 pivot = Vector2.zero;

        public AutoSlicingMethodType autoSlicingMethod = AutoSlicingMethodType.DeleteAll;

        public SpriteAlignment spriteAlignment;

        public SlicingType slicingType;

        public int minimumSpriteSize = 4;

        public bool keepEmptyRects;
    }
}
#endif

#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;

namespace Voltrig.VoltSpriter
{
    public partial class VSData : ScriptableObject //VSSpriteRect
    {
        [System.Serializable]
        private class VSSpriteRect : IVSSpriteRect 
        {
            public SpriteRect sr;
            public int id = -1;
            public bool isSelected = false;
            public bool isHighlighted = false;
            public bool isFiltered = false;

            public bool isValid => sr != null;
            public bool duplicateName = false;

            public SpriteRect SR => sr;

            public int ID => id;

            public Rect Rect 
            {
                get => sr.rect; 
                set => sr.rect = value;
            }

            public string Name 
            { 
                get => sr.name; 
                set => sr.name = value;
            }

            public bool IsSelected => isSelected;

            public bool IsHighlighted => isHighlighted;

            public bool IsDuplicateName => duplicateName;

            public bool IsFiltered => isFiltered;

            public VSSpriteRect()
            {

            }

            public VSSpriteRect(SpriteRect sr)
            {
                this.sr = sr;
            }

            public VSSpriteRect(VSSpriteRect copy) 
            {
                sr = new SpriteRect();

                sr.name = copy.sr.name;
                sr.rect = copy.sr.rect;
                sr.border = copy.sr.border;
                sr.alignment = copy.sr.alignment;
            }

            public override string ToString()
            {
                return $"{sr.name} {id}";
            }
        }
    }
}
#endif


#if COM_UNITY_2D_SPRITE
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    public interface IVSSpriteRect 
    {
        SpriteRect SR { get; }

        int ID {get;}

        Rect Rect {get; set;}

        string Name {get; set;}

        bool IsSelected {get;}

        bool IsHighlighted {get;}

        bool IsDuplicateName{get;}

        bool IsFiltered{get;}

        string ToString();
    }
}

#endif
#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;

namespace Voltrig.VoltSpriter
{
    public partial class VSModule //SpritePivotHandler
    {
        private class SpritePivotHandler
        {
            private VSModule mod;

            private const float c_capSize = 8.0f;
            private const float c_capClickSize = 20.0f;
            private Vector2 dragStart = Vector2.zero;
            private Rect curRect;
            private Vector2 curPos = Vector2.zero;
            private Vector2 lastPivot = Vector2.zero;
            private Vector2 curPivot = Vector2.zero;
            private int draggedID = -1;

            private readonly Color handleColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            private readonly Color handleSelectedColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);

            internal bool IsDragging => draggedID >= 0;

            public SpritePivotHandler(VSModule module)
            {
                this.mod = module;
            }

            public void Reset()
            {
                draggedID = -1;
                dragStart = Vector2.zero;
            }

            public void DrawGUI()
            {
                if (mod._ev.type == EventType.MouseDown)
                {
                    if (mod._ev.button == 0)
                    {
                        draggedID = -1;
                        for (int i = 0; i < mod.data.selectAmount; i++)
                        {
                            mod.srBuf = mod.data.GetSelection(i);
                            MouseDown(mod.srBuf.SR);
                        }
                    }
                    mod._requestRepaint = true;
                }
                if (mod._ev.type == EventType.MouseUp)
                {
                    if (mod._ev.button == 0)
                    {
                        draggedID = -1;
                        mod._requestRepaint = true;
                    }
                }
                if (mod._ev.type == EventType.MouseDrag)
                {
                    if (mod._ev.button == 0)
                    {
                        MouseDrag();
                    }
                }
                if (mod._ev.type == EventType.Repaint)
                {
                    for (int i = 0; i < mod.data.selectAmount; i++)
                    {
                        mod.srBuf = mod.data.GetSelection(i);
                        Repaint(mod.srBuf.SR);
                    }
                }
            }

            private void Repaint(SpriteRect sr)
            {
                Vector2 position = VSUtils.PivotToRectPosition(sr.rect, sr.pivot);

                position = mod.VectorToHandleSpace(position);
                float halfCapSize = c_capSize * 0.5f;

                int i = 0;
                DrawCap(PivotCap(), i); i++;

                Rect PivotCap() { return new Rect(position.x - halfCapSize, position.y - halfCapSize, c_capSize, c_capSize); }
  
                void DrawCap(Rect rect, int id)
                {
                    if (id == draggedID)
                    {
                        EditorGUI.DrawRect(rect, handleSelectedColor);
                    }
                    else
                    {
                        EditorGUI.DrawRect(rect, handleColor);
                    }
                }
                //Debug.Log($"Mouse: {_mousePos} rect: {rb} scroll: {_scrollPosition} zoom: {_zoom} zero:{pos} bottom:{rectBuf}");
            }

            private void MouseDown(SpriteRect sr)
            {
                if (draggedID != -1) return; //we've already found a drag target

                Vector2 position = VSUtils.PivotToRectPosition(sr.rect, sr.pivot);

                position = mod.VectorToHandleSpace(position);

                float halfCapSize = c_capClickSize * 0.5f;

                Rect pivotRect = new Rect(position.x - halfCapSize, position.y - halfCapSize, c_capClickSize, c_capClickSize);

                if (pivotRect.Contains(mod._ev.mousePosition))
                {
                    draggedID = 0;
                    dragStart = mod._ev.mousePosition;
                    curPos = sr.rect.position;
                    curPivot = sr.pivot;
                    curRect = sr.rect;
                    lastPivot = curPivot;
                    //Debug.Log($"Cap pressed : {id} pos: {curPos} size: {curSize}");
                    mod._ev.Use();
                }

                //Debug.Log($"Mouse: {mod._ev.mousePosition} rect: {rb} scroll: {mod._scrollPosition} zoom: {mod._zoom} zero:{pos}");
            }

            private void MouseDrag()
            {
                if (draggedID == -1)
                {
                    return;
                }

                Vector2 pos = new Vector2(curPos.x +  (mod._ev.mousePosition.x - dragStart.x) / mod._zoom,
                                          (dragStart.y - mod._ev.mousePosition.y) / mod._zoom + curPos.y);

                Vector2 pivot = VSUtils.RectPositionToPivot(curRect, pos) + curPivot;
                Vector2 newPivot = pivot;
                //Make sure that the pivot is valid.

                if (lastPivot != newPivot)
                {
                    SpriteAlignment spriteAlignment = SpriteAlignment.Custom;

                    if (mod._ev.shift)
                    {
                        //TODO
                        spriteAlignment = VSUtils.PivotToAlignment(ref newPivot);
                        //Snap pivot to alignment.
                    }

                    lastPivot = newPivot;

                    for (int i = 0; i < mod.data.selectAmount; i++)
                    {
                        mod.srBuf = mod.data.GetSelection(i);
                        SpriteRect sr = mod.srBuf.SR;

                        sr.pivot = newPivot;
                        sr.alignment = spriteAlignment;
                    }

                    mod.SetDataModified();
                    mod._requestWindowRepaint = true;
                }

                //Debug.Log($"Pivot dragStart: {dragStart} mousePos: {pos} curPivot: {curPivot} pivot: {pivot} newPivot: {newPivot} zoom: {mod._zoom}");
            }
        }
    }
}
#endif

#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;

namespace Voltrig.VoltSpriter
{
    public partial class VSModule //SpriteRectHandler
    {
        private class SpriteRectHandler
        {
            private VSModule mod;

            private const int c_NWID = 0;
            private const int c_NEID = 1;
            private const int c_SWID = 2;
            private const int c_SEID = 3;

            private const float c_capSize = 8.0f;
            private const float c_capClickSize = 20.0f;
            private Vector2 dragStart = Vector2.zero;
            private Vector2 lastSize = Vector2.zero;
            private Vector2 curSize = Vector2.zero;
            private Vector2 lastPos = Vector2.zero;
            private Vector2 curPos = Vector2.zero;
            private Rect[] snappedRects = new Rect[0];
            private int draggedID = -1;

            private readonly Color handleColor = new Color(0.3f, 0.3f, 1.0f, 1.0f);
            private readonly Color handleSelectedColor = new Color(0.3f, 1.0f, 0.6f, 1.0f);

            private Vector2 v2Buf;

            internal bool IsDragging => draggedID >= 0;

            public SpriteRectHandler(VSModule module)
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
                Rect rb = mod.RectToHandleSpace(sr.rect);
                float halfCapSize = c_capSize * 0.5f;

                int i = 0;
                DrawCap(NWCap(), i); i++;
                DrawCap(NECap(), i); i++;
                DrawCap(SWCap(), i); i++;
                DrawCap(SECap(), i); i++;

                Rect NWCap() { return new Rect(rb.xMin - halfCapSize, rb.yMin - halfCapSize, c_capSize, c_capSize); }
                Rect NECap() { return new Rect(rb.xMax - halfCapSize, rb.yMin - halfCapSize, c_capSize, c_capSize); }
                Rect SWCap() { return new Rect(rb.xMin - halfCapSize, rb.yMax - halfCapSize, c_capSize, c_capSize); }
                Rect SECap() { return new Rect(rb.xMax - halfCapSize, rb.yMax - halfCapSize, c_capSize, c_capSize); }

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

                Vector3 pos = Handles.inverseMatrix.MultiplyPoint(Vector3.zero);
                Rect rb = sr.rect;
                float halfCapSize = c_capClickSize * 0.5f;

                rb.x = Mathf.Round((rb.x - pos.x) * mod._zoom);
                rb.y = Mathf.Round((pos.y - rb.y - rb.height) * mod._zoom);
                rb.width = rb.width * mod._zoom;
                rb.height = rb.height * mod._zoom;

                int i = 0;
                CheckCap(NWCap(), i); i++;
                CheckCap(NECap(), i); i++;
                CheckCap(SWCap(), i); i++;
                CheckCap(SECap(), i); i++;

                Rect NWCap() { return new Rect(rb.xMin - halfCapSize, rb.yMin - halfCapSize, c_capClickSize, c_capClickSize); }
                Rect NECap() { return new Rect(rb.xMax - halfCapSize, rb.yMin - halfCapSize, c_capClickSize, c_capClickSize); }
                Rect SWCap() { return new Rect(rb.xMin - halfCapSize, rb.yMax - halfCapSize, c_capClickSize, c_capClickSize); }
                Rect SECap() { return new Rect(rb.xMax - halfCapSize, rb.yMax - halfCapSize, c_capClickSize, c_capClickSize); }

                void CheckCap(Rect rect, int id)
                {
                    if (rect.Contains(mod._ev.mousePosition))
                    {
                        draggedID = id;
                        dragStart = mod._ev.mousePosition;
                        SnapSelectionRects();
                        curPos = sr.rect.position;
                        curSize = sr.rect.size;
                        lastPos = curPos;
                        lastSize = curSize;
                        //Debug.Log($"Cap pressed : {id} pos: {curPos} size: {curSize}");
                        mod._ev.Use();
                    }
                }

                //Debug.Log($"Mouse: {mod._ev.mousePosition} rect: {rb} scroll: {mod._scrollPosition} zoom: {mod._zoom} zero:{pos}");
            }

            private void MouseDrag()
            {
                if (draggedID == -1)
                {
                    return;
                }

                Vector2 size = new Vector2((int)Mathf.Round(curSize.x), (int)Mathf.Round(curSize.y));
                Vector2 newSize = size;

                Vector2 pos = new Vector2((int)Mathf.Round(curPos.x), (int)Mathf.Round(curPos.y));
                Vector2 newPos = pos;

                if (draggedID == c_NWID)
                {
                    newPos.x = (int)Mathf.Round(pos.x + (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newSize.x = (int)Mathf.Round(size.x - (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newSize.y = (int)Mathf.Round(size.y - (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                }
                else if (draggedID == c_SWID)
                {
                    newPos.x = (int)Mathf.Round(pos.x + (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newPos.y = (int)Mathf.Round(pos.y - (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                    newSize.x = (int)Mathf.Round(size.x - (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newSize.y = (int)Mathf.Round(size.y + (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                }
                else if (draggedID == c_NEID)
                {
                    newSize.x = (int)Mathf.Round(size.x + (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newSize.y = (int)Mathf.Round(size.y - (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                }
                else
                {
                    newPos.y = (int)Mathf.Round(pos.y - (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                    newSize.x = (int)Mathf.Round(size.x + (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                    newSize.y = (int)Mathf.Round(size.y + (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                }

                //Make sure that the size is valid.
                if (newSize.x < 1)
                {
                    newSize.x = 1;
                }

                if (newSize.y < 1)
                {
                    newSize.y = 1;
                }

                if (lastSize != newSize || lastPos != newPos)
                {
                    Vector2 posOffset = newPos - curPos;
                    Vector2 posBuf;
                    lastSize = newSize;
                    lastPos = newPos;

                    for (int i = 0; i < mod.data.selectAmount; i++)
                    {
                        mod.srBuf = mod.data.GetSelection(i);
                        SpriteRect sr = mod.srBuf.SR;
                        Rect rect = snappedRects[i];

                        posBuf = rect.position + posOffset;
                        if (posBuf.x >= rect.xMax)
                        {
                            posBuf.x = rect.xMax - 1.0f;
                        }
                        if (posBuf.y >= rect.yMax)
                        {
                            posBuf.y = rect.yMax - 1.0f;
                        }

                        rect.position = posBuf;
                        rect.width = newSize.x;
                        rect.height = newSize.y;
                        sr.rect = rect;
                    }

                    mod.SetDataModified();
                    mod._requestWindowRepaint = true;
                }

                //Debug.Log($"Border drag: {borderDragStart} mousePos: {_ev.mousePosition} curSize: {curSize} newSize: {newSize}");
            }

            private void SnapSelectionRects()
            {
                if (snappedRects.Length != mod.data.selectAmount)
                {
                    snappedRects = new Rect[mod.data.selectAmount];
                }

                for (int i = 0; i < mod.data.selectAmount; i++)
                {
                    snappedRects[i] = mod.data.GetSelection(i).Rect;
                }
            }
        }
    }
}
#endif

#if COM_UNITY_2D_SPRITE
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    public partial class VSModule //BordersHandler
    {
        private class SpriteBordersHandler
        {
            private VSModule mod;

            private const int c_leftID = 0;
            private const int c_topID = 1;
            private const int c_rightID = 2;
            private const int c_bottomID = 3;

            private const float c_capSize = 8.0f;
            private const float c_capClickSize = 20.0f;
            private Vector2 dragStart = Vector2.zero;
            private float lastSize = 0.0f;
            private float curSize = 0;
            private int draggedID = -1;

            private readonly Color handleColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            private readonly Color handleSelectedColor = new Color(0.6f, 1.0f, 0.3f, 1.0f);

            private Rect rectBuf;
            private Vector2 v2Buf;

            internal bool IsDragging => draggedID >= 0;

            public SpriteBordersHandler(VSModule module)
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
                Vector4 vec4Buf = sr.border;
                Rect rb = mod.RectToHandleSpace(sr.rect);
                float size;
                float halfCapSize = c_capSize * 0.5f;

                int i = 0;
                size = GetSize(i); rectBuf = LeftRect(); DrawRect(rectBuf); rectBuf.xMin = rectBuf.xMax; DrawCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = TopRect(); DrawRect(rectBuf); rectBuf.yMin = rectBuf.yMax; DrawCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = RightRect(); DrawRect(rectBuf); rectBuf.xMax = rectBuf.xMin; DrawCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = BottomRect(); DrawRect(rectBuf); rectBuf.yMax = rectBuf.yMin; DrawCap(rectBuf, i); i++;

                Rect LeftRect() { return new Rect(rb.xMin, rb.yMin, size, rb.height); }
                Rect TopRect() { return new Rect(rb.xMin, rb.yMin, rb.width, size); }
                Rect RightRect() { return new Rect(rb.xMax - size, rb.yMin, size, rb.height); }
                Rect BottomRect() { return new Rect(rb.xMin, rb.yMax - size, rb.width, size); }

                float GetSize(int id)
                {
                    return vec4Buf[id] * mod._zoom;
                }

                void DrawRect(Rect rect)
                {
                    EditorGUI.DrawRect(rect, mod.spriteBorderColor);
                }

                void DrawCap(Rect rect, int id)
                {
                    v2Buf = rectBuf.center;
                    rect.xMin = v2Buf.x - halfCapSize;
                    rect.xMax = v2Buf.x + halfCapSize;
                    rect.yMin = v2Buf.y - halfCapSize;
                    rect.yMax = v2Buf.y + halfCapSize;
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
                Vector4 vec4Buf = sr.border;
                Rect rb = sr.rect;
                float size;
                float halfCapSize = c_capClickSize * 0.5f;

                rb.x = Mathf.Round((rb.x - pos.x) * mod._zoom);
                rb.y = Mathf.Round((pos.y - rb.y - rb.height) * mod._zoom);
                rb.width = rb.width * mod._zoom;
                rb.height = rb.height * mod._zoom;

                int i = 0;
                size = GetSize(i); rectBuf = LeftRect(); CheckCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = TopRect(); CheckCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = RightRect(); CheckCap(rectBuf, i); i++;
                size = GetSize(i); rectBuf = BottomRect(); CheckCap(rectBuf, i); i++;

                Rect LeftRect() { return CreateRect(new Vector2(rb.xMin + size, rb.yMin + rb.height * 0.5f)); }
                Rect TopRect() { return CreateRect(new Vector2(rb.xMin + rb.width * 0.5f, rb.yMin + size)); }
                Rect RightRect() { return CreateRect(new Vector2(rb.xMax - size, rb.yMin + rb.height * 0.5f)); }
                Rect BottomRect() { return CreateRect(new Vector2(rb.xMin + rb.width * 0.5f, rb.yMax - size)); }

                float GetSize(int id)
                {
                    return vec4Buf[id] * mod._zoom;
                }

                float GetSizeWithoutZoom(int id)
                {
                    return vec4Buf[id];
                }

                Rect CreateRect(Vector2 value)
                {
                    return new Rect(value.x - halfCapSize, value.y - halfCapSize, c_capClickSize, c_capClickSize);
                }

                void CheckCap(Rect rect, int id)
                {
                    if (rect.Contains(mod._ev.mousePosition))
                    {
                        draggedID = id;
                        dragStart = mod._ev.mousePosition;
                        curSize = GetSizeWithoutZoom(id);
                        lastSize = curSize;
                        //Debug.Log($"Cap pressed : {id}");
                        mod._ev.Use();
                    }
                }

                //Debug.Log($"Mouse: {_ev.mousePosition} rect: {rb} scroll: {_scrollPosition} zoom: {_zoom} zero:{pos} bottom:{rectBuf}");
            }

            private void MouseDrag()
            {
                if (draggedID == -1)
                {
                    return;
                }

                int size = (int)Mathf.Round(curSize);
                int newSize;

                if (draggedID == c_leftID)
                {
                    newSize = (int)Mathf.Round(size + (mod._ev.mousePosition.x - dragStart.x) / mod._zoom);
                }
                else if (draggedID == c_rightID)
                {
                    newSize = (int)Mathf.Round(size + (dragStart.x - mod._ev.mousePosition.x) / mod._zoom);
                }
                else if (draggedID == c_topID)
                {
                    newSize = (int)Mathf.Round(size + (mod._ev.mousePosition.y - dragStart.y) / mod._zoom);
                }
                else
                {
                    newSize = (int)Mathf.Round(size + (dragStart.y - mod._ev.mousePosition.y) / mod._zoom);
                }

                if (newSize < 0)
                {
                    newSize = 0;
                }

                if (lastSize != newSize)
                {
                    System.Func<Vector4, Rect, Vector4> toNewBorderA;

                    switch (draggedID)
                    {
                        case c_leftID:
                            toNewBorderA = BorderLeft;
                            break;
                        case c_topID:
                            toNewBorderA = BorderTop;
                            break;
                        case c_rightID:
                            toNewBorderA = BorderRight;
                            break;
                        case c_bottomID:
                            toNewBorderA = BorderBottom;
                            break;
                        default:
                            toNewBorderA = delegate { return Vector4.zero; };
                            Debug.LogError("[VoltSpriter] Border switch Error");
                            break;
                    }

                    lastSize = newSize;
                    Rect rect;
                    Vector4 border;
                    for (int i = 0; i < mod.data.selectAmount; i++)
                    {
                        mod.srBuf = mod.data.GetSelection(i);
                        SpriteRect sr = mod.srBuf.SR;
                        rect = sr.rect;
                        border = toNewBorderA(sr.border, rect);
                        sr.border = border;
                    }

                    mod.SetDataModified();
                    mod._requestWindowRepaint = true;
                }

                Vector4 BorderLeft(Vector4 vec, Rect r)
                {
                    vec.x = Mathf.Clamp(newSize, 0, (int)r.width);
                    return vec;
                }

                Vector4 BorderTop(Vector4 vec, Rect r)
                {
                    vec.y = Mathf.Clamp(newSize, 0, (int)r.height);
                    return vec;
                }

                Vector4 BorderRight(Vector4 vec, Rect r)
                {
                    vec.z = Mathf.Clamp(newSize, 0, (int)r.width);
                    return vec;
                }

                Vector4 BorderBottom(Vector4 vec, Rect r)
                {
                    vec.w = Mathf.Clamp(newSize, 0, (int)r.height);
                    return vec;
                }

                //Debug.Log($"Border drag: {borderDragStart} mousePos: {_ev.mousePosition} curSize: {curSize} newSize: {newSize}");
            }
        }
    }
}
#endif

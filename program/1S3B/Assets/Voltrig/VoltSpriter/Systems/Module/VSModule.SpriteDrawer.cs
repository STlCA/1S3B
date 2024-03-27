#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Voltrig.VoltSpriter
{
    public partial class VSModule //SpriteDrawer
    {
        private class SpriteDrawer
        {
            private Color lineTransparency = new Color(1, 1, 1, 0.75f);
            private Color color = Color.white;
            private readonly Color spriteColor = new Color(0.9f, 0.9f, 0.9f);
            private readonly Color spriteShadeColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            private IVSSpriteRect srBuf;

            private Vector3[] vecBuf4 = new Vector3[4];
            private readonly int[] rectIndicesBuf4 = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };

            //Reflection
            private ApplyMaterialD ApplyWireMaterialF;
            private ApplyMaterialD applyDottedWireMaterialF;
            private delegate void ApplyMaterialD(UnityEngine.Rendering.CompareFunction zTest);

            public SpriteDrawer()
            {
                //Initialize reflection
                const string m1Name = "ApplyWireMaterial";
                const string m2Name = "ApplyDottedWireMaterial";

                MethodInfo applyWireMaterialM = typeof(HandleUtility).GetMethod(m1Name, BindingFlags.Static | BindingFlags.NonPublic, null, new System.Type[] {typeof(UnityEngine.Rendering.CompareFunction)}, null);
                MethodInfo applyDottedWireMaterialM = typeof(HandleUtility).GetMethod(m2Name, BindingFlags.Static | BindingFlags.NonPublic, null, new System.Type[] { typeof(UnityEngine.Rendering.CompareFunction)}, null);

                ApplyWireMaterialF = (ApplyMaterialD)Delegate.CreateDelegate(typeof(ApplyMaterialD), applyWireMaterialM);
                applyDottedWireMaterialF = (ApplyMaterialD)Delegate.CreateDelegate(typeof(ApplyMaterialD), applyDottedWireMaterialM);
            }

            public void DrawSprites(VSData data, float zoom)
            {
                if (data.isFiltered)
                {
                    DrawFilteredSprites(data, zoom);
                }
                else
                {
                    DrawNormalSprites(data, zoom);
                }
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawFilteredSprites(VSData data, float zoom)
            {
                if (!BeginLineDrawing(Handles.matrix, false, GL.LINES))
                {
                    return;
                }

                color = spriteShadeColor;
                for (int i = 0; i < data.filteredAmount; i++)
                {
                    srBuf = data.GetFiltered(i);
                    DrawRoundedRectHandle(srBuf.Rect, 1.0f / zoom);
                }

                EndLineDrawing();

                if (!BeginLineDrawing(Handles.matrix, false, GL.LINES))
                {
                    return;
                }

                color = spriteColor;
                for (int i = 0; i < data.filteredAmount; i++)
                {
                    srBuf = data.GetFiltered(i);
                    DrawRoundedRectHandle(srBuf.Rect);
                }

                EndLineDrawing();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawNormalSprites(VSData data, float zoom)
            {
                if (!BeginLineDrawing(Handles.matrix, false, GL.LINES))
                {
                    return;
                }

                color = spriteShadeColor;
                for (int i = 0; i < data.spriteAmount; i++)
                {
                    srBuf = data.GetSprite(i);
                    DrawRoundedRectHandle(srBuf.Rect, 1.0f / zoom);
                }

                EndLineDrawing();

                if (!BeginLineDrawing(Handles.matrix, false, GL.LINES))
                {
                    return;
                }

                color = spriteColor;
                for (int i = 0; i < data.spriteAmount; i++)
                {
                    srBuf = data.GetSprite(i);
                    DrawRoundedRectHandle(srBuf.Rect);
                }

                EndLineDrawing();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawRoundedRectHandle(Rect rect)
            {
                rect.x = Mathf.Round(rect.x);
                rect.y = Mathf.Round(rect.y);

                vecBuf4[0].Set(rect.xMin, rect.yMin, 0.0f);
                vecBuf4[1].Set(rect.xMax, rect.yMin, 0.0f);
                vecBuf4[2].Set(rect.xMax, rect.yMax, 0.0f);
                vecBuf4[3].Set(rect.xMin, rect.yMax, 0.0f);

                DrawLines(vecBuf4, rectIndicesBuf4);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawRoundedRectHandle(Rect rect, float offset)
            {
                rect.x = Mathf.Round(rect.x) + offset;
                rect.y = Mathf.Round(rect.y) + offset;

                vecBuf4[0].Set(rect.xMin, rect.yMin, 0.0f);
                vecBuf4[1].Set(rect.xMax, rect.yMin, 0.0f);
                vecBuf4[2].Set(rect.xMax, rect.yMax, 0.0f);
                vecBuf4[3].Set(rect.xMin, rect.yMax, 0.0f);

                DrawLines(vecBuf4, rectIndicesBuf4);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawLines(Vector3[] lineSegments)
            {
                for (int i = 0; i < lineSegments.Length; i += 2)
                {
                    var p1 = lineSegments[i + 0];
                    var p2 = lineSegments[i + 1];
                    GL.Vertex(p1);
                    GL.Vertex(p2);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void DrawLines(Vector3[] points, int[] segmentIndices)
            {
                for (int i = 0; i < segmentIndices.Length; i += 2)
                {
                    var p1 = points[segmentIndices[i + 0]];
                    var p2 = points[segmentIndices[i + 1]];
                    GL.Vertex(p1);
                    GL.Vertex(p2);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EndLineDrawing()
            {
                GL.End();
                GL.PopMatrix();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool BeginLineDrawing(Matrix4x4 matrix, bool dottedLines, int mode)
            {
                Color col = color * lineTransparency;
                if (dottedLines)
                {
                    applyDottedWireMaterialF(Handles.zTest);
                }
                else
                {
                    ApplyWireMaterialF(Handles.zTest);
                }
                GL.PushMatrix();
                GL.MultMatrix(matrix);
                GL.Begin(mode);
                GL.Color(col);
                return true;
            }
        }
    }
}
#endif

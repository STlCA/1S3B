#if COM_UNITY_2D_SPRITE
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

namespace Voltrig.VoltSpriter
{
    internal partial class VSSpritePanel //DataClickInfo
    {
        private class DataClickInfo
        {
            private readonly float [] xOffsets;
            private readonly float yOffset;

            private Rect renderRect;

            private readonly VSSpritePanel spritePanel;
            public Vector2 pos;
            public bool isValid = false;
            private int xID = 0;
            private int yID = 0;
            private const int minX = 1;
            private int elementIndex = -1;
            private int elementAmount = -1;

            //TEMP
            internal int ElementIndex => elementIndex;
                
            public DataClickInfo(VSSpritePanel spritePanel) 
            {
                this.spritePanel = spritePanel;
                List<VSSpritePanelColumn> configs = spritePanel.columns;
                xOffsets = new float[configs.Count];
                yOffset = spritePanel.yPadding + spritePanel.rowBaseHeight;
                float sum = 0.0f;

                for(int i = 0; i < configs.Count; i++) 
                {
                    xOffsets[i] = sum + configs[i].settings.minColumnWidth * 0.5f;
                    sum += configs[i].settings.minColumnWidth;
                }
            }

            public void UpdateRenderRect(Rect rect) 
            {
                renderRect = rect;
            }

            public void Select(int x, int y) 
            {
                xID = Mathf.Clamp(x, minX, spritePanel.columns.Count-1);
                yID = Mathf.Clamp(y, 0, spritePanel.data.spriteAmount-1);
                pos.x = renderRect.x + xOffsets[xID];
                pos.y = renderRect.y + yOffset * 0.5f + yID * yOffset;

                elementIndex = x + y * elementAmount;

                isValid = true;
            }

            public void MoveUp() 
            {
                if (yID <= 0)
                {
                    return;
                }
                yID--;
                pos.y = renderRect.y + yOffset * 0.5f + yID * yOffset;

                elementIndex -= elementAmount;

                VSConsole.Log(this, "Move clicked!");
            }

            public void MoveDown() 
            {
                if (yID >= spritePanel.data.spriteAmount - 1)
                {
                    return;
                }
                yID++;
                pos.y = renderRect.y + yOffset * 0.5f + yID * yOffset;

                elementIndex += elementAmount;
                
                VSConsole.Log(this, "Move clicked!");
            }

            public void MoveLeft() 
            {
                if (xID <= minX)
                {
                    return;
                }

                xID--;
                pos.x = renderRect.x + xOffsets[xID];

                elementIndex--;

                VSConsole.Log(this, "Move clicked!");
            }

            public void MoveRight() 
            {
                if (xID >= spritePanel.columns.Count - 1)
                {
                    return;
                }

                xID++;
                pos.x = renderRect.x + xOffsets[xID];

                elementIndex++;

                VSConsole.Log(this, "Move clicked!");
            }

            public void Update(VSEvent ev, int xID, int elementIndex, int elementAmount) 
            {
                pos = ev.mousePosition;
                this.xID = xID;
                this.yID = (int)((pos.y - renderRect.y) / yOffset);
                this.elementIndex = elementIndex;
                this.elementAmount = elementAmount;
                isValid = true;
            }

            public void FocusOnCurrent()
            {
                GUI.FocusControl($"{dataGUIControlIDpreffix}{elementIndex}");

                VSConsole.Log(this, $"Focused on {dataGUIControlIDpreffix}{elementIndex}");
            }

            public void Unfocus()
            {
                GUIUtility.keyboardControl = 0;
            }
        }
    }
}
#endif

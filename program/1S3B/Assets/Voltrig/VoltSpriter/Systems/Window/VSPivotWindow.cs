#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    internal class VSPivotWindow : EditorWindow
    {
        private GUIStyle backgroundStyle = "grey_border";

        private static long lastClosedTime;

        private const int windowHeight = buttonSize * 4;
        private const int windowWidth = buttonSize * 3;

        private UnityEngine.Event _ev;

        private GUIContent[] pivotGCs;
        private SpriteAlignment selectedAlignment;

        private System.Action<SpriteAlignment> onSelectA;
        private const int buttonSize = 28;

        private void Init(Rect buttonRect, GUIContent[] pivotGCs, SpriteAlignment selectedAlignment, System.Action<SpriteAlignment> onSelectA)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            this.pivotGCs = pivotGCs;
            this.selectedAlignment = selectedAlignment;
            this.onSelectA = onSelectA;

            var windowSize = new Vector2(windowWidth, windowHeight);
            ShowAsDropDown(buttonRect, windowSize);

            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            Repaint();
        }

        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += Close;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= Close;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            lastClosedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        internal static bool ShowAtPosition(Rect buttonRect, GUIContent[] pivotGCs, SpriteAlignment selectedAlignment, System.Action<SpriteAlignment> onSelectA)
        {
            // We could not use realtimeSinceStartUp since it is set to 0 when entering/exitting playmode, we assume an increasing time when comparing time.
            long nowMilliSeconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            bool justClosed = nowMilliSeconds < lastClosedTime + 50;
            if (!justClosed)
            {
                if (Event.current != null)
                {
                    Event.current.Use();
                }

                VSPivotWindow spriteEditorMenu = CreateInstance<VSPivotWindow>();
                spriteEditorMenu.Init(buttonRect, pivotGCs, selectedAlignment, onSelectA);
                return true;
            }
            return false;
        }

        private void OnGUI()
        {
            _ev = UnityEngine.Event.current;

            Rect drawRect = new Rect(0, 0, buttonSize, buttonSize);

            drawRect = PivotButton(drawRect, SpriteAlignment.TopLeft);
            drawRect = PivotButton(drawRect, SpriteAlignment.TopCenter);
            drawRect = PivotButton(drawRect, SpriteAlignment.TopRight);
            drawRect.x = 0;
            drawRect.y += buttonSize;

            drawRect = PivotButton(drawRect, SpriteAlignment.LeftCenter);
            drawRect = PivotButton(drawRect, SpriteAlignment.Center);
            drawRect = PivotButton(drawRect, SpriteAlignment.RightCenter);
            drawRect.x = 0;
            drawRect.y += buttonSize;

            drawRect = PivotButton(drawRect, SpriteAlignment.BottomLeft);
            drawRect = PivotButton(drawRect, SpriteAlignment.BottomCenter);
            drawRect = PivotButton(drawRect, SpriteAlignment.BottomRight);
            drawRect.x = 0;
            drawRect.y += buttonSize;
            drawRect.width = buttonSize * 3;

            drawRect = PivotButton(drawRect, SpriteAlignment.Custom);
        }

        private Rect PivotButton(Rect drawRect, SpriteAlignment alignment)
        {
            if(alignment == selectedAlignment)
            {
                GUI.color = Color.yellow;
            }

            if (GUI.Button(drawRect, pivotGCs[(int)alignment]))
            {
                onSelectA?.Invoke(alignment);
                Close();
            }

            GUI.color = Color.white;

            drawRect.x += buttonSize;
            return drawRect;
        }
    }
}
#endif

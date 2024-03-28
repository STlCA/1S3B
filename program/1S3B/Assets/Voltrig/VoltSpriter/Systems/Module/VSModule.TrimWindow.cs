#if COM_UNITY_2D_SPRITE
using System;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    internal class TrimWindow : EditorWindow
    {
        private GUIContent trimButtonGC = new GUIContent("Trim", "Trims the selection.");
        private GUIContent trimAlphaValueGC = new GUIContent("Alpha", "All alpha values lower or equal to this will be trimmed. 0 is full transparency and 255 is full opacity. \nSet this value to 254 to trim all alpha.");
        private GUIStyle backgroundStyle = "grey_border";

        private static long lastClosedTime;
        private static TrimSettings settings;
        private VSModule owner;

        private const int labelWidth = 80;
        private const int windowHeight = 48;
        private const int windowWidth = 196;

        private UnityEngine.Event _ev;

        private void Init(Rect buttonRect, VSModule owner, ISpriteEditorDataProvider dataProvider)
        {
            if (settings == null)
            {
                settings = CreateInstance<TrimSettings>();
            }

            this.owner = owner;

            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
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

        internal static bool ShowAtPosition(Rect buttonRect, VSModule owner, ISpriteEditorDataProvider dataProvider)
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

                TrimWindow spriteEditorMenu = CreateInstance<TrimWindow>();
                spriteEditorMenu.Init(buttonRect, owner, dataProvider);
                return true;
            }
            return false;
        }

        private void OnGUI()
        {
            _ev = UnityEngine.Event.current;

            GUILayout.Space(4);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.wideMode = true;

            GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, backgroundStyle);

            GUILayout.Space(2f);

            settings.trimAlpha = (byte)Mathf.Clamp(EditorGUILayout.IntField(trimAlphaValueGC, settings.trimAlpha), 0, byte.MaxValue - 1);

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(owner.data.selectAmount == 0);

            if (GUILayout.Button(trimButtonGC))
            {
                owner.TrimSelection(settings.trimAlpha);
                owner.RepaintIfNeccessary();
            }

            EditorGUI.EndDisabledGroup();

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.control
                    && _ev.keyCode == KeyCode.Z)
                {
                    owner.Revert();
                    _ev.Use();
                    owner.Repaint();
                }
            }
        }

        [Serializable]
        private class TrimSettings : ScriptableObject
        {
            public byte trimAlpha = 0;
        }
    }
}
#endif

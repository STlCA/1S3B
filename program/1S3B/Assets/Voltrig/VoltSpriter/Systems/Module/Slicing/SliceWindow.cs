#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor.U2D.Sprites;
using UnityEditor;
using System;

namespace Voltrig.VoltSpriter
{
    internal class SliceWindow : EditorWindow
    {
        private GUIContent trimButtonGC = new GUIContent("Slice", "Trims the selection.");
        private GUIContent trimAlphaValueGC = new GUIContent("Alpha", "All alpha values lower or equal to this will be trimmed. 0 is full transparency and 255 is full opacity. \nSet this value to 254 to trim all alpha.");
        private GUIStyle backgroundStyle = "grey_border";

        private static long lastClosedTime;
        private static SliceSettings settings;
        private VSModule owner;

        private const int labelWidth = 80;
        private const int windowHeight = 215;
        private const int windowWidth = 300;

        private UnityEngine.Event _ev;

        private static Styles styles;
        private ITextureDataProvider textureDataProvider;

        private void Init(Rect buttonRect, VSModule owner, ISpriteEditorDataProvider dataProvider)
        {
            if (settings == null)
            {
                settings = CreateInstance<SliceSettings>();
            }

            this.owner = owner;
            textureDataProvider = dataProvider.GetDataProvider<ITextureDataProvider>();

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
            long nowMilliSeconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            bool justClosed = nowMilliSeconds < lastClosedTime + 50;
            if (!justClosed)
            {
                if (Event.current != null)
                {
                    Event.current.Use();
                }

                SliceWindow spriteEditorMenu = CreateInstance<SliceWindow>();
                spriteEditorMenu.Init(buttonRect, owner, dataProvider);
                return true;
            }
            return false;
        }

        private void OnGUI()
        {
            if (styles == null)
            {
                styles = new Styles();
            }

            // Leave some space above the elements
            GUILayout.Space(4);

            EditorGUIUtility.labelWidth = 124f;
            EditorGUIUtility.wideMode = true;

            GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, styles.background);

            EditorGUI.BeginChangeCheck();
            SlicingType slicingType = settings.slicingType;
            slicingType = (SlicingType)EditorGUILayout.EnumPopup(styles.typeGC, slicingType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(settings, "Change slicing type");
                settings.slicingType = slicingType;
            }
            switch (slicingType)
            {
                case SlicingType.GridByCellSize:
                case SlicingType.GridByCellCount:
                    OnGridGUI();
                    break;
                default:
                    break;
            }
            OnPivotGUI();

            GUILayout.Space(2f);
            EditorGUI.BeginChangeCheck();
            int slicingMethod = (int)settings.autoSlicingMethod;
            slicingMethod = EditorGUILayout.Popup(styles.methodGC, slicingMethod, styles.slicingMethodOptions);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(settings, "Change Slicing Method");
                settings.autoSlicingMethod = (AutoSlicingMethodType)slicingMethod;
            }

            if (owner.data.selectAmount > 0)
            {
                EditorGUILayout.HelpBox(styles.slicingSelectionGC);
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);

            if (GUILayout.Button(styles.sliceButtonGC))
            {
                DoSlicing();
            }

            GUILayout.EndHorizontal();
        }

        private void DoSlicing()
        {
            switch (settings.slicingType)
            {
                case SlicingType.GridByCellCount:
                case SlicingType.GridByCellSize:
                {
                    DoGridSlicing();
                    break;
                }
                case SlicingType.Automatic:
                {
                    DoAutomaticSlicing();
                    break;
                }
            }
        }

        private void TwoIntFields(GUIContent label, GUIContent labelX, GUIContent labelY, ref int x, ref int y)
        {
            float height = EditorGUIUtility.singleLineHeight;
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, EditorGUIUtility.labelWidth, height, height, EditorStyles.numberField);

            Rect labelRect = rect;
            labelRect.width = EditorGUIUtility.labelWidth;
            labelRect.height = EditorGUIUtility.singleLineHeight;

            GUI.Label(labelRect, label);

            Rect fieldRect = rect;
            fieldRect.width -= EditorGUIUtility.labelWidth;
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.x += EditorGUIUtility.labelWidth;
            fieldRect.width /= 2;
            fieldRect.width -= 2;

            EditorGUIUtility.labelWidth = 12;

            x = EditorGUI.IntField(fieldRect, labelX, x);
            fieldRect.x += fieldRect.width + 3;
            y = EditorGUI.IntField(fieldRect, labelY, y);

            EditorGUIUtility.labelWidth = labelRect.width;
        }

        private void OnGridGUI()
        {
            int width, height;
            textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
            var texture = textureDataProvider.GetReadableTexture2D();
            int maxWidth = texture != null ? width : 4096;
            int maxHeight = texture != null ? height : 4096;

            if (settings.slicingType == SlicingType.GridByCellCount)
            {
                int x = (int)settings.gridCellCount.x;
                int y = (int)settings.gridCellCount.y;

                EditorGUI.BeginChangeCheck();
                TwoIntFields(styles.columnAndRowGC, styles.columnGC, styles.rowGC, ref x, ref y);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(settings, "Change column & row");

                    settings.gridCellCount.x = Mathf.Clamp(x, 1, maxWidth);
                    settings.gridCellCount.y = Mathf.Clamp(y, 1, maxHeight);
                }
            }
            else
            {
                int x = (int)settings.gridSpriteSize.x;
                int y = (int)settings.gridSpriteSize.y;

                EditorGUI.BeginChangeCheck();
                TwoIntFields(styles.pixelSizeGC, styles.xGC, styles.yGC, ref x, ref y);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(settings, "Change grid size");

                    settings.gridSpriteSize.x = Mathf.Clamp(x, 1, maxWidth);
                    settings.gridSpriteSize.y = Mathf.Clamp(y, 1, maxHeight);
                }
            }

            {
                int x = (int)settings.gridSpriteOffset.x;
                int y = (int)settings.gridSpriteOffset.y;

                EditorGUI.BeginChangeCheck();
                TwoIntFields(styles.offsetGC, styles.xGC, styles.yGC, ref x, ref y);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(settings, "Change grid offset");

                    settings.gridSpriteOffset.x = Mathf.Clamp(x, 0, maxWidth - settings.gridSpriteSize.x);
                    settings.gridSpriteOffset.y = Mathf.Clamp(y, 0, maxHeight - settings.gridSpriteSize.y);
                }
            }

            {
                int x = (int)settings.gridSpritePadding.x;
                int y = (int)settings.gridSpritePadding.y;

                EditorGUI.BeginChangeCheck();
                TwoIntFields(styles.paddingGC, styles.xGC, styles.yGC, ref x, ref y);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(settings, "Change grid padding");

                    settings.gridSpritePadding.x = Mathf.Clamp(x, 0, maxWidth);
                    settings.gridSpritePadding.y = Mathf.Clamp(y, 0, maxHeight);
                }
            }

            EditorGUI.BeginChangeCheck();
            bool keepEmptyRects = EditorGUILayout.Toggle(styles.keepEmptyRectsGC, settings.keepEmptyRects);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(settings, "Keep Empty Rects");
                settings.keepEmptyRects = keepEmptyRects;
            }
        }

        private void OnPivotGUI()
        {
            EditorGUI.BeginChangeCheck();
            int alignment = (int)settings.spriteAlignment;
            alignment = EditorGUILayout.Popup(styles.pivotGC, alignment, styles.spriteAlignmentOptions);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(settings, "Change Alignment");
                settings.spriteAlignment = (SpriteAlignment)alignment;
                settings.pivot = VSUtils.AlignmentToPivot(settings.spriteAlignment, settings.pivot);
            }

            Vector2 pivot = settings.pivot;
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(alignment != (int)SpriteAlignment.Custom))
            {
                pivot = EditorGUILayout.Vector2Field(styles.customPivotGC, pivot);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(settings, "Change custom pivot");

                settings.pivot = pivot;
            }
        }

        private void DoAutomaticSlicing()
        {
            owner.DoAutomaticSlicing(settings);
        }

        private void DoGridSlicing()
        {
            if (settings.slicingType == SlicingType.GridByCellCount)
            {
                DetemineGridCellSizeWithCellCount();
            }

            owner.DoGridSlicing(settings);
        }

        private void DetemineGridCellSizeWithCellCount()
        {
            int width, height;
            textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
            var texture = textureDataProvider.GetReadableTexture2D();
            int maxWidth = texture != null ? width : 4096;
            int maxHeight = texture != null ? height : 4096;

            settings.gridSpriteSize.x = (maxWidth - settings.gridSpriteOffset.x - (settings.gridSpritePadding.x * settings.gridCellCount.x)) / settings.gridCellCount.x;
            settings.gridSpriteSize.y = (maxHeight - settings.gridSpriteOffset.y - (settings.gridSpritePadding.y * settings.gridCellCount.y)) / settings.gridCellCount.y;

            settings.gridSpriteSize.x = Mathf.Clamp(settings.gridSpriteSize.x, 1, maxWidth);
            settings.gridSpriteSize.y = Mathf.Clamp(settings.gridSpriteSize.y, 1, maxHeight);
        }

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle notice;

            public Styles()
            {
                notice = new GUIStyle(GUI.skin.label);
                notice.alignment = TextAnchor.MiddleCenter;
                notice.wordWrap = true;
            }

            public readonly GUIContent[] spriteAlignmentOptions =
            {
                new GUIContent("Center"),
                new GUIContent("Top Left"),
                new GUIContent("Top"),
                new GUIContent("Top Right"),
                new GUIContent("Left"),
                new GUIContent("Right"),
                new GUIContent("Bottom Left"),
                new GUIContent("Bottom"),
                new GUIContent("Bottom Right"),
                new GUIContent("Custom")
            };

            public readonly GUIContent[] slicingMethodOptions =
            {
                new GUIContent("Delete Existing", "Delete all existing sprite assets before the slicing operation"),
                new GUIContent("Smart", "Try to match existing sprite rects to sliced rects from the slicing operation"),
                new GUIContent("Safe", "Keep existing sprite rects intact")
            };

            public readonly GUIContent methodGC = new GUIContent("Method");
            public readonly GUIContent pivotGC = new GUIContent("Pivot");
            public readonly GUIContent typeGC = new GUIContent("Type");
            public readonly GUIContent sliceButtonGC = new GUIContent("Slice");
            public readonly GUIContent columnAndRowGC = new GUIContent("Column & Row");
            public readonly GUIContent columnGC = new GUIContent("C");
            public readonly GUIContent rowGC = new GUIContent("R");
            public readonly GUIContent pixelSizeGC = new GUIContent("Pixel Size");
            public readonly GUIContent xGC = new GUIContent("X");
            public readonly GUIContent yGC = new GUIContent("Y");
            public readonly GUIContent offsetGC = new GUIContent("Offset");
            public readonly GUIContent paddingGC = new GUIContent("Padding");
            public readonly GUIContent automaticSlicingHintGC = new GUIContent("To obtain more accurate slicing results, manual slicing is recommended!");
            public readonly GUIContent customPivotGC = new GUIContent("Custom Pivot");
            public readonly GUIContent keepEmptyRectsGC = new GUIContent("Keep Empty Rects");
            public readonly GUIContent slicingSelectionGC = new GUIContent("Slicing selection");
        }
    }
}
#endif

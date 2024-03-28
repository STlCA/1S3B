#if COM_UNITY_2D_SPRITE
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    [System.Serializable]
    internal partial class VSSpritePanel
    {
        public VSData data { get; set; }

        private List<VSSpritePanelColumn> columns;
        private GUIContent[] pivotGCs;
        private GenericMenu configsVisibilityMenu;

        private Color selectColor = new Color(0.3f, 1.0f, 0.3f);
        private Color highlightColor = new Color(0.3f, 0.3f, 1.0f);
        private Color highlightSelectedColor = new Color(0.0f, 1.0f, 1.0f);
        private Color duplicateNameColor = new Color(1.0f, 0.3f, 0.3f);
        private const string dataGUIControlIDpreffix = "DataGUI";

        private Vector2 scrollPos = Vector2.zero;

        private GUIStyle labelStyle;
        private GUIStyle fieldStyle;
        private GUIStyle popupStyle;
        private DataClickInfo clickData;

        //Sizes
        private float xPadding = 1.0f;
        private float yPadding = 2.0f;
        private float rowBaseHeight = 16.0f;
        private float rowSize;

        //Globals
        private float _totalColumnsWidth;
        private int rowStartIndex;
        private int rowEndIndex;

        //Buffers
        private string sBuf;
        private int iBuf;
        private float fBuf;
        private System.Enum eBuf;
        private IVSSpriteRect sprBuf;
        private StringBuilder dataGUIIdStringBuilder;

        private bool initialized => (labelStyle != null);

        private VSEvent _ev;

        private bool shouldRepaint = false;

        public VSSpritePanel(VSData data)
        {
            const int columnsAmount = 13;

            this.data = data;

            int i = 0;

            VSSettings settings = VSSettings.GetOrCreateSettings();

            columns = new List<VSSpritePanelColumn>(columnsAmount)
            {
                new VSSpritePanelColumn("Index of the sprite"         ,i++, settings.spritePanelSettings.idColumn),
                new VSSpritePanelColumn("Name of the sprite"          ,i++, settings.spritePanelSettings.nameColumn),
                new VSSpritePanelColumn("X position of the sprite"    ,i++, settings.spritePanelSettings.xColumn),
                new VSSpritePanelColumn("Y Position of the sprite"    ,i++, settings.spritePanelSettings.yColumn),
                new VSSpritePanelColumn("Width of the sprite"         ,i++, settings.spritePanelSettings.widthColumn),
                new VSSpritePanelColumn("Height of the sprite"        ,i++, settings.spritePanelSettings.heightColumn),
                new VSSpritePanelColumn("Left border of the sprite"   ,i++, settings.spritePanelSettings.borderLColumn),
                new VSSpritePanelColumn("Top border of the sprite"    ,i++, settings.spritePanelSettings.borderTColumn),
                new VSSpritePanelColumn("Right border of the sprite"  ,i++, settings.spritePanelSettings.borderRColumn),
                new VSSpritePanelColumn("Bottom border of the sprite" ,i++, settings.spritePanelSettings.borderBColumn),
                new VSSpritePanelColumn("Pivot type"                  ,i++, settings.spritePanelSettings.pivotColumn),
                new VSSpritePanelColumn("Pivot x value"               ,i++, settings.spritePanelSettings.pivotXColumn),
                new VSSpritePanelColumn("Pivot y value"               ,i++, settings.spritePanelSettings.pivotYColumn)
            };

            pivotGCs = new GUIContent[10];
            pivotGCs[(int)SpriteAlignment.TopLeft]      = new GUIContent(settings.spritePanelSettings.pivotSettings.topLeft, "Top Left");
            pivotGCs[(int)SpriteAlignment.TopCenter]    = new GUIContent(settings.spritePanelSettings.pivotSettings.topCenter, "Top Center");
            pivotGCs[(int)SpriteAlignment.TopRight]     = new GUIContent(settings.spritePanelSettings.pivotSettings.topRight, "Top Right");
            pivotGCs[(int)SpriteAlignment.LeftCenter]   = new GUIContent(settings.spritePanelSettings.pivotSettings.leftCenter, "Left Center");
            pivotGCs[(int)SpriteAlignment.Center]       = new GUIContent(settings.spritePanelSettings.pivotSettings.center, "Center");
            pivotGCs[(int)SpriteAlignment.RightCenter]  = new GUIContent(settings.spritePanelSettings.pivotSettings.rightCenter, "Right Center");
            pivotGCs[(int)SpriteAlignment.BottomLeft]   = new GUIContent(settings.spritePanelSettings.pivotSettings.bottomLeft, "Bottom Left");
            pivotGCs[(int)SpriteAlignment.BottomCenter] = new GUIContent(settings.spritePanelSettings.pivotSettings.bottomCenter, "Bottom Center");
            pivotGCs[(int)SpriteAlignment.BottomRight]  = new GUIContent(settings.spritePanelSettings.pivotSettings.bottomRight, "Bottom Right");
            pivotGCs[(int)SpriteAlignment.Custom]       = new GUIContent(settings.spritePanelSettings.pivotSettings.custom, "Custom");

            CreateMenu();

            void OnConfigPressed(object item)
            {
                VSSpritePanelColumn config = (VSSpritePanelColumn)item;
                config.settings.isVisible = !config.settings.isVisible;

                CreateMenu();
            }

            void CreateMenu()
            {
                configsVisibilityMenu = new GenericMenu();

                for (i = 0; i < columns.Count; i++)
                {
                    configsVisibilityMenu.AddItem(columns[i].gc, columns[i].settings.isVisible, OnConfigPressed, columns[i]);
                }
            }
        }

        /// <summary>
        /// Displays the GUI of Sprite Panel. Returns TRUE if it should repaint.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        public bool OnGUI(Rect rect, VSEvent ev)
        {
            if (data == null)
            {
                return false;
            }

            if (!initialized)
            {
                Init();
            }

            this._ev = ev;

            UpdateGlobals(rect);
            ProcessInput(rect);
            SpriteGUI(rect);

            return shouldRepaint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateGlobals(Rect rect)
        {
            rowSize = rowBaseHeight + yPadding;
            shouldRepaint = false;
        }

        private void ProcessInput(Rect rect)
        {
            if(_ev.type == EventType.ScrollWheel)
            {
                clickData.Unfocus();
            }

            if(_ev.type == EventType.KeyDown)
            {
                if (_ev.control)
                {
                    if (_ev.keyCode == KeyCode.Alpha1)
                    {
                        data.AddIndicesToSelectionByXThenY();
                        data.spriteEditor.SetDataModified();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.Alpha2)
                    {
                        data.AddIndicesToSelectionByYThenX();
                        data.spriteEditor.SetDataModified();
                        _ev.Use();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateRowsRange(Rect rect)
        {
            Rect scrollRect = rect;
            rowStartIndex = (int)(scrollPos.y / rowSize);
            rowEndIndex = (int)((scrollPos.y + scrollRect.height) / rowSize) + 1;

            if (rowStartIndex < 0)
            {
                rowStartIndex = 0;
            }

            if (rowEndIndex < 0)
            {
                rowEndIndex = 0;
            }
        }

        private void Init()
        {
            labelStyle = EditorStyles.label;
            fieldStyle = EditorStyles.numberField;
            popupStyle = EditorStyles.popup;
            clickData = new DataClickInfo(this);
        }

        public void ScrollToSelection()
        {
            float rowSize = rowBaseHeight + yPadding;

            clickData.Unfocus();

            if (data.isFiltered)
            {
                for (int i = 0; i < data.filteredAmount; i++)
                {
                    IVSSpriteRect spr = data.GetFiltered(i);
                    if (spr.IsSelected)
                    {
                        scrollPos.y = rowSize * i;
                        break;
                    }
                }
            }
            else
            {
                IVSSpriteRect spr = data.GetSelection(0);

                if (data.showMode == VSData.WindowShowMode.Selected)
                {
                    scrollPos.y = 0;
                }
                else
                {
                    scrollPos.y = rowSize * spr.ID;
                }
            }
        }

        public void ScrollToHighlight()
        {
            float rowSize = rowBaseHeight + yPadding;

            clickData.Unfocus();

            if (data.isFiltered)
            {
                for (int i = 0; i < data.filteredAmount; i++)
                {
                    IVSSpriteRect spr = data.GetFiltered(i);
                    if (spr.IsHighlighted)
                    {
                        scrollPos.y = rowSize * i;
                        break;
                    }
                }
            }
            else
            {
                if (data.showMode == VSData.WindowShowMode.Selected)
                {
                    for (int i = 0; i < data.selectAmount; i++)
                    {
                        IVSSpriteRect spr = data.GetSelection(i);

                        if (spr.IsHighlighted)
                        {
                            scrollPos.y = rowSize * i;
                            break;
                        }
                    }
                }
                else
                {
                    IVSSpriteRect spr = data.GetHighlight(0);
                    scrollPos.y = rowSize * spr.ID;
                }
            }
        }

        private void LabelGUI(Rect rect)
        {
            if (_ev.type == EventType.MouseDown)
            {
                if (_ev.button == 1 && rect.Contains(_ev.mousePosition))
                {
                    configsVisibilityMenu.ShowAsContext();
                    _ev.Use();
                }
            }

            Rect rectBuf = new Rect(rect);
            rectBuf.y += yPadding;
            rectBuf.x = rectBuf.x - scrollPos.x;

            for (int i = 0; i < columns.Count; i++)
            {
                if (!columns[i].settings.isVisible)
                {
                    continue;
                }

                rectBuf.width = columns[i].currentColumnWidth;
                GUI.Label(rectBuf, columns[i].gcShort, labelStyle);
                rectBuf.x += rectBuf.width + xPadding;
            }

            rectBuf.y += yPadding;
        }

        private void SpriteGUI(Rect rect)
        {
            int rows = GetVisibleRowsAmount();

            Rect spriteRowRect = new Rect(rect);
            spriteRowRect.height = rowBaseHeight;

            rect.y += rowBaseHeight;
            rect.height -= rowBaseHeight;
           
            int dataAmount;

            if (data.isFiltered)
            {
                dataAmount = data.filteredAmount;
            }
            else if (data.showMode == VSData.WindowShowMode.Selected
                    && data.selectAmount > 0)
            {
                dataAmount = data.selectAmount;
            }
            else
            {
                dataAmount = data.spriteAmount;
            }

            bool isVerticalScrollbarShown = (dataAmount * rowSize > rect.height);

            UpdateColumnSizes(rect, isVerticalScrollbarShown);
            LabelGUI(spriteRowRect);
            spriteRowRect.y += rowBaseHeight + yPadding * 2;

            Rect scrollRectView = new Rect(rect);
            scrollRectView.width = _totalColumnsWidth;
            scrollRectView.height = rows * rowBaseHeight + rows * yPadding + yPadding * 2;

            Rect scrollRect = new Rect(rect);

            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, scrollRectView);
            UpdateRowsRange(scrollRect);
            spriteRowRect.y += rowSize * rowStartIndex;

            //Show sprite list
            SpriteGUIControls(scrollRect);

            Vector2 heightOffset = new Vector2(0.0f, scrollPos.y);

            if (data.isFiltered)
            {
                ShowFiltered();
            }
            else if (data.showMode == VSData.WindowShowMode.Selected
                    && data.selectAmount > 0)
            {
                ShowSelected();
            }
            else
            {
                ShowAll();
            }

            GUI.EndScrollView(true);

            void ShowFiltered()
            {
                dataAmount = data.filteredAmount;

                for (int i = rowStartIndex; i < dataAmount && i < rowEndIndex; i++)
                {
                    DataGUI(spriteRowRect, data.GetFiltered(i), i, heightOffset);
                    spriteRowRect.y += rowSize;
                    if (_ev.type == EventType.Used)
                    {
                        break;
                    }
                }
            }

            void ShowSelected()
            {
                dataAmount = data.selectAmount;

                for (int i = rowStartIndex; i < dataAmount && i < rowEndIndex; i++)
                {
                    DataGUI(spriteRowRect, data.GetSelection(i), i, heightOffset);
                    spriteRowRect.y += rowSize;
                    if (_ev.type == EventType.Used)
                    {
                        break;
                    }
                }
            }

            void ShowAll()
            {
                dataAmount = data.spriteAmount;

                for (int i = rowStartIndex; i < dataAmount && i < rowEndIndex; i++)
                {
                    DataGUI(spriteRowRect, data.GetSprite(i), i, heightOffset);
                    spriteRowRect.y += rowSize;
                    if (_ev.type == EventType.Used)
                    {
                        break;
                    }
                }
            }

            int GetVisibleRowsAmount()
            {
                if (data.showMode == VSData.WindowShowMode.Selected && data.selectAmount > 0)
                {
                    return data.selectAmount;
                }
                else
                {
                    return data.spriteAmount;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DataGUI(Rect rect, IVSSpriteRect vsSprite, int rowIndex, Vector2 heightOffset)
        {
            bool shouldUseTheEvent = false;
            Vector2 mousePositionWithOffset = _ev.mousePosition + heightOffset;

            Rect rectBuf = new Rect(rect);
            SpriteRect sprite = vsSprite.SR;

            if (vsSprite.IsDuplicateName)
            {
                GUI.backgroundColor = duplicateNameColor;
            }
            else if (vsSprite.IsHighlighted && vsSprite.IsSelected)
            {
                GUI.backgroundColor = highlightSelectedColor;
            }
            else if (vsSprite.IsHighlighted)
            {
                GUI.backgroundColor = highlightColor;
            }
            else if (vsSprite.IsSelected)
            {
                GUI.backgroundColor = selectColor;
            }

            if (_ev.type == EventType.MouseDown)
            {
                if (_ev.shift)
                {
                    bool isOver = rect.Contains(mousePositionWithOffset);
                    if (isOver)
                    {
                        data.Select(vsSprite, _ev.button == 0);
                        shouldUseTheEvent = true;
                    }
                }
            }

            if (_ev.type == EventType.MouseDrag)
            {
                if (_ev.shift)
                {
                    bool isOver = rect.Contains(mousePositionWithOffset);
                    if (isOver)
                    {
                        //Debug.Log($"Mouse drag : {vsSprite.ToString()}");
                        data.Select(vsSprite, _ev.button == 0);
                        shouldUseTheEvent = true;
                    }
                }
            }

            if (_ev.type == EventType.MouseMove)
            {
                if (rect.Contains(mousePositionWithOffset))
                {
                    //Debug.Log($"sprite:{vsSprite.id} rect:{rect} mouse move : {_ev.mousePosition}");
                    data.Highlight(vsSprite, true);
                }
                else
                {
                    data.Highlight(vsSprite, false);
                }
            }

            //if(_ev.type == EventType.Repaint)
            //{
            //    Debug.Log($"PAINT sprite:{vsSprite.id} rect:{rect}");
            //}

            int columnElements = 13;
            int i = 0;
            int elementIndex = 0;

            if (PreC()) { EditorGUI.BeginDisabledGroup(true); EditorGUI.IntField(rectBuf, vsSprite.ID, fieldStyle); EditorGUI.EndDisabledGroup(); PostC(); } i++;
            if (PreC()) { sBuf = TextField(sprite.name); if (PostC()) { ApplyName(sBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.rect.x); if (PostC()) { ApplyRectX(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.rect.y); if (PostC()) { ApplyRectY(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.rect.width); if (PostC()) { ApplyRectWidth(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.rect.height); if (PostC()) { ApplyRectHeight(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.border.x); if (PostC()) { ApplyBorderX(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.border.y); if (PostC()) { ApplyBorderY(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.border.z); if (PostC()) { ApplyBorderZ(iBuf); } } i++;
            if (PreC()) { iBuf = IntField((int)sprite.border.w); if (PostC()) { ApplyBorderW(iBuf); } } i++;
            if (PreC()) { PivotField(sprite); PostC(); } i++;

            EditorGUI.BeginDisabledGroup(sprite.alignment != SpriteAlignment.Custom);
            if (PreC()){ fBuf = FloatField(sprite.pivot.x); if (PostC()) { ApplyPivotX(fBuf); } } i++;
            if (PreC()){ fBuf = FloatField(sprite.pivot.y); if (PostC()) { ApplyPivotY(fBuf); } } i++;
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = Color.white;

            if (shouldUseTheEvent)
            {
                _ev.Use();
            }

            /*
            void DebugPrint()
            {
                if (isFirst)
                {
                    VSConsole.Log(this, $"First element name displayed. Event : {_ev.type}, " +
                        $"elementIndex:{elementIndex}, " +
                        $"ClickData.ElementIndex:{clickData.ElementIndex}");
                    isFirst = false;
                }
            }
            */

            bool PreC()
            {
                if (!columns[i].settings.isVisible)
                {
                    return false;
                }

                elementIndex = i + columnElements * rowIndex;

                GUI.SetNextControlName(dataGUIControlIDpreffix + elementIndex.ToString());

                rectBuf.width = columns[i].currentColumnWidth;

                if (_ev.type == EventType.MouseDown)
                {
                    if (_ev.button == 0 && rectBuf.Contains(mousePositionWithOffset))
                    {
                        clickData.Update(_ev, i, elementIndex, columnElements);
                        shouldUseTheEvent = true;
                    }
                }

                EditorGUI.BeginChangeCheck();

                return true;
            }

            bool PostC()
            {
                rectBuf.x += rectBuf.width + xPadding;

                if (EditorGUI.EndChangeCheck())
                {
                    data.spriteEditor.SetDataModified();
                    return true;
                }
                return false;
            }

            string TextField(string val)
            {
                return EditorGUI.TextField(rectBuf, val, fieldStyle);
            }

            int IntField(int val)
            {
                return EditorGUI.IntField(rectBuf, val, fieldStyle);
            }

            void PivotField(SpriteRect val)
            {
                GUIContent gc = pivotGCs[(int)val.alignment];

                if (GUI.Button(rectBuf, gc, popupStyle))
                {
                    if (VSPivotWindow.ShowAtPosition(rectBuf, pivotGCs, val.alignment, OnSelect))
                    {
                        GUIUtility.ExitGUI();
                    }
                }

                void OnSelect(SpriteAlignment alignment)
                {
                    data.spriteEditor.SetDataModified();

                    if (ShouldSingleedit())
                    {
                        val.alignment = alignment;
                    }
                    else
                    {
                        SpriteAlignment al = alignment;

                        for (int id = 0; id < data.selectAmount; id++)
                        {
                            sprBuf = data.GetSelection(id);
                            sprBuf.SR.alignment = al;
                        }
                    }
                }
            }

            float FloatField(float val)
            {
                return EditorGUI.FloatField(rectBuf, val, fieldStyle);
            }

            void ApplyName(string val)
            {
                if (ShouldSingleedit())
                {
                    sprite.name = val;
                }
                else
                {
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.Name = val;
                    }
                }
            }

            void ApplyRectX(int val)
            {
                if (ShouldSingleedit())
                {
                    sprite.rect = new Rect(sprite.rect) { x = val };
                }
                else
                {
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.Rect = new Rect(sprBuf.Rect) { x = val };
                    }
                }
            }

            void ApplyRectY(int val)
            {
                if (ShouldSingleedit())
                {
                    sprite.rect = new Rect(sprite.rect) { y = val };
                }
                else
                {
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.Rect = new Rect(sprBuf.Rect) { y = val };
                    }
                }
            }

            void ApplyRectWidth(int val)
            {
                if (ShouldSingleedit())
                {
                    sprite.rect = new Rect(sprite.rect) { width = val };
                }
                else
                {
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.Rect = new Rect(sprBuf.Rect) { width = val };
                    }
                }
            }

            void ApplyRectHeight(int val)
            {
                if (ShouldSingleedit())
                {
                    sprite.rect = new Rect(sprite.rect) { height = val };
                }
                else
                {
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.Rect = new Rect(sprBuf.Rect) { height = val };
                    }
                }
            }

            void ApplyBorderX(int val)
            {
                if (ShouldSingleedit())
                {
                    Vector4 vec = sprite.border;
                    vec.x = val;
                    sprite.border = vec;
                }
                else
                {
                    Vector4 vec;
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        vec = sprBuf.SR.border; vec.x = Mathf.Clamp(val, 0, sprBuf.Rect.width);
                        sprBuf.SR.border = vec;
                    }
                }
            }

            void ApplyBorderY(int val)
            {
                if (ShouldSingleedit())
                {
                    Vector4 vec = sprite.border;
                    vec.y = val;
                    sprite.border = vec;
                }
                else
                {
                    Vector4 vec;
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        vec = sprBuf.SR.border; vec.y = Mathf.Clamp(val, 0, sprBuf.Rect.width);
                        sprBuf.SR.border = vec;
                    }
                }
            }

            void ApplyBorderZ(int val)
            {
                if (ShouldSingleedit())
                {
                    Vector4 vec = sprite.border;
                    vec.z = val;
                    sprite.border = vec;
                }
                else
                {
                    Vector4 vec;
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        vec = sprBuf.SR.border; vec.z = Mathf.Clamp(val, 0, sprBuf.SR.rect.width);
                        sprBuf.SR.border = vec;
                    }
                }
            }

            void ApplyBorderW(int val)
            {
                if (ShouldSingleedit())
                {
                    Vector4 vec = sprite.border;
                    vec.w = val;
                    sprite.border = vec;
                }
                else
                {
                    Vector4 vec;
                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        vec = sprBuf.SR.border; vec.w = Mathf.Clamp(val, 0, sprBuf.SR.rect.width);
                        sprBuf.SR.border = vec;
                    }
                }
            }

            void ApplyPivotX(float val)
            {
                if (ShouldSingleedit())
                {
                    Vector2 vec = sprite.pivot; vec.x = val; sprite.pivot = vec;
                }
                else
                {
                    Vector2 vec = sprite.pivot; vec.x = val;

                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.SR.pivot = vec;
                    }
                }
            }

            void ApplyPivotY(float val)
            {
                if (ShouldSingleedit())
                {
                    Vector2 vec = sprite.pivot; vec.y = val; sprite.pivot = vec;
                }
                else
                {
                    Vector2 vec = sprite.pivot; vec.y = val;

                    for (int id = 0; id < data.selectAmount; id++)
                    {
                        sprBuf = data.GetSelection(id);
                        sprBuf.SR.pivot = vec;
                    }
                }
            }

            bool ShouldSingleedit()
            {
                return !data.windowMultiedit || !vsSprite.IsSelected || data.selectAmount == 1;
            }
        }

        private void SpriteGUIControls(Rect rect)
        {
            if (_ev.type == EventType.Repaint)
            {
                clickData.UpdateRenderRect(rect);
            }

            if (_ev.type == EventType.MouseDown)
            {
                if (_ev.button == 0)
                {
                    DataClickMouse();
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (clickData.isValid && _ev.alt)
                {
                    if (_ev.keyCode == KeyCode.DownArrow)
                    {
                        DataClickKeyDown();
                        _ev.Use();
                    }
                    else if (_ev.keyCode == KeyCode.UpArrow)
                    {
                        DataClickKeyUp();
                        _ev.Use();
                    }
                    else if (_ev.keyCode == KeyCode.LeftArrow)
                    {
                        DataClickKeyLeft();
                        _ev.Use();
                    }
                    else if (_ev.keyCode == KeyCode.RightArrow)
                    {
                        DataClickKeyRight();
                        _ev.Use();
                    }
                }
            }

            void DataClickMouse()
            {
                clickData.isValid = false;
            }

            void DataClickKeyUp()
            {
                clickData.MoveUp();
                clickData.FocusOnCurrent();
                shouldRepaint = true;
            }

            void DataClickKeyDown()
            {
                clickData.MoveDown();
                clickData.FocusOnCurrent();
                shouldRepaint = true;
            }

            void DataClickKeyLeft()
            {
                clickData.MoveLeft();
                clickData.FocusOnCurrent();
                shouldRepaint = true;
            }

            void DataClickKeyRight()
            {
                clickData.MoveRight();
                clickData.FocusOnCurrent();
                shouldRepaint = true;
            }
        }

        private void UpdateColumnSizes(Rect rect, bool isVerticalScrollbarShown)
        {
            const float verticalScrollbarSize = 16.0f;
            float elasticRatio = 0.0f;
            int visibleComponents = 0;
            float minWidth = 0.0f;

            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].settings.isElastic)
                {
                    elasticRatio += 1.0f;
                }

                if (columns[i].settings.isVisible)
                {
                    visibleComponents += 1;
                    minWidth += columns[i].settings.minColumnWidth;
                }

                columns[i].currentColumnWidth = columns[i].settings.minColumnWidth;
            }

            float paddingWidth = xPadding * (visibleComponents -1);
            minWidth += paddingWidth;

            if (isVerticalScrollbarShown)
            {
                minWidth += verticalScrollbarSize;
            }

            float freeWidth = rect.width - minWidth;
            float width = 0.0f;

            if(freeWidth > 0.0f)
            {
                float freeWidthPerComponent = freeWidth / elasticRatio;

                for (int i = 0; i < columns.Count; i++)
                {
                    if (!columns[i].settings.isVisible)
                    {
                        continue;
                    }

                    if (columns[i].settings.isElastic)
                    {
                        columns[i].currentColumnWidth = columns[i].settings.minColumnWidth + freeWidthPerComponent;
                    }
                    else
                    {
                        columns[i].currentColumnWidth = columns[i].settings.minColumnWidth;
                    }

                    width += columns[i].currentColumnWidth;
                }
            }
            else
            {
                width = minWidth;
            }

            _totalColumnsWidth = width;
        }
    }
}
#endif
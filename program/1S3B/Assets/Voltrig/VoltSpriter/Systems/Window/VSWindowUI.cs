#if COM_UNITY_2D_SPRITE
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649

namespace Voltrig.VoltSpriter
{
    public partial class VSWindowUI
    {
        private readonly GUIContent gcModeAll = new GUIContent("Show All", "Shows all sprites normally.");
        private readonly GUIContent gcModeNone = new GUIContent("Show Sel", "Shows selected sprites only.");
        private readonly GUIContent gcMultieditOn = new GUIContent("Multi", "Edit whole selection at once.");
        private readonly GUIContent gcMultieditOff = new GUIContent("Single", "Edit one item at once.");
        private readonly GUIContent gcScrollToSelOn = new GUIContent("Scroll On", "Scrolls to highlights and selection.");
        private readonly GUIContent gcScrollToSelOff = new GUIContent("Scroll Off", "Doesn't scroll to highglihts and selection.");
        private readonly GUIContent gcFilterLabel = new GUIContent("Filter", "Type in a string, which will be used to filter sprite names.");
        private readonly GUIContent gcReplaceLabel = new GUIContent("Replace", "Type in a string to replace the filtered value.");

        private VSWindow window;
        private VSSpritePanel spritePanel;

        private VSEvent _ev;
        private VSData _data;
        private bool ignoreWindowCausedEvents = false;

        private string filterStr = string.Empty;
        private string replaceStr = string.Empty;

        //Globals 
        private bool _replaceFieldClicked = false;
        private bool shouldRepaint = false;

        public VSData data
        {
            set
            {
                if (_data == value)
                {
                    return;
                }
                _data = value;
                OnDataUpdate();
            }
        }

        private bool initialized => (spritePanel != null);

        public VSWindowUI(VSWindow window)
        {
            spritePanel = new VSSpritePanel(_data);
            this.window = window;
        }

        private void OnDataUpdate()
        {
            if (_data == null)
            {
                return;
            }

            spritePanel.data = _data;
            _data.OnSelectionChangeE += Data_OnSelectionChangeE;
            _data.OnHighlightChangeE += Data_OnHighlightChangeE;
            _data.OnFilteredChangeE += Data_OnFilteredChangeE;
            _data.OnAutoindiceE += Data_OnAutoindiceE;
            _data.ClearFilter();
        }

        public void ShowGUI(Rect rect)
        {
            if (_data == null)
            {
                return;
            }

            UpdateGlobals();

            if (_ev.type == EventType.Layout)
            {
                return;
            }

            ignoreWindowCausedEvents = true;

            float headerHeight = 32.0f;

            Rect headerRect = new Rect(rect);
            headerRect.height = headerHeight;
            HeaderGUI(headerRect);
            rect.y += headerRect.height;
            rect.height -= headerHeight;

            shouldRepaint = spritePanel.OnGUI(rect, _ev);

            ignoreWindowCausedEvents = false;

            if (shouldRepaint)
            {
                window.Repaint();
            }
        }

        private void UpdateGlobals()
        {
            _ev = new VSEvent(Event.current);
            shouldRepaint = false;
        }

        private void HeaderGUI(Rect rect)
        {
            Rect rectBuf = new Rect(rect);
            float totalWidth = rectBuf.width;
            float elementWidth = 70.0f;
            rectBuf.width = elementWidth;
            float height = rectBuf.height;
            float halfHeight = height * 0.5f;

            rectBuf.height = halfHeight;
            SpriteModeButtonGUI(rectBuf); AfterHeaderElement();
            MultieditModeGUI(rectBuf); AfterHeaderElement();
            ScrollToSelectedGUI(rectBuf); LastHeaderElement(3);

            rectBuf.x = 0;
            rectBuf.y += halfHeight;
            rectBuf.width = rect.width * 0.5f;

            FilterGUI(rectBuf); rectBuf.x += rectBuf.width;
            ReplaceGUI(rectBuf);
            rectBuf.x = 0.0f;
            rectBuf.width = totalWidth;

            void AfterHeaderElement()
            {
                rectBuf.x += elementWidth;
            }

            void LastHeaderElement(int elements)
            {
                rectBuf.x += elementWidth;
                rectBuf.width = totalWidth - elementWidth * elements;
            }

            void FilterGUI(Rect r)
            {
                float width = r.width;
                r.width = elementWidth;
                GUI.Label(r, gcFilterLabel);
                r.x += elementWidth;
                r.width = width - elementWidth;

                EditorGUI.BeginChangeCheck();
                filterStr = EditorGUI.TextField(r, filterStr);
                if (EditorGUI.EndChangeCheck())
                {
                    _data.ClearFilter();
                    _data.Filter(filterStr);
                }
            }

            void ReplaceGUI(Rect r)
            {
                bool disabled = string.IsNullOrEmpty(filterStr);
                EditorGUI.BeginDisabledGroup(disabled);
                float width = r.width;
                r.width = elementWidth;
                GUI.Label(r, gcReplaceLabel);
                r.x += elementWidth;
                r.width = width - elementWidth;

                if (_ev.type == EventType.MouseDown)
                {
                    if (_ev.button == 0)
                    {
                        _replaceFieldClicked = r.Contains(_ev.mousePosition);
                    }
                }
                if (_ev.type == EventType.KeyDown)
                {
                    if (_replaceFieldClicked && _ev.keyCode == KeyCode.Return)
                    {
                        filterStr = replaceStr;
                        replaceStr = string.Empty;
                        _data.ReplaceFilteredTo(filterStr);
                        _ev.Use();
                    }
                }
                replaceStr = EditorGUI.TextField(r, replaceStr);
                EditorGUI.EndDisabledGroup();
            }

            void ScrollToSelectedGUI(Rect r)
            {
                if (_data.scrollToSelection)
                {
                    GUI.color = Color.green;
                    if (GUI.Button(r, gcScrollToSelOn))
                    {
                        _data.scrollToSelection = false;
                    }
                }
                else
                {
                    if (GUI.Button(r, gcScrollToSelOff))
                    {
                        _data.scrollToSelection = true;
                    }
                }

                GUI.color = Color.white;
            }

            void SpriteModeButtonGUI(Rect r)
            {
                if (_data.showMode == VSData.WindowShowMode.Selected)
                {
                    GUI.color = Color.green;
                    if (GUI.Button(r, gcModeNone))
                    {
                        _data.showMode = VSData.WindowShowMode.All;
                    }
                }
                else if (_data.showMode == VSData.WindowShowMode.All)
                {
                    GUI.color = Color.white;
                    if (GUI.Button(r, gcModeAll))
                    {
                        _data.showMode = VSData.WindowShowMode.Selected;
                    }
                }

                GUI.color = Color.white;
            }

            void MultieditModeGUI(Rect r)
            {
                if (_data.windowMultiedit)
                {
                    GUI.color = Color.green;
                    if (GUI.Button(r, gcMultieditOn))
                    {
                        _data.windowMultiedit = false;
                    }
                }
                else
                {
                    if (GUI.Button(r, gcMultieditOff))
                    {
                        _data.windowMultiedit = true;
                    }
                }

                GUI.color = Color.white;
            }
        }

        private void Data_OnAutoindiceE()
        {
            window.Repaint();
        }

        private void Data_OnFilteredChangeE()
        {
            window.Repaint();
        }

        private void Data_OnHighlightChangeE()
        {
            if (!ignoreWindowCausedEvents && _data.scrollToSelection)
            {
                if (_data.highlightAmount > 0)
                {
                    spritePanel.ScrollToHighlight();
                }
                else if (_data.selectionAmount > 0)
                {
                    spritePanel.ScrollToSelection();
                }
            }
            window.Repaint();
        }

        private void Data_OnSelectionChangeE()
        {
            if (!ignoreWindowCausedEvents && _data.scrollToSelection && _data.selectAmount > 0)
            {
                spritePanel.ScrollToSelection();
            }

            window.Repaint();
        }
    }
}
#endif

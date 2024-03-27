#if COM_UNITY_2D_SPRITE
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    public partial class VSModule : SpriteEditorModuleBase
    {
        public override string moduleName => "Volt Spriter";

        private System.Action drawGUIA;

        private ISpriteEditorDataProvider provider;

        public VSData data;
        private SpriteBordersHandler bordersHandler;
        private SpriteRectHandler spriteRectHandler;
        private SpritePivotHandler spritePivotHandler;
        private SpriteScaleHandler spriteScaleHandler;
        private SpriteDrawer spriteDrawer;

        //GUIContents
        private GUIContent selectLabelGC = new GUIContent("Select mode");
        private GUIContent translateLabelGC = new GUIContent("Translate mode");
        private GUIContent createLabelGC = new GUIContent("Create mode");
        private GUIContent scaleLabelGC = new GUIContent("Scale mode");
        private GUIContent editLabelGC = new GUIContent("Edit mode");
        private GUIContent borderLabelGC = new GUIContent("Borders mode");
        private GUIContent rectLabelGC = new GUIContent("Rect mode");
        private GUIContent pivotLabelGC = new GUIContent("Pivot mode");
        private GUIContent vsWindowButtonGC = new GUIContent("VS Window");
        private GUIContent trimButtonGC = new GUIContent("Trim");
        private GUIContent sliceButtonGC = new GUIContent("Slice");
        private GUIContent settingsButtonGC = new GUIContent("Settings");
        private GUIStyle whiteLabelStyle;
        private GUIStyle blackLabelStyle;

        //Buffers
        private List<IVSSpriteRect> srBufL = new List<IVSSpriteRect>();
        private IVSSpriteRect srBuf;
        private StringBuilder spriteNameStringBuilder;

        //Reflection
        private bool isInitialized => m_zoomF != null;
        private FieldInfo m_zoomF;
        private FieldInfo m_scrollPositionF;
        private FieldInfo m_shouldDrawSpriteBoxesF;
        private MethodInfo m_getPermamentControlIDM;
        private MethodInfo createTemporaryDuplicateM;

        //Globals
        private UnityEngine.Event _ev;
        private Matrix4x4 _handleMatrix;
        private Vector2 _dragStart;
        private Vector2 _dragEnd;
        private Vector2 _mousePos;
        private Vector2 _zeroPos;
        private Vector2 _scrollPosition;
        private Vector2 _mouseLabelOffset = new Vector2(12.0f, 12.0f);
        private int arrowMoveAmount = 1;
        private int arrowMoveCtrlAmount = 5;
        private float _zoom;
        private float _zoomOnePixOffset;
        private bool _isDragging = false;
        private bool _requestRepaint = false;
        private bool _requestWindowRepaint = false;
        private bool _shouldIForceRepaint = true;
        private IVSSpriteRect _overlapSprite = null;
        private bool _moveSelectionExecutedAtLeastOnce = false;

        // Colors
        private readonly Color highlightFillColor = new Color(0.3f, 0.3f, 0.3f, 0.25f);
        private readonly Color highlightColor = new Color(0.3f, 0.3f, 1.0f);
        private readonly Color highlightShadeColor = new Color(0.1f, 0.1f, 0.3f, 0.7f);
        private readonly Color selectBoxColor = new Color(1.0f, 0.0f, 0.0f);
        private readonly Color selectBoxShadeColor = new Color(0.3f, 0.0f, 0.0f, 0.7f);
        private readonly Color selectColor = new Color(0.0f, 1.0f, 0.0f);
        private readonly Color selectShadeColor = new Color(0.0f, 0.3f, 0.0f, 0.7f);
        private readonly Color spriteBorderColor = new Color(1.0f, 0.3f, 0.3f, 0.4f);
        private readonly Color filteredFillColor = new Color(0.4f, 0.0f, 0.4f, 0.25f);

        // Control IDs
        private int _spriteWindowControlID = 0;

        // Trim 
        private byte trimAlpha = 0;

        // Modes
        private bool shouldExitEditMode = false;
        private bool shouldExitBorderMode = false;
        private bool shouldExitRectMode = false;
        private bool shouldExitPivotMode = false;
        private bool shouldExitScaleMode = false;

        //Settings
        internal VSSettings Settings { get; private set; }

        /// <summary>
        /// Toggle the injected field for disabling the built-in sprite box drawing.
        /// If the injection failed it will always be true.
        /// </summary>
        private bool ShouldDrawSpriteBoxes
        {
            get
            {
                if (m_shouldDrawSpriteBoxesF == null)
                {
                    return true;
                }
                else
                {
                    return (bool)m_shouldDrawSpriteBoxesF.GetValue(spriteEditor);
                }
            }

            set
            {
                if (m_shouldDrawSpriteBoxesF == null)
                {
                    return;
                }

                m_shouldDrawSpriteBoxesF.SetValue(spriteEditor, value);
            }
        }

        // Called when the module is activated by user
        public override void OnModuleActivate()
        {
            Initialize();
            ShouldDrawSpriteBoxes = false;
        }

        // Called when user switches to another module
        public override void OnModuleDeactivate()
        {
            ShouldDrawSpriteBoxes = true;
            VS.inst.module = null;
        }

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                if (data.CheckDuplicatedNames(true))
                {
                    data.FixDuplicatedNames();
                    Debug.Log("[VoltSpriter] Marked and fixed duplicate names in sprites.");
                    provider.SetSpriteRects(data.spriteRects);
                    return true;
                }
                else
                {
                    provider.SetSpriteRects(data.spriteRects);
                    return true;
                }
            }
            else
            {
                Initialize();
                return true;
            }
        }

        public override bool CanBeActivated()
        {
            return true;
        }

        /// <inheritdoc/>
        public override void DoToolbarGUI(Rect drawArea)
        {
            _shouldIForceRepaint = false;

            if (whiteLabelStyle == null)
            {
                InitStyle();
            }

            float offset = 80.0f;
            float bigSize = 80.0f;
            float normalSize = 64.0f;
            float smallSize = 50.0f;

            float currentSize = Mathf.Min(bigSize, (drawArea.width));
            Rect buttonRect = new Rect(offset, 0.0f, currentSize, drawArea.height);
            drawArea.width -= buttonRect.width;

            EditorGUI.BeginDisabledGroup(VS.inst.window != null);

            if (GUI.Button(buttonRect, vsWindowButtonGC, EditorStyles.toolbarButton))
            {
                VSWindow.OpenWindow();
            }

            EditorGUI.EndDisabledGroup();

            UpdateDrawArea(smallSize);

            if (GUI.Button(buttonRect, sliceButtonGC, EditorStyles.toolbarPopup))
            {
                if (SliceWindow.ShowAtPosition(buttonRect, this, provider))
                {
                    GUIUtility.ExitGUI();
                }
            }

            UpdateDrawArea(smallSize);

            if (GUI.Button(buttonRect, trimButtonGC, EditorStyles.toolbarPopup))
            {
                if (TrimWindow.ShowAtPosition(buttonRect, this, provider))
                {
                    GUIUtility.ExitGUI();
                }
            }

            UpdateDrawArea(normalSize);

            if (GUI.Button(buttonRect, settingsButtonGC, EditorStyles.toolbarButton))
            {
                SettingsService.OpenProjectSettings(VSSettings.settingsPath);
            }

            _shouldIForceRepaint = true;

            void UpdateDrawArea(float size)
            {
                currentSize = Mathf.Min(size, (drawArea.width));
                buttonRect = new Rect(buttonRect.x + buttonRect.width, 0.0f, currentSize, drawArea.height);
                drawArea.width -= buttonRect.width;
            }
        }

        // Any last GUI draw. This is in the SpriteEditorWindow's space.
        // Any GUI draw will appear on top
        public override void DoPostGUI()
        {

        }

        // Called after SpriteEditorWindow drawn the sprite.
        // UnityEditor.Handles draw calls will operate in Texture space
        public override void DoMainGUI()
        {
            _shouldIForceRepaint = false;

            if (!isInitialized)
            {
                Initialize();
            }

            if (whiteLabelStyle == null)
            {
                InitStyle();
            }

            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            DrawGUI();

            RepaintIfNeccessary();

            _shouldIForceRepaint = true;
        }

        internal void TrimSelection(byte trimAlpha)
        {
            this.trimAlpha = trimAlpha;

            if (data.selectAmount == 0)
            {
                return;
            }

            ITextureDataProvider textureProvider = provider.GetDataProvider<ITextureDataProvider>();

            if (textureProvider == null)
            {
                VSConsole.LogError(this, "Texture provider is null!");
                return;
            }

            Texture2D texture = textureProvider.GetReadableTexture2D();
            bool shouldSetDataAsModified = false;

            if (texture == null)
            {
                VSConsole.LogError(this, "Texture is null!");
                return;
            }

            bool[] m_AlphaPixelCache = CreateAlphaPixelCache();

            List<IVSSpriteRect> spritesForDeletion = new List<IVSSpriteRect>();

            for (int i = 0; i < data.selectAmount; i++)
            {
                IVSSpriteRect spriteRect = data.GetSelection(i);
                TrimSprite(spriteRect);
            }

            data.DeleteSprites(spritesForDeletion);

            if (shouldSetDataAsModified)
            {
                spriteEditor.SetDataModified();
                _requestRepaint = true;
                _requestWindowRepaint = true;
            }

            void TrimSprite(IVSSpriteRect sprite)
            {
                Rect rect = sprite.Rect;

                rect = VSUtils.FitRectIntoTexture(rect, texture);

                int xMin = (int)rect.xMax;
                int xMax = (int)rect.xMin;
                int yMin = (int)rect.yMax;
                int yMax = (int)rect.yMin;

                for (int y = (int)rect.yMin; y < (int)rect.yMax; y++)
                {
                    for (int x = (int)rect.xMin; x < (int)rect.xMax; x++)
                    {
                        if (PixelHasAlpha(x, y))
                        {
                            xMin = Mathf.Min(xMin, x);
                            xMax = Mathf.Max(xMax, x);
                            yMin = Mathf.Min(yMin, y);
                            yMax = Mathf.Max(yMax, y);
                        }
                    }
                }

                if (xMin > xMax || yMin > yMax)
                {
                    rect = new Rect(0, 0, 0, 0);
                }
                else
                {
                    int width = xMax - xMin + 1;
                    int height = yMax - yMin + 1;
                    rect = new Rect(xMin, yMin, width, height);
                }

                if (rect.width <= 0 && rect.height <= 0)
                {
                    spritesForDeletion.Add(sprite);
                    shouldSetDataAsModified = true;
                }
                else
                {
                    if (sprite.Rect != rect)
                    {
                        sprite.Rect = rect;
                        shouldSetDataAsModified = true;
                    }
                }
            }

            bool PixelHasAlpha(int x, int y)
            {
                int index = y * (int)texture.width + x;
                return m_AlphaPixelCache[index];
            }

            bool[] CreateAlphaPixelCache()
            {
                bool[] alphaPixelCache = new bool[texture.width * texture.height];
                Color32[] pixels = texture.GetPixels32();

                for (int i = 0; i < pixels.Length; i++)
                {
                    alphaPixelCache[i] = pixels[i].a > trimAlpha;
                }

                return alphaPixelCache;
            }
        }

        internal void RepaintIfNeccessary()
        {
            if (_requestRepaint)
            {
                Repaint();
                _requestRepaint = false;
            }

            if (_requestWindowRepaint)
            {
                VS.inst.window?.Repaint();

                _requestWindowRepaint = false;
            }
        }

        internal void Revert()
        {
            spriteEditor.ApplyOrRevertModification(false);
        }

        internal void Repaint()
        {
            if (_shouldIForceRepaint)
            {
                EditorWindow window = (EditorWindow)spriteEditor;
                window?.Repaint();
            }
            else
            {
                spriteEditor.RequestRepaint();
            }
        }

        private void Initialize()
        {
            Settings = VSSettings.GetOrCreateSettings();
            Settings.OnValidateE += Settings_OnValidateE;

            bordersHandler = new SpriteBordersHandler(this);
            spriteRectHandler = new SpriteRectHandler(this);
            spritePivotHandler = new SpritePivotHandler(this);
            spriteScaleHandler = new SpriteScaleHandler(this);
            spriteDrawer = new SpriteDrawer();
            InitializeData();

            drawGUIA = DrawGUI_Selecting;

            InitializeReflection();
            _spriteWindowControlID = GetPermanentControlID();

            VS.inst.module = this;
            spriteEditor.enableMouseMoveEvent = true;

            spriteNameStringBuilder = new StringBuilder(GetSpriteNamePrefix() + "_");
        }

        private string GetSpriteNamePrefix()
        {
            if (provider == null)
            {
                return string.Empty;
            }

            ITextureDataProvider textureProvider = provider.GetDataProvider<ITextureDataProvider>();
 
            string spriteAssetPath = AssetDatabase.GetAssetPath(textureProvider.texture);
            return Path.GetFileNameWithoutExtension(spriteAssetPath);
        }

        private int GetPermanentControlID()
        {
            if (m_getPermamentControlIDM == null)
            {
                return 0;
            }
            else
            {
                return (int)m_getPermamentControlIDM.Invoke(null, new object[0]);
            }
        }

        private Texture2D CreateTemporaryDuplicate(Texture2D original, int width, int height)
        {
            if (createTemporaryDuplicateM == null)
            {
                return null;
            }
            else
            {
                return (Texture2D)createTemporaryDuplicateM.Invoke(null, new object[]
                {
                    original,
                    width,
                    height
                });
            }
        }

        private void InitializeData()
        {
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<VSData>();
                data.hideFlags = HideFlags.HideAndDontSave;
            }

            provider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();
            if (provider == null)
            {
                return;
            }

            data.SetSprites(provider.GetSpriteRects());

            data.OnSelectionChangeE += Data_OnSelectionChangeE;
            data.OnHighlightChangeE += Data_OnHighlightChangeE;
            data.OnFilteredChangeE += Data_OnFilteredChangeE;
            data.OnConfigChangeE += Data_OnConfigChangeE;
            data.spriteEditor = spriteEditor;
        }

        private void InitStyle()
        {
            whiteLabelStyle = new GUIStyle(EditorStyles.label);
            blackLabelStyle = new GUIStyle(EditorStyles.label);

            whiteLabelStyle.normal.textColor = Color.white;
            whiteLabelStyle.clipping = TextClipping.Overflow;
            whiteLabelStyle.fontStyle = Settings.hoverFontStyle;
            whiteLabelStyle.fontSize = Settings.hoverTextSize;

            blackLabelStyle.normal.textColor = Color.black;
            blackLabelStyle.clipping = TextClipping.Overflow;
            blackLabelStyle.fontStyle = Settings.hoverFontStyle;
            blackLabelStyle.fontSize = Settings.hoverTextSize;
        }

        private void InitializeReflection()
        {
            InitReflectionSpriteEditorWindow();
            InitReflectionGUIUtility();
            InitReflectionSpriteUtility();
        }

        private void InitReflectionSpriteEditorWindow()
        {
            System.Type spriteEditorWindowT;
            if (!VSReflection.GetTypeFromAssembly<ISpriteEditor>("UnityEditor.U2D.Sprites.SpriteEditorWindow", out spriteEditorWindowT))
            {
                return;
            }

            m_zoomF = spriteEditorWindowT.GetField("m_Zoom", BindingFlags.Instance | BindingFlags.NonPublic);
            m_scrollPositionF = spriteEditorWindowT.GetField("m_ScrollPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            m_shouldDrawSpriteBoxesF = spriteEditorWindowT.GetField("m_shouldDrawSpriteBoxes", BindingFlags.Instance | BindingFlags.Public);
        }

        private void InitReflectionGUIUtility()
        {
            System.Type guiUtilityT = typeof(GUIUtility);

            m_getPermamentControlIDM = guiUtilityT.GetMethod("GetPermanentControlID", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic);
        }

        private void InitReflectionSpriteUtility()
        {
            System.Type SpriteUtilityT;

            SpriteUtilityT = typeof(EditorGUI).Assembly.GetType("UnityEditor.SpriteUtility");

            createTemporaryDuplicateM = SpriteUtilityT.GetMethod("CreateTemporaryDuplicate", BindingFlags.Static | BindingFlags.Public);
        }

        private void DrawGUI()
        {
            UpdateGlobals(); //Should be always called in the beggining
            DrawGUI_Always();

            drawGUIA();
        }

        private void DrawGUI_Always()
        {
            if (_ev.type == EventType.MouseDown)
            {
                if (_ev.button == 0)
                {
                    _isDragging = true;
                    _dragStart = _mousePos;
                    if (_ev.alt)
                    {
                        _ev.Use();
                    }

                    GUIUtility.hotControl = _spriteWindowControlID;
                    GUIUtility.keyboardControl = _spriteWindowControlID;
                }
            }
            if (_ev.type == EventType.MouseDrag)
            {
                if (_isDragging)
                {
                    _requestRepaint = true;
                }
            }
            if (_ev.type == EventType.MouseUp)
            {
                if (_ev.button == 0)
                {
                    _isDragging = false;
                    _dragEnd = _mousePos;
                    if (_ev.alt)
                    {
                        _ev.Use();
                    }

                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
            }
            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.control)
                {
                    if (_ev.keyCode == KeyCode.S)
                    {
                        Save();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.Z)
                    {
                        Revert();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.C)
                    {
                        Copy();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.D)
                    {
                        Copy();
                        Paste(false);
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.V)
                    {
                        Paste(true);
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.T)
                    {
                        TrimSelection(trimAlpha);
                        _requestWindowRepaint = true;
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.X)
                    {
                        data.FlipSelection(false);
                        spriteEditor.SetDataModified();
                        _requestWindowRepaint = true;
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.Y)
                    {
                        data.FlipSelection(true);
                        spriteEditor.SetDataModified();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.R)
                    {
                        data.RotateSelection();
                        spriteEditor.SetDataModified();
                        _requestWindowRepaint = true;
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.Alpha1)
                    {
                        data.AddIndicesToSelectionByXThenY();
                        spriteEditor.SetDataModified();
                        _ev.Use();
                    }
                    if (_ev.keyCode == KeyCode.Alpha2)
                    {
                        data.AddIndicesToSelectionByYThenX();
                        spriteEditor.SetDataModified();
                        _ev.Use();
                    }
                    if (data.selectionAmount > 0)
                    {
                        if (_ev.keyCode == KeyCode.LeftArrow)
                        {
                            MoveSelection(Vector2Int.left * arrowMoveCtrlAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.UpArrow)
                        {
                            MoveSelection(Vector2Int.up * arrowMoveCtrlAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.RightArrow)
                        {
                            MoveSelection(Vector2Int.right * arrowMoveCtrlAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.DownArrow)
                        {
                            MoveSelection(Vector2Int.down * arrowMoveCtrlAmount);
                            _ev.Use();
                        }
                    }
                }
                else
                {
                    if (data.selectionAmount > 0)
                    {
                        if (_ev.keyCode == KeyCode.LeftArrow)
                        {
                            MoveSelection(Vector2Int.left * arrowMoveAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.UpArrow)
                        {
                            MoveSelection(Vector2Int.up * arrowMoveAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.RightArrow)
                        {
                            MoveSelection(Vector2Int.right * arrowMoveAmount);
                            _ev.Use();
                        }
                        if (_ev.keyCode == KeyCode.DownArrow)
                        {
                            MoveSelection(Vector2Int.down * arrowMoveAmount);
                            _ev.Use();
                        }
                    }
                }
            }
            if (_ev.type == EventType.Repaint)
            {
                spriteDrawer.DrawSprites(data, _zoom);
                DrawSelectedSprites();
                DrawFiltered();
                DrawHighlights();
            }
        }

        private void DrawGUI_Selecting()
        {
            if (_ev.type == EventType.MouseUp)
            {
                if (_ev.button == 0)
                {
                    SelectSprites(_dragStart, _dragEnd);
                    _requestRepaint = true;
                }
                HighlightClear();
            }

            if (_ev.type == EventType.MouseDown)
            {
                if (_ev.button == 0)
                {
                    if (_ev.shift)
                    {
                        if (IsClickOnSprite(_mousePos))
                        {
                            SelectSprite(_mousePos);
                        }
                    }
                    else
                    {
                        if (IsClickOnSelection(_mousePos))
                        {
                            drawGUIA = DrawGUI_Translate;
                        }
                        else if (IsClickOnSprite(_mousePos))
                        {
                            SelectSprite(_mousePos);

                            drawGUIA = DrawGUI_Translate;
                        }
                    }
                }
            }

            if (_ev.type == EventType.MouseMove)
            {
                HighlightSprites(_mousePos);
            }

            if (_ev.type == EventType.MouseDrag)
            {
                if (_ev.button == 0)
                {
                    HighlightSprites(_dragStart, _mousePos);
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.keyCode == KeyCode.Delete
                    && data.selectAmount > 0)
                {
                    DeleteSelection();
                    _ev.Use();
                }
                if (data.selectAmount == 0
                    && _ev.control)
                {
                    drawGUIA = DrawGUI_Create;
                }
                if (data.selectAmount > 0
                    && _ev.control)
                {
                    drawGUIA = DrawGUI_Edit;
                }

                if (data.selectAmount > 0
                    && _ev.keyCode == KeyCode.S)
                {
                    //TODO : Add scaling mode
                    //drawGUIA = DrawGUI_Scale;
                }

                if (data.selectAmount > 0
                    && _ev.keyCode == KeyCode.B
                    && !_ev.control)
                {
                    drawGUIA = DrawGUI_Border;
                }

                if (data.selectAmount > 0
                    && _ev.keyCode == KeyCode.R
                    && !_ev.control)
                {
                    drawGUIA = DrawGUI_Rect;
                }

                if (data.selectAmount > 0
                    && _ev.keyCode == KeyCode.P
                    && !_ev.control)
                {
                    drawGUIA = DrawGUI_Pivot;
                }
            }

            if (_ev.type == EventType.Repaint)
            {
                if (_isDragging)
                {
                    DrawSelectionBox(_dragStart, _mousePos);
                }

                DrawModeLabel(selectLabelGC);

                if (_overlapSprite != null)
                {
                    DrawMouseLabel(_overlapSprite.Name);
                }
            }
        }

        private void DrawGUI_Translate()
        {
            if (_ev.type == EventType.MouseDrag)
            {
                Vector2 delta = _ev.delta;
                delta.y = -delta.y;
                MoveSelection(delta / _zoom);
            }

            if (_ev.type == EventType.MouseUp)
            {
                MoveSelectionEnd();
                drawGUIA = DrawGUI_Selecting;
            }

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(translateLabelGC);
            }
        }

        private void DrawGUI_Create()
        {
            if (_ev.type == EventType.KeyUp)
            {
                if (!_ev.control)
                {
                    drawGUIA = DrawGUI_Selecting;
                    _requestRepaint = true;
                }
            }
            if (_ev.type == EventType.MouseUp)
            {
                if (_ev.button == 0)
                {
                    CreateSprite(_dragStart, _mousePos);
                    _requestRepaint = true;
                }
            }
            if (_ev.type == EventType.Repaint)
            {
                if (_isDragging)
                    DrawSelectionBox(_dragStart, _mousePos);
                DrawModeLabel(createLabelGC);
            }
        }

        private void DrawGUI_Edit()
        {
            if (_ev.type == EventType.KeyUp)
            {
                if (!_ev.control)
                {
                    shouldExitEditMode = true;
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.control)
                {
                    shouldExitEditMode = false;
                }
            }

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(editLabelGC);
            }

            if (Settings.editBorders)
            {
                bordersHandler.DrawGUI();
            }
            if (Settings.editRect)
            {
                spriteRectHandler.DrawGUI();
            }
            if (Settings.editPivot)
            {
                spritePivotHandler.DrawGUI();
            }

            if (shouldExitEditMode)
            {
                if (!bordersHandler.IsDragging
                    && !spriteRectHandler.IsDragging
                    && !spritePivotHandler.IsDragging)
                {
                    drawGUIA = DrawGUI_Selecting;
                    bordersHandler.Reset();
                    spriteRectHandler.Reset();
                    spritePivotHandler.Reset();
                    _requestRepaint = true;
                    shouldExitEditMode = false;
                }
            }
        }

        private void DrawGUI_Border()
        {
            if (_ev.type == EventType.KeyUp)
            {
                if (_ev.keyCode == KeyCode.B)
                {
                    shouldExitBorderMode = true;
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.keyCode == KeyCode.B)
                {
                    shouldExitBorderMode = false;
                }
            }

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(borderLabelGC);
            }

            bordersHandler.DrawGUI();

            if (shouldExitBorderMode)
            {
                if (!bordersHandler.IsDragging)
                {
                    drawGUIA = DrawGUI_Selecting;
                    bordersHandler.Reset();
                    _requestRepaint = true;
                    shouldExitBorderMode = false;
                }
            }
        }

        private void DrawGUI_Rect()
        {
            if (_ev.type == EventType.KeyUp)
            {
                if (_ev.keyCode == KeyCode.R)
                {
                    shouldExitRectMode = true;
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.keyCode == KeyCode.R)
                {
                    shouldExitRectMode = false;
                }
            }

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(rectLabelGC);
            }

            spriteRectHandler.DrawGUI();

            if (shouldExitRectMode)
            {
                if (!spriteRectHandler.IsDragging)
                {
                    drawGUIA = DrawGUI_Selecting;
                    bordersHandler.Reset();
                    _requestRepaint = true;
                    shouldExitRectMode = false;
                }
            }
        }

        private void DrawGUI_Pivot()
        {
            if (_ev.type == EventType.KeyUp)
            {
                if (_ev.keyCode == KeyCode.P)
                {
                    shouldExitPivotMode = true;
                }
            }

            if (_ev.type == EventType.KeyDown)
            {
                if (_ev.keyCode == KeyCode.P)
                {
                    shouldExitPivotMode = false;
                }
            }

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(pivotLabelGC);
            }

            spritePivotHandler.DrawGUI();

            if (shouldExitPivotMode)
            {
                if (!spritePivotHandler.IsDragging)
                {
                    drawGUIA = DrawGUI_Selecting;
                    bordersHandler.Reset();
                    _requestRepaint = true;
                    shouldExitPivotMode = false;
                }
            }
        }

        private void DrawGUI_Scale()
        {
            if (_ev.type == EventType.KeyUp
                && _ev.keyCode == KeyCode.S)
            {
                shouldExitScaleMode = true;
            }

            spriteScaleHandler.DrawGUI();

            if (_ev.type == EventType.Repaint)
            {
                DrawModeLabel(scaleLabelGC);
            }

            if (shouldExitScaleMode)
            {
                drawGUIA = DrawGUI_Selecting;
                spriteScaleHandler.Reset();
                _requestRepaint = true;
                shouldExitScaleMode = false;
            }
        }

        private void UpdateGlobals()
        {
            _ev = UnityEngine.Event.current;
            _requestRepaint = false;
            _handleMatrix = Handles.matrix;
            _mousePos = Handles.inverseMatrix.MultiplyPoint(_ev.mousePosition);
            _zeroPos = Handles.inverseMatrix.MultiplyPoint(Vector3.zero);
            _zoom = (float)m_zoomF.GetValue(spriteEditor);
            _scrollPosition = (Vector2)m_scrollPositionF.GetValue(spriteEditor);
            _zoomOnePixOffset = 1.0f / _zoom;
        }

        private void Save()
        {
            spriteEditor.ApplyOrRevertModification(true);
        }

        private void Copy()
        {
            data.CopySelection();
        }

        private void Paste(bool pasteUnderMouse)
        {
            if (pasteUnderMouse)
            {
                data.PasteSelection(_mousePos);
            }
            else
            {
                data.PasteSelection();
            }
        }

        private void CreateSprite(Vector2 start, Vector2 end)
        {
            Rect rect = VSUtils.PointsToRoundedRect(start, end);
            if (rect.width <= 0 || rect.height <= 0)
            {
                return;
            }

            IVSSpriteRect spr = data.CreateSprite(rect);
            data.Select(spr, true);
            spriteEditor.SetDataModified();
        }

        private void DeleteSelection()
        {
            data.DeleteSelection();
            spriteEditor.SetDataModified();
        }

        private void MoveSelection(Vector2Int amount)
        {
            if (amount.x == 0 && amount.y == 0)
            {
                return;
            }

            data.MoveSelection(amount);
            _moveSelectionExecutedAtLeastOnce = true;
            _requestRepaint = true;
            _requestWindowRepaint = true;
            spriteEditor.SetDataModified();
        }

        private void MoveSelection(Vector2 amount)
        {
            data.MoveSelection(amount);
            _moveSelectionExecutedAtLeastOnce = true;

            _requestRepaint = true;
            _requestWindowRepaint = true;
        }

        private void MoveSelectionEnd()
        {
            if (!_moveSelectionExecutedAtLeastOnce)
            {
                return;
            }

            data.RoundSelectionRects();
            _requestRepaint = true;
            _requestWindowRepaint = true;
            _moveSelectionExecutedAtLeastOnce = false;
            spriteEditor.SetDataModified();
        }

        private void SelectSprites(Vector2 start, Vector2 end)
        {
            Rect rect = VSUtils.PointsToRoundedRect(start, end);

            GetOverlapSprites(srBufL, rect);

            if (!UnityEngine.Event.current.shift)
            {
                data.ClearSelection();
            }
            for (int i = 0; i < srBufL.Count; i++)
            {
                data.Select(srBufL[i], true);
            }

        }

        private void SelectSprite(Vector2 pos)
        {
            GetOverlapSprites(srBufL, pos);

            if (!UnityEngine.Event.current.shift)
            {
                data.ClearSelection();
            }
            if (srBufL.Count > 0)
            {
                data.Select(srBufL[0], true);
            }
        }

        private void HighlightSprites(Vector2 start, Vector2 end)
        {
            Rect rect = VSUtils.PointsToRoundedRect(start, end);

            GetOverlapSprites(srBufL, rect);

            data.ClearHighlights();

            for (int i = 0; i < srBufL.Count; i++)
            {
                data.Highlight(srBufL[i], true);
            }
        }

        private void HighlightSprites(Vector2 pos)
        {
            GetOverlapSprites(srBufL, pos);

            data.ClearHighlights();

            for (int i = 0; i < srBufL.Count; i++)
            {
                data.Highlight(srBufL[i], true);
            }
        }

        private void HighlightClear()
        {
            data.ClearHighlights();
        }

        private void DrawModeLabel(GUIContent gc)
        {
            Vector3 pos = Handles.inverseMatrix.MultiplyPoint(_scrollPosition);
            pos.x += 4.0f * _zoomOnePixOffset;
            pos.y -= 4.0f * _zoomOnePixOffset;
            Handles.Label(pos, gc, blackLabelStyle);
            pos.x -= _zoomOnePixOffset;
            pos.y += _zoomOnePixOffset;
            Handles.Label(pos, gc, whiteLabelStyle);
        }

        private void DrawMouseLabel(string text)
        {
            Vector3 pos = _handleMatrix.inverse.MultiplyPoint(_ev.mousePosition + _mouseLabelOffset);
            Vector3 posBuf = pos;
            posBuf.x = pos.x + _zoomOnePixOffset; posBuf.y = pos.y + _zoomOnePixOffset; Handles.Label(posBuf, text, blackLabelStyle);
            posBuf.x = pos.x + _zoomOnePixOffset; posBuf.y = pos.y - _zoomOnePixOffset; Handles.Label(posBuf, text, blackLabelStyle);
            posBuf.x = pos.x - _zoomOnePixOffset; posBuf.y = pos.y - _zoomOnePixOffset; Handles.Label(posBuf, text, blackLabelStyle);
            posBuf.x = pos.x - _zoomOnePixOffset; posBuf.y = pos.y + _zoomOnePixOffset; Handles.Label(posBuf, text, blackLabelStyle);

            Handles.Label(pos, text, whiteLabelStyle);
        }

        private void DrawSelectionBox(Vector2 start, Vector2 end)
        {
            Handles.color = selectBoxShadeColor;
            VSUtils.DrawRoundedRectHandle(start, end, 1.0f / _zoom);
            Handles.color = selectBoxColor;
            VSUtils.DrawRoundedRectHandle(start, end);
        }

        private void DrawHighlights()
        {
            Handles.color = highlightShadeColor;
            for (int i = 0; i < data.highlightAmount; i++)
            {
                srBuf = data.GetHighlight(i);
                EditorGUI.DrawRect(RectToHandleSpace(srBuf.Rect), highlightFillColor);
                VSUtils.DrawRoundedRectHandle(srBuf.Rect, 1.0f / _zoom);
            }

            Handles.color = highlightColor;
            for (int i = 0; i < data.highlightAmount; i++)
            {
                srBuf = data.GetHighlight(i);
                VSUtils.DrawRoundedRectHandle(srBuf.Rect);
            }

            Handles.color = Color.white;

        }

        private void DrawFiltered()
        {
            if (!data.isFiltered) return;

            for (int i = 0; i < data.filteredAmount; i++)
            {
                srBuf = data.GetFiltered(i);
                EditorGUI.DrawRect(RectToHandleSpace(srBuf.Rect), filteredFillColor);
            }
        }

        private void DrawSelectedSprites()
        {
            Handles.color = selectShadeColor;
            for (int i = 0; i < data.selectAmount; i++)
            {
                VSUtils.DrawRoundedRectHandle(data.GetSelection(i).Rect, 1.0f / _zoom);
            }

            Handles.color = selectColor;
            for (int i = 0; i < data.selectAmount; i++)
            {
                VSUtils.DrawRoundedRectHandle(data.GetSelection(i).Rect);
            }

            Handles.color = Color.white;
        }

        private bool IsClickOnSelection(Vector2 pos)
        {
            for (int i = 0; i < data.selectAmount; i++)
            {
                if (data.GetSelection(i).Rect.Contains(pos))
                    return true;
            }
            return false;
        }

        private bool IsClickOnSprite(Vector2 pos)
        {
            for (int i = 0; i < data.spriteAmount; i++)
            {
                if (data.GetSprite(i).Rect.Contains(pos))
                    return true;
            }
            return false;
        }

        private void GetOverlapSprites(List<IVSSpriteRect> list, Rect rect)
        {
            list.Clear();

            for (int i = 0; i < data.spriteAmount; i++)
            {
                srBuf = data.GetSprite(i);
                if (srBuf.Rect.Overlaps(rect))
                {
                    list.Add(srBuf);
                }
            }
        }

        private void GetOverlapSprites(List<IVSSpriteRect> list, Vector2 pos)
        {

            list.Clear();
            _overlapSprite = null;
            for (int i = 0; i < data.spriteAmount; i++)
            {
                srBuf = data.GetSprite(i);
                if (srBuf.Rect.Contains(pos))
                {
                    list.Add(srBuf);
                    _overlapSprite = srBuf;
                }
            }
        }

        private Rect RectToHandleSpace(Rect rect)
        {
            rect.x = Mathf.Round((rect.x - _zeroPos.x) * _zoom);
            rect.y = Mathf.Round((_zeroPos.y - rect.y - rect.height) * _zoom);
            rect.width = rect.width * _zoom;
            rect.height = rect.height * _zoom;

            return rect;
        }

        private Vector2 VectorToHandleSpace(Vector2 vector)
        {
            vector.x = Mathf.Round((vector.x - _zeroPos.x) * _zoom);
            vector.y = Mathf.Round((_zeroPos.y - vector.y) * _zoom);

            return vector;
        }

        private void SetDataModified()
        {
            spriteEditor.SetDataModified();
        }

        //Slicing
        private Texture2D GetTextureToSlice()
        {
            ITextureDataProvider textureProvider = provider.GetDataProvider<ITextureDataProvider>();

            int width;
            int height;

            textureProvider.GetTextureActualWidthAndHeight(out width, out height);
            Texture2D readableTexture = textureProvider.GetReadableTexture2D();
            if (readableTexture == null
                || (readableTexture.width == width && readableTexture.height == height))
            {
                return readableTexture;
            }

            // we want to slice based on the original texture slice. Upscale the imported texture
            Texture2D texture = CreateTemporaryDuplicate(readableTexture, width, height);
            return texture;
        }

        private Rect[] GenerateGridSpriteRectangles(SliceSettings settings, Texture2D texture)
        {
            int selectAmount = data.selectAmount;

            if (selectAmount == 0)
            {
                return InternalSpriteUtility.GenerateGridSpriteRectangles(texture,
                                                                          settings.gridSpriteOffset,
                                                                          settings.gridSpriteSize,
                                                                          settings.gridSpritePadding,
                                                                          settings.keepEmptyRects);
            }
            else
            {
                List<Rect> rects = new List<Rect>();
                Vector2 pos;

                for (int i = 0; i < selectAmount; i++)
                {
                    Rect spriteRect = data.GetSelection(i).Rect;

                    int xSteps = Mathf.FloorToInt((spriteRect.width + settings.gridSpritePadding.x - settings.gridSpriteOffset.x) / (settings.gridSpriteSize.x + settings.gridSpritePadding.x));
                    int ySteps = Mathf.FloorToInt((spriteRect.height + settings.gridSpritePadding.y - settings.gridSpriteOffset.y) / (settings.gridSpriteSize.y + settings.gridSpritePadding.y));

                    for (int y = 0; y < ySteps; y++)
                    {
                        for (int x = 0; x < xSteps; x++)
                        {
                            pos.x = settings.gridSpriteOffset.x + spriteRect.x + x * settings.gridSpriteSize.x + x * settings.gridSpritePadding.x;
                            pos.y = settings.gridSpriteOffset.y + spriteRect.y + y * settings.gridSpriteSize.y + y * settings.gridSpritePadding.y;

                            rects.Add(new Rect(pos.x, pos.y, settings.gridSpriteSize.x, settings.gridSpriteSize.y));
                        }
                    }
                }

                return rects.ToArray();
            }
        }

        public void DoGridSlicing(SliceSettings settings)
        {
            Texture2D textureToUse = GetTextureToSlice();

            Rect[] frames;
            frames = GenerateGridSpriteRectangles(settings, textureToUse);

            if (settings.autoSlicingMethod == AutoSlicingMethodType.DeleteAll)
            {
                if (data.selectAmount == 0)
                {
                    data.DeleteAllSprites();
                }
                else
                {
                    data.DeleteSelection();
                }
            }

            int index = 0;

            foreach (Rect frame in frames)
            {
                AddSprite(frame, settings.spriteAlignment, settings.pivot, settings.autoSlicingMethod, ref index);
            }

            spriteEditor.SetDataModified();
            Repaint();
        }

        public void DoAutomaticSlicing(SliceSettings settings)
        {
            Texture2D textureToUse = GetTextureToSlice();

            List<Rect> frames = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles(textureToUse, settings.minimumSpriteSize, 0));

            ITextureDataProvider textureDataProvider = provider.GetDataProvider<ITextureDataProvider>();
            textureDataProvider.GetTextureActualWidthAndHeight(out int textureWidth, out _);

            if (data.selectAmount == 0)
            {
                if(settings.autoSlicingMethod == AutoSlicingMethodType.DeleteAll)
                {
                    data.DeleteAllSprites();
                }
            }
            else
            {
                ClampRectsToSelection(frames);
                data.DeleteSelection();
            }

            frames = VSUtils.SortRects(frames, textureWidth);

            int index = 0;

            foreach (Rect frame in frames)
            {
                AddSprite(frame, settings.spriteAlignment, settings.pivot, settings.autoSlicingMethod, ref index);
            }

            spriteEditor.SetDataModified();
            Repaint();
        }

        private void ClampRectsToSelection(List<Rect> rects)
        {
            int selectionAmount = data.selectAmount;
  
            bool intersectsAny;

            for(int x = 0; x < rects.Count; x++)
            {
                Rect rect = rects[x];
                intersectsAny = false;

                for (int y = 0; y < selectionAmount; y++)
                {
                    Rect selectionRect = data.GetSelection(y).Rect;

                    if (rect.Overlaps(selectionRect))
                    {
                        rects[x] = VSUtils.FitRectIntoAnother(rect, selectionRect);
                        intersectsAny = true;
                        break;
                    }
                }

                if (!intersectsAny)
                {
                    rects.RemoveAt(x);
                    x--;
                }
            }
        }

        private void AddSprite(Rect frame, SpriteAlignment alignment, Vector2 pivot, AutoSlicingMethodType slicingMethod, ref int index)
        {
            if (slicingMethod != AutoSlicingMethodType.DeleteAll)
            {
                IVSSpriteRect existingSprite = data.GetExistingOverlappingSprite(frame);
                if (existingSprite != null)
                {
                    if (slicingMethod == AutoSlicingMethodType.Smart)
                    {
                        existingSprite.Rect = frame;
                        existingSprite.SR.alignment = (SpriteAlignment)alignment;
                        existingSprite.SR.pivot = pivot;
                    }
                }
                else
                {
                    while (data.CreateSprite(frame, alignment, pivot, GenerateSpriteNameWithIndex(index++), Vector4.zero) == -1)
                    {

                    }
                }
            }
            else
            {
                while (data.CreateSprite(frame, alignment, pivot, GenerateSpriteNameWithIndex(index++), Vector4.zero) == -1)
                {

                }
            }
        }

        private string GenerateSpriteNameWithIndex(int index)
        {
            int originalLength = spriteNameStringBuilder.Length;
            spriteNameStringBuilder.Append(index);

            string name = spriteNameStringBuilder.ToString();
            spriteNameStringBuilder.Length = originalLength;
            return name;
        }

        //Events
        private void Data_OnConfigChangeE()
        {
            Repaint();
        }

        private void Data_OnFilteredChangeE()
        {
            Repaint();
        }

        private void Data_OnHighlightChangeE()
        {
            Repaint();
        }

        private void Data_OnSelectionChangeE()
        {
            Repaint();

        }

        private void Settings_OnValidateE()
        {
            VS.inst.window?.Repaint();
        }
    }
}
#endif

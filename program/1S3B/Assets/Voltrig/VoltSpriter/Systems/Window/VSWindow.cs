#if COM_UNITY_2D_SPRITE
using UnityEngine;
using UnityEditor;

#pragma warning disable 0649

namespace Voltrig.VoltSpriter
{
    public class VSWindow : EditorWindow
    {
        [SerializeField] 
        private Texture iconTexture;

        private VS vs;
        private VSStyle style;
        private VSWindowUI ui;

        private bool isInitialized => vs != null;

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            if (vs.style == null)
            {
                return;
            }
            vs.style.OnValidateE -= VSStyle_OnValidateE;

            vs.window = null;
        }

        private void OnGUI()
        {
            if (!isInitialized)
            {
                Init();
            }

            if (!IsStyleValid())
            {
                return;
            }

            //Event currentEvent = new Event(Event.current);

            if (vs.module == null)
            {
                NoModuleGUI();
            }
            else
            {
                NormalGUI();
            }

            //if(currentEvent.type != EventType.Layout && currentEvent.type != EventType.Repaint && currentEvent.type != EventType.MouseMove)
            //{
            //    Debug.Log($"Event: CUR: type:{currentEvent.type} key:{currentEvent.keyCode} com:{currentEvent.commandName} AFTER: type:{Event.current.type} key:{Event.current.keyCode} com:{currentEvent.commandName}");
            //}
        }

        [MenuItem("Window/Voltrig/Volt Spriter")]
        internal static void OpenWindow()
        {
            VSWindow window = EditorWindow.GetWindow<VSWindow>("Volt Spriter");
            window.Init();
            window.InitIcon();
        }

        private void VSStyle_OnValidateE()
        {
            Repaint();
        }

        private void InitIcon() 
        {
            if(iconTexture != null)
            {
                titleContent = new GUIContent("Volt Spriter", iconTexture);
            }
            else
            {
                titleContent = new GUIContent("Volt Spriter");
            }
        }

        private void Init()
        {
            if (isInitialized)
            {
                return;
            }

            vs = VS.inst;

            style = VS.inst.style;
            if(style == null)
            {
                Debug.LogError(VSErrors.INSTALLATION_ERROR);
                return;
            }

            wantsMouseMove = true;

            vs.style.OnValidateE += VSStyle_OnValidateE;
            vs.OnModuleChangeE += VS_OnModuleChangeE;
            vs.window = this;

            ui = new VSWindowUI(this);
             
            VS_OnModuleChangeE(vs.module);
        }

        private void NoModuleGUI() 
        {
            GUILayout.Label("Please open Sprite editor with Volt Spriter to use this window.");
        }

        private void NormalGUI() 
        {
            Rect rect = position;
            rect.x = 0.0f;
            rect.y = 0.0f;

            //Apply padding
            rect.x += 2.0f;
            rect.y += 2.0f;
            rect.width -= 4.0f;
            rect.height -= 4.0f;

            ui.ShowGUI(rect);
        }

        private bool IsStyleValid() 
        {
            if(vs.style == null)
            {
                Rect rect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
                GUI.Label(rect, "Style is corruped. To fix please reinstall the asset.");
                return false;
            }
            return true;
        }

        private void VS_OnModuleChangeE(VSModule module)
        {
            if (module != null)
            {
                ui.data = module.data;
            }
            else
            {
                ui.data = null;
            }

            Repaint();
        }
    }
}
#endif

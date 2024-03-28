using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
namespace Voltrig.VoltSpriter
{
    internal partial class VSStyle : ScriptableObject //VSStyleEditor
    {
        [CustomEditor(typeof(VSStyle))]
        public class VSStyleEditor : Editor
        {
            private VSStyle style;
            private SerializedProperty normSP;
            private GUIContent normGC;

            private void OnEnable() 
            {
                style = target as VSStyle;

                normSP = serializedObject.FindProperty("norm");

                normGC = new GUIContent("Normal");
            }

            public override void OnInspectorGUI() 
            {
                serializedObject.Update();
                
                EditorGUILayout.PropertyField(normSP, normGC , true);

                if(GUILayout.Button("Reset to default")) 
                {
                    Undo.RecordObject(style, "Reset VSStyle to default");
                    style.norm.ResetToDefault();
                    style.OnValidate();
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}


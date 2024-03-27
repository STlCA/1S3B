using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Voltrig.VoltSpriter
{
    public partial class VSSpritePanelSettings // PropertyDrawer
    {
        const int fieldsAmount = 13;

        [CustomPropertyDrawer(typeof(VSSpritePanelSettings))]
        public class PropertyDrawer : UnityEditor.PropertyDrawer
        {
            private Dictionary<int, GUIContent> nameHashToGUIContent = new Dictionary<int, GUIContent>();

            private GUIContent minWidthGC;
            private GUIContent isElasticGC;
            private GUIContent isVisibleGC;
            private GUIContent useIconGC;
            private GUIContent shortNameGC;
            private GUIContent longNameGC;
            private GUIContent iconGC;

            private const int minWidthSize = 40;
            private const int isElasticSize = 40;
            private const int isVisibleSize = 40;
            private const int useIconSize = 50;
            private const int shortNameSize = 70;
            private const int longNameSize = 80;
            private const int iconSize = 120;

            private const int padding = 5;

            private const string pivotSettingsName = "pivotSettings";
            private const string vsSpritePanelColumnSettingsName = "VSSpritePanelColumnSettings";

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight * (fieldsAmount + 3) //1 is for the main label, 2 for padding
                    + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(pivotSettingsName)); 
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                Rect labelsRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                labelsRect.height = EditorGUIUtility.singleLineHeight;
                labelsRect.y = labelsRect.y + EditorGUIUtility.singleLineHeight;

                Rect propertyRect = new Rect(position.x, labelsRect.y, position.width, EditorGUIUtility.singleLineHeight);

                //Initialize GCs if needed
                if (minWidthGC == null)
                {
                    minWidthGC = new GUIContent("Width", "Minimum width that this column will allows itself to be resized to.");
                    isElasticGC = new GUIContent("Elastic", "Is this column automatically resizing to fit the available space?");
                    isVisibleGC = new GUIContent("Visible", "Is this column visible?");
                    useIconGC = new GUIContent("Use Icon", "Should the icon be used instead of text?");
                    shortNameGC = new GUIContent("Short Name", "Short name of the property for display.");
                    longNameGC = new GUIContent("Long Name", "Long name of the property for display.");
                    iconGC = new GUIContent("Icon", "Icon to display for this property.");
                }

                //Display pivot icons
                SerializedProperty pivotSettingsSP = property.FindPropertyRelative(pivotSettingsName);
                EditorGUI.PropertyField(propertyRect, pivotSettingsSP, true);
                float height = EditorGUI.GetPropertyHeight(pivotSettingsSP);
                propertyRect.y += height + EditorGUIUtility.singleLineHeight;
                labelsRect.y += height + EditorGUIUtility.singleLineHeight;

                //Display labels
                labelsRect = DrawLabel(labelsRect, minWidthSize, minWidthGC);
                labelsRect = DrawLabel(labelsRect, isElasticSize, isElasticGC);
                labelsRect = DrawLabel(labelsRect, isVisibleSize, isVisibleGC);
                labelsRect = DrawLabel(labelsRect, useIconSize, useIconGC);
                labelsRect = DrawLabel(labelsRect, shortNameSize, shortNameGC);
                labelsRect = DrawLabel(labelsRect, longNameSize, longNameGC);
                labelsRect = DrawLabel(labelsRect, iconSize, iconGC);

                // Don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;

                property.Next(true);

                propertyRect.y += EditorGUIUtility.singleLineHeight;

                do
                {
                    if (property.type.Equals(vsSpritePanelColumnSettingsName))
                    {
                        DrawRowProperty(propertyRect, property);
                        propertyRect.y += EditorGUIUtility.singleLineHeight;
                    }
                }
                while (property.Next(false));

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Rect DrawLabel(Rect rect, float size, GUIContent labelGC)
            {
                Rect drawRect = new Rect(rect.x, rect.y, size, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(drawRect, labelGC);
                rect.x = rect.x + size + padding;
                return rect;
            }

            private void DrawRowProperty(Rect position, SerializedProperty property)
            {
                GUIContent label = GetOrCreateGCForProperty(property);

                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Calculate rects
                position = DrawProperty(position, "minColumnWidth", minWidthSize,   property);
                position = DrawProperty(position, "isElastic",      isElasticSize,  property);
                position = DrawProperty(position, "isVisible",      isVisibleSize,  property);
                position = DrawProperty(position, "useIcon",        useIconSize,    property);
                position = DrawProperty(position, "shortName",      shortNameSize,  property);
                position = DrawProperty(position, "longName",       longNameSize,   property);
                position = DrawProperty(position, "icon",           iconSize,       property);

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Rect DrawProperty(Rect rect, string propertyName, float size, SerializedProperty property)
            {
                Rect drawRect = new Rect(rect.x, rect.y, size, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(drawRect, property.FindPropertyRelative(propertyName), GUIContent.none);
                rect.x = rect.x + size + padding;
                return rect;
            }

            private GUIContent GetOrCreateGCForProperty(SerializedProperty property)
            {
                GUIContent gc;

                if (nameHashToGUIContent.TryGetValue(property.displayName.GetHashCode(), out gc))
                {
                    return gc;
                }
                else
                {
                    VSConsole.Log(this, "Creating new guiContent");
                    gc = new GUIContent(property.displayName);

                    nameHashToGUIContent[property.displayName.GetHashCode()] = gc;

                    return gc;
                }
            }
        }
    }
}
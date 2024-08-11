using UnityEditor;
using UnityEngine;

namespace LightBuzz.HandTracking.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            SerializedProperty sourceProp = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);

            if (sourceProp != null)
            {
                return sourceProp.boolValue != condHAtt.hideInInspector;
            }

            // No matching property found, always draw
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                // The property is not being drawn
                // We want to undo the spacing added by the Unity's layout system
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
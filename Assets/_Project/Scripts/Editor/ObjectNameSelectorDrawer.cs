using ControllThemAll.Runtime.Shared;
using UnityEditor;
using UnityEngine;

namespace ControllThemAll.EditorExtensions
{
    [CustomPropertyDrawer(typeof(ObjectNameSelectorAttribute))]
    public class ObjectNameSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, $"Use {typeof(ObjectNameSelectorAttribute)} with string fields only");
                return;
            }

            ObjectNameSelectorAttribute objectNameSelectorAttribute = attribute as ObjectNameSelectorAttribute;
            Object currentObject = null;

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                currentObject = FindObjectByName(objectNameSelectorAttribute.Type, property.stringValue);
            }

            EditorGUI.BeginChangeCheck();
            Object selectedObject = EditorGUI.ObjectField(
                position,
                label,
                currentObject,
                objectNameSelectorAttribute.Type,
                false);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = selectedObject != null ? selectedObject.name : string.Empty;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }


        private Object FindObjectByName(System.Type type, string name)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, type);

                if (asset != null && asset.name == name)
                {
                    return asset;
                }
            }

            return null;
        }
    }
}
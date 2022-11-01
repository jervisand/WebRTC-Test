using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nagih
{
    [CustomEditor(typeof(EventSystemToggle))]
    public class EventSystemToggleEditor : Editor
    {
        public SerializedProperty ActiveProperty;
        public SerializedProperty DeactiveProperty;

        private ReorderableList _relatedSelectables;

        private void OnEnable()
        {
            ActiveProperty = serializedObject.FindProperty("Active");
            DeactiveProperty = serializedObject.FindProperty("Deactive");
            DrawRelatedSelectables();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(ActiveProperty);
            EditorGUILayout.PropertyField(DeactiveProperty);
            EditorGUILayout.Space();

            _relatedSelectables.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRelatedSelectables()
        {
            _relatedSelectables = new ReorderableList(serializedObject, serializedObject.FindProperty("RelatedSelectables"), true, true, true, true);
            _relatedSelectables.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(UtilEditor.CalculateColumn(rect, 1, 2, 15, 0), "Selectable");
                EditorGUI.LabelField(UtilEditor.CalculateColumn(rect, 2, 2, 20, 0), "Direction");
            };

            _relatedSelectables.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _relatedSelectables.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(UtilEditor.CalculateColumn(rect, 1, 2, 0, 0), element.FindPropertyRelative("Selectable"), GUIContent.none);
                EditorGUI.PropertyField(UtilEditor.CalculateColumn(rect, 2, 2, 10, 10), element.FindPropertyRelative("Direction"), GUIContent.none);
            };
        }
    }
}
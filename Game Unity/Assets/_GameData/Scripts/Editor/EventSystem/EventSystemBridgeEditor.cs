using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nagih
{
    [CustomEditor(typeof(EventSystemBridge))]
    public class EventSystemBridgeEditor : Editor
    {
        public SerializedProperty SelectableProperty;

        private ReorderableList _bridges;

        private void OnEnable()
        {
            SelectableProperty = serializedObject.FindProperty("Selectable");
            DrawBridges();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UtilEditor.DrawSpaceGUI(1);

            EditorGUILayout.PropertyField(SelectableProperty);
            UtilEditor.DrawSpaceGUI(1);

            _bridges.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBridges()
        {
            _bridges = new ReorderableList(serializedObject, serializedObject.FindProperty("Bridges"), true, true, true, true);
            _bridges.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(UtilEditor.CalculateColumn(rect, 1, 3, 15, 0), "Direction");
                EditorGUI.LabelField(UtilEditor.CalculateColumn(rect, 2, 3, 20, 0), "From");
                EditorGUI.LabelField(UtilEditor.CalculateColumn(rect, 3, 3, 15, 0), "To");
            };

            _bridges.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _bridges.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(UtilEditor.CalculateColumn(rect, 1, 3, 0, 0), element.FindPropertyRelative("Direction"), GUIContent.none);
                EditorGUI.PropertyField(UtilEditor.CalculateColumn(rect, 2, 3, 10, 10), element.FindPropertyRelative("From"), GUIContent.none);
                EditorGUI.PropertyField(UtilEditor.CalculateColumn(rect, 3, 3, 10, 10), element.FindPropertyRelative("To"), GUIContent.none);
            };
        }
    }
}
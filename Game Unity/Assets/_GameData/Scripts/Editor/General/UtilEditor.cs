using UnityEditor;
using UnityEngine;

namespace Nagih
{
    public class UtilEditor : Editor
    {
        public static Rect CalculateColumn(Rect rect, int columnNumber, int maxColumn, float xPadding, float xWidth)
        {
            float xPosition = rect.x + xPadding + (rect.width * (columnNumber - 1) / maxColumn);
            return new Rect(xPosition, rect.y, rect.width / maxColumn - xWidth, EditorGUIUtility.singleLineHeight);
        }

        public static void DrawSpaceGUI(int amountOfSpace)
        {
            for (int i = 0; i < amountOfSpace; i++)
            {
                EditorGUILayout.Space();
            }
        }
    }
}
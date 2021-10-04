using UnityEditor;
using UnityEngine;

namespace RedDev.Helpers
{
    public static class GizmosExtensions
    {

        public static void DrawText(Vector3 position, Color color, int fontSize, FontStyle fontStyle, string text)
        {
#if UNITY_EDITOR
            GUIContent textContent = new GUIContent(text);

            GUIStyle style = new GUIStyle();
            if (color != null)
                style.normal.textColor = color;
            if (fontSize > 0)
                style.fontSize = fontSize;
            style.fontStyle = fontStyle;

            Vector2 textSize = style.CalcSize(textContent);
            Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);

            // Проверка, если камера смотрит в противоположную сторону - текст не должен быть виден.
            if (screenPoint.z > 0)
            {
                var worldPosition =
                    Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f,
                        screenPoint.y + textSize.y * 0.5f, screenPoint.z));
                Handles.Label(worldPosition, textContent, style);
            }
#endif
        }

    }
}
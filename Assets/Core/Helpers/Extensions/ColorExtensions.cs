using UnityEngine;
using UnityEngine.UI;

namespace RedDev.Helpers.ColorExtensions {
    public static class ColorExtensions {
        public static bool EqualsOpaque(this Color color, Color other) {
            return (color.r == other.r) && (color.g == other.g) && (color.b == other.b);
        }

        public static string ToHexString(this Color color, bool withGrid = true) {
            var colorString = ColorUtility.ToHtmlStringRGBA(color);
            return withGrid ? "#" + colorString : colorString;
        }

        public static Color WithAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        public static Color WithB(this Color color, float b) {
            color.b = b;
            return color;
        }

        public static Color WithG(this Color color, float g) {
            color.r = g;
            return color;
        }

        public static Color WithR(this Color color, float r) {
            color.r = r;
            return color;
        }
    }

    public static class GraphicColorSetters {
        public static void SetAlpha(this Graphic graphic, float alpha) {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        public static void SetColorOnly(this Graphic graphic, Color color) {
            color.a = graphic.color.a;
            graphic.color = color;
        }
    }

    public static class SpriteRendererColorSetters {
        public static void SetAlpha(this SpriteRenderer renderer, float alpha) {
            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }

        public static void SetColorOnly(this SpriteRenderer renderer, Color color) {
            color.a = renderer.color.a;
            renderer.color = color;
        }
    }

    public static class StringColorFormatter {
        public static string GetColored(object obj, string color) {
            var formatString = color.Contains("#") ? ColoredTextFormat : ColoredTextFormatWithGrid;
            return string.Format(formatString, obj, color);
        }

        public static string GetColored(object obj, Color color) {
            var colorString = color.ToHexString();
            return string.Format(ColoredTextFormat, obj, colorString);
        }

        public static string SetColor(this string text, string color) {
            return GetColored(text, color);
        }

        public static string SetColor(this string text, Color color) {
            return GetColored(text, color);
        }

        private const string ColoredTextFormat = "<color={1}>{0}</color>";
        private const string ColoredTextFormatWithGrid = "<color=#{1}>{0}</color>";
    }
}
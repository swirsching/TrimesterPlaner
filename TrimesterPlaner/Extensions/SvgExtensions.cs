﻿using Svg.Transforms;
using Svg;

namespace TrimesterPlaner.Extensions
{
    public static class SvgExtensions
    {
        public static T Translate<T>(this T transformable, float x, float y) where T : ISvgTransformable
        {
            transformable.Transforms ??= [];
            transformable.Transforms.Add(new SvgTranslate(x, y));
            return transformable;
        }

        public static T Rotate<T>(this T transformable, float angle, float centerX = 0, float centerY = 0) where T : ISvgTransformable
        {
            transformable.Transforms ??= [];
            transformable.Transforms.Add(new SvgRotate(angle, centerX, centerY));
            return transformable;
        }

        public static bool IsShortEnough(this string text, string fontFamily, int fontSize, int? maxWidth)
        {
            if (maxWidth is null)
            {
                return true;
            }

            return new SvgText(text)
            {
                FontFamily = fontFamily,
                FontSize = fontSize,
            }.Bounds.Width < maxWidth;
        }

        public static string Shorten(this string text, string fontFamily, int fontSize, int? maxWidth)
        {
            if (text.IsShortEnough(fontFamily, fontSize, maxWidth))
            {
                return text;
            }

            return text[..^1].Shorten(fontFamily, fontSize, maxWidth);
        }
    }
}
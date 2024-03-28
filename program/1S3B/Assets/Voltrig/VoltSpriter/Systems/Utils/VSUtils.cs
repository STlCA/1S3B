using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Voltrig.VoltSpriter 
{
    internal static class VSUtils 
    {
        private static Rect rectBuf;
        private static Vector3 [] vecBuf4 = new Vector3[4];
        private static readonly int [] rectIndicesBuf4 = new int[]{0, 1, 1, 2, 2, 3, 3, 0};
        private static Vector4 vec4Buf;
        private static readonly SpriteAlignment[] indexToAlignment = new SpriteAlignment[]
        {
            SpriteAlignment.TopLeft,
            SpriteAlignment.TopCenter,
            SpriteAlignment.TopRight,
            SpriteAlignment.LeftCenter,
            SpriteAlignment.Center,
            SpriteAlignment.RightCenter,
            SpriteAlignment.BottomLeft,
            SpriteAlignment.BottomCenter,
            SpriteAlignment.BottomRight
        };
        private const int indexToAlignmentWidth = 3;
        
        public static Rect PointsToRect(Vector2 start, Vector2 end) 
        {
            if(start.x > end.x) 
            {
                float buf = start.x;
                start.x = end.x;
                end.x = buf;
            }
            if(start.y > end.y) 
            {
                float buf = start.y;
                start.y = end.y;
                end.y = buf;
            }

            return new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
        }

        public static Rect PointsToRoundedRect(Vector2 start, Vector2 end) 
        {
            start.x = Mathf.Round(start.x);
            start.y = Mathf.Round(start.y);
            end.x = Mathf.Round(end.x);
            end.y = Mathf.Round(end.y);

            if(start.x > end.x) 
            {
                float buf = start.x;
                start.x = end.x;
                end.x = buf;
            }

            if(start.y > end.y) 
            {
                float buf = start.y;
                start.y = end.y;
                end.y = buf;
            }

            return new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
        }

        public static void DrawRectHandle(Rect rect) 
        {
            vecBuf4[0].Set(rect.xMin, rect.yMin, 0.0f);
            vecBuf4[1].Set(rect.xMax, rect.yMin, 0.0f);
            vecBuf4[2].Set(rect.xMax, rect.yMax, 0.0f);
            vecBuf4[3].Set(rect.xMin, rect.yMax, 0.0f);
            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static void DrawRectHandle(Vector2 start, Vector2 end) 
        {
            vecBuf4[0].Set(start.x, start.y, 0.0f);
            vecBuf4[1].Set(end.x, start.y, 0.0f);
            vecBuf4[2].Set(end.x, end.y, 0.0f);
            vecBuf4[3].Set(start.x, end.y, 0.0f);

            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static void DrawRoundedRectHandle(Vector2 start, Vector2 end) 
        {
            start.x = Mathf.Round(start.x);
            start.y = Mathf.Round(start.y);
            end.x = Mathf.Round(end.x);
            end.y = Mathf.Round(end.y);

            vecBuf4[0].Set(start.x, start.y, 0.0f);
            vecBuf4[1].Set(end.x, start.y, 0.0f);
            vecBuf4[2].Set(end.x, end.y, 0.0f);
            vecBuf4[3].Set(start.x, end.y, 0.0f);

            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static void DrawRoundedRectHandle(Vector2 start, Vector2 end, float offset) 
        {
            start.x = Mathf.Round(start.x) + offset;
            start.y = Mathf.Round(start.y) + offset;
            end.x = Mathf.Round(end.x) + offset;
            end.y = Mathf.Round(end.y) + offset;

            vecBuf4[0].Set(start.x, start.y, 0.0f);
            vecBuf4[1].Set(end.x, start.y, 0.0f);
            vecBuf4[2].Set(end.x, end.y, 0.0f);
            vecBuf4[3].Set(start.x, end.y, 0.0f);

            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static void DrawRoundedRectHandle(Rect rect) 
        {
            rect.x = Mathf.Round(rect.x);
            rect.y = Mathf.Round(rect.y);

            vecBuf4[0].Set(rect.xMin, rect.yMin, 0.0f);
            vecBuf4[1].Set(rect.xMax, rect.yMin, 0.0f);
            vecBuf4[2].Set(rect.xMax, rect.yMax, 0.0f);
            vecBuf4[3].Set(rect.xMin, rect.yMax, 0.0f);

            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static void DrawRoundedRectHandle(Rect rect, float offset) 
        {
            rect.x = Mathf.Round(rect.x) + offset;
            rect.y = Mathf.Round(rect.y) + offset;
           
            vecBuf4[0].Set(rect.xMin, rect.yMin, 0.0f);
            vecBuf4[1].Set(rect.xMax, rect.yMin, 0.0f);
            vecBuf4[2].Set(rect.xMax, rect.yMax, 0.0f);
            vecBuf4[3].Set(rect.xMin, rect.yMax, 0.0f);

            Handles.DrawLines(vecBuf4, rectIndicesBuf4);
        }

        public static Vector2 PivotToRectPosition(Rect rect, Vector2 pivot)
        {
            Vector2 difference = rect.size;
            Vector2 position = new Vector2(rect.xMin + difference.x * pivot.x,
                                           rect.yMin + difference.y * pivot.y);

            return position;
        }

        public static Vector2 RectPositionToPivot(Rect rect, Vector2 position)
        {
            Vector2 difference = rect.size;
            Vector2 pivot = new Vector2((position.x - rect.xMin) / difference.x, 
                                        (position.y - rect.yMin) / difference.y);

            return pivot;
        }

        public static SpriteAlignment PivotToAlignment(ref Vector2 pivot)
        {
            //Convert [0 1] to [0 2] space
            pivot = pivot * 2.0f;

            int xID = Mathf.RoundToInt(pivot.x);
            int yID = 2 - Mathf.RoundToInt(pivot.y);

            xID = Mathf.Clamp(xID, 0, 2);
            yID = Mathf.Clamp(yID, 0, 2);
            pivot.x = xID * 0.5f;
            pivot.y = 1 - yID * 0.5f;

            return indexToAlignment[xID + yID * indexToAlignmentWidth];
        }

        public static Vector2 AlignmentToPivot(SpriteAlignment alignment, Vector2 customOffset)
        {
            switch (alignment)
            {
                case SpriteAlignment.TopLeft:
                    return new Vector2(0f, 1f);
                case SpriteAlignment.TopCenter:
                    return new Vector2(0.5f, 1f);
                case SpriteAlignment.TopRight:
                    return new Vector2(1f, 1f);
                case SpriteAlignment.LeftCenter:
                    return new Vector2(0f, 0.5f);
                case SpriteAlignment.Center:
                    return new Vector2(0.5f, 0.5f);
                case SpriteAlignment.RightCenter:
                    return new Vector2(1f, 0.5f);
                case SpriteAlignment.BottomLeft:
                    return new Vector2(0f, 0f);
                case SpriteAlignment.BottomCenter:
                    return new Vector2(0.5f, 0f);
                case SpriteAlignment.BottomRight:
                    return new Vector2(1f, 0f);
                case SpriteAlignment.Custom:
                    return customOffset;
            }

            return Vector2.zero;
        }

        public static int RectCompareByX(Rect r1, Rect r2)
        {
            return r1.x.CompareTo(r2.x);
        }

        public static int RectCompareByXThenY(Rect r1, Rect r2)
        {
            return (int)((r2.y - r1.y) * 10000 + r1.x - r2.x);
        }

        public static int RectCompareByYThenX(Rect r1, Rect r2)
        {
            return (int)((r1.x - r2.x) * 10000 + r2.y - r1.y);
        }

        public static List<Rect> SortRects(List<Rect> rects, int textureWidth)
        {
            List<Rect> result = new List<Rect>();

            while (rects.Count > 0)
            {
                Rect r = rects[rects.Count - 1];
                Rect sweepRect = new Rect(0, r.yMin, textureWidth, r.height);

                List<Rect> rowRects = RectSweep(rects, sweepRect);

                if (rowRects.Count > 0)
                {
                    result.AddRange(rowRects);
                }
                else
                {
                    result.AddRange(rects);
                    break;
                }
            }

            return result;
        }

        public static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
        {
            if (rects == null || rects.Count == 0)
            {
                return new List<Rect>();
            }

            List<Rect> containedRects = new List<Rect>();

            foreach (Rect rect in rects)
            {
                if (rect.Overlaps(sweepRect))
                {
                    containedRects.Add(rect);
                }
            }

            foreach (Rect rect in containedRects)
            {
                rects.Remove(rect);
            }

            containedRects.Sort(RectCompareByX);

            return containedRects;
        }

        public static Rect FitRectIntoTexture(Rect rect, Texture2D texture)
        {
            if (rect.xMin < 0)
            {
                rect.xMin = 0;
            }

            if (rect.xMax > texture.width)
            {
                rect.xMax = texture.width;
            }

            if (rect.yMin < 0)
            {
                rect.yMin = 0;
            }

            if (rect.yMax > texture.height)
            {
                rect.yMax = texture.height;
            }

            return rect;
        }

        public static Rect FitRectIntoAnother(Rect rect, Rect another)
        {
            if(rect.xMin < another.xMin)
            {
                rect.xMin = another.xMin;
            }

            if(rect.xMax > another.xMax)
            {
                rect.xMax = another.xMax;
            }

            if(rect.yMin < another.yMin)
            {
                rect.yMin = another.yMin;
            }

            if(rect.yMax > another.yMax)
            {
                rect.yMax = another.yMax;
            }

            return rect;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another 
        /// specified string according the type of search to use for the specified string.
        /// </summary>
        /// <param name="str">The string performing the replace method.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string replace all occurrences of <paramref name="oldValue"/>. 
        /// If value is equal to <c>null</c>, than all occurrences of <paramref name="oldValue"/> will be removed from the <paramref name="str"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. 
        /// If <paramref name="oldValue"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparisonType)
        {
            // Check inputs.
            if (str == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(str));
            }
            if (str.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return str;
            }
            if (oldValue == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (oldValue.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length.");
            }

            // Prepare string builder for storing the processed string.
            // Note: StringBuilder has a better performance than String by 30-40%.
            StringBuilder resultStringBuilder = new StringBuilder(str.Length);

            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(newValue);

            // Replace all values.
            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
            {
                // Append all characters until the found replacement.
                int charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append(str, startSearchFromIndex, charsUntilReplacment);
                }

                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append(newValue);
                }

                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }

            // Append the last part to the result.
            int @charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilStringEnd);

            return resultStringBuilder.ToString();
        }
    }
}

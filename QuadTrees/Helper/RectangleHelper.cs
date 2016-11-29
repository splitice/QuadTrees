using System;
using System.Drawing;

namespace QuadTrees.Helper
{
    internal static class RectangleHelper
    {
        public static bool Intersects(this RectangleF a, RectangleF b)
        {
            if (b.X < a.Right && a.X < b.Right && b.Y < a.Bottom)
                return a.Y < b.Bottom;
            return false;
        }
        public static bool Intersects(this Rectangle a, Rectangle b)
        {
            if (b.X < a.Right && a.X < b.Right && b.Y < a.Bottom)
                return a.Y < b.Bottom;
            return false;
        }
    }
}
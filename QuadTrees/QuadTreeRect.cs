﻿using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRect;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRect<T> : QuadTreeCommon<T, QuadTreeRectNode<T, Rectangle>, Rectangle> where T : IRectQuadStorable
    {
        public QuadTreeRect(Rectangle rect) : base(rect)
        {
        }

        public QuadTreeRect(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public QuadTreeRect()
        {
        }

        protected override QuadTreeRectNode<T, Rectangle> CreateNode(Rectangle rect)
        {
            System.Diagnostics.Debug.Assert(int.MaxValue > (rect.Width * rect.Height) , "Node rectangle area datatype capacity");
            return new QuadTreeRectNode<T>(rect);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using QuadTrees.Common;
using QuadTrees.QTreePoint;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Points in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePoint<T> : QuadTreeCommon<T, QuadTreePointNode<T>, Rectangle> where T : IPointQuadStorable
    {
        public QuadTreePoint(Rectangle rect)
            : base(rect)
        {
        }

        public QuadTreePoint(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public QuadTreePoint()
            : base()
        {

        }

        protected override QuadTreePointNode<T> CreateNode(Rectangle rect)
        {
            System.Diagnostics.Debug.Assert(int.MaxValue > (rect.Width * rect.Height), "Node rectangle area datatype capacity");
            return new QuadTreePointNode<T>(rect);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using QuadTrees.Common;
using QuadTrees.QTreePointF;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Points in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePointF<T> : QuadTreeFCommon<T, QuadTreePointFNode<T>, RectangleF> where T : IPointFQuadStorable
    {
        public QuadTreePointF(RectangleF rect)
            : base(rect)
        {
        }

        public QuadTreePointF(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        protected override QuadTreePointFNode<T> CreateNode(RectangleF rect)
        {
            return new QuadTreePointFNode<T>(rect);
        }
    }
}

﻿using System;
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

        public QuadTreePointF()
            : base()
        {

        }

        protected override QuadTreePointFNode<T> CreateNode(RectangleF rect)
        {
            System.Diagnostics.Debug.Assert(!float.IsInfinity(rect.Width * rect.Height), "Node rectangle area datatype capacity");
            return new QuadTreePointFNode<T>(rect);
        }
    }
}

using System.Drawing;
using QuadTrees.Common;
using QuadTrees.Helper;

namespace QuadTrees.QTreeRectF
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public abstract class QuadTreeRectNode<T, TQuery> : QuadTreeFNodeCommon<T, QuadTreeRectNode<T, TQuery>, TQuery> where T : IRectFQuadStorable
    {
        public QuadTreeRectNode(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectNode(QuadTreeRectNode<T, TQuery> parent, RectangleF rect) : base(parent, rect)
        {
        }

        protected override bool CheckContains(RectangleF rectangleF, T data)
        {
            return rectangleF.Contains(data.Rect);
        }
    }

    public class QuadTreeRectFNode<T> : QuadTreeRectNode<T, RectangleF> where T : IRectFQuadStorable
    {
        public QuadTreeRectFNode(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectFNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectFNode(QuadTreeRectFNode<T> parent, RectangleF rect) : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, RectangleF> CreateNode(RectangleF rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreeRectFNode<T>(this, rectangleF);
        }

        protected override bool CheckIntersects(RectangleF searchRect, T data)
        {
            return searchRect.Intersects(data.Rect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, RectangleF>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(RectangleF search, RectangleF rect)
        {
            return search.Contains(rect);
        }

        protected override bool QueryIntersects(RectangleF search, RectangleF rect)
        {
            return search.Intersects(rect);
        }
        protected override PointF GetMortonPoint(T p)
        {
            return p.Rect.Location;//todo: center?
        }
    }
}
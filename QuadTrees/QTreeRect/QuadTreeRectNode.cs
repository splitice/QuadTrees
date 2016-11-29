using System.Drawing;
using QuadTrees.Common;
using QuadTrees.Helper;

namespace QuadTrees.QTreeRect
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public abstract class QuadTreeRectNode<T, TQuery> : QuadTreeNodeCommon<T, QuadTreeRectNode<T, TQuery>, TQuery> where T : IRectQuadStorable
    {
        public QuadTreeRectNode(Rectangle rect) : base(rect)
        {
        }

        public QuadTreeRectNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectNode(QuadTreeRectNode<T, TQuery> parent, Rectangle rect) : base(parent, rect)
        {
        }

        protected override bool CheckContains(Rectangle rectangle, T data)
        {
            return rectangle.Contains(data.Rect);
        }
    }

    public class QuadTreeRectNode<T> : QuadTreeRectNode<T, Rectangle> where T : IRectQuadStorable
    {
        public QuadTreeRectNode(Rectangle rect) : base(rect)
        {
        }

        public QuadTreeRectNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectNode(QuadTreeRectNode<T> parent, Rectangle rect) : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, Rectangle> CreateNode(Rectangle Rectangle)
        {
            VerifyNodeAssertions(Rectangle);
            return new QuadTreeRectNode<T>(this, Rectangle);
        }

        protected override bool CheckIntersects(Rectangle searchRect, T data)
        {
            return searchRect.IntersectsWith(data.Rect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, Rectangle>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(Rectangle search, Rectangle rect)
        {
            return search.Contains(rect);
        }

        protected override bool QueryIntersects(Rectangle search, Rectangle rect)
        {
            return search.IntersectsWith(rect);
        }
        protected override Point GetMortonPoint(T p)
        {
            return p.Rect.Location;//todo: center?
        }
    }
}
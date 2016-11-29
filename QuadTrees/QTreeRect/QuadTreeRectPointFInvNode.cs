using System.Drawing;
using QuadTrees.Common;

namespace QuadTrees.QTreeRect
{

    public class QuadTreeRectPointInvNode<T> : QuadTreeRectNode<T, Point> where T : IRectQuadStorable
    {
        public QuadTreeRectPointInvNode(Rectangle rect) : base(rect)
        {
        }

        public QuadTreeRectPointInvNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectPointInvNode(QuadTreeRectNode<T, Point> parent, Rectangle rect)
            : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, Point> CreateNode(Rectangle rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreeRectPointInvNode<T>(this, rectangleF);
        }

        protected override bool CheckIntersects(Point searchRect, T data)
        {
            return data.Rect.Contains(searchRect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, Point>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(Point search, Rectangle rect)
        {
            return false;
        }

        protected override bool QueryIntersects(Point search, Rectangle rect)
        {
            return rect.Contains(search);
        }

        protected override Point GetMortonPoint(T p)
        {
            return p.Rect.Location;//todo: center?
        }
    }
}
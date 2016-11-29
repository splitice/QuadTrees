using System.Drawing;
using QuadTrees.Common;

namespace QuadTrees.QTreeRectF
{

    public class QuadTreeRectPointInvNode<T> : QuadTreeRectNode<T, PointF> where T : IRectFQuadStorable
    {
        public QuadTreeRectPointInvNode(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectPointInvNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectPointInvNode(QuadTreeRectNode<T, PointF> parent, RectangleF rect)
            : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, PointF> CreateNode(RectangleF rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreeRectPointInvNode<T>(this, rectangleF);
        }

        protected override bool CheckIntersects(PointF searchRect, T data)
        {
            return data.Rect.Contains(searchRect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, PointF>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(PointF search, RectangleF rect)
        {
            return false;
        }

        protected override bool QueryIntersects(PointF search, RectangleF rect)
        {
            return rect.Contains(search);
        }

        protected override PointF GetMortonPoint(T p)
        {
            return p.Rect.Location;//todo: center?
        }
    }
}
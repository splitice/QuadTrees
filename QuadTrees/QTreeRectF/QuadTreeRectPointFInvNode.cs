using System.Drawing;
using QuadTrees.Common;

namespace QuadTrees.QTreeRectF
{

    public class QuadTreeRectPointFInvNode<T> : QuadTreeRectNode<T, PointF> where T : IRectFQuadStorable
    {
        public QuadTreeRectPointFInvNode(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectPointFInvNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectPointFInvNode(QuadTreeRectNode<T, PointF> parent, RectangleF rect)
            : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, PointF> CreateNode(RectangleF rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreeRectPointFInvNode<T>(this, rectangleF);
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
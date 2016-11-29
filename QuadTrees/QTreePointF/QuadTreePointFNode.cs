using System.Diagnostics;
using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees.QTreePointF
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePointFNode<T> : QuadTreeFNodeCommon<T, QuadTreePointFNode<T>> where T : IPointFQuadStorable
    {
        public QuadTreePointFNode(RectangleF rect)
            : base(rect)
        {
        }

        public QuadTreePointFNode(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        internal QuadTreePointFNode(QuadTreePointFNode<T> parent, RectangleF rect)
            : base(parent, rect)
        {
        }

        protected override QuadTreePointFNode<T> CreateNode(RectangleF rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreePointFNode<T>(this, rectangleF);
        }

        protected override bool CheckContains(RectangleF rectangleF, T data)
        {
            return rectangleF.Contains(data.Point);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreePointFNode<T>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool CheckIntersects(RectangleF searchRect, T data)
        {
            return CheckContains(searchRect, data);
        }

        protected override PointF GetMortonPoint(T p)
        {
            return p.Point;
        }
    }
}
using System.Diagnostics;
using System.Drawing;
using QuadTrees.Common;

namespace QuadTrees.QTreePoint
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePointNode<T> : QuadTreeNodeCommon<T, QuadTreePointNode<T>> where T : IPointQuadStorable
    {
        public QuadTreePointNode(Rectangle rect)
            : base(rect)
        {
        }

        public QuadTreePointNode(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        internal QuadTreePointNode(QuadTreePointNode<T> parent, Rectangle rect)
            : base(parent, rect)
        {
        }

        protected override QuadTreePointNode<T> CreateNode(Rectangle rectangleF)
        {
            VerifyNodeAssertions(rectangleF);
            return new QuadTreePointNode<T>(this, rectangleF);
        }

        protected override bool CheckContains(Rectangle rectangleF, T data)
        {
            if (rectangleF.Size.IsEmpty)
            {
                return data.Point == rectangleF.Location;
            }
            return rectangleF.Contains(data.Point);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreePointNode<T>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool CheckIntersects(Rectangle searchRect, T data)
        {
            return CheckContains(searchRect, data);
        }

        protected override Point GetMortonPoint(T p)
        {
            return p.Point;
        }
    }
}
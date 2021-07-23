using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRectPointFInverse<T> : QuadTreeFCommon<T, QuadTreeRectNode<T, PointF>, PointF> where T : IRectFQuadStorable
    {
        public QuadTreeRectPointFInverse(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectPointFInverse(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        protected override QuadTreeRectNode<T, PointF> CreateNode(RectangleF rect)
        {
            return new QuadTreeRectPointFInvNode<T>(rect);
        }
    }
}

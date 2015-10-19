using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRect;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRectPointInverse<T> : QuadTreeCommon<T, QuadTreeRectNode<T, PointF>, PointF> where T : IRectQuadStorable
    {
        public QuadTreeRectPointInverse(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectPointInverse(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        public QuadTreeRectPointInverse()
        {
        }

        protected override QuadTreeRectNode<T, PointF> CreateNode(RectangleF rect)
        {
            return new QuadTreeRectPointInvNode<T>(rect);
        }
    }
}

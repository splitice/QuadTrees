using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRectF<T> : QuadTreeFCommon<T, QuadTreeRectNode<T, RectangleF>, RectangleF> where T : IRectFQuadStorable
    {
        public QuadTreeRectF(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRectF(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        protected override QuadTreeRectNode<T, RectangleF> CreateNode(RectangleF rect)
        {
            return new QuadTreeRectFNode<T>(rect);
        }
    }
}

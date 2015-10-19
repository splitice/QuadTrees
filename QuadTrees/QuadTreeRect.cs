using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreeRect;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRect<T> : QuadTreeCommon<T, QuadTreeRectNode<T, RectangleF>, RectangleF> where T : IRectQuadStorable
    {
        public QuadTreeRect(RectangleF rect) : base(rect)
        {
        }

        public QuadTreeRect(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        public QuadTreeRect()
        {
        }

        protected override QuadTreeRectNode<T, RectangleF> CreateNode(RectangleF rect)
        {
            return new QuadTreeRectNode<T>(rect);
        }
    }
}

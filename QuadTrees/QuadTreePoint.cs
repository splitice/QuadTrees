using System.Drawing;
using QuadTrees.Common;
using QuadTrees.QTreePoint;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Points in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePoint<T> : QuadTreeCommon<T, QuadTreePointNode<T>, RectangleF> where T : IPointQuadStorable
    {
        public QuadTreePoint(RectangleF rect) : base(rect)
        {
        }

        public QuadTreePoint(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        public QuadTreePoint(): base()
        {
            
        } 

        protected override QuadTreePointNode<T> CreateNode(RectangleF rect)
        {
            return new QuadTreePointNode<T>(rect);
        }
    }
}

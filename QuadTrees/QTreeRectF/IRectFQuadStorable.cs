using System.Drawing;

namespace QuadTrees.QTreeRectF
{
    /// <summary>
    /// Interface to define Rect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IRectFQuadStorable
    {
        /// <summary>
        /// The RectangleF that defines the object's boundaries.
        /// </summary>
        RectangleF Rect { get; }
    }
}
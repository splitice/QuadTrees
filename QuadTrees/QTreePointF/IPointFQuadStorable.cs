using System.Drawing;

namespace QuadTrees.QTreePointF
{
    /// <summary>
    /// Interface to define Rect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IPointFQuadStorable
    {
        /// <summary>
        /// The PointF that defines the object's boundaries.
        /// </summary>
        PointF Point { get; }
    }
}
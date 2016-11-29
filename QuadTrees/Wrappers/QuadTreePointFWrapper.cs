using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadTrees.QTreePointF;

namespace QuadTrees.Wrappers
{
    /// <summary>
    /// A simple container for a point in a QuadTree
    /// </summary>
    public struct QuadTreePointFWrapper: IPointFQuadStorable
    {
        private PointF _point;

        public PointF Point
        {
            get { return _point; }
        }

        public QuadTreePointFWrapper(PointF point)
        {
            _point = point;
        }
    }
}

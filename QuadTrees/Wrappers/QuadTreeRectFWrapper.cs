using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadTrees.QTreeRectF;

namespace QuadTrees.Wrappers
{
    /// <summary>
    /// A simple container for a rectangle in a QuadTree
    /// </summary>
    public struct QuadTreeRectFWrapper : IRectFQuadStorable
    {
        private RectangleF _rect;

        public RectangleF Rect
        {
            get { return _rect; }
        }

        public QuadTreeRectFWrapper(RectangleF rect)
        {
            _rect = rect;
        }
    }
}

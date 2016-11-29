using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadTrees.QTreeRect;

namespace QuadTrees.Wrappers
{
    /// <summary>
    /// A simple container for a rectangle in a QuadTree
    /// </summary>
    public struct QuadTreeRectWrapper : IRectQuadStorable
    {
        private Rectangle _rect;

        public Rectangle Rect
        {
            get { return _rect; }
        }

        public QuadTreeRectWrapper(Rectangle rect)
        {
            _rect = rect;
        }
    }
}

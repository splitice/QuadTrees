using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using QuadTrees.QTreePoint;
using QuadTrees.QTreeRect;

namespace QuadTrees.Tests
{
    [TestFixture]
    public class TestRectPoint
    {
        class QTreeObject : IRectQuadStorable
        {
            private RectangleF _rect;

            public RectangleF Rect
            {
                get { return _rect; }
            }

            public QTreeObject(RectangleF rect)
            {
                _rect = rect;
            }
        }
        [TestCase]
        public void TestListQuery()
        {
            QuadTreeRectPointInverse<QTreeObject> qtree = new QuadTreeRectPointInverse<QTreeObject>();
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.GetObjects(new PointF(11,11));
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryOutput()
        {
            var list = new List<QTreeObject>();
            QuadTreeRectPointInverse<QTreeObject> qtree = new QuadTreeRectPointInverse<QTreeObject>();
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            qtree.GetObjects(new PointF(11, 11), list);
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryEnum()
        {

            QuadTreeRectPointInverse<QTreeObject> qtree = new QuadTreeRectPointInverse<QTreeObject>();
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.EnumObjects(new PointF(11, 11));
            Assert.AreEqual(1, list.Count());
        }
        [TestCase]
        public void TestListGetAll()
        {
            QuadTreeRectPointInverse<QTreeObject> qtree = new QuadTreeRectPointInverse<QTreeObject>();
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.GetAllObjects();
            Assert.AreEqual(2, list.Count());
        }
    }
}

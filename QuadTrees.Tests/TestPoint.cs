using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using QuadTrees.QTreePointF;
using QuadTrees.QTreeRectF;

namespace QuadTrees.Tests
{
    [TestFixture]
    public class TestPoint
    {
        class QTreeObject: IPointFQuadStorable
        {
            private PointF _rect;

            public PointF Point
            {
                get { return _rect; }
            }

            public QTreeObject(PointF rect)
            {
                _rect = rect;
            }
        }
        [TestCase]
        public void TestListQuery()
        {
            QuadTreePointF<QTreeObject> qtree = new QuadTreePointF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new PointF(10,10)),
                new QTreeObject(new PointF(-1000,1000))
            });

            var list = qtree.GetObjects(new RectangleF(9, 9, 20, 20));
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryOutput()
        {
            QuadTreePointF<QTreeObject> qtree = new QuadTreePointF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new PointF(10,10)),
                new QTreeObject(new PointF(-1000,1000))
            }); ;

            var list = new List<QTreeObject>();
            qtree.GetObjects(new RectangleF(9, 9, 20, 20), list);
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryEnum()
        {
            QuadTreePointF<QTreeObject> qtree = new QuadTreePointF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new PointF(10,10)),
                new QTreeObject(new PointF(-1000,1000))
            });

            var list = qtree.EnumObjects(new RectangleF(9, 9, 20, 20));
            Assert.AreEqual(1, list.Count());
        }
        [TestCase]
        public void TestListGetAll()
        {
            QuadTreePointF<QTreeObject> qtree = new QuadTreePointF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new PointF(10,10)),
                new QTreeObject(new PointF(-1000,1000))
            });

            var list = qtree.GetAllObjects();
            Assert.AreEqual(2, list.Count());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using QuadTrees.QTreeRectF;

namespace QuadTrees.Tests
{
    [TestFixture]
    public class TestRectangle
    {
        class QTreeObject: IRectFQuadStorable
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
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.GetObjects(new RectangleF(9, 9, 20, 20));
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryOutput()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = new List<QTreeObject>();
            qtree.GetObjects(new RectangleF(9, 9, 20, 20), list);
            Assert.AreEqual(1, list.Count);
        }
        [TestCase]
        public void TestListQueryEnum()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.EnumObjects(new RectangleF(9, 9, 20, 20));
            Assert.AreEqual(1, list.Count());
        }
        [TestCase]
        public void TestListGetAll()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            qtree.AddRange(new List<QTreeObject>
            {
                new QTreeObject(new RectangleF(10,10,10,10)),
                new QTreeObject(new RectangleF(-1000,1000,10,10))
            });

            var list = qtree.GetAllObjects();
            Assert.AreEqual(2, list.Count());
        }
        [TestCase]
        public void TestAddMany()
        {
            Random r = new Random(1000);
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            for (int i = 0; i < 10000; i++)
            {
                qtree.Add(new QTreeObject(new RectangleF(r.Next(0, 1000) / 1000f, r.Next(0, 1000) / 1000f, r.Next(1000, 20000) / 1000f, r.Next(1000, 20000) / 1000f)));
            }

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);

            result = qtree.GetObjects(new RectangleF(-.100f, -.100f, .200f, .200f));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
        }

        [TestCase]
        public void TestBulkAddManyThreaded()
        {
            Random r = new Random(1000);
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            List<QTreeObject> list = new List<QTreeObject>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new QTreeObject(new RectangleF(r.Next(0, 1000) / 1000f, r.Next(0, 1000) / 1000f, r.Next(1000, 20000) / 1000f, r.Next(1000, 20000) / 1000f)));
            }
            qtree.AddBulk(list, 1);

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);

            result = qtree.GetObjects(new RectangleF(-.100f, -.100f, .200f, .200f));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
        }

        [TestCase]
        public void TestBulkAddMany()
        {
            Random r = new Random(1000);
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            List<QTreeObject> list= new List<QTreeObject>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new QTreeObject(new RectangleF(r.Next(0, 1000) / 1000f, r.Next(0, 1000) / 1000f, r.Next(1000, 20000) / 1000f, r.Next(1000, 20000) / 1000f)));
            }
            qtree.AddBulk(list);

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);

            result = qtree.GetObjects(new RectangleF(-.100f, -.100f, .200f, .200f));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
        }

        [TestCase]
        public void TestAddManySame()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            List<QTreeObject> list = new List<QTreeObject>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new QTreeObject(new RectangleF(1, 1, 1, 1)));
            }
            qtree.AddRange(list);

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
            Assert.AreEqual(10000, result.Count);
        }

        [TestCase]
        public void TestBulkAddManySame()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            List<QTreeObject> list = new List<QTreeObject>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new QTreeObject(new RectangleF(1,1,1,1)));
            }
            qtree.AddBulk(list);

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
            Assert.AreEqual(10000, result.Count);
        }

        [TestCase]
        public void TestAddSameIndividual()
        {
            QuadTreeRectF<QTreeObject> qtree = new QuadTreeRectF<QTreeObject>(new RectangleF(float.MinValue/2,float.MinValue/2,float.MaxValue,float.MaxValue));
            List<QTreeObject> list = new List<QTreeObject>();
            for (int i = 0; i < 10000; i++)
            {
                qtree.Add(new QTreeObject(new RectangleF(1, 1, 1, 1)));
            }

            var result = qtree.GetObjects(new RectangleF(-100, -100, 200, 200));
            Assert.AreEqual(result.Distinct().Count(), result.Count);
            Assert.AreEqual(10000, result.Count);
        }
    }
}

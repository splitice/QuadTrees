using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public QuadTreePoint(RectangleF rect)
            : base(rect)
        {
        }

        public QuadTreePoint(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public QuadTreePoint()
            : base()
        {

        }

        protected override QuadTreePointNode<T> CreateNode(RectangleF rect)
        {
            return new QuadTreePointNode<T>(rect);
        }

        UInt32 EncodeMorton2(UInt32 x, UInt32 y)
        {
            return (Part1By1(y) << 1) + Part1By1(x);
        }

        UInt32 Part1By1(UInt32 x)
        {
            x &= 0x0000ffff;                  // x = ---- ---- ---- ---- fedc ba98 7654 3210
            x = (x ^ (x << 8)) & 0x00ff00ff; // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x << 4)) & 0x0f0f0f0f; // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x << 2)) & 0x33333333; // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x << 1)) & 0x55555555; // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            return x;
        }

        private UInt32 MortonIndex2(PointF pointF, float minX, float minY, float maxX, float maxY)
        {
            pointF = new PointF(pointF.X - minX, pointF.Y - minY);
            var pX = (UInt32)(UInt16.MaxValue * pointF.X / (maxX - minX));
            var pY = (UInt32)(UInt16.MaxValue * pointF.Y / (maxY - minY));

            return EncodeMorton2(pX, pY);
        }

        public void AddRangeLarge(T[] points)
        {
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var p in points)
            {
                var point = p.Point;
                if (point.X > maxX)
                {
                    maxX = point.X;
                }
                if (point.X < minX)
                {
                    minX = point.X;
                }
                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
                if (point.Y < minY)
                {
                    minY = point.Y;
                }
            }
            var range = points.Select((a) => new KeyValuePair<UInt32, T>(MortonIndex2(a.Point,minX,minY,maxX,maxY), a)).OrderBy((a) => a.Key).ToArray();
            InsertStore(QuadTreePointRoot, range, 0, range.Length);
        }

        private const int desired = 8;
        private void InsertStore(QuadTreePointNode<T> node, KeyValuePair<uint, T>[] range, int start, int end)
        {
            var count = end - start;
            if (count > desired && node.QuadRect.Width > 0.1 && node.QuadRect.Height > 0.1)
            {
                var quater = count/4;
                var quater1 = start + quater + (count % 4);
                var quater2 = quater1 + quater;
                var quater3 = quater2 + quater;
                PointF middlePoint = range[quater2].Value.Point;
                if (node.Contains(middlePoint) && node.QuadRect.Right != middlePoint.X && node.QuadRect.Bottom != middlePoint.Y)
                {
                    node.Subdivide(middlePoint);
                }
                else
                {
                    node.Subdivide();
                }
                InsertStore(node.ChildTl, range, start, quater1);
                InsertStore(node.ChildTr, range, quater1, quater2);
                InsertStore(node.ChildBl, range, quater2, quater3);
                InsertStore(node.ChildBr, range, quater3, end);
            }
            else
            {
                for (; start < end; start++)
                {
                    var t = range[start].Value;
                    var qto = new QuadTreeObject<T, QuadTreePointNode<T>>(t);
                    node.Insert(qto);
                    WrappedDictionary.Add(t,qto);
                }
            }
        }
    }
}

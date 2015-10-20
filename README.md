# QuadTrees

[![Build Status](https://travis-ci.org/splitice/QuadTrees.png?branch=master)](https://travis-ci.org/splitice/QuadTrees)

High Performance Quad Tree Implementations for C# (Point, Rect and PointInv)

## Example

```
QuadTreeRect<QTreeObject> qtree = new QuadTreeRect<QTreeObject>();
qtree.AddRange(new List<QTreeObject>
{
	new QTreeObject(new RectangleF(10,10,10,10)), // Expected result
	new QTreeObject(new RectangleF(-1000,1000,10,10))
});

var list = new List<QTreeObject>();
qtree.GetObjects(new RectangleF(9, 9, 20, 20), list);
```
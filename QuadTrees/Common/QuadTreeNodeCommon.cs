using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using QuadTrees.Helper;

namespace QuadTrees.Common
{
    public abstract class QuadTreeNodeCommon<T, TNode, TQuery> where TNode : QuadTreeNodeCommon<T, TNode, TQuery>
    {
        #region Constants

        // How many objects can exist in a QuadTree before it sub divides itself
        public const int MaxObjectsPerNode = 10;//scales up to about 16 on removal

        #endregion

        #region Private Members

        private QuadTreeObject<T, TNode>[] _objects = null;
        private int _objectCount = 0;

        protected RectangleF Rect; // The area this QuadTree represents

        private readonly TNode _parent = null; // The parent of this quad

        private TNode _childTl = null; // Top Left Child
        private TNode _childTr = null; // Top Right Child
        private TNode _childBl = null; // Bottom Left Child
        private TNode _childBr = null; // Bottom Right Child

        #endregion

        #region Public Properties

        /// <summary>
        /// The area this QuadTree represents.
        /// </summary>
        internal virtual RectangleF QuadRect
        {
            get { return Rect; }
        }

        /// <summary>
        /// How many total objects are contained within this QuadTree (ie, includes children)
        /// </summary>
        public int Count
        {
            get
            {
                int count = _objectCount;

                // Add the objects that are contained in the children
                if (ChildTl != null)
                {
                    count += ChildTl.Count + ChildTr.Count + ChildBl.Count + ChildBr.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Count all nodes in the graph (Edge + Leaf)
        /// </summary>
        public int CountNodes
        {
            get
            {

                int count = _objectCount;

                // Add the objects that are contained in the children
                if (ChildTl != null)
                {
                    count += ChildTl.CountNodes + ChildTr.CountNodes + ChildBl.CountNodes + ChildBr.CountNodes + 4;
                }

                return count;
            }
        }

        /// <summary>
        /// Returns true if this is a empty leaf node
        /// </summary>
        public bool IsEmpty
        {
            get { return ChildTl == null && _objectCount == 0; }
        }

        public TNode ChildTl
        {
            get { return _childTl; }
        }

        public TNode ChildTr
        {
            get { return _childTr; }
        }

        public TNode ChildBl
        {
            get { return _childBl; }
        }

        public TNode ChildBr
        {
            get { return _childBr; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        protected QuadTreeNodeCommon(RectangleF rect)
        {
            Rect = rect;
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        protected QuadTreeNodeCommon(float x, float y, float width, float height)
        {
            Rect = new RectangleF(x, y, width, height);
        }


        internal QuadTreeNodeCommon(TNode parent, RectangleF rect)
            : this(rect)
        {
            _parent = parent;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Add an item to the object list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        private void Add(QuadTreeObject<T, TNode> item)
        {
            if (_objects == null)
            {
                _objects = new QuadTreeObject<T, TNode>[MaxObjectsPerNode];
            }
            else if (_objectCount >= _objects.Length)
            {
                //Debug.Assert(_parent != null);
                //throw new IndexOutOfRangeException("Only "+MaxObjectsPerNode+" objects can be stored in each node. Given no subdivisions are you pushing the same unique value in multiple times?");
                var old = _objects;
                _objects = new QuadTreeObject<T, TNode>[old.Length * 2];
                Array.Copy(old,_objects,old.Length);
            }

            item.Owner = this as TNode;
            _objects[_objectCount ++] = item;
        }


        /// <summary>
        /// Remove an item from the object list.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        private void Remove(QuadTreeObject<T, TNode> item)
        {
            if (_objects == null) return;

            int removeIndex = Array.IndexOf(_objects,item,0,_objectCount);
            if (removeIndex < 0) return;

            if (_objectCount == 1)
            {
                _objects = null;
                _objectCount = 0;
            }
            else
            {
                _objects[removeIndex] = _objects[_objectCount - 1];
                _objects[_objectCount - 1] = null;
                _objectCount --;
            }
        }

        /// <summary>
        /// Automatically subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        internal PointF Subdivide()
        {
            // We've reached capacity, subdivide...
            PointF mid = new PointF(Rect.X + (Rect.Width / 2), Rect.Y + (Rect.Height / 2));

            Subdivide(mid);

            return mid;
        }


        /// <summary>
        /// Manually subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        public void Subdivide(PointF mid)
        {
            // We've reached capacity, subdivide...
            _childTl = CreateNode(new RectangleF(Rect.Left, Rect.Top, mid.X - Rect.Left, mid.Y - Rect.Top));
            _childTr = CreateNode(new RectangleF(mid.X, Rect.Top, Rect.Right - mid.X, mid.Y - Rect.Top));
            _childBl = CreateNode(new RectangleF(Rect.Left, mid.Y, mid.X - Rect.Left, Rect.Bottom - mid.Y));
            _childBr = CreateNode(new RectangleF(mid.X, mid.Y, Rect.Right - mid.X, Rect.Bottom - mid.Y));

            var nodeList = new QuadTreeObject<T, TNode>[_objectCount];
            int nodeIdx = 0;

            // If they're completely contained by the quad, bump objects down
            for (int i = 0; i < _objectCount; i++)
            {
                var obj = _objects[i];
                TNode destTree = GetDestinationTree(obj);

                // Insert to the appropriate tree, remove the object, and back up one in the loop
                if (destTree == this)
                {
                    nodeList[nodeIdx++] = obj;
                }
                else
                {
                    destTree.Insert(obj);
                }
            }

            _objectCount = nodeIdx;
            _objects = nodeIdx == 0 ? null : nodeList;
        }

        protected void VerifyNodeAssertions(RectangleF rectangleF)
        {
            Debug.Assert(rectangleF.Width > 0);
            Debug.Assert(rectangleF.Height > 0);
        }

        protected abstract TNode CreateNode(RectangleF rectangleF);

        public IEnumerable<TNode> GetChildren()
        {
            yield return ChildTl;
            yield return ChildTr;
            yield return ChildBl;
            yield return ChildBr;
        }

        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private TNode GetDestinationTree(QuadTreeObject<T, TNode> item)
        {
            if (ChildTl.ContainsObject(item))
            {
                return ChildTl;
            }
            if (ChildTr.ContainsObject(item))
            {
                return ChildTr;
            }
            if (ChildBl.ContainsObject(item))
            {
                return ChildBl;
            }
            if (ChildBr.ContainsObject(item))
            {
                return ChildBr;
            }

            // If a child can't contain an object, it will live in this Quad
            // This is usually when == midpoint
            return this as TNode;
        }

        internal void Relocate(QuadTreeObject<T, TNode> item)
        {
            // Are we still inside our parent?
            if (ContainsObject(item))
            {
                // Good, have we moved inside any of our children?
                if (ChildTl != null)
                {
                    TNode dest = GetDestinationTree(item);
                    if (item.Owner != dest)
                    {
                        // Delete the item from this quad and add it to our child
                        // Note: Do NOT clean during this call, it can potentially delete our destination quad
                        TNode formerOwner = item.Owner;
                        Delete(item, false);
                        dest.Insert(item);

                        // Clean up ourselves
                        formerOwner.CleanUpwards();
                    }
                }
            }
            else
            {
                // We don't fit here anymore, move up, if we can
                if (_parent != null)
                {
                    _parent.Relocate(item);
                }
            }
        }

        internal void CleanThis()
        {
            if (ChildTl != null)
            {
                // If all the children are empty leaves, delete all the children
                if (ChildTl.IsEmpty &&
                    ChildTr.IsEmpty &&
                    ChildBl.IsEmpty &&
                    ChildBr.IsEmpty)
                {
                    _childTl = null;
                    _childTr = null;
                    _childBl = null;
                    _childBr = null;

                    if (_parent != null && _objectCount == 0)
                    {
                        CleanUpwards();
                    }
                }
            }
        }


        internal void CleanUpwards()
        {
            if (_parent != null && _objectCount == 0)
            {
                _parent.CleanThis();
            }
        }

        #endregion
        public bool ContainsPoint(PointF point)
        {
            return Rect.Contains(point);
        }

        public abstract bool ContainsObject(QuadTreeObject<T, TNode> qto);

        #region Internal Methods

        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        public void Clear()
        {
            // Clear out the children, if we have any
            if (ChildTl != null)
            {
                // Set the children to null
                _childTl = _childTr = _childBl = _childBr = null;
            }

            // Clear any objects at this level
            if (_objects != null)
            {
                _objectCount = 0;
                _objects = null;
            }
            else
            {
                Debug.Assert(_objectCount == 0);
            }
        }


        /// <summary>
        /// Deletes an item from this QuadTree. If the object is removed causes this Quad to have no objects in its children, it's children will be removed as well.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="clean">Whether or not to clean the tree</param>
        public void Delete(QuadTreeObject<T, TNode> item, bool clean)
        {
            if (item.Owner != null)
            {
                if (item.Owner == this)
                {
                    Remove(item);
                    if (clean)
                    {
                        CleanUpwards();
                    }
                }
                else
                {
                    item.Owner.Delete(item, clean);
                }
            }
        }



        /// <summary>
        /// Insert an item into this QuadTree object.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        public void Insert(QuadTreeObject<T, TNode> item)
        {
            // If this quad doesn't contain the items rectangle, do nothing, unless we are the root
            if (!CheckContains(Rect, item.Data))
            {
                Debug.Assert(_parent != null,
                    "We are not the root, and this object doesn't fit here. How did we get here?");
                if (_parent != null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    _parent.Insert(item);
                }
                return;
            }

            if (_objects == null ||
                (ChildTl == null && _objectCount + 1 <= MaxObjectsPerNode))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (ChildTl == null)
                {
                    Subdivide();
                }

                // Find out which tree this object should go in and add it there
                TNode destTree = GetDestinationTree(item);
                if (destTree == this)
                {
                    Add(item);
                }
                else
                {
                    destTree.Insert(item);
                }
            }
        }

        protected abstract bool CheckContains(RectangleF rectangleF, T data);


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The RectangleF to find objects in.</param>
        public List<T> GetObjects(TQuery searchRect)
        {
            var results = new List<T>();
            GetObjects(searchRect, results.Add);
            return results;
        }

        protected abstract bool QueryContains(TQuery search, RectangleF rect);
        protected abstract bool QueryIntersects(TQuery search, RectangleF rect);

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The RectangleF to find objects in.</param>
        public IEnumerable<T> EnumObjects(TQuery searchRect)
        {
            Stack<TNode> stack = new Stack<TNode>();
            Stack<TNode> allStack = null;
            TNode node = this as TNode;
            do
            {
                if (QueryContains(searchRect, node.Rect))
                {
                    // If the search area completely contains this quad, just get every object this quad and all it's children have
                    allStack = allStack ?? new Stack<TNode>();
                    do
                    {
                        if (node._objects != null)
                        {
                            for (int i = 0; i < _objectCount; i++)
                            {
                                var y = node._objects[i];
                                yield return y.Data;
                            }
                        }
                        if (node.ChildTl != null)
                        {
                            allStack.Push(node.ChildTl);
                            allStack.Push(node.ChildTr);
                            allStack.Push(node.ChildBl);
                            allStack.Push(node.ChildBr);
                        }
                        if (allStack.Count == 0)
                        {
                            break;
                        }
                        node = allStack.Pop();
                    } while (true);
                }
                else if (QueryIntersects(searchRect, node.Rect))
                {
                    // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                    if (node._objects != null)
                    {
                        for (int i = 0; i < node._objectCount; i++)
                        {
                            QuadTreeObject<T, TNode> t = node._objects[i];
                            if (CheckIntersects(searchRect, t.Data))
                            {
                                yield return t.Data;
                            }
                        }
                    }

                    // Get the objects for the search RectangleF from the children
                    if (node.ChildTl != null)
                    {
                        stack.Push(node.ChildTl);
                        stack.Push(node.ChildTr);
                        stack.Push(node.ChildBl);
                        stack.Push(node.ChildBr);
                    }
                }
                if (stack.Count == 0)
                {
                    break;
                }
                node = stack.Pop();
            } while (true);
        }

        protected abstract bool CheckIntersects(TQuery searchRect, T data);

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="searchRect">The RectangleF to find objects in.</param>
        /// <param name="put"></param>
        public void GetObjects(TQuery searchRect, Action<T> put)
        {
            // We can't do anything if the results list doesn't exist
            if (QueryContains(searchRect,Rect))
            {
                // If the search area completely contains this quad, just get every object this quad and all it's children have
                GetAllObjects(put);
            }
            else if (QueryIntersects(searchRect,Rect))
            {
                // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
                if (_objects != null)
                {
                    for (int i = 0; i < _objectCount; i++)
                    {
                        var data = _objects[i].Data;
                        if (CheckIntersects(searchRect, data))
                        {
                            put(data);
                        }
                    }
                }

                // Get the objects for the search RectangleF from the children
                if (ChildTl != null)
                {
                    Debug.Assert(ChildTl != this);
                    Debug.Assert(ChildTr != this);
                    Debug.Assert(ChildBl != this);
                    Debug.Assert(ChildBr != this);
                    ChildTl.GetObjects(searchRect, put);
                    ChildTr.GetObjects(searchRect, put);
                    ChildBl.GetObjects(searchRect, put);
                    ChildBr.GetObjects(searchRect, put);
                }
                else
                {
                    Debug.Assert(ChildTr == null);
                    Debug.Assert(ChildBl == null);
                    Debug.Assert(ChildBr == null);
                }
            }
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        /// <param name="put">A reference to a list in which to store the objects.</param>
        public void GetAllObjects(Action<T> put)
        {
            // If this Quad has objects, add them
            if (_objects != null)
            {
                Debug.Assert(_objectCount != 0);
                Debug.Assert(_objectCount == _objects.Count((a)=>a!=null));

                for (int i = 0; i < _objectCount; i++)
                {
                    put(_objects[i].Data);
                }
            }
            else
            {
                Debug.Assert(_objectCount == 0);
            }

            // If we have children, get their objects too
            if (ChildTl != null)
            {
                ChildTl.GetAllObjects(put);
                ChildTr.GetAllObjects(put);
                ChildBl.GetAllObjects(put);
                ChildBr.GetAllObjects(put);
            }
        }

        #endregion
    }

    public abstract class QuadTreeNodeCommon<T, TNode> : QuadTreeNodeCommon<T, TNode, RectangleF>
        where TNode : QuadTreeNodeCommon<T, TNode>
    {
        protected QuadTreeNodeCommon(RectangleF rect) : base(rect)
        {
        }

        protected QuadTreeNodeCommon(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public QuadTreeNodeCommon(TNode parent, RectangleF rect) : base(parent, rect)
        {
        }


        protected override bool QueryContains(RectangleF search, RectangleF rect)
        {
            return search.Contains(rect);
        }

        protected override bool QueryIntersects(RectangleF search, RectangleF rect)
        {
            return search.Intersects(rect);
        }
    }

    public abstract class QuadTreeNodeCommonPoint<T, TNode> : QuadTreeNodeCommon<T, TNode, PointF>
        where TNode : QuadTreeNodeCommon<T, TNode, PointF>
    {
        protected QuadTreeNodeCommonPoint(RectangleF rect)
            : base(rect)
        {
        }

        protected QuadTreeNodeCommonPoint(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public QuadTreeNodeCommonPoint(TNode parent, RectangleF rect)
            : base(parent, rect)
        {
        }


        protected override bool QueryContains(PointF search, RectangleF rect)
        {
            return rect.Contains(search);
        }

        protected override bool QueryIntersects(PointF search, RectangleF rect)
        {
            return false;
        }
    }

}

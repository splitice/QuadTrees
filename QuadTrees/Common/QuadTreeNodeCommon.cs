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
        private const int MaxObjectsPerNode = 8;//scales up to about 16 on removal

        #endregion

        #region Private Members

        private QuadTreeObject<T, TNode>[] _objects = null;
        private int _objectCount = 0;

        private RectangleF _rect; // The area this QuadTree represents

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
        public RectangleF QuadRect
        {
            get { return _rect; }
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
                if (_childTl != null)
                {
                    count += _childTl.Count + _childTr.Count + _childBl.Count + _childBr.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Returns true if this is a empty leaf node
        /// </summary>
        public bool IsEmpty
        {
            get { return _childTl == null && _objectCount == 0; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        protected QuadTreeNodeCommon(RectangleF rect)
        {
            _rect = rect;
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        protected QuadTreeNodeCommon(int x, int y, int width, int height)
        {
            _rect = new RectangleF(x, y, width, height);
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
        /// Subdivide this QuadTree and move it's children into the appropriate Quads where applicable.
        /// </summary>
        private void Subdivide()
        {
            // We've reached capacity, subdivide...
            PointF size = new PointF(_rect.Width / 2, _rect.Height / 2);
            PointF mid = new PointF(_rect.X + size.X, _rect.Y + size.Y);

            _childTl = CreateNode(new RectangleF(_rect.Left, _rect.Top, size.X, size.Y));
            _childTr = CreateNode(new RectangleF(mid.X, _rect.Top, size.X, size.Y));
            _childBl = CreateNode(new RectangleF(_rect.Left, mid.Y, size.X, size.Y));
            _childBr = CreateNode(new RectangleF(mid.X, mid.Y, size.X, size.Y));

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
            if (nodeIdx == 0)
            {
                _objects = null;
            }
        }

        protected abstract TNode CreateNode(RectangleF rectangleF);


        /// <summary>
        /// Get the child Quad that would contain an object.
        /// </summary>
        /// <param name="item">The object to get a child for.</param>
        /// <returns></returns>
        private TNode GetDestinationTree(QuadTreeObject<T, TNode> item)
        {
            if (CheckContains(_childTl.QuadRect, item.Data))
            {
                return _childTl;
            }
            if (CheckContains(_childTr.QuadRect, item.Data))
            {
                return _childTr;
            }
            if (CheckContains(_childBl.QuadRect, item.Data))
            {
                return _childBl;
            }
            if (CheckContains(_childBr.QuadRect, item.Data))
            {
                return _childBr;
            }
            
            // If a child can't contain an object, it will live in this Quad
            // This is usually when == midpoint
            return this as TNode;
        }


        private void Relocate(QuadTreeObject<T, TNode> item)
        {
            // Are we still inside our parent?
            if (CheckContains(QuadRect, item.Data))
            {
                // Good, have we moved inside any of our children?
                if (_childTl != null)
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
            if (_childTl != null)
            {
                // If all the children are empty leaves, delete all the children
                if (_childTl.IsEmpty &&
                    _childTr.IsEmpty &&
                    _childBl.IsEmpty &&
                    _childBr.IsEmpty)
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

        #region Internal Methods

        /// <summary>
        /// Clears the QuadTree of all objects, including any objects living in its children.
        /// </summary>
        public void Clear()
        {
            // Clear out the children, if we have any
            if (_childTl != null)
            {
                _childTl.Clear();
                _childTr.Clear();
                _childBl.Clear();
                _childBr.Clear();

                // Set the children to null
                _childTl = null;
                _childTr = null;
                _childBl = null;
                _childBr = null;
            }

            // Clear any objects at this level
            if (_objects != null)
            {
                _objectCount = 0;
                _objects = null;
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
            if (!CheckContains(_rect, item.Data))
            {
                Debug.Assert(_parent == null,
                    "We are not the root, and this object doesn't fit here. How did we get here?");
                if (_parent == null)
                {
                    // This object is outside of the QuadTree bounds, we should add it at the root level
                    Add(item);
                }
                return;
            }

            if (_objects == null ||
                (_childTl == null && _objectCount + 1 <= MaxObjectsPerNode))
            {
                // If there's room to add the object, just add it
                Add(item);
            }
            else
            {
                // No quads, create them and bump objects down where appropriate
                if (_childTl == null)
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
                if (QueryContains(searchRect, node._rect))
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
                        if (node._childTl != null)
                        {
                            allStack.Push(node._childTl);
                            allStack.Push(node._childTr);
                            allStack.Push(node._childBl);
                            allStack.Push(node._childBr);
                        }
                        if (allStack.Count == 0)
                        {
                            break;
                        }
                        node = allStack.Pop();
                    } while (true);
                }
                else if (QueryIntersects(searchRect, node._rect))
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
                    if (node._childTl != null)
                    {
                        stack.Push(node._childTl);
                        stack.Push(node._childTr);
                        stack.Push(node._childBl);
                        stack.Push(node._childBr);
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
            if (QueryContains(searchRect,_rect))
            {
                // If the search area completely contains this quad, just get every object this quad and all it's children have
                GetAllObjects(put);
            }
            else if (QueryIntersects(searchRect,_rect))
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
                if (_childTl != null)
                {
                    Debug.Assert(_childTl != this);
                    Debug.Assert(_childTr != this);
                    Debug.Assert(_childBl != this);
                    Debug.Assert(_childBr != this);
                    _childTl.GetObjects(searchRect, put);
                    _childTr.GetObjects(searchRect, put);
                    _childBl.GetObjects(searchRect, put);
                    _childBr.GetObjects(searchRect, put);
                }
                else
                {
                    Debug.Assert(_childTr == null);
                    Debug.Assert(_childBl == null);
                    Debug.Assert(_childBr == null);
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
            if (_childTl != null)
            {
                _childTl.GetAllObjects(put);
                _childTr.GetAllObjects(put);
                _childBl.GetAllObjects(put);
                _childBr.GetAllObjects(put);
            }
        }


        /// <summary>
        /// Moves the QuadTree object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        public void Move(QuadTreeObject<T, TNode> item)
        {
            if (item.Owner != null)
            {
                item.Owner.Relocate(item);
            }
            else
            {
                Relocate(item);
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

        protected QuadTreeNodeCommon(int x, int y, int width, int height) : base(x, y, width, height)
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

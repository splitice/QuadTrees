using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuadTrees.Common
{
    public abstract class QuadTreeFCommon<TObject, TNode, TQuery> : ICollection<TObject> where TNode : QuadTreeFNodeCommon<TObject, TNode, TQuery>
    {
        #region Private Members

        internal readonly Dictionary<TObject, QuadTreeObject<TObject, TNode>> WrappedDictionary = new Dictionary<TObject, QuadTreeObject<TObject, TNode>>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        // Alternate method, use Parallel arrays

        // The root of this quad tree
        protected readonly TNode QuadTreePointRoot;

        #endregion

        protected abstract TNode CreateNode(RectangleF rect);

        #region Constructor

        /// <summary>
        /// Initialize a QuadTree covering the full range of values possible
        /// </summary>
        protected QuadTreeFCommon()
        {
            QuadTreePointRoot =
                CreateNode(new RectangleF(float.MinValue/2, float.MinValue/2, float.MaxValue, float.MaxValue));
        } 

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        protected QuadTreeFCommon(RectangleF rect)
        {
            QuadTreePointRoot = CreateNode(rect);
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        protected QuadTreeFCommon(float x, float y, float width, float height)
        {
            QuadTreePointRoot = CreateNode(new RectangleF(x, y, width, height));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the RectangleF that bounds this QuadTree
        /// </summary>
        public RectangleF QuadRect
        {
            get { return QuadTreePointRoot.QuadRect; }
        }

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The RectangleF to find objects in.</param>
        public List<TObject> GetObjects(TQuery rect)
        {
            return QuadTreePointRoot.GetObjects(rect);
        }

        /// <summary>
        /// Query the QuadTree and return an enumerator for the results
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public IEnumerable<TObject> EnumObjects(TQuery rect)
        {
            return QuadTreePointRoot.EnumObjects(rect);
        } 


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The RectangleF to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        public void GetObjects(TQuery rect, List<TObject> results)
        {
            Action<TObject> cb = results.Add;
#if DEBUG
            cb = (a) =>
            {
                Debug.Assert(!results.Contains(a));
                results.Add(a);
            };
#endif
            QuadTreePointRoot.GetObjects(rect, cb);
        }


        public void GetObjects(TQuery rect, Action<TObject> add)
        {
            QuadTreePointRoot.GetObjects(rect, add);
        }

        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        public IEnumerable<TObject> GetAllObjects()
        {
            return WrappedDictionary.Keys;
        }


        /// <summary>
        /// Moves the object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        public bool Move(TObject item)
        {
            QuadTreeObject<TObject, TNode> obj;
            if (WrappedDictionary.TryGetValue(item, out obj))
            {
                obj.Owner.Relocate(obj);

                Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
                return true;
            }
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            return false;
        }

        #endregion

        #region ICollection<T> Members

        ///<summary>
        ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(TObject item)
        {
            var wrappedObject = new QuadTreeObject<TObject, TNode>(item);
            if (WrappedDictionary.ContainsKey(item))
            {
                throw new ArgumentException("Object already exists in index");
            }
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            WrappedDictionary.Add(item, wrappedObject);
            QuadTreePointRoot.Insert(wrappedObject, true);
            //Debug.Assert(WrappedDictionary.Values.Distinct().Count() == WrappedDictionary.Count);
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
        }


        ///<summary>
        ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            WrappedDictionary.Clear();
            QuadTreePointRoot.Clear();
        }


        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(TObject item)
        {
            return WrappedDictionary.ContainsKey(item);
        }


        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        ///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        ///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        ///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        ///<exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type <paramref name="T" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        public void CopyTo(TObject[] array, int arrayIndex)
        {
            WrappedDictionary.Keys.CopyTo(array, arrayIndex);
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        public int Count
        {
            get { return WrappedDictionary.Count; }
        }

        /// <summary>
        /// Count the number of nodes in the tree
        /// </summary>
        public int CountNodes
        {
            get { return QuadTreePointRoot.CountNodes; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        ///</returns>
        ///
        public bool IsReadOnly
        {
            get { return false; }
        }

        ///<summary>
        ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        ///
        ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(TObject item)
        {
            QuadTreeObject<TObject, TNode> obj;
            if (WrappedDictionary.TryGetValue(item, out obj))
            {
                var owner = obj.Owner;
                bool r = owner.Remove(obj);
                Debug.Assert(r);
                r = WrappedDictionary.Remove(item);
                Debug.Assert(r);
                owner.CleanUpwards();
                Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
                return true;
            }
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            return false;
        }

        /// <summary>
        /// Remove all objects matching an expression (lambda)
        /// </summary>
        /// <param name="whereExpr"></param>
        /// <returns></returns>
        public bool RemoveAll(Func<TObject,bool> whereExpr)
        {
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            var owners = new HashSet<TNode>();
            var set = new List<QuadTreeObject<TObject,TNode>>();
            foreach(var kv in WrappedDictionary){
                if (!whereExpr(kv.Key)) continue;
                set.Add(kv.Value);
            } 

            //Dictionary removals can happen in the background
            Action dictRemovalProc = () =>
            {
                foreach (var s in set)
                {
                    bool r = WrappedDictionary.Remove(s.Data);
                    Debug.Assert(r);
                }
            };
            var bgTaskCancel = new CancellationTokenSource();
            var bgTask = Task.Run(dictRemovalProc, bgTaskCancel.Token);
            
            //Process
            foreach (var s in set)
            {
                var owner = s.Owner;
                if (owner.Parent != null)
                {
                    Debug.Assert(owner.Parent.GetChildren().Any((a) => a == owner));
                }
                bool r = owner.Remove(s);
                Debug.Assert(r);

                owners.Add(owner);
            }
            var ret = set.Count != 0;

            //Cleanup tree
            var ownersNew = new HashSet<TNode>();
            while (owners.Any())
            {
                foreach (var qto in owners)
                {
                    if (qto.CleanThis() && qto.Parent != null)
                    {
                        ownersNew.Add(qto.Parent);
                    }
                }
                var placeholder = owners;
                owners = ownersNew;
                ownersNew = placeholder;
                ownersNew.Clear();
            }

            var bgTaskStatus = bgTask.Status;
            if (bgTaskStatus == TaskStatus.Running)
            {
                bgTask.Wait();
            }
            else if(bgTaskStatus != TaskStatus.RanToCompletion)
            {
                bgTaskCancel.Cancel();
                dictRemovalProc();
            }
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            return ret;
        }

        #endregion

        #region IEnumerable<T> and IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<TObject> GetEnumerator()
        {
            return WrappedDictionary.Keys.GetEnumerator();
        }


        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Add a range of objects to the Quad Tree
        /// </summary>
        /// <param name="points"></param>
        public void AddRange(IEnumerable<TObject> points)
        {
            //TODO: more optimially?
            int origCount = WrappedDictionary.Count;
            foreach (var ap in points)
            {
                Add(ap);
            }
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            Debug.Assert(WrappedDictionary.Count == origCount + points.Count());
        }

        public void AddBulk(IEnumerable<TObject> points, int threadLevel = 0)
        {
            QuadTreePointRoot.AddBulk(points.ToArray(), (a) =>
            {
                var qto = new QuadTreeObject<TObject, TNode>(a);
                WrappedDictionary.Add(a, qto);
                return qto;
            }, threadLevel);
            Debug.Assert(WrappedDictionary.Count == QuadTreePointRoot.Count);
            Debug.Assert(WrappedDictionary.Count == points.Count());
        }

        /// <summary>
        /// Get stats from the tree
        /// </summary>
        /// <param name="internalNodes"></param>
        /// <param name="leafNodes"></param>
        public void TreeStats(out int internalNodes, out int leafNodes)
        {
            leafNodes = WrappedDictionary.Count;
            internalNodes = QuadTreePointRoot.CountNodes - leafNodes;
        }
    }
}

namespace QuadTrees.Common
{
    /// <summary>
    /// Used internally to attach an Owner to each object stored in the QuadTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPointNode"></typeparam>
    public class QuadTreeObject<T, TPointNode> where TPointNode: class
    {
        private TPointNode _owner;
        private T _data;

        /// <summary>
        /// The wrapped data value
        /// </summary>
        public T Data
        {
            get { return _data; }
        }

        /// <summary>
        /// The QuadTreeNode that owns this object
        /// </summary>
        internal TPointNode Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// Wraps the data value
        /// </summary>
        /// <param name="data">The data value to wrap</param>
        public QuadTreeObject(T data)
        {
            _data = data;
        }
    }
}
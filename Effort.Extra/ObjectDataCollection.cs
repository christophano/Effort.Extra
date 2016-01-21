
namespace Effort.Extra
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A keyed collection for ObjectData
    /// </summary>
    internal class ObjectDataCollection : KeyedCollection<Guid, ObjectData>
    {
        /// <summary>
        /// Extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        protected override Guid GetKeyForItem(ObjectData item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            return item.Identifier;
        }

        /// <summary>
        /// If the data already exists in the collection then it is updated, otherwise it is added.
        /// </summary>
        /// <param name="data">The data.</param>
        public void AddOrUpdate(ObjectData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            if (Contains(data.Identifier)) Remove(data.Identifier);
            Add(data);
        }

        /// <summary>
        /// Tries to get the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c>, if the key exists, otherwise <c>false</c>.</returns>
        public bool TryGetValue(Guid key, out ObjectData data)
        {
            data = Contains(key) ? this[key] : null;
            return data != null;
        }
    }
}
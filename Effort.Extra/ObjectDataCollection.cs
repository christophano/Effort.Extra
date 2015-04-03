
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
            if (Contains(data.Identifier)) Remove(data.Identifier);
            Add(data);
        }

        public bool TryGetValue(Guid key, out ObjectData data)
        {
            data = Contains(key) ? this[key] : null;
            return data != null;
        }
    }
}
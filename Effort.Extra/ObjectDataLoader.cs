
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Effort.DataLoaders;

    public class ObjectDataLoader : IDataLoader
    {
        private static readonly ObjectDataCollection DataCollection = new ObjectDataCollection();

        public ObjectDataLoader() { }

        public ObjectDataLoader(ObjectData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Argument = data.Identifier.ToString();
            DataCollection.AddOrUpdate(data);
        }

        public string Argument { get; set; }

        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            Guid id;
            if (Guid.TryParse(Argument, out id))
            {
                ObjectData data;
                if (DataCollection.TryGetValue(id, out data))
                {
                    return new ObjectDataLoaderFactory(data);
                }
                throw new KeyNotFoundException(String.Format("The key '{0}' was not found in the data collection.", id));
            }
            throw new InvalidOperationException(String.Format("Unable to parse '{0}' as a guid.", Argument));
        }
    }
}
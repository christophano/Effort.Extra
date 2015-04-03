
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Effort.DataLoaders;

    internal class ObjectDataLoaderFactory : ITableDataLoaderFactory
    {
        private static readonly Type LoaderType = typeof(ObjectTableDataLoader<>);
        private readonly ObjectData data;

        public ObjectDataLoaderFactory(ObjectData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            this.data = data;
        }

        public void Dispose() { }

        public ITableDataLoader CreateTableDataLoader(TableDescription table)
        {
            Contract.Requires<ArgumentNullException>(table != null);
            if (!data.HasTable(table.Name)) return new EmptyTableDataLoader();
            var entityType = data.TableType(table.Name);
            var type = LoaderType.MakeGenericType(entityType);
            var constructor = type.GetConstructor(new[]
            {
                typeof (TableDescription), 
                typeof (IEnumerable<>).MakeGenericType(entityType)
            });
            return (ITableDataLoader)constructor.Invoke(new[] { table, data.GetTable(table.Name) });
        }
    }
}

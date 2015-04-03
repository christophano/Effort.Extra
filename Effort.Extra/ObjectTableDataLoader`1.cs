
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using Effort.DataLoaders;

    internal class ObjectTableDataLoader<T> : ITableDataLoader
    {
        private readonly TableDescription table;
        private readonly IEnumerable<T> entities;
        private readonly Lazy<Func<T, object[]>> formatter;

        public ObjectTableDataLoader(TableDescription table, IEnumerable<T> entities)
        {
            Contract.Requires<ArgumentNullException>(table != null);
            Contract.Requires<ArgumentNullException>(entities != null);
            this.table = table;
            this.entities = entities;
            formatter = new Lazy<Func<T, object[]>>(CreateFormatter);
        }

        protected Func<T, object[]> CreateFormatter()
        {
            var type = typeof(T);
            var param = Expression.Parameter(type, "x");
            var initialisers = table.Columns
                .Select(c => Expression.TypeAs(Expression.Property(param, type.GetProperty(c.Name)), typeof(object)));
            var newArray = Expression.NewArrayInit(typeof(object), initialisers);
            return Expression.Lambda<Func<T, object[]>>(newArray, param).Compile();
        }

        public IEnumerable<object[]> GetData()
        {
            return entities.Select(formatter.Value);
        }
    }
}

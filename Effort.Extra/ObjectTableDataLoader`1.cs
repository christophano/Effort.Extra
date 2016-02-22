
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.DataLoaders;

    internal class ObjectTableDataLoader<T> : ITableDataLoader
    {
        private static readonly BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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
                .Select(column => GetProperty(type, column))
                .Select(property => ToExpression(param, property))
                .Select(expression => CastExpression(expression));
            var newArray = Expression.NewArrayInit(typeof(object), initialisers);
            return Expression.Lambda<Func<T, object[]>>(newArray, param).Compile();
        }

        private static PropertyInfo GetProperty(Type parentType, ColumnDescription column)
        {
            return parentType.GetProperty(column.Name, PropertyFlags)
                   ?? parentType.GetProperties(PropertyFlags)
                                .SingleOrDefault(p => MatchColumnAttribute(p, column));
        }

        private static Expression ToExpression(ParameterExpression parameter, PropertyInfo property)
        {
            if (property == null) return Expression.Constant(null);
            return Expression.Property(parameter, property);
        }

        private static Expression CastExpression(Expression expression)
        {
            return Expression.TypeAs(expression, typeof(object));
        }

        private static bool MatchColumnAttribute(PropertyInfo property, ColumnDescription column)
        {
            if (property.PropertyType != column.Type) return false;
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null) return false;
            return columnAttribute.Name == column.Name;
        }

        public IEnumerable<object[]> GetData()
        {
            return entities.Select(formatter.Value);
        }
    }
}

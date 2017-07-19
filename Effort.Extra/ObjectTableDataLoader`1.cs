
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Core.Mapping;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.DataLoaders;
    using Microsoft.CSharp.RuntimeBinder;
    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

    internal class ObjectTableDataLoader<T> : ITableDataLoader
    {
        private const BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly TableDescription description;
        private readonly ObjectDataTable<T> table;
        private readonly Lazy<Func<T, object[]>> formatter;

        public ObjectTableDataLoader(TableDescription description, ObjectDataTable<T> table)
        {
            this.description = description ?? throw new ArgumentNullException(nameof(description));
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            formatter = new Lazy<Func<T, object[]>>(CreateFormatter);
        }

        protected Func<T, object[]> CreateFormatter()
        {
            var type = typeof(T);
            var param = Expression.Parameter(type, "x");
            var initialisers = description.Columns
                .Select(column => table.PropertyMappings.Single(m => m.Column.Name == column.Name))
                .Select(map => ToExpression(param, map))
                .Select(expression => CastExpression(expression));
            var newArray = Expression.NewArrayInit(typeof(object), initialisers);
            return Expression.Lambda<Func<T, object[]>>(newArray, param).Compile();
        }

        private string GetDiscriminator(T item)
        {
            return table.GetDiscriminator(item);
        }

        private Expression ToExpression(ParameterExpression parameter, ScalarPropertyMapping map)
        {
            if (map.Column.Name == table.DiscriminatorColumn)
            {
                return Expression.Call(Expression.Constant(table), typeof(ObjectDataTable<T>).GetMethod(nameof(GetDiscriminator), BindingFlags.Instance | BindingFlags.NonPublic), parameter);
            }
            var binder = Binder.GetMember(CSharpBinderFlags.None, map.Property.Name, typeof(T),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var expression = Expression.Dynamic(binder, typeof (object), parameter);
            return Expression.TryCatch(expression, Expression.Catch(typeof(RuntimeBinderException), Expression.Constant(null)));
        }

        private static Expression CastExpression(Expression expression)
        {
            return Expression.TypeAs(expression, typeof(object));
        }

        public IEnumerable<object[]> GetData()
        {
            var results = table.Select(formatter.Value);
            return results;
        }
    }
}

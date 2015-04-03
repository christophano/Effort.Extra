
namespace Effort.Extra.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Effort.DataLoaders;

    internal static class Builder
    {
        public static TableDescription CreateTableDescription(string name, Type type)
        {
            var ctor = typeof(TableDescription).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
            var columns = type.GetProperties().Select(CreateColumnDescription).ToArray();

            return (TableDescription)ctor.Invoke(new object[] { name, columns });
        }

        public static ColumnDescription CreateColumnDescription(PropertyInfo property)
        {
            var ctor = typeof(ColumnDescription).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
            return (ColumnDescription)ctor.Invoke(new object[] { property.Name, property.PropertyType });
        }
    }
}
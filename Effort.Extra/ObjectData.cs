namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Linq;

    public abstract class ObjectData
    {
        private readonly MetadataWorkspace metadata;
        private readonly IDictionary<Type, ObjectDataTable> tables = new Dictionary<Type, ObjectDataTable>();

        protected ObjectData(MetadataWorkspace metadata)
        {
            this.metadata = metadata;
        }
        
        internal virtual Guid Identifier { get; } = Guid.NewGuid();
        
        /// <summary>
        /// Returns the table specified by name. If a table with the specified name does not already exist, it will be created.
        /// </summary>
        /// <typeparam name="T">The type of entity that the table should contain.</typeparam>
        /// <param name="tableName">
        /// Name of the table.
        /// <remarks>
        /// If this value is null then the name of the entity will be used.
        /// </remarks>
        /// </param>
        /// <returns>The existing table with the specified name, if it exists. Otherwise, a new table will be created.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the table exists, but the element type specified is incorrect.
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// public class Person
        /// {
        ///     public string Name { get; set; }
        /// }
        /// ...
        /// var data = new ObjectData();
        /// var table = data.Table&lt;Person>();
        /// table.Add(new Person { Name = "Fred" });
        /// table.Add(new Person { Name = "Jeff" });
        /// foreach (var person in data.Table&lt;Person>())
        /// {
        ///     Debug.Print(person.Name);
        /// }
        /// // prints:
        /// // Fred
        /// // Jeff
        /// </code>
        /// </example>
        public ObjectDataTable<T> Table<T>()
        {
            if (!tables.TryGetValue(typeof(T), out var table) || table == null)
            {
                var entitySetMapping = GetEntitySetMapping<T>(metadata);
                table = new ObjectDataTable<T>(GetTableName(entitySetMapping), GetPropertyMappings(entitySetMapping));
                tables[typeof(T)] = table;
            }
            return table as ObjectDataTable<T> ?? 
                   throw new InvalidOperationException($"A table for the type '{typeof(T).Name}' already exists, but the element type is incorrect.\r\nExpected type: '{typeof(T).Name}'\r\nActual type: '{table.GetType().GetGenericArguments()[0].Name}'");
        }
        
        private static EntitySetMapping GetEntitySetMapping<T>(MetadataWorkspace metadata)
        {
            var objectItemCollection = (ObjectItemCollection)metadata.GetItemCollection(DataSpace.OCSpace);
            var entityType = metadata.GetItems<EntityType>(DataSpace.OCSpace)
                .Single(e => objectItemCollection.GetClrType(e) == typeof(T));
            var entitySetContainer = metadata.GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);
            var entitySetMapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSpace)
                .Single()
                .EntitySetMappings
                .Single(m => m.EntitySet == entitySetContainer);
            return entitySetMapping;
        }

        private static string GetTableName(EntitySetMapping entitySetMapping)
        {
            var entitySet = entitySetMapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;
            return entitySet.MetadataProperties["Table"]?.Value?.ToString() ?? entitySet.Name;
        }
        
        private static IEnumerable<ScalarPropertyMapping> GetPropertyMappings(EntitySetMapping entitySetMapping)
        {
            var propertyMappings = entitySetMapping.EntityTypeMappings
                .Single()
                .Fragments
                .Single()
                .PropertyMappings
                .OfType<ScalarPropertyMapping>()
                .ToArray();
            return propertyMappings;
        }
        
        internal bool HasTable(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            return tables.Values.Any(t => t.TableName == tableName);
        }

        internal Type TableType(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            var table = tables.Values.SingleOrDefault(t => t.TableName == tableName);
            if (table != null)
            {
                return table.GetType().GetGenericArguments()[0];
            }
            throw new InvalidOperationException($"No table with the name '{tableName}' defined.");
        }

        internal ObjectDataTable GetTable(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            return tables.Values.SingleOrDefault(t => t.TableName == tableName);
        }
    }
}

namespace Effort.Extra
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// An object used to create and access collections of entities.
    /// </summary>
    public class ObjectData
    {
        private readonly IDictionary<string, IEnumerable> tables = new Dictionary<string, IEnumerable>();
        private readonly Guid identifier = Guid.NewGuid();
        private readonly Func<string, string> generateTableName;

        /// <summary>
        /// Initialises a new instance of <c>ObjectData</c>.
        /// </summary>
        /// <param name="tableNamingStrategy">
        /// The strategy to use when creating default table names.
        /// </param>
        public ObjectData(TableNamingStrategy tableNamingStrategy = TableNamingStrategy.EntityName)
        {
            var pluralisationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-GB"));
            switch (tableNamingStrategy)
            {
                case TableNamingStrategy.Pluralised:
                    generateTableName = pluralisationService.Pluralize;
                    break;
                case TableNamingStrategy.Singularised:
                    generateTableName = pluralisationService.Singularize;
                    break;
                case TableNamingStrategy.EntityName:
                    generateTableName = s => s;
                    break;
            }
        }

        internal virtual Guid Identifier { get { return identifier; } }

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
        /// public class Fella
        /// {
        ///     public string Name { get; set; }
        /// }
        /// ...
        /// var data = new ObjectData();
        /// var table = data.Table&lt;Fella>();
        /// table.Add(new Fella { Name = "Fred" });
        /// table.Add(new Fella { Name = "Jeff" });
        /// foreach (var fella in data.Table&lt;Fella>())
        /// {
        ///     Debug.Print(fella.Name);
        /// }
        /// // prints:
        /// // Fred
        /// // Jeff
        /// </code>
        /// </example>
        public IList<T> Table<T>(string tableName = null)
        {
            tableName = tableName ?? generateTableName(typeof(T).Name);
            IEnumerable table;
            if (!tables.TryGetValue(tableName, out table) || table == null)
            {
                table = new List<T>();
                tables[tableName] = table;
            }
            if (table is IList<T>)
            {
                return (IList<T>)table;
            }
            var message = String.Format(
                "A table with the name '{0}' already exists, but the element type is incorrect.\r\nExpected type: '{1}'\r\nActual type: '{2}'",
                tableName, typeof(T).Name, table.GetType().GetGenericArguments()[0].Name);
            throw new InvalidOperationException(message);
        }

        internal bool HasTable(string tableName)
        {
            Contract.Requires<ArgumentNullException>(tableName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(tableName));
            return tables.ContainsKey(tableName);
        }

        internal Type TableType(string tableName)
        {
            Contract.Requires<ArgumentNullException>(tableName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(tableName));
            IEnumerable table;
            if (tables.TryGetValue(tableName, out table))
            {
                return table.GetType().GetGenericArguments()[0];
            }
            throw new InvalidOperationException(String.Format("No table with the name '{0}' defined.", tableName));
        }

        internal object GetTable(string tableName)
        {
            Contract.Requires<ArgumentNullException>(tableName != null);
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(tableName));
            IEnumerable table;
            tables.TryGetValue(tableName, out table);
            return table;
        }
    }
}
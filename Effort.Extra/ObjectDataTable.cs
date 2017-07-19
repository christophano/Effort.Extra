namespace Effort.Extra
{
    using System.Collections.Generic;
    using System.Data.Entity.Core.Mapping;

    public abstract class ObjectDataTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataTable{T}"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="propertyMappings"></param>
        internal ObjectDataTable(string tableName, IEnumerable<ScalarPropertyMapping> propertyMappings)
        {
            TableName = tableName;
            PropertyMappings = propertyMappings;
        }

        public string TableName { get; }
        
        internal IEnumerable<ScalarPropertyMapping> PropertyMappings { get; }
        
        /// <summary>
        /// Gets or sets the discriminator column name.
        /// </summary>
        /// <value>
        /// The discriminator column name.
        /// </value>
        public string DiscriminatorColumn { get; set; } = "Discriminator";
    }
}
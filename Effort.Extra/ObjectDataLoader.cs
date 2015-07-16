﻿
namespace Effort.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Effort.DataLoaders;

    /// <summary>
    /// An implementation of <c>IDataLoader</c> for <c>ObjectData</c>.
    /// </summary>
    public class ObjectDataLoader : IDataLoader
    {
        private static readonly ObjectDataCollection DataCollection = new ObjectDataCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataLoader"/> class.
        /// </summary>
        public ObjectDataLoader() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataLoader"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ObjectDataLoader(ObjectData data)
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Argument = data.Identifier.ToString();
            DataCollection.AddOrUpdate(data);
        }

        /// <summary>
        /// Gets or sets the argument that describes the complete state of the data loader.
        /// </summary>
        /// <value>
        /// The argument.
        /// </value>
        public string Argument { get; set; }

        /// <summary>
        /// Creates a table data loader factory.
        /// </summary>
        /// <returns>
        /// A table data loader factory.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Thrown if no object data with a key matching the <see cref="Argument"/> is held in the <see cref="DataCollection"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the <see cref="Argument"/> is not a valid <see cref="Guid"/>.
        /// </exception>
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
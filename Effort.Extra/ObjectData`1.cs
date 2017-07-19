
namespace Effort.Extra
{
    using System.Data.Entity;

    /// <summary>
    /// An object used to create and access collections of entities.
    /// </summary>
    public class ObjectData<TContext> : ObjectData where TContext : DbContext
    {
        /// <summary>
        /// Initialises a new instance of <c>ObjectData</c>. This constructor should only be called for Model First or Database First Scenarios.
        /// </summary>
        /// <param name="modelName">
        /// The name of the model, as used in the entity connection string metadata.
        /// This value must be provided for Model First or Database First scenarios and must be omitted for Code First scenarios.
        /// </param>
        public ObjectData(string modelName = null) : base(MetadataWorkspaceFactory.Create<TContext>(modelName)) { }
    }
}
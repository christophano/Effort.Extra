
namespace Effort.Extra
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public static class MetadataWorkspaceFactory
    {
        public static MetadataWorkspace Create<T>(string modelName) where T : DbContext
        {
            return String.IsNullOrWhiteSpace(modelName)
                ? CreateForCodeFirst<T>()
                : CreateForModelOrDatabaseFirst<T>(modelName);
        }
        
        private static MetadataWorkspace CreateForModelOrDatabaseFirst<T>(string modelName) where T : DbContext
        {
            return new MetadataWorkspace(new[] { $"res://*/{modelName}.csdl", $"res://*/{modelName}.ssdl", $"res://*/{modelName}.msl" }, new[] {typeof(T).Assembly});
        }

        private static MetadataWorkspace CreateForCodeFirst<T>() where T : DbContext
        {
            var ctor = typeof(T).GetConstructor(new[] {typeof(string)}) ?? throw new InvalidOperationException($"Type: '{typeof(T).Name}' must have a constructor accepting a connection string.");
            var context = (T)ctor.Invoke(new object[] {"App=Effort.Extra"});
            try
            {
                using (var stream = new MemoryStream())
                using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    EdmxWriter.WriteEdmx(context, writer);
                    stream.Seek(0, SeekOrigin.Begin);
                    var edmx = XDocument.Load(stream);
                    if (edmx.Root == null) throw new InvalidOperationException("Generated edmx file is empty");
                    var runtime = edmx.Root.Elements().First(e => e.Name.LocalName == "Runtime");

                    var conceptualModels = runtime.Elements().First(e => e.Name.LocalName == "ConceptualModels").Elements().First();
                    var storageModels = runtime.Elements().First(e => e.Name.LocalName == "StorageModels").Elements().First();
                    var mappings = runtime.Elements().First(e => e.Name.LocalName == "Mappings").Elements().First();
                    
                    var items = new EdmItemCollection(new[] { XmlReader.Create(new StringReader(conceptualModels.ToString())) });
                    var storageItems = new StoreItemCollection(new[] { XmlReader.Create(new StringReader(storageModels.ToString())) });
                    var mappingCollection = new StorageMappingItemCollection(items, storageItems, new[] { XmlReader.Create(new StringReader(mappings.ToString())) });
                    
                    return new MetadataWorkspace(() => items, () => storageItems, () => mappingCollection);
                }
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
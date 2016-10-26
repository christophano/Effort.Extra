[![Build status](https://ci.appveyor.com/api/projects/status/nmnb8cobvp6t5w7u?svg=true)](https://ci.appveyor.com/project/christophano/effort-extra)

# Extra Effort
Extra Effort is an extension to the excellent [Effort](https://github.com/tamasflamich/effort) library that provides an additional data loader implementation.


### Data Loaders
Data Loaders are a method by which you can initialise your database with data already loaded.
Effort comes with a couple of data loaders build in (read more on [Tamás Flamich's blog](https://tflamichblog.wordpress.com/2013/01/22/data-loaders-in-effort/)).
The Extra Effort data loader allows you to create entities in code and add them to strongly typed collections, where each collection represents an Entity Framework set.

### Usage
Using the data loader is as simple as passing it to the Effort connection factory methods, as in the example below.
```csharp
var data = new ObjectData();
// add entities
var dataLoader = new ObjectDataLoader(data);
var connection = DbConnectionFactory.CreateTransient(dataLoader);
```

Adding entities to the `ObjectData` instance is as simple as calling the `Table` method with the generic parameter of the type you want to add. The table name is optional.
```csharp
data.Table<Fella>().Add(new Fella { Name = "Jeff" });
```

If the entity contains a db generated identity field, then this should be provided too. This will enable you to create relations between entities.
```csharp
data.Table<Fella>().Add(new Fella { Id = 1, Name = "Jeff" });
data.Table<Dog>().Add(new Dog { Id = 1, OwnerId = 1, Name = "Jim" });
```

If your schema uses a different naming convention that the entity type name, then you can simply provide the set name when calling the `Table` method.
```csharp
data.Table<Fella>("People").Add(new Fella { Id = 1, Name = "Jeff" });
```

If your schema calls for multiple sets of the same type, each collection is keyed on the set name, so multiple sets are supported.
```csharp
data.Table<Fella>("SomeFellas").Add(new Fella { Id = 1, Name = "Jeff" });
data.Table<Fella>("OtherFellas").Add(new Fella { Id = 1, Name = "Jim" });
```

# ObjectPort
Ultra fast binary serializer for .NET

## Features

* Serialization of all primitive types, strings, enums, date types
* Serialization of arrays and collections: List, HashSet, LinkedList, Queue, SortedSet, Stack
* Serialization of dictionaries: Dictionary, SortedList, SortedDictionary
* Any nesting level of arrays or dictionaries (i.e. arrays of arrays, dictionaries of dictionaries, arrays of dictionaries, etc)
* Polymorphic hierarchies serialization
* Anonymous types serializations
* .NET Core support
* The fastest in the class of binary serializers (see [Benchmarks](Benchmarks.md))

## Usage
In order to serialize / deserialize a custom type it should be registered:
```csharp
Serializer.RegisterTypes(new [] { typeof(MyType1), typeof(MyType2) })
```
to serialize:
```csharp
Serializer.Serialize(stream, myObj);
```

to deserialize:
```csharp
var myObj = (MyObjType)Serializer.Deserialize(stream);
```
## Documentation
More details about ObjectPort can be found at the [Documentation](Documentation.md) section.

# ObjectPort

## Getting Started
To install ObjectPort run the following command from the [Package Manager Console](http://docs.nuget.org/ndocs/tools/package-manager-console):
```PowerShell
Install-Package ObjectPort
```
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
or
```csharp	
var myObj = Serializer.Deserialize<MyObjType>(stream);
```

## Overview

ObjectPort is ultra-fast (faster than most of analogous solutions - see [Benchmarks](Benchmarks.md)) binary serializer. It isn't version tolerant (i.e. it doesn't store any version information and therefore can't handle properly different versions of an object). So its main application area is messages serialization however it also can be used for the objects persistence in case if the version awareness isn't required. ObjectPort supports serialization of most of .NET primitive types, anonymous types, polymorphic hierarchies, wide range of .NET collections and dictionaries.
In order to serialize or deserialize a custom type this type should be registered inside the ObjectPort first. This allows to build compiled serialization/deserialization mechanism which is able to write instances of the type to a stream and build the instances of the type by reading their serialized information from a stream. Supported BCL types (like primitive types, collections, dictionaries, etc) or non-root types don't require explicit registration (see Registration for more details). Once registered the registration information is kept during entire lifecycle of the application so after the registration a custom type can be serialized or deserialized at any point of execution. 

## Registration

As it has been already mentioned the registration is obligatory part of serialization and deserialization process which is intended to create a run-time infrastructure which in turn "knows" how to serialize or deserialize the specific types (those which were registered). Internally the registration creates set of compiled lambda expressions which are responsible to serialize or deserialize specific type and assign some identifier (type id) to these compiled expressions. This type id is crucial part of the registration since this is only way to let ObjectPort know which type should be deserialized from a stream. 
Typically type identifiers are specified during the types registrations as parameters:
```csharp
Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [1] = typeof(MyType1), [2] = typeof(MyType2) });
```
Or if they are omitted the identifiers are automatically generated and assigned to the type registrations:
```csharp	
Serializer.RegisterTypes(new [] { typeof(MyType1), typeof(MyType2) })
```
The latter scenario may be not practical in case of distributed instances which exchange the serialized data because there is no guarantee that type identifiers will be identical for different instances. Therefore for such scenarios the registration with explicit type identifiers specification is recommended option. However if number of registrations and their order are the same for all instances the types will be assigned to consistent identifiers which will guarantee the same interpretation of a type id for all instances.

## Root and non-root types
Every custom type which instance is passed to the serializer explicitly is treated as root type. In the following example MyType is root type.
```csharp
var myObj = new MyType();
...
Serializer.Serialize(stream, myObj);
```
Root types require explicit registration to let the serializer know how to serialize or deserialize them. Without that any attempt to serialize or deserialize a root type will generate exception. A non-root types are types which are referenced by a root type or another non-root type. In the following example MyTypeNonRoot is non-root type
```csharp
public class MyNonRootType
{
	public int Member;
}

public class MyRootType
{
	public MyNonRootType Member1;
	public int Member2;
}
...
Serializer.RegisterTypes(new [] { typeof(MyRootType) })

var myObj = new MyRootType ();
...
Serializer.Serialize(stream, myObj);
```
because there is no code which would serialize it explicitly – instead it’s serialized with the MyTypeRoot serialization.

## Implicit Registration

In some cases the explicit registration of a type can be omitted but please note that it concerns only scenarios when type identifiers can be generated automatically (see Registration for more details) because in case of implicit registration there is no way to control a preferable type id. So implicit registration feature can be utilized in the following cases: 
- if there is single instance of the application which doesn't need to share the registrations information with other instances
- if multiple instances of the application use the same registration procedure (the number and order of registrations are identical)
- for the debugging purposes.

For all other cases it's recommended to explicitly register all necessary types with their identifiers.
In contrast to the root types non-root types don't require explicit registration, they are registered automatically during the registration of their root types. So, in previous example ObjectPort will be able to detect that MyTypeNonRoot should be registered during the MyTypeRoot registration since it performs traversal of the entire root type graph (all members, sub-members and so on) and registers all types referenced by the members of the root type graph.
Apart from the automatic registration of underlying types for any root type ObjectPort supports the following forms of the implicit registration:
- implicit registration of a collection element type
- implicit registration of a dictionary key and value types
- implicit registration of a Nullable underlying type
- implicit registration of all primitive types

As for the last item, unlike the custom types implicit registration when they obtain dynamic type ids the primitive types always are assigned with the same type ids from reserved range which starts with 65000. So it's guaranteed that every primitive type will be identically recognized for the all application instances.


## Polymorphic Serialization

ObjectPort is able to serialize and deserialize polymorphic hierarchies of types as part of collections or dictionary elements or type members. Consider the following example:
```csharp
public class MyBaseType
{
	public int Member;
}

public class MyDerivedType1 : MyBaseType
{
	public int Member1;
}

public class MyDerivedType2 : MyDerivedType1
{
	public int Member2;
}

public class MyRootType
{
    public MyBaseType PolymorphicMember;
    public IEnumerable<MyBaseType> CollectionOfPolymorphicElements;
}
```
If it's needed to serialize or deserialize MyRootType where PolymorphicMember can be initialized with any of type in the MyBaseType hierarchy all derived types should be explicitly registered:
```csharp
Serializer.RegisterTypes(new [] { typeof(MyBaseType), typeof(MyDerivedType1), typeof(MyDerivedType2) })
```
or
```csharp
Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [1] = typeof(MyBaseType), [2] = typeof(MyDerivedType1), [3] = typeof(MyDerivedType2) })
```
for explicit registration.
Then ObjectPort can serialize or deserialize the types which reference the hierarchy:
```csharp
var myObj = new MyRootType
{
    PolymorphicMember = new MyBaseType
    {
        Member = 10
    },
    CollectionOfPolymorphicElements = new MyBaseType[]
    {
        new MyBaseType { Member = 20 },
        new MyDerivedType1 { Member = 21, Member1 = 22 },
        new MyDerivedType2 { Member = 23, Member1 = 24,  Member1 = 25 }
    }
}
...
Serializer.Serialize(stream, myObj);
...
var myDeserializedObj = Serializer.Deserialize(stream);
```
or
```csharp
var myObj = new MyRootType
{
    PolymorphicMember = new MyDerivedType2
    {
        Member = 10,
        Member1 = 11,
        Member2 = 12
    },
    CollectionOfPolymorphicElements = new MyBaseType[]
    {
        new MyBaseType { Member = 20 },
        new MyDerivedType1 { Member = 21, Member1 = 22 },
        new MyDerivedType2 { Member = 23, Member1 = 24,  Member1 = 25 }
    }    
}
...
Serializer.Serialize(stream, myObj);
...
var myDeserializedObj = Serializer.Deserialize(stream);
```
In case if the root of a hierarchy is an abstract class or an interface they shouldn't be registered:
```csharp
public interface IMyInterface
{
	public int Member;
}

public class MyDerivedType1 : IMyInterface
{
	public int Member1;
}

public class MyDerivedType2 : MyDerivedType1
{
	public int Member2;
}

public class MyRootType
{
    public IMyInterface PolymorphicMember;
    public IEnumerable<IMyInterface> CollectionOfPolymorphicElements;
}
...
Serializer.RegisterTypes(new [] { typeof(MyDerivedType1), typeof(MyDerivedType2) })
```
## Anonymous Types Serialization

Probably the most unique feature ob ObjectPort is ability to serialize and deserialize anonymous types. Frnakly speaking that was the initial motivation which the rest of the functionality has grown up around. So the serialization and deserialization of an anonymous object may look as follows:
```csharp
public class MyClass
{
    public int Member;
}

var myObj = new
{
    Field1 = 343,
    Field2 = "45454",
    Field3 = new
    {
        Field7 = new MyClass
        {
            Member = 555,
        },
    }
}
...
Serializer.RegisterTypes(new[] { myObj.GetType() });
...
Serializer.Serialize(stream, myObj);
...
var deserializedObj = Serializer.Deserialize(stream);
```
So even if only deserialization is needed it still requires the registration of sample anonymous object type and the structure of sample object should be identical to what is expected to be serialzied.

## Concurrency

ObjectPort calls are concurrency tolerant. That concerns all its calls but there are some performance aspects regarding calling different methods of ObjectPort in concurrent way which should be taken into consideration. It's safe to call RegisterTypes, Serialize and Deserialize methods concurrently but multiple concurrent RegisterTypes call may introduce some latency because these methods are lock based unlike Serialize and Deserialize which utilize some kind of lock-free algorithm.

## Stream Format
TBD



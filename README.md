# DevExpress.Data.Extensions

This project currently contains only one class: **CriteriaBuilder**. It parses [Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/) and builds a [CriteriaOperator](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.class) object.

## Usage

The **CriteriaBuilder** class is intended for the sole purpose of building [CriteriaOperator](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.class) objects in a type-safe manner. It supports a limited set of expressions. Those that are necessary to define an expression compatible with [Criteria Language Syntax](https://documentation.devexpress.com/CoreLibraries/4928).

The **CriteriaBuilder** class has only one public method: **Build**. This method has accepts type arguments: **TContext** and **TResult**.

* **TContext** - The root class against which the [CriteriaOperator](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.class) will be evaluated. For example, to build a [CriteriaOperator](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.class) for a `List<Person>` object, pass the `Person` class to the **Build** method.
* **TResult** (optional) - The expression result type. It stands for type safety only. The actual type of an object retrieved after evaluating a [CriteriaOperator](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.class) object is not guaranteed to be the same as **TResult**. This argument is optional. Without **TResult** provided the result type is considered to be **bool**.

The **Build** method accepts one argument: **exp**. **exp** is an [Expression Tree](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/) that can be automatically created from a lambda expression by the C# compiler.

Here is the short list of supported features:

1. Binary operations:

    ```csharp
    CriteriaOperator filter = CriteriaBuilder.Build<Person>(p => p.Name == "John");
    ```

    Produces: *[Name] = 'John'*.

2. Operations with collections:

    ```csharp
    CriteriaOperator filter = CriteriaBuilder.Build<Person>(p => p.Accounts.Average(a => a.Amount) == 75);
    ```

    Produces: *[Accounts][].Avg([Amount]) = 75.0m*.

3. Build-in functions, such as string.Concat, string.Contains, string.EndsWith, etc.

    ```csharp
    CriteriaOperator expr = CriteriaBuilder.Build<Person, string>(p => string.Concat(p.FirstName, " ", p.LastName)); 
    ```

    Produces: *Concat([FirstName], ' ', [LastName])*.

4. Custom functions. If a method name matches a [registered custom function](https://documentation.devexpress.com/CoreLibraries/DevExpress.Data.Filtering.CriteriaOperator.RegisterCustomFunction.method), the method call will be translated into a custom function operator.

    ```charp
    CriteriaOperator expr = CriteriaBuilder.Build<MyClass, string>(c => MyFunctions.MyFunction(c.Name, "John"));
    // ..
    public static class MyFunctions {
      publci staic string MyFunction(string x, string y) { .. }
    }
    ```

    Produces: *MyFunction([Name], 'John')*.

5. DateTime functions.

    ```csharp
    CriteriaOperator expr = CriteriaBuilder.Build<Order, DateTime>(o => o.OrderDate.AddDays(1));
    ```

    Produces: *AddDays([OrderDate], 1.0)*

6. Math functions.

    ```csharp
    CriteriaOperator expr = CriteriaBuilder.Build<MyClass, double>(c => Math.Acos(c.Angle));
    ```

    Produces: *Acos([Angle])*.

7. Match any value in an array.

    ```csharp
    string[] names = new string[] { "John", "Bob", "Nick" };
    CriteriaOperator filter = CriteriaBuilder.Build<Person>(p => names.Contains(p.Name));
    ```

    Produces: *[Name] In ('John', 'Bob', 'Nick')*.

8. Ternary opeator.

    ```csharp
    CriteriaOperator expr = CriteriaBuilder.Build<Task, int>(t =>
      t.Status == "Pending" ? 0 :
      t.Status == "Running" ? 1 : 2);
    ```

    Produces: *Iif([Status] = 'Pending', 1, [Status] = 'Running', 2, 3)*.

For more examples, refer to [**Criteria Builder** Tests](https://github.com/Astak/DevExpress.Data.Extensions/tree/master/DevExpress.Data.Extensions.Tests/CriteriaBuilderTests).

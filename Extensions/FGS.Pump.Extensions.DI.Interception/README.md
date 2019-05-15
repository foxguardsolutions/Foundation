# Dynamic Interception

## Overview

The `FGS.Pump.Extensions.DI.Interception` project provides types and methods to wire up _interceptors_ using custom attributes on classes and methods.

Interceptors a proxy objects that are written and instantiated dynamically at runtime.

![proxy design pattern in UML](https://upload.wikimedia.org/wikipedia/commons/thumb/7/75/Proxy_pattern_diagram.svg/1920px-Proxy_pattern_diagram.svg.png) image source: Wikipeda

Interceptors are often used to augment the functionality of existing classes to handle cross-cutting concerns such as database transaction management, logging, or fault tolerance.

## Adding a new interceptor

This project abstracts away nearly all the complexity of setting up an attribute-based interceptor. As a result, adding a new interceptor is a simple three-step process.

### 1. Create a custom attribute

```cs
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MyAttribute : Attribute
{
    // Optional properties and constructors
}
```

### 2. Create an interceptor

```cs
using FGS.Pump.Extensions.DI.Interception;

public class MyInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        // Do whatever you need before the method call

        // Proceed with the invocation of the intercepted method
        invocation.Proceed();

        // Do whatever you need after the method call
        // `invocation.ReturnValue` will hold the return value from the invocation if you need to access it.
    }

    public async Task InterceptAsync(IAsyncInvocation invocation)
    {
        // Do whatever you need to before the method call; awaits are supported

        // Proceed with the invocation of the intercepted method (be sure to await it)
        await invocation.ProceedAsync();

        // Do whatever you need after the method call
        // `invocation.ReturnValue` will hold the return value from the invocation if you need to access it.
    }
}
```

If you need access to the attribute (e.g. to get a property value from it), you can add it as a constructor parameter and it will be injected when the interceptor is created.

### 3. Register the interceptor

```cs
using FGS.Pump.Extensions.DI.Interception;

public class MyAutofacModule : Autofac.Module
{
    protected override void Load(Autofac.ContainerBuilder builder)
    {
        // other registrations
        // ...

        builder.RegisterModule<AttributeBasedInterceptionModule<MyAttribute, MyInterceptor>>();
    }
}
```

## Using an interceptor

Given an interceptor like the one described above, applying to classes/methods in the solution is trivial. There are two ways to do this.

### Add the custom attribute to a method

```cs
public void SomeClass
{
    [MyAttribute]
    public virtual void Foo()
    {
        // ...
    }
}
```

The method **must be virtual** since the generated proxy will be a subclass of `SomeClass` that overrides `Foo()`.

### Add the custom attribute to a class

```cs
[MyAttribute]
public void SomeClass
{
    public virtual void Foo()
    {
        // ...
    }
}
```

Adding the attribute to the class is shorthand for adding it to all of the virtual methods of the class.

## Using multiple interceptors

Multiple intercepors can be added to the same class or method to attach multiple behaviors to it. In such cases, the generated proxies will form a chain and the `IInvocation.Proceed()` and `IAsyncInvocation.ProceedAsync()` methods will invoke the next link in the chain. The last interceptor in the chain will point to the original class or method.

Multiple attributes can be added to the same method

```cs
public void SomeClass
{
    [MyAttribute]
    [MyOtherAttribute]
    public virtual void Foo()
    {
        // ...
    }
}
```

Multiple attributes can be added to the same class

```cs
[MyAttribute]
[MyOtherAttribute]
public void SomeClass
{
    public virtual void Foo()
    {
        // ...
    }
}
```

Finally attributes can be added to both the class and the method

```cs
[MyAttribute]
public void SomeClass
{
    [MyOtherAttribute]
    public virtual void Foo()
    {
        // ...
    }
}
```

Note the ordering of the chain of interceptors is dependent only on the order in which the respective Autofac registrations are executed and not on the order the attributes are listed on the method and/or class.

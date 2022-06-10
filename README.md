[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=quokka-dev_quokkadev-cqrs-decorators&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=quokka-dev_quokkadev-cqrs-decorators) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=quokka-dev_quokkadev-cqrs-decorators&metric=coverage)](https://sonarcloud.io/summary/new_code?id=quokka-dev_quokkadev-cqrs-decorators) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=quokka-dev_quokkadev-cqrs-decorators&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=quokka-dev_quokkadev-cqrs-decorators) [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=quokka-dev_quokkadev-cqrs-decorators&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=quokka-dev_quokkadev-cqrs-decorators) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=quokka-dev_quokkadev-cqrs-decorators&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=quokka-dev_quokkadev-cqrs-decorators) ![publish workflow](https://github.com/quokka-dev/quokkadev-cqrs/actions/workflows/publish.yml/badge.svg) 

# QuokkaDev.Cqrs.Decorators

QuokkaDev.Cqrs.Decorators is a package containing some decorators for extending ICommandDispather and IQueryDispatcher with some useful cross cutting concerns. Actually 4 decorators are provided:

- `CommandLoggerDecorator`
- `QueryLoggerDecorator`
- `CommandValidationDecorator`
- `QueryValidationDecorator`

## CommandLoggerDecorator
Allow logging command request and responses
#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
builder.Services.AddCommandLogging(opts => { 
    opts.IsResponseLoggingEnabled = true;
    opts.IsResponseLoggingEnabled = true;
});
```

## QueryLoggerDecorator
Allow logging query request and responses

#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
builder.Services.AddQueryLogging(opts => { 
    opts.IsResponseLoggingEnabled = true;
    opts.IsResponseLoggingEnabled = true;
});
```

## CommandValidationDecorator
Validate a command using a Validator created with [FluentValidation](https://docs.fluentvalidation.net/en/latest)

#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
builder.Services.AddCommandValidation();
```
Please, note that the extension method only register the validation decorator, it is your responsibility register Validators in the Dependency Injection Container

## QueryValidationDecorator
Validate a query using a Validator created with [FluentValidation](https://docs.fluentvalidation.net/en/latest)

#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
builder.Services.AddQueryValidation();
```
Please, note that the extension method only register the validation decorator, it is your responsibility register Validators in the Dependency Injection Container

## Register multiple decorators
You can combine multiple decorators on `ICommandDispatcher` or `IQueryDispatcher` using 'And' extensions methods. For example for registering Validation and Logging decorators to `IQueryDispatcher` try:

#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
builder.Services.AddQueryValidation().AndQueryLogging(opts => { 
    opts.IsResponseLoggingEnabled = true;
    opts.IsResponseLoggingEnabled = true;
});
```

The decorators are executed in reverse order respect to the extensions method calls. In the abbove example the execution order is:

`QueryLoggingDecorator` --> `QueryValidationDecorator` --> `QueryDispatcher`

## Create custom decorators
You can create your custom decorator implementing `ICommandDispatcher` or `IQueryDispatcher`. For Example:

#### **`program.cs`**
```csharp
public class MyCustomDecorator : IQueryDispatcher
{
    private readonly IQueryDispatcher dispatcher;    
    public MyCustomDecorator(IQueryDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;        
    }
    public async Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation) where TQuery : IQuery<TQueryResult>
    {
        // Run code before dispatching query
        var result = await dispatcher.Dispatch<TQuery, TQueryResult>(query, cancellation);
        // Run code after query dispatched
        return result
    }
    public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult>
    {
        return Dispatch<TQuery, TQueryResult>(query, CancellationToken.None);
    }
}
```
For register your custom decorator you must decorate the `IQueryDispatcher` interface. You can do it using [Scrutor](https://github.com/khellang/Scrutor) (QuokkaDev.Cqrs.Decorators use Scrutor in his extensions method)
#### **`program.cs`**
```csharp
builder.Services.AddCQRS(Assembly.GetExecutingAssembly());
// Register validation and logging
builder.Services.AddQueryValidation().AndQueryLogging(opts => { 
    opts.IsResponseLoggingEnabled = true;
    opts.IsResponseLoggingEnabled = true;
});
// Register MyCustomValidator using Scrutor extensions methods
builder.Services.Decorate<IQueryDispatcher>((inner, provider) => new MyCustomeDecorator(inner));
```
The result is:
`MyCustomValidator` --> `QueryLoggingDecorator` --> `QueryValidationDecorator` --> `QueryDispatcher`

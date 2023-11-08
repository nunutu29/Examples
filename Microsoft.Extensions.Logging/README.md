# Usage

```csharp
public MyClass
{
    public MyClass(ILogger<MyClass> logger)
    {
        _logger = new LoggerHelper<MyClass>(logger).WithProperty("name", "SomeValue");
    }

    private readonly ILogger<MyClass> _logger;
}
```

Ever wanted to add a custom property only inside the logger of that particular instance ? Here you go.

In my case I had a worker class instantiated multiple times in parallel with an unique name each. I needed that every single instance could write that name without the need of repeat it in every log line.

I was also using NLog, but I guess this solution works with other loggers too.

## Usage

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

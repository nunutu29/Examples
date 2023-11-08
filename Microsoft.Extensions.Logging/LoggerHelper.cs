using Microsoft.Extensions.Logging;
using System.Collections;

namespace Example;

public partial class LoggerHelper<T> : ILogger<T>
{
    private readonly ILogger _logger;

    public LoggerHelper(ILogger logger)
    {
        _logger = logger;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (_extraProperties != null)
        {
            var customState = new CustomState<TState>(state, formatter, _extraProperties);
            _logger.Log(logLevel, eventId, customState, exception, customState.Formatter);
        }
        else
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}

public partial class LoggerHelper<T>
{
    private List<KeyValuePair<string, object>> _extraProperties;

    public LoggerHelper<T> WithProperty(string name, object value)
    {
        var properties = _extraProperties ??= new List<KeyValuePair<string, object>>();
        properties.Add(new KeyValuePair<string, object>(name, value));
        return this;
    }
    
    private class CustomState<TState> : IEnumerable<KeyValuePair<string, object>>
    {
        public CustomState(TState state, 
            Func<TState, Exception, string> formatter,
            IEnumerable<KeyValuePair<string, object>> customProps)
        {
            _state = state;
            _formatter = formatter;

            var list = new List<KeyValuePair<string, object>>();

            list.AddRange(customProps);

            if (_state is IReadOnlyList<KeyValuePair<string, object>> originalProps)
            {
                list.AddRange(originalProps);
            }

            _props = list;

        }

        private readonly TState _state;
        private readonly Func<TState, Exception, string> _formatter;
        private readonly IList<KeyValuePair<string, object>> _props;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _props.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public string Formatter(CustomState<TState> customState, Exception ex)
        {
            var formatter = customState._formatter;
            var state = customState._state;

            return formatter(state, ex);
        }
    }
}

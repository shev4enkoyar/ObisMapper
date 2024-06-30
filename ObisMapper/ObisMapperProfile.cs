using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ObisMapper;

public class ObisMappingConfiguration<TDestination>
{
    private readonly Dictionary<string, Action<TDestination, object, string>> _mappings = new();
    private readonly List<(Regex, Action<TDestination, object, string>)> _patternMappings = new();

    public ObisMappingConfiguration<TDestination> CreateMap<TProperty>(IEnumerable<string> obisCodes,
        Expression<Func<TDestination, TProperty>> propertyExpression,
        Func<object, TProperty?, TProperty>? transform = null)
    {
        var propertyInfo = GetPropertyInfo(propertyExpression);
        if (propertyInfo == null)
            throw new InvalidOperationException("Invalid property expression");

        foreach (var obisCode in obisCodes) SetObisProcessor(obisCode, propertyInfo, transform);

        return this;
    }

    public ObisMappingConfiguration<TDestination> CreateMap<TProperty>(string obisCode,
        Expression<Func<TDestination, TProperty>> propertyExpression,
        Func<object, TProperty?, TProperty>? transform = null)
    {
        var propertyInfo = GetPropertyInfo(propertyExpression);
        if (propertyInfo == null)
            throw new InvalidOperationException("Invalid property expression");

        SetObisProcessor(obisCode, propertyInfo, transform);
        return this;
    }

    public ObisMappingConfiguration<TDestination> CreateMap(string obisPattern,
        Action<TDestination, object, string> action)
    {
        var regex = new Regex(obisPattern);
        _patternMappings.Add((regex, action));
        return this;
    }

    private static PropertyInfo? GetPropertyInfo<TProperty>(
        Expression<Func<TDestination, TProperty>> propertyExpression)
    {
        return propertyExpression.Body switch
        {
            MemberExpression memberExpr => memberExpr.Member as PropertyInfo,
            UnaryExpression { Operand: MemberExpression memberExpr } => memberExpr.Member as PropertyInfo,
            _ => null
        };
    }

    private void SetObisProcessor<TProperty>(string obisCode, PropertyInfo propertyInfo,
        Func<object, TProperty?, TProperty>? transform = null)
    {
        _mappings[obisCode] = (dest, value, currentObis) =>
        {
            var currentValue = propertyInfo.GetValue(dest);
            var transformedValue = transform != null ? transform(value, (TProperty?)currentValue) : (TProperty)value;
            propertyInfo.SetValue(dest, transformedValue);
        };
    }

    public Action<TDestination, object, string>? GetMapping(string obisCode)
    {
        return _mappings.GetValueOrDefault(obisCode);
    }

    public Action<TDestination, object, string>? GetPatternMapping(string obisCode)
    {
        foreach (var (regex, action) in _patternMappings)
            if (regex.IsMatch(obisCode))
                return action;

        return null;
    }
}
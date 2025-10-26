using System.Linq;
using System.Linq.Dynamic.Core;

namespace BlogApp.Domain.Common.Dynamic;

public static class IQueryableDynamicFilterExtensions
{
    private static readonly string[] _orders = { "asc", "Ascending", "desc", "Descending" };
    private static readonly string[] _logics = { "and", "or", "And", "Or" };

    private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
    {
        { "eq", "=" },
        { "equals", "=" },
        { "neq", "!=" },
        { "notequals", "!=" },
        { "lt", "<" },
        { "lessthan", "<" },
        { "lte", "<=" },
        { "lessthanorequals", "<=" },
        { "gt", ">" },
        { "greaterthan", ">" },
        { "gte", ">=" },
        { "greaterthanorequals", ">=" },
        { "isnull", "== null" },
        { "isnotnull", "!= null" },
        { "startswith", "StartsWith" },
        { "endswith", "EndsWith" },
        { "contains", "Contains" },
        { "doesnotcontain", "Contains" }
    };

    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery? dynamicQuery)
    {
        if (dynamicQuery?.Filter is not null)
            query = Filter(query, dynamicQuery.Filter);
        if (dynamicQuery?.Sort is not null && dynamicQuery.Sort.Any())
            query = Sort(query, dynamicQuery.Sort);
        return query;
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        IList<Filter> filters = GetAllFilters(filter);
        string where = Transform(filter, filters);

        if (string.IsNullOrWhiteSpace(where))
            return queryable;

        object?[] parameters = filters.Select(f => (object?)f.Value).ToArray();
        queryable = queryable.Where(where, parameters);

        return queryable;
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        foreach (Sort item in sort)
        {
            if (string.IsNullOrEmpty(item.Field))
                throw new ArgumentException("Invalid Field");
            if (string.IsNullOrEmpty(item.Dir) || !_orders.Contains(item.Dir))
                throw new ArgumentException("Invalid Order Type");
        }

        if (sort.Any())
        {
            string ordering = string.Join(separator: ",", values: sort.Select(s => $"{s.Field} {s.Dir}"));
            return queryable.OrderBy(ordering);
        }

        return queryable;
    }

    public static IList<Filter> GetAllFilters(Filter filter)
    {
        List<Filter> filters = new();
        GetFilters(filter, filters);
        return filters;
    }

    private static void GetFilters(Filter filter, IList<Filter> filters)
    {
        filters.Add(filter);
        if (filter.Filters is not null && filter.Filters.Any())
            foreach (Filter item in filter.Filters)
                GetFilters(item, filters);
    }

    public static string Transform(Filter filter, IList<Filter> filters)
    {
        bool hasChildren = filter.Filters is not null && filter.Filters.Any();
        if (hasChildren)
        {
            string? logic = filter.Logic;
            if (string.IsNullOrWhiteSpace(logic))
                throw new ArgumentException("Invalid Logic");

            string logicNormalized = logic.ToLowerInvariant();
            if (!_logics.Contains(logic) && !_logics.Contains(logicNormalized))
                throw new ArgumentException("Invalid Logic");

            string[] childExpressions = filter.Filters!
                .Select(child => Transform(child, filters))
                .Where(expression => !string.IsNullOrWhiteSpace(expression))
                .ToArray();

            if (childExpressions.Length == 0)
                return string.Empty;

            string joinedExpression = string.Join($" {logicNormalized} ", childExpressions);
            return childExpressions.Length > 1 ? $"({joinedExpression})" : joinedExpression;
        }

        if (string.IsNullOrWhiteSpace(filter.Field))
            throw new ArgumentException("Invalid Field");

        string operatorToken = (filter.Operator ?? string.Empty).ToLowerInvariant();
        if (!_operators.TryGetValue(operatorToken, out string? comparison))
            throw new ArgumentException("Invalid Operator");

        string comparisonValue = comparison!;

        int index = filters.IndexOf(filter);
        if (index < 0)
            throw new ArgumentException("Filter could not be located in collection");

        if (operatorToken is not ("isnull" or "isnotnull") && string.IsNullOrWhiteSpace(filter.Value))
            return string.Empty;

        if (operatorToken == "doesnotcontain")
        {
            string? loweredValue = filters[index].Value?.ToLowerInvariant();
            filters[index].Value = loweredValue;
            return $"(!np({filter.Field}).ToLower().Contains(@{index}))";
        }

        if (comparisonValue is "StartsWith" or "EndsWith" or "Contains")
        {
            string? loweredValue = filters[index].Value?.ToLowerInvariant();
            filters[index].Value = loweredValue;
            return $"(np({filter.Field}).ToLower().{comparisonValue}(@{index}))";
        }

        if (operatorToken is "isnull" or "isnotnull")
            return $"np({filter.Field}) {comparisonValue}";

        return $"np({filter.Field}) {comparisonValue} @{index}";
    }
}

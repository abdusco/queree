using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StringToExpression.LanguageDefinitions;

namespace Queree.DynamicQuery
{
    public static class QueryableExtensions
    {
        private static readonly ODataFilterLanguage ODataFilter = new ODataFilterLanguage();

        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> queryable, Query query)
        {
            if (query == null) return queryable;

            return queryable
                .ApplyFilter(query.Filter)
                .ApplyOrderBy(query.OrderBy)
                .ApplyPagination(query.Skip, query.Top);
        }

        private static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, int skip, int top) =>
            queryable.Skip(skip).Take(top);

        private static IQueryable<T> ApplyFilter<T>(this IQueryable<T> queryable, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return queryable;
            }

            try
            {
                var pred = ODataFilter.Parse<T>(filter);
                return queryable.Where(pred);
            }
            catch (Exception e)
            {
                throw new FormatException("invalid filter", e);
            }
        }

        private static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> queryable, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return queryable;
            }

            var parts = orderBy.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var descending = false;
            if (parts.Length > 1)
            {
                descending = parts[1].ToLowerInvariant() == "desc";
            }

            var propertyName = parts[0];
            return descending ? queryable.OrderByDescending(propertyName) : queryable.OrderBy(propertyName);
        }

        private static IOrderedQueryable<T> OrderBy<T>(
            this IQueryable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        private static IOrderedQueryable<T> OrderByDescending<T>(
            this IQueryable<T> source,
            string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            string property,
            string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<T>) result;
        }
    }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using StringToExpression.LanguageDefinitions;

namespace Queree
{
    public class Query
    {
        private const int MaxPageSize = 50;
        private int _top = MaxPageSize;
        [FromQuery(Name = "$skip")] public int Skip { get; set; } = 0;

        [FromQuery(Name = "$top")]
        public int Top
        {
            get => _top;
            set => _top = Math.Min(MaxPageSize, value);
        }

        [FromQuery(Name = "$filter")] public string Filter { get; set; }
        [FromQuery(Name = "$orderby")] public string OrderBy { get; set; }
    }

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

            var selector = CreateSelectorExpression<T>(parts[0]);
            return descending ? queryable.OrderByDescending(selector) : queryable.OrderBy(selector);
        }


        public static Expression<Func<T, string>> CreateSelectorExpression<T>(string propertyName)
        {
            var paramterExpression = Expression.Parameter(typeof(T));
            return (Expression<Func<T, string>>) Expression.Lambda(
                Expression.PropertyOrField(paramterExpression, propertyName),
                paramterExpression);
        }
    }

    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(Query query)
        {
            if (query.OrderBy != null)
            {
            }

            return Ok(_dbContext.Users.ApplyQuery(query).ToList());
        }
    }
}
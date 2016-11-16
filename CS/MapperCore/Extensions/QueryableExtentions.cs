using System;
using System.Linq;
using MapperExpression.Core;
using System.Linq.Expressions;
namespace MapperExpression.Extensions
{
    /// <summary>
    /// Extentions for the IQueryable
    /// </summary>
    public static class QueryableExtentions
    {
        #region IQueryable Extentions 

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TTarget">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TTarget>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TTarget : class
        {
            // Do not use the MethodBase.GetCurrentMethod().Name call because it is not efficient.
            return CreateSortedMethodCall<TSource, TTarget, IOrderedQueryable<TSource>>(query, "OrderBy", sortedPropertyDestName);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TTarget">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource, TTarget>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TTarget : class
        {
            // Do not use the MethodBase.GetCurrentMethod().Name call because it is not efficient.
            return CreateSortedMethodCall<TSource, TTarget, IOrderedQueryable<TSource>>(query, "OrderByDescending", sortedPropertyDestName);
        }

        /// <summary>
        ///  Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TTarget">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        public static IOrderedQueryable<TSource> ThenBy<TSource, TTarget>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TTarget : class
        {
            // Do not use the MethodBase.GetCurrentMethod().Name call because it is not efficient.
            return CreateSortedMethodCall<TSource, TTarget, IOrderedQueryable<TSource>>(query, "ThenBy", sortedPropertyDestName);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TTarget">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        public static IOrderedQueryable<TSource> ThenByDescending<TSource, TTarget>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TTarget : class
        {
            // Do not use the MethodBase.GetCurrentMethod().Name call because it is not efficient.
            return CreateSortedMethodCall<TSource, TTarget, IOrderedQueryable<TSource>>(query, "ThenByDescending", sortedPropertyDestName);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the destination object.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TTarget">Type of destination.</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        public static IQueryable<TTarget> Select<TSource, TTarget>(this IQueryable<TSource> query)
            where TSource : class
            where TTarget : class
        {
            return GetSelect<TSource, TTarget>(query, null);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the destination object.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TTarget">Type of destination.</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="mapperName">Name of the mapper.</param>
        /// <returns></returns>
        public static IQueryable<TTarget> Select<TSource, TTarget>(this IQueryable<TSource> query, string mapperName)
            where TSource : class
            where TTarget : class
        {
            return GetSelect<TSource, TTarget>(query, mapperName);
        }

        /// <summary>
        /// Filter a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TTarget">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="predicate">Function to test each element with respect to a condition.</param>
        /// <returns></returns>
        public static IQueryable<TTarget> Where<TSource, TTarget>(this IQueryable<TTarget> query, Expression<Func<TSource, bool>> predicate)
        {
            // For don't call the same method(same signature)
            return Queryable.Where(query, predicate.ConvertTo<TSource, TTarget>());
        }

        #endregion

        #region Private methods

        private static TQueryable CreateSortedMethodCall<TSource, TTarget, TQueryable>(IQueryable<TSource> query, string methodName, string sortedPropertySourceName)
            where TSource : class
            where TTarget : class
            where TQueryable : class, IQueryable<TSource>
        {
            MapperConfiguration<TSource, TTarget> mapper = Mapper.GetMapper<TSource, TTarget>();
            var prop = mapper.GetLambdaDest(sortedPropertySourceName);
            var lambda = mapper.GetSortedExpression(sortedPropertySourceName);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable),
                methodName,
                new Type[] { typeof(TSource), prop.Type },
                query.Expression,
                Expression.Quote(lambda));
            return query.Provider.CreateQuery<TSource>(resultExp) as TQueryable;
        }

        private static IQueryable<TTarget> GetSelect<TSource, TTarget>(IQueryable<TSource> query, string mapperName)
            where TSource : class
            where TTarget : class
        {
            // It's the same don't need mapper.
            if (typeof(TSource) == typeof(TTarget))
            {
                return (IQueryable<TTarget>)query;
            }
            return query.Select(Mapper.GetMapper<TSource, TTarget>(mapperName).GetLambdaExpression());
        }

        #endregion
    }
}

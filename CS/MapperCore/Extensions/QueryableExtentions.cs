using System;
using System.Linq;
using MapperExpression.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Extensions
{
    public static class QueryableExtentions
    {
        #region Extentions IQueryable

        /// <summary>
        ///  Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest,IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }


        /// <summary>
        ///  Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenByDescending<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {

            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        ///Projects each element of a sequence into a new form by incorporating the destination object
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <returns></returns>
        public static IQueryable<TDest> Select<TSource, TDest>(this IQueryable<TSource> query)
            where TSource : class
            where TDest : class
        {

            return query.Select(GetMapper<TSource, TDest>().GetLambdaExpression());
        }

        //public static IQueryable<TSource> Where<TSource,TDest>(this IQueryable<TSource> source, Expression<Func<TDest, bool>> predicate)
        //    where TSource : class
        //    where TDest : class
        //{
        //   TODO
        //}


        #endregion

        #region Private methods

        private static TQueryable CreateMethodCall<TSource, TDest,TQueryable>(IQueryable<TSource> query, string methodName, string sortedPropertySourceName)
            where TSource : class
            where TDest : class
            where TQueryable : class, IQueryable<TSource>
        {
            MapperConfiguration<TSource, TDest> mapper = GetMapper<TSource, TDest>();
            var prop = mapper.GetPropertyInfoSource(sortedPropertySourceName);
            var lambda = mapper.GetSortedExpression(sortedPropertySourceName);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable),
                methodName,
                new Type[] { typeof(TSource), prop.PropertyType },
                query.Expression,
                Expression.Quote(lambda));
            return query.Provider.CreateQuery<TSource>(resultExp) as TQueryable;
        }

        private static MapperConfiguration<TSource, TDest> GetMapper<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            return Mapper.GetMapper<TSource, TDest>();
        }
        #endregion
    }
}

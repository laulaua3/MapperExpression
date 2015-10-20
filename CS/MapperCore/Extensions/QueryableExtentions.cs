using System;
using System.Linq;
using MapperExpression.Core;
using System.Linq.Expressions;
using System.Reflection;
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
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="sortedPropertyDestName">Name of the destination property</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            //Do not use the MethodBase.GetCurrentMethod () method. Name because it is not efficient
            return CreateSortedMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, "OrderBy", sortedPropertyDestName);
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
            //Do not use the MethodBase.GetCurrentMethod () method. Name because it is not efficient
            return CreateSortedMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, "OrderByDescending", sortedPropertyDestName);
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
            //Do not use the MethodBase.GetCurrentMethod () method. Name because it is not efficient
            return CreateSortedMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, "ThenBy" , sortedPropertyDestName);
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
            //Do not use the MethodBase.GetCurrentMethod () method. Name because it is not efficient
            return CreateSortedMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, "ThenByDescending", sortedPropertyDestName);
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

        /// <summary>
        /// Filter a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of destination</typeparam>
        /// <param name="query">Sequence values to classify.</param>
        /// <param name="predicate">Function to test each element with respect to a condition.</param>
        /// <returns></returns>
        public static IQueryable<TDest> Where<TSource, TDest>(this IQueryable<TDest> query, Expression<Func<TSource, bool>> predicate)
        {
            //For don't call the same method
            return Queryable.Where(query, predicate.ConvertTo<TSource, TDest>());
        }
        
        #endregion

        #region Private methods

        private static TQueryable CreateSortedMethodCall<TSource, TDest, TQueryable>(IQueryable<TSource> query, string methodName, string sortedPropertySourceName)
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

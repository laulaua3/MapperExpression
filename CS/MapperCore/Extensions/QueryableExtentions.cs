using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapperExpression.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Extensions
{
    public static class QueryableExtentions
    {
        #region Extentions IQueryable

        /// <summary>
        ///  Trie les éléments d'une séquence dans l'ordre croissant selon une clé.
        /// </summary>
        /// <typeparam name="TSource">type de la source</typeparam>
        /// <typeparam name="TDest">type de destination</typeparam>
        /// <param name="query">Séquence de valeurs à classer.</param>
        /// <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        ///  Trie les éléments d'une séquence dans l'ordre décroissant selon une clé.
        /// </summary>
        /// <typeparam name="TSource">type de la source</typeparam>
        /// <typeparam name="TDest">type de destination</typeparam>
        /// <param name="query">Séquence de valeurs à classer.</param>
        /// <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest,IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }


        /// <summary>
        ///  Trie les éléments d'une séquence dans l'ordre croissant selon une clé.
        /// </summary>
        /// <typeparam name="TSource">type de la source</typeparam>
        /// <typeparam name="TDest">type de destination</typeparam>
        /// <param name="query">Séquence de valeurs à classer.</param>
        /// <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {
            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        ///  Trie les éléments d'une séquence dans l'ordre décroissant selon une clé.
        /// </summary>
        /// <typeparam name="TSource">type de la source</typeparam>
        /// <typeparam name="TDest">type de destination</typeparam>
        /// <param name="query">Séquence de valeurs à classer.</param>
        /// <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> ThenByDescending<TSource, TDest>(this IQueryable<TSource> query, string sortedPropertyDestName)
            where TSource : class
            where TDest : class
        {

            return CreateMethodCall<TSource, TDest, IOrderedQueryable<TSource>>(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName);
        }

        /// <summary>
        /// Projette chaque élément d'une séquence dans un nouveau formulaire en incorporant l'objet de destination
        /// </summary>
        /// <typeparam name="TSource">type de la source</typeparam>
        /// <typeparam name="TDest">type de destination</typeparam>
        /// <param name="query">Séquence de valeurs à classer.</param>
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
        //    var mapper = GetMapper<TSource, TDest>();
        //    mapper.GetSortedExpression("");
        //    MapperExpressionVisitor visitor = new MapperExpressionVisitor(false, mapper.);
        //}
        
    
        #endregion

        #region Méthodes privées

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

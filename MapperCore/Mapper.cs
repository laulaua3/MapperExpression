using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using MapperCore.Core;
using System.Data.Linq;

namespace MapperCore
{

    /// <summary>
    /// Class de base pour l'accès au mapping
    /// </summary>
    public static class Mapper
    {
        private static MapperConfigurationBase current;
        private static Func<Type, object> constructorFunc;
        #region Méthodes publiques
        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <param name="source">The business.</param>
        /// <returns></returns>
        public static TDest MapTo<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            try
            {
                var query = GetMapper<TSource, TDest>().GetFuncDelegate();
                if (query != null)
                {
                    return query(source);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return null;
        }


        //public static TDest Map<TSource, TDest>(TSource source)
        //    where TSource : class
        //    where TDest : class
        //{

        //    return GetMapper<TSource, TDest>().GetFuncDelegate()(source);

        //}
        /// <summary>
        /// Gets the query expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <returns></returns>
        public static Expression<Func<TSource, TDest>> GetQueryExpression<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            return GetMapper<TSource, TDest>().GetLambdaExpression();
        }
        /// <summary>
        /// Creates the map.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <returns></returns>
        public static MapperConfiguration<TSource, TDest> CreateMap<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            var map = MapperConfigurationRegister.Instance.Find(typeof(TSource), typeof(TDest));
            if (map == null)
            {
                map = new MapperConfiguration<TSource, TDest>();
                MapperConfigurationRegister.Instance.Add(map);
            }
            return map as MapperConfiguration<TSource, TDest>;
        }
        
        /// <summary>
        /// Efface tout les mappeur existants
        /// </summary>
        public static void Reset()
        {
            MapperConfigurationRegister.Instance.Clear();
        }
        /// <summary>
        /// Gets the data load options for linqToSql.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <returns></returns>
        public static DataLoadOptions GetDataLoadOptionsLinq<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            var map = GetMapper<TSource, TDest>();
            return map.GetDataLoadOptionsLinq();
        }
        /// <summary>
        /// Initialises les mappeurs.
        /// </summary>
        public static void Initialize()
        {
            var configRegister = MapperConfigurationRegister.Instance;
            foreach (var mapper in configRegister)
            {
                mapper.CreateMappingExpression(constructorFunc);
            }
        }
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <returns></returns>
        public static Func<TSource, TDest> GetQuery<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            return GetMapper<TSource, TDest>().GetFuncDelegate();
        }


        public static void ConstructServicesUsing(Func<Type, object> constructor)
        {
            constructorFunc = constructor;
        }


        #endregion

        #region Méthodes privées
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <returns></returns>
        private static MapperConfiguration<TSource, TDest> GetMapper<TSource, TDest>()
            where TSource : class
            where TDest : class
        {

            GetMapper(typeof(TSource), typeof(TDest));

            return (current as MapperConfiguration<TSource, TDest>);

        }
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <param name="tSource">The t source.</param>
        /// <param name="tDest">The t dest.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Impossible de trouver la configuration entre le type source + tSource.Name +  et le type destination + tDest.Name</exception>
        private static MapperConfigurationBase GetMapper(Type tSource, Type tDest)
        {
            if (current == null || (current != null && current.TypeDest.FullName != tDest.FullName && current.TypeSource.FullName != tSource.FullName))
                current = MapperConfigurationRegister.Instance.Find(tSource, tDest);
            if (current == null)
            {
                throw new Exception("Impossible de trouver la configuration entre le type source" + tSource.Name + " et le type destination" + tDest.Name);
            }
            return current;

        }
        #endregion
    }
}

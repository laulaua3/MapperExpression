using System;
using System.Data.Linq;
using System.Linq.Expressions;
using MapperCore.Core;
using MapperCore.Exception;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace MapperCore
{

    /// <summary>
    /// Class de base pour l'accès au mapping
    /// </summary>
    public static class Mapper
    {
        #region Variables

        private static MapperConfigurationBase current;
        private static Func<Type, object> constructorFunc;

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <param name="source">The business.</param>
        /// <returns></returns>
        public static TDest Map<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            try
            {
                MapperConfiguration<TSource, TDest> mapper = GetMapper<TSource, TDest>();
                Func<TSource, TDest> query = mapper.GetFuncDelegate();
                if (query != null)
                {
                    TDest result = query(source);
                    //Action à exécutées après le mapping
                    mapper.ExecuteAfterActions(source, result);

                    return result;
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
            return null;
        }

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
            MapperConfigurationBase map = MapperConfigurationContainer.Instance.Find(typeof(TSource), typeof(TDest));
            if (map == null)
            {
                map = new MapperConfiguration<TSource, TDest>();
                MapperConfigurationContainer.Instance.Add(map);
            }
            return map as MapperConfiguration<TSource, TDest>;
        }

        /// <summary>
        /// Indique le service d'injection à utilisée
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public static void ConstructServicesUsing(Func<Type, object> constructor)
        {
            constructorFunc = constructor;
        }

        /// <summary>
        /// Efface tout les mappeur existants
        /// </summary>
        public static void Reset()
        {
            MapperConfigurationContainer.Instance.Clear();
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
            MapperConfiguration<TSource, TDest> map = GetMapper<TSource, TDest>();
            return map.GetDataLoadOptionsLinq();
        }

        /// <summary>
        /// Initialises les mappeurs.
        /// </summary>
        public static void Initialize()
        {
            MapperConfigurationContainer configRegister = MapperConfigurationContainer.Instance;

            foreach (MapperConfigurationBase mapper in configRegister)
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
        #endregion

        #region Méthodes privées

        internal static MapperConfiguration<TSource, TDest> GetMapper<TSource, TDest>()
            where TSource : class
            where TDest : class
        {
            if (current == null || (current != null && current.TypeDest.FullName != typeof(TDest).FullName && current.TypeSource.FullName != typeof(TSource).FullName))
                current = GetMapper(typeof(TSource), typeof(TDest));

            return (current as MapperConfiguration<TSource, TDest>);

        }

        internal static MapperConfigurationBase GetMapper(Type tSource, Type tDest)
        {
            MapperConfigurationBase mapConfig = MapperConfigurationContainer.Instance.Find(tSource, tDest);
            if (mapConfig != null)
            {
                return mapConfig;
            }
            else
            {
                throw new NoFoundMapperException(tSource, tDest);
            }
        }
      
       


        #endregion
    }
}

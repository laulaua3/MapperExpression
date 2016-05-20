using System;
using System.Linq.Expressions;
using MapperExpression.Core;
using MapperExpression.Exceptions;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MapperExpression
{

    /// <summary>
    ///Base class for each access to mapper
    /// </summary>
    public static class Mapper
    {
        #region Variables


        private static Func<Type, object> constructorFunc;

        #endregion

        #region Public methods

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the dest.</typeparam>
        /// <param name="source">the source object.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// A new  object of <typeparamref name="TTarget"></typeparamref>
        /// </returns>
        public static TTarget Map<TSource, TTarget>(TSource source, string name = null)
            where TSource : class
            where TTarget : class
        {
            // No source, no target.
            if (source == null)
                return null;
            TTarget result = null;
            try
            {
                MapperConfiguration<TSource, TTarget> mapper = GetMapper<TSource, TTarget>(name);
                Func<TSource, TTarget> query = mapper.GetFuncDelegate();
                if (query != null)
                {
                    result = query(source);
                    // Action performed after the mapping
                    mapper.ExecuteAfterActions(source, result);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Maps the specified source to the target.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        public static void Map<TSource, TTarget>(TSource source, TTarget target, string name = null)
         where TSource : class
         where TTarget : class
        {

            Contract.Requires(source != null);
            Contract.Requires(target != null);
            TTarget result = null;
            try
            {
                MapperConfiguration<TSource, TTarget> mapper = GetMapper<TSource, TTarget>(name);
                Action<TSource, TTarget> query = mapper.GetDelegateForExistingTarget() as Action<TSource, TTarget>;
                if (query != null)
                {
                    query(source, target);
                    // Action performed after the mapping
                    mapper.ExecuteAfterActions(source, result);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the query expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the dest.</typeparam>
        /// <returns></returns>
        public static Expression<Func<TSource, TTarget>> GetQueryExpression<TSource, TTarget>()
            where TSource : class
            where TTarget : class
        {
            return GetMapper<TSource, TTarget>().GetLambdaExpression();
        }

        /// <summary>
        /// Creates a mapper.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the dest.</typeparam>
        /// <returns></returns>
        public static MapperConfiguration<TSource, TTarget> CreateMap<TSource, TTarget>(string name = null)
            where TSource : class
            where TTarget : class
        {
            // We do not use the method because it GetMapper throw an exception if not found
            MapperConfigurationBase map = MapperConfigurationCollectionContainer.Instance.Find(typeof(TSource), typeof(TTarget), name);
            if (map == null)
            {
                string finalName = string.IsNullOrEmpty(name) ? "s" + MapperConfigurationCollectionContainer.Instance.Count.ToString() : name;
                map = new MapperConfiguration<TSource, TTarget>(finalName);
                MapperConfigurationCollectionContainer.Instance.Add(map);
            }
            return map as MapperConfiguration<TSource, TTarget>;
        }
        /// <summary>
        /// Indicates the injection service used
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public static void ConstructServicesUsing(Func<Type, object> constructor)
        {
            constructorFunc = constructor;
        }

        /// <summary>
        /// Remove all mappers
        /// </summary>
        ///<remarks>Use for your units test only</remarks>
        public static void Reset()
        {
            MapperConfigurationCollectionContainer.Instance.Clear();
        }
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static MapperConfiguration<TSource, TDest> GetMapper<TSource, TDest>(string name = null)
            where TSource : class
            where TDest : class
        {

            return (GetMapper(typeof(TSource), typeof(TDest), name) as MapperConfiguration<TSource, TDest>);

        }
        /// <summary>
        /// Initialise the mappers.
        /// </summary>
        /// <remarks>Use only the application initialization</remarks>
        public static void Initialize()
        {
            MapperConfigurationCollectionContainer configRegister = MapperConfigurationCollectionContainer.Instance;
            for (int i = 0; i < configRegister.Count; i++)
            {
                configRegister[i].Initialize(constructorFunc);
            }


        }

        /// <summary>
        /// Gets the delegate of the mapping.
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
        /// <summary>
        /// Gets the properties not mapped.
        /// </summary>
        public static PropertiesNotMapped GetPropertiesNotMapped<TSource, TDest>(string name = null)
             where TSource : class
            where TDest : class
        {
            var mapper = GetMapper<TSource, TDest>(name);
            return mapper.GetPropertiesNotMapped();
        }
        #endregion

        #region Internals methods        


        internal static MapperConfigurationBase GetMapper(Type tSource, Type tDest, string name = null)
        {
            MapperConfigurationBase mapConfig = MapperConfigurationCollectionContainer.Instance.Find(tSource, tDest, name);
            if (mapConfig == null)
                throw new NoFoundMapperException(tSource, tDest);

            return mapConfig;
        }

        #endregion
    }
    /// <summary>
    ///Base class for each access to mapper(simplified access)
    /// </summary>
    /// <typeparam name="TTarget">The type of the dest.</typeparam>
    public static class Mapper<TTarget>
         where TTarget : class
    { 
        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        public static TTarget Map<TSource>(TSource source, string name = null)
            where TSource : class
        {

            return Mapper.Map<TSource, TTarget>(source, name);
        }
    }
}
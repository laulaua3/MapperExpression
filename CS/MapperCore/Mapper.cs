using System;
using System.Linq.Expressions;
using MapperExpression.Core;
using MapperExpression.Exception;

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
        /// <returns>A new  object of <typeparamref name="TTarget"></typeparamref></returns>
        public static TTarget Map<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class
        {
            TTarget result = null;
            try
            {
                MapperConfiguration<TSource, TTarget> mapper = GetMapper<TSource, TTarget>();
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
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the dest.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void Map<TSource, TTarget>(TSource source, TTarget target)
         where TSource : class
         where TTarget : class
        {
            TTarget result = null;
            try
            {
                MapperConfiguration<TSource, TTarget> mapper = GetMapper<TSource, TTarget>();
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
        public static MapperConfiguration<TSource, TTarget> CreateMap<TSource, TTarget>()
            where TSource : class
            where TTarget : class
        {
            // We do not use the method because it GetMapper throw an exception if not found
            MapperConfigurationBase map = MapperConfigurationContainer.Instance.Find(typeof(TSource), typeof(TTarget));
            if (map == null)
            {
                map = new MapperConfiguration<TSource, TTarget>("s" + MapperConfigurationContainer.Instance.Count);
                MapperConfigurationContainer.Instance.Add(map);
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
            MapperConfigurationContainer.Instance.Clear();
        }

        /// <summary>
        /// Initialise the mappers.
        /// </summary>
        /// <remarks>Use only the application initialization</remarks>
        public static void Initialize()
        {
            MapperConfigurationContainer configRegister = MapperConfigurationContainer.Instance;

            foreach (MapperConfigurationBase mapper in configRegister)
            {
                mapper.CreateMappingExpression(constructorFunc);
                mapper.CreateMemberAssignementForExistingTarget();
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
        /// Gets the mapper.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static MapperConfigurationBase GetMapper(Predicate<MapperConfigurationBase> predicate)
        {
            return MapperConfigurationContainer.Instance.Find(predicate);
        }

        #endregion

        #region Internals methods

        internal static MapperConfiguration<TSource, TDest> GetMapper<TSource, TDest>()
            where TSource : class
            where TDest : class
        {

            return (GetMapper(typeof(TSource), typeof(TDest)) as MapperConfiguration<TSource, TDest>);

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
        /// <returns></returns>
        public static TTarget Map<TSource>(TSource source)
            where TSource : class
        {

            return Mapper.Map<TSource, TTarget>(source);
        }
    }
}
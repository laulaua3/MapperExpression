using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exception;
using System.Diagnostics.Contracts;

namespace MapperExpression.Core
{

    /// <summary>
    /// Principal mapper
    /// </summary>
    /// <typeparam name="TSource">The type of source.</typeparam>
    /// <typeparam name="TDest">Type of destination.</typeparam>
    public class MapperConfiguration<TSource, TDest>
        : MapperConfigurationBase
    {
        #region Variables        
        /// <summary>
        /// The actions after map
        /// </summary>
        protected readonly List<Action<TSource, TDest>> actionsAfterMap;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates a new instance of<see cref="MapperConfiguration{TSource, TDest}"/> class.
        /// </summary>
        internal MapperConfiguration(string paramName)
            : base(typeof(TSource), typeof(TDest), paramName)
        {
            propertiesMapping = new List<Tuple<LambdaExpression, LambdaExpression, bool>>();
            propertiesToIgnore = new List<PropertyInfo>();
            actionsAfterMap = new List<Action<TSource, TDest>>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TSource, TDest>> GetLambdaExpression()
        {
            MemberInitExpression exp = GetMemberInitExpression();
            return Expression.Lambda<Func<TSource, TDest>>(exp, paramClassSource);
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <returns></returns>
        public Func<TSource, TDest> GetFuncDelegate()
        {
            return GetDelegate() as Func<TSource, TDest>;
        }

        /// <summary>
        /// Fors the member.
        /// </summary>
        /// <param name="getPropertySource">The get property source.</param>
        /// <param name="getPropertyDest">The get property dest.</param>
        /// <param name="checkIfNull">Vérifie si la propriété source n'est pas nul avant d'affecté la valeur (récursif)</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> ForMember(Expression<Func<TSource, object>> getPropertySource, Expression<Func<TDest, object>> getPropertyDest, bool checkIfNull = false)
        {
            // Adding in the list for further processing
            ForMember(getPropertySource as LambdaExpression, getPropertyDest as LambdaExpression, checkIfNull);
            return this;
        }

        /// <summary>
        /// Ignores the specified property dest.
        /// </summary>
        /// <param name="propertyDest">The property dest.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> Ignore(Expression<Func<TDest, object>> propertyDest)
        {
            // Adding in the list for further processing
            propertiesToIgnore.Add(GetPropertyInfo(propertyDest));
            return this;
        }

        /// <summary>
        /// Action to perform after the mapping
        /// </summary>
        /// <param name="actionAfterMap">Action a réalisée</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> AfterMap(Action<TSource, TDest> actionAfterMap)
        {
            // Adding in the list for further processing
            actionsAfterMap.Add(actionAfterMap);
            return this;
        }

        /// <summary>
        /// Excecutes the after actions.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        public void ExecuteAfterActions(TSource source, TDest dest)
        {
            for (int i = 0; i < actionsAfterMap.Count; i++)
            {

                var action = actionsAfterMap[i];
                if (action == null)
                {
                    throw new NoActionAfterMappingException();
                }
                action(source, dest);
            }
        }

        /// <summary>
        /// Reverses the map.
        /// </summary>
        /// <returns>the new Mapper</returns>
        public MapperConfiguration<TDest, TSource> ReverseMap()
        {
            MapperConfiguration<TDest, TSource> map = GetMapper(typeof(TDest), typeof(TSource), false) as MapperConfiguration<TDest, TSource>;

            if (map != null)
            {
                throw new MapperExistException(typeof(TDest), typeof(TSource));
            }
            map = new MapperConfiguration<TDest, TSource>("s" + MapperConfigurationContainer.Instance.Count + 1);
            CreateCommonMember();
            // Path is the mapping of existing properties and inverse relationships are created
            for (int i = 0; i < propertiesMapping.Count; i++)
            {
                Tuple<LambdaExpression, LambdaExpression, bool> item = propertiesMapping[i];
                PropertyInfo propertyDest = GetPropertyInfo(item.Item1);
                if (propertyDest.CanWrite)
                    map.ForMember(item.Item2, item.Item1, item.Item3);
            }

            MapperConfigurationContainer.Instance.Add(map);
            return map;
        }

        /// <summary>
        /// Indicates whether one uses the service injection
        /// </summary>
        public MapperConfiguration<TSource, TDest> ConstructUsingServiceLocator()
        {
            UseServiceLocator = true;
            return this;
        }

        #endregion

        #region Private methods

        internal LambdaExpression GetSortedExpression(string propertySource)
        {
            Contract.Assert(!string.IsNullOrEmpty(propertySource));
            Expression result = null;
            var exp = propertiesMapping.Find(x => GetPropertyInfo(x.Item2).Name == propertySource);
            if (exp == null)
            {
                throw new PropertyNoExistException(propertySource, typeof(TDest));
            }
            // To change the parameter
            var visitor = new MapperExpressionVisitor(paramClassSource);
            result = visitor.Visit(exp.Item1);
            return Expression.Lambda(result, paramClassSource);

        }

        #endregion
    }
}

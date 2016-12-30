using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exceptions;
using System.Diagnostics.Contracts;
using System.Collections.ObjectModel;
using System.Linq;

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
        readonly IList<Action<TSource, TDest>> actionsAfterMap;
        /// <summary>
        /// The actions after map
        /// </summary>
        protected ReadOnlyCollection<Action<TSource, TDest>> ActionsAfterMap
        {
            get
            {
                return new ReadOnlyCollection<Action<TSource, TDest>>(actionsAfterMap);
            }
        }

        #endregion


        /// <summary>
        /// Instantiates a new instance of<see cref="MapperConfiguration{TSource, TDest}"/> class.
        /// </summary>
        internal MapperConfiguration(string paramName, string mapperName = null)
            : base(typeof(TSource), typeof(TDest), paramName, mapperName)
        {

            actionsAfterMap = new List<Action<TSource, TDest>>();
        }

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
            return (Func<TSource, TDest>)GetDelegate();
        }

        /// <summary>
        /// Fors the member.
        /// </summary>
        /// <param name="getPropertySource">The get property source.</param>
        /// <param name="getPropertyDest">The get property dest.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> ForMember<TPropertySource, TPropertyDest>(Expression<Func<TSource, TPropertySource>> getPropertySource, Expression<Func<TDest, TPropertyDest>> getPropertyDest)
        {
            // Adding in the list for further processing
            ForMemberBase(getPropertySource.Body, getPropertyDest.Body, false);
            return this;
        }
        /// <summary>
        /// Fors the member.
        /// </summary>
        /// <typeparam name="TPropertySource">The type of the property source.</typeparam>
        /// <typeparam name="TPropertyDest">The type of the property dest.</typeparam>
        /// <param name="getPropertySource">The get property source.</param>
        /// <param name="getPropertyDest">The get property dest.</param>
        /// <param name="checkIfNull">if set to <c>true</c> [check if null].</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> ForMember<TPropertySource, TPropertyDest>(Expression<Func<TSource, TPropertySource>> getPropertySource, Expression<Func<TDest, TPropertyDest>> getPropertyDest, bool checkIfNull)
        {
            // Adding in the list for further processing
            ForMemberBase(getPropertySource.Body, getPropertyDest.Body, checkIfNull);
            return this;
        }
        /// <summary>
        /// Fors the member.
        /// </summary>
        /// <typeparam name="TPropertySource">The type of the property source.</typeparam>
        /// <typeparam name="TPropertyDest">The type of the property dest.</typeparam>
        /// <param name="getPropertySource">The get property source.</param>
        /// <param name="getPropertyDest">The get property dest.</param>
        /// <param name="mapperName">Name of the mapper.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> ForMember<TPropertySource, TPropertyDest>(Expression<Func<TSource, TPropertySource>> getPropertySource, Expression<Func<TDest, TPropertyDest>> getPropertyDest, string mapperName)
        {
            // Adding in the list for further processing
            ForMemberBase(getPropertySource.Body, getPropertyDest.Body, true, mapperName);
            return this;
        }
        /// <summary>
        /// Ignores the specified property dest.
        /// </summary>
        /// <param name="propertyDest">The property dest.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> Ignore<TProperty>(Expression<Func<TDest, TProperty>> propertyDest)
        {

            return IgnoreBase(propertyDest) as MapperConfiguration<TSource, TDest>;
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
            if (actionsAfterMap.Count > 0)
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
        }

        /// <summary>
        /// Reverses the map.
        /// </summary>
        /// <param name="name">The name of the mapper.</param>
        /// <returns>
        /// the new Mapper
        /// </returns>
        /// <exception cref="MapperExistException"></exception>
        public MapperConfiguration<TDest, TSource> ReverseMap(string name = null)
        {
            MapperConfigurationBase map = GetMapper(typeof(TDest), typeof(TSource), false, name);

            if (map != null)
            {
                throw new MapperExistException(typeof(TDest), typeof(TSource));
            }
            string finalName = string.IsNullOrEmpty(name) ? "s" + (MapperConfigurationCollectionContainer.Instance.Count).ToString() : name;
            map = new MapperConfiguration<TDest, TSource>(finalName);
            MapperConfigurationCollectionContainer.Instance.Add(map);
            CreateCommonMember();

            // Path is the mapping of existing properties and inverse relationships are created
            for (int i = 0; i < PropertiesMapping.Count; i++)
            {
                Tuple<Expression, Expression, bool, string> item = PropertiesMapping[i];
                PropertyInfo propertyDest = GetPropertyInfo(item.Item1);
                if (propertyDest.CanWrite)
                {
                    if (!string.IsNullOrEmpty(item.Item4))
                    {
                        //Find the reverse mapper
                        var reverseMapper = GetMapper(item.Item2.Type, item.Item1.Type, false);
                        if (reverseMapper != null)
                        {
                            map.ForMemberBase(item.Item2, item.Item1, item.Item3, reverseMapper.Name);
                        }
                    }
                    else
                    {
                        if (item.Item1.NodeType == ExpressionType.MemberAccess)
                        {
                            map.ForMemberBase(item.Item2, item.Item1, item.Item3, item.Item4);
                        }
                    }

                }
            }


            return map as MapperConfiguration<TDest, TSource>;
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

    }
}
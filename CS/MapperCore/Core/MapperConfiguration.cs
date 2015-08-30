using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exception;

namespace MapperExpression.Core
{

    /// <summary>
    /// Mappeur principal
    /// </summary>
    /// <typeparam name="TSource">Le type de la source.</typeparam>
    /// <typeparam name="TDest">Le type de la destination.</typeparam>
    public class MapperConfiguration<TSource, TDest>
        : MapperConfigurationBase
    {
        #region Variables

        protected readonly List<Action<TSource, TDest>> actionsAfterMap;

        #endregion

        #region Constructeur

        /// <summary>
        /// Instancie un nouvelle instance de <see cref="MapperConfiguration{TSource, TDest}"/> class.
        /// </summary>
        internal MapperConfiguration()
            : base(typeof(TSource), typeof(TDest))
        {
            propertiesMapping = new List<Tuple<LambdaExpression, LambdaExpression, bool>>();
            propertiesToIgnore = new List<PropertyInfo>();
            actionsAfterMap = new List<Action<TSource, TDest>>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TSource, TDest>> GetLambdaExpression()
        {
            MemberInitExpression exp = GetMemberInitExpression();
            // Expression<Func<ClassSource, ClassDestination>> lambdaExecute = (c1) => new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
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
            //Ajout dans la liste pour le traitement ultérieur
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

            propertiesToIgnore.Add(GetPropertyInfo(propertyDest));
            return this;
        }

        /// <summary>
        /// Action à exécuter après le mapping
        /// </summary>
        /// <param name="actionAfterMap">Action a réalisée</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> AfterMap(Action<TSource, TDest> actionAfterMap)
        {
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
                actionsAfterMap[i](source, dest);
            }
        }

        /// <summary>
        /// Reverses the map.
        /// </summary>
        /// <returns>the new Mapper</returns>
        public MapperConfiguration<TDest, TSource> ReverseMap()
        {
            //On recherche le mapper inverse
            MapperConfiguration<TDest, TSource> map = GetMapper(typeof(TDest), typeof(TSource), false) as MapperConfiguration<TDest, TSource>;
            //on lève un exception si celui existe déjà
            if (map != null)
            {
                throw new MapperExistException(typeof(TDest), typeof(TSource));
            }
            map = new MapperConfiguration<TDest, TSource>();
            //On parcours les propriétés de mapping de l'existant et on crée les relations inverses
            for (int i = 0; i < propertiesMapping.Count; i++)
            {
                Tuple<LambdaExpression, LambdaExpression, bool> item = propertiesMapping[i];
                PropertyInfo propertyDest = GetPropertyInfo(item.Item1);
                //Si la propriété de destination n'est pas en lecture seul
                if (propertyDest.CanWrite)
                    map.ForMember(item.Item2, item.Item1, item.Item3);
            }
            MapperConfigurationContainer.Instance.Add(map);
            return map;
        }

        /// <summary>
        /// Indique si l'on utilise le service d'injection
        /// </summary>
        public MapperConfiguration<TSource, TDest> ConstructUsingServiceLocator()
        {
            UseServiceLocator = true;
            return this;
        }

        #endregion

        #region Méthodes privées

        internal override void CreateMappingExpression(Func<Type, object> constructor)
        {
            if (!isInitialized)
            {
                //on le met avant le traitement pour éviter les boucles récursives
                isInitialized = true;
                constructorFunc = constructor;
                CreateCommonMember();

                for (int i = 0; i < propertiesMapping.Count; i++)
                {
                    LambdaExpression getPropertySource = propertiesMapping[i].Item1;
                    LambdaExpression getPropertyDest = propertiesMapping[i].Item2;
                    //On va chercher les propriétés choisies
                    PropertyInfo memberSource = GetPropertyInfo(getPropertySource);
                    PropertyInfo memberDest = GetPropertyInfo(getPropertyDest);
                    CreateMemberAssignement(memberSource, memberDest);
                }
                //création du delegate
                GetFuncDelegate();
            }
        }
  
        internal LambdaExpression GetSortedExpression(string propertySource)
        {
            Expression result = null;
            var exp = propertiesMapping.Find(x => GetPropertyInfo(x.Item2).Name == propertySource);
            if (exp == null)
            {
                throw new PropertyNoExistException(propertySource, typeof(TDest));
            }
            var property = GetPropertyInfo(exp.Item2);
            var visitor = new MapperExpressionVisitor(false, paramClassSource);
            result = visitor.Visit(exp.Item1);
            return Expression.Lambda(result, paramClassSource);

        }

        internal PropertyInfo GetPropertyInfoSource(string propertyName)
        {
            var exp = propertiesMapping.Find(x => GetPropertyInfo(x.Item2).Name == propertyName);
            if (exp == null)
            {
                throw new PropertyNoExistException(propertyName, typeof(TDest));
            }
            var property = GetPropertyInfo(exp.Item2);
            return property;
        }

        #endregion
    }
}

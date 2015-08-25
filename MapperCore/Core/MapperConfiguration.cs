using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperCore.Core
{

    /// <summary>
    /// Mappeur principal
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    public class MapperConfiguration<TSource, TDest> : MapperConfigurationBase
    {

        private Func<TSource, TDest> delegateExpression;

        private List<Tuple<Expression<Func<TSource, object>>, Expression<Func<TDest, object>>>> customPropertiesMapping;


        public MapperConfiguration()
            : base(typeof(TSource), typeof(TDest))
        {
            this.customPropertiesMapping = new List<Tuple<Expression<Func<TSource, object>>, Expression<Func<TDest, object>>>>();
        }

        #region Methodes public

        /// <summary>
        /// Gets the lambda expression.
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TSource, TDest>> GetLambdaExpression()
        {

            var exp = this.GetMemberInitExpression();
            // Expression<Func<ClassSource, ClassDestination>> lambdaExecute = (c1) => new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            return Expression.Lambda<Func<TSource, TDest>>(exp, paramClassSource);
        }
        public Func<TSource, TDest> GetFuncDelegate()
        {
            if (delegateExpression == null)
            {
                delegateExpression = GetLambdaExpression().Compile();
            }
            return delegateExpression;
        }
        public MapperConfiguration<TSource, TDest> ConstructUsingServiceLocator()
        {
            this.UseServiceLocator = true;
            return this;
        }
        /// <summary>
        /// Fors the member.
        /// </summary>
        /// <param name="getPropertySource">The get property source.</param>
        /// <param name="getPropertyDest">The get property dest.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> ForMember(Expression<Func<TSource, object>> getPropertySource, Expression<Func<TDest, object>> getPropertyDest)
        {
            //Ajout dans la liste pour le traitement ultérieur
            this.customPropertiesMapping.Add(Tuple.Create<Expression<Func<TSource, object>>, Expression<Func<TDest, object>>>(getPropertySource, getPropertyDest));
            return this;
        }


        /// <summary>
        /// Ignores the specified property dest.
        /// </summary>
        /// <param name="propertyDest">The property dest.</param>
        /// <returns></returns>
        public MapperConfiguration<TSource, TDest> Ignore(Expression<Func<TDest, object>> propertyDest)
        {
            this.customPropertiesMapping.RemoveAll(d => GetPropertyInfo(d.Item2) != null);
            return this;
        }

        /// <summary>
        /// Reverses the map.
        /// </summary>
        /// <returns>the new Mapper</returns>
        public MapperConfiguration<TDest, TSource> ReverseMap()
        {
            var map = GetMapper(typeof(TDest), typeof(TSource), true) as MapperConfiguration<TDest, TSource>;
            map = new MapperConfiguration<TDest, TSource>();
            for (int i = 0; i < this.customPropertiesMapping.Count; i++)
            {
                Tuple<Expression<Func<TSource, object>>, Expression<Func<TDest, object>>> item = this.customPropertiesMapping[i];
                map.ForMember(item.Item2, item.Item1);
            }
            MapperConfigurationRegister.Instance.Add(map);
            return map;
        }

        /// <summary>
        /// Gets the data load options for LinqToSql.
        /// </summary>
        /// <returns></returns>
        public DataLoadOptions GetDataLoadOptionsLinq()
        {
            DataLoadOptions options = new DataLoadOptions();
            var propertiesLinq = this.customPropertiesMapping.Where(p => this.GetPropertyInfo(p.Item1).PropertyType.BaseType.Name == "EntityBase");
            foreach (var prop in propertiesLinq)
            {
                options.LoadWith(prop.Item1);
            }
            return options;
        }
        /// <summary>
        /// Creates the member assignement.
        /// </summary>
        /// <exception cref="Exception">
        /// La propriété ' + memberDest.Name + ' de destination est en lecture seule
        /// or
        /// La propriété ' + memberSource.Name + 'n'existe pas pour le type ' + memberSource.DeclaringType.ToString() + '
        /// </exception>
        public override void CreateMappingExpression(Func<Type, object> constructor = null)
        {

            if (!_isInitialized)
            {

                //on le met avant le traitement pour éviter les boucles récursives
                this._isInitialized = true;
                this.constructorFunc = constructor;
                this.CreateCommonMember();

                for (int i = 0; i < this.customPropertiesMapping.Count; i++)
                {
                    Expression<Func<TSource, object>> getPropertySource = this.customPropertiesMapping[i].Item1;
                    Expression<Func<TDest, object>> getPropertyDest = this.customPropertiesMapping[i].Item2;
                    //On va chercher les propriétés choisies
                    PropertyInfo memberSource = this.GetPropertyInfo(getPropertySource);
                    PropertyInfo memberDest = this.GetPropertyInfo(getPropertyDest);
                    CreateMemberAssignement(memberSource, memberDest, true);
                }
                //création du delegate
                this.GetFuncDelegate();
                this.GetDelegate();
            }
        }

        #endregion

        #region Methodes privées

        private PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyExpression)
        {
            switch (propertyExpression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    return (((propertyExpression.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo);

                case ExpressionType.MemberAccess:
                    return ((propertyExpression.Body as MemberExpression).Member as PropertyInfo);
                default:
                    throw new NotImplementedException("Ce type d'expression n'est pas valide");
            }
        }

        #endregion
    }
}

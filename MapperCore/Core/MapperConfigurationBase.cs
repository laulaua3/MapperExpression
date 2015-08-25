using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperCore.Core
{
   
    /// <summary>
    /// Class de base de la gestion du mapping
    /// </summary>
    public abstract class MapperConfigurationBase
    {


        protected ParameterExpression paramClassSource;
        protected Expression lambdaExecute;
        protected Delegate delegateCall;

        protected Func<Type, object> constructorFunc;
        protected bool _isInitialized = false;

        public bool UseServiceLocator { get; protected set; }
        /// <summary>
        /// Gets the type source.
        /// </summary>
        /// <value>
        /// The type source.
        /// </value>
        public Type TypeSource { get; private set; }
        /// <summary>
        /// Gets the type dest.
        /// </summary>
        /// <value>
        /// The type dest.
        /// </value>
        public Type TypeDest { get; private set; }
        /// <summary>
        /// Gets or sets the member to map.
        /// </summary>
        /// <value>
        /// The member to map.
        /// </value>
        public List<MemberAssignment> MemberToMap { get; protected set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfigurationBase"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public MapperConfigurationBase(Type source, Type destination)
        {
            this.TypeDest = destination;
            this.TypeSource = source;
            this.paramClassSource = Expression.Parameter(source, "source");
            this.MemberToMap = new List<MemberAssignment>();
        }

        /// <summary>
        /// Changes the original source.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="paramSource">The parameter source.</param>
        /// <returns></returns>
        public List<MemberAssignment> ChangeSource(PropertyInfo property, ParameterExpression paramSource)
        {
            if (!_isInitialized)
            {
                this.CreateMappingExpression(constructorFunc);
            }
            var membersTransformed = new List<MemberAssignment>();
            this.MemberToMap.ForEach((m) =>
                {
                    //Propriété de la classe source référençant la classe enfant
                    var innerProperty = Expression.Property(paramSource, property);
                    //Propriété de la class enfant
                    if (m.Expression is MemberExpression)
                    {
                        var outerProperty = Expression.Property(innerProperty, (m.Expression as MemberExpression).Member as PropertyInfo);
                        //Affectation de la propriété de destination à la propriété de la class enfant
                        var expBind = Expression.Bind(m.Member, outerProperty);

                        membersTransformed.Add(expBind);
                    }

                });
            return membersTransformed;
        }



        public Delegate GetDelegate()
        {
            if (this.delegateCall == null)
            {
                var exp = this.GetMemberInitExpression();

                this.delegateCall = Expression.Lambda(exp, paramClassSource).Compile();
            }
            return this.delegateCall;
        }

        /// <summary>
        /// Creates the mapping expression.
        /// </summary>
        public virtual void CreateMappingExpression(Func<Type, object> constructor)
        {
            
            if (!this._isInitialized)
            {
                this._isInitialized = true;
                
                this.constructorFunc = constructor;
                CreateCommonMember();
                this.GetDelegate();
            }
        }

        protected MapperConfigurationBase GetMapper(Type tSource, Type tDest, bool exceptionOnNoFound = true)
        {
            MapperConfigurationBase mapperExterne = null;

            mapperExterne = MapperConfigurationRegister.Instance.Find(tSource, tDest);
            //on levé une exception si rien n'est trouvé
            if (mapperExterne == null && exceptionOnNoFound)
                throw new Exception("Les types '" + tSource.Name + "' et '" + tDest.Name + "' ne sont pas du même type ou ne sont pas mappés");

            return mapperExterne;
        }

        protected void CreateCommonMember()
        {
            var propertiesSource = this.TypeSource.GetProperties();

            foreach (var propSource in propertiesSource)
            {
                var propDest = this.TypeDest.GetProperty(propSource.Name);
                if (propDest != null && propDest.CanWrite)
                {
                    CreateMemberAssignement(propSource, propDest, false);
                }
            }
        }

        protected void CreateMemberAssignement(PropertyInfo memberSource, PropertyInfo memberDest, bool exceptionMapperNoFound)
        {
            MapperConfigurationBase mapperExterne = null;
            //On supprime l'ancien (si appel plusieurs fois de la méthode)
            this.MemberToMap.RemoveAll(m => m.Member.Name == memberSource.Name);

            if (!memberDest.CanWrite)
            {
                throw new Exception("La propriété '" + memberDest.Name + "' de destination est en lecture seule");
            }
            //Cas des listes
            if (memberSource.PropertyType.GetInterfaces().Count(t => t == typeof(IList)) > 0)
            {
                //Type de la liste source
                var sourceTypeList = memberSource.PropertyType.GetGenericArguments()[0];
                // Type de la liste de destination
                var destTypeList = memberDest.PropertyType.GetGenericArguments()[0];
                mapperExterne = this.GetMapper(sourceTypeList, destTypeList);
                mapperExterne.CreateMappingExpression(constructorFunc);

                this.CheckAndRemoveMemberDest(memberDest.Name);
                //Suppression du mapping d'origine pour cette propriété
                this.CheckAndRemoveMemberSource(memberSource.Name);


                //Appel de la méthode pour récupère l'expression lambda pour le select
                MethodInfo methodeGetExpression = mapperExterne.GetType().GetMethod("GetLambdaExpression");
                var expression = methodeGetExpression.Invoke(mapperExterne, null) as Expression;
                //On cherche la méthode select
                MethodInfo selectMethod = null;
                var selectsMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "Select");
                //Pour prendre la bonne (y en a 2)
                foreach (MethodInfo m in selectsMethod)
                {
                    var parameters = m.GetParameters().Where(p => p.Name.Equals("selector"));
                    foreach (ParameterInfo p in parameters)
                    {
                        if (p.ParameterType.GetGenericArguments().Count() == 2)
                        {
                            selectMethod = (MethodInfo)p.Member;
                            break;
                        }
                    }
                    if (selectMethod != null)
                        break;
                }
                //On crée l'appel à la méthode  Select
                Expression select = Expression.Call(selectMethod.MakeGenericMethod(sourceTypeList, destTypeList), new Expression[] { Expression.Property(paramClassSource, memberSource), expression });
                //On crée l'appel à la méthode  ToList
                Expression toList = Expression.Call(typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(destTypeList), select);
                Expression asExp = Expression.TypeAs(toList, memberDest.PropertyType);
                //test si la propriété source est nul
                var checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, memberSource), Expression.Constant(null));
                var expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(null, memberDest.PropertyType));
                //Affectation de la propriétés de destination
                var expBind = Expression.Bind(memberDest, expCondition);
                this.MemberToMap.Add(expBind);
            }
            else
            {
                //Si le type est différent on va regarde si l'on n'a pas un mappeur
                if (memberSource.PropertyType != memberDest.PropertyType)
                {
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, exceptionMapperNoFound);

                }
                //On regarde si la propriété source existe dans la source
                var property = memberSource.ReflectedType.GetProperty(memberSource.Name);
                if (property != null)
                {
                    //Suppression du mapping d'origine de destination pour cette propriétés
                    this.CheckAndRemoveMemberDest(memberDest.Name);
                    //Suppression du mapping d'origine pour cette propriétés
                    this.CheckAndRemoveMemberSource(memberSource.Name);
                    if (mapperExterne == null)
                        mapperExterne = MapperConfigurationRegister.Instance.Find(memberSource.PropertyType, memberDest.PropertyType);
                    if (mapperExterne != null)
                    {
                        var checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, property), Expression.Constant(null));
                        //Création de la nouvelle class de destination
                        var newClassDest = Expression.New(mapperExterne.TypeDest);

                        //On crée la nouvelle affectation des propriétés avec la source
                        var newMembers = mapperExterne.ChangeSource(property, this.paramClassSource);

                        //Initialisation de l'affectation des propriétés de l'objet de destination
                        var exp = Expression.MemberInit(newClassDest, newMembers);

                        //Création de la condition de test
                        var expCondition = Expression.Condition(checkIfNull, exp, Expression.Constant(null, memberDest.PropertyType));

                        //Affectation de la propriétés de destination
                        var expBind = Expression.Bind(memberDest, expCondition);
                        this.MemberToMap.Add(expBind);
                    }
                    else if (memberDest.PropertyType == property.PropertyType)
                    {
                        MemberExpression memberClassSource = Expression.Property(paramClassSource, property);
                        var expBind = Expression.Bind(memberDest, memberClassSource);
                        this.MemberToMap.Add(expBind);
                    }
                }
                else
                {
                    throw new Exception("La propriété '" + memberSource.Name + "'n'existe pas pour le type '" + memberSource.DeclaringType.ToString() + "'");
                }
            }

        }

        protected void CheckAndRemoveMemberDest(string properyName)
        {
            if (this.MemberToMap.Exists(m => m.Member.Name == properyName))
            {
                this.MemberToMap.RemoveAll(m => m.Member.Name == properyName);
            }

        }

        protected void CheckAndRemoveMemberSource(string properyName)
        {
            if (this.MemberToMap.Exists(m => m.Expression is MemberExpression && (m.Expression as MemberExpression).Member.Name == properyName))
            {
                this.MemberToMap.RemoveAll(m => m.Expression is MemberExpression && (m.Expression as MemberExpression).Member.Name == properyName);
            }

        }

        protected MemberInitExpression GetMemberInitExpression()
        {
            Type typeDest = this.TypeDest;
            if (this.UseServiceLocator)
            {
                if (constructorFunc != null)
                    typeDest = constructorFunc(typeDest).GetType();
                else
                {
                    throw new Exception("aucun servicelocator configuré");
                }
            }
            var newClassDest = Expression.New(typeDest);
            //new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            var exp = Expression.MemberInit(newClassDest, this.MemberToMap);
            return exp;
        }
    }

}

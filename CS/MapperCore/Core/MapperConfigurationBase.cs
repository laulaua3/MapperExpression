using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exception;
using System.Data.Linq;

namespace MapperExpression.Core
{
    /// <summary>
    /// Class de base de la gestion du mapping
    /// </summary>
    public abstract class MapperConfigurationBase
    {
        #region Variables

        protected ParameterExpression paramClassSource;
        protected Delegate delegateCall;
        protected Func<Type, object> constructorFunc;
        protected bool isInitialized = false;
        protected List<Tuple<LambdaExpression, LambdaExpression, bool>> propertiesMapping;
        protected List<PropertyInfo> propertiesToIgnore;

        #endregion

        #region Proprietes

        /// <summary>
        /// Indique si l'on utilise le service d'injection
        /// </summary>
        public bool UseServiceLocator { get; protected set; }

        /// <summary>
        /// Gets the type source.
        /// </summary>
        public Type TypeSource { get; private set; }

        /// <summary>
        /// Gets the type dest.
        /// </summary>
        public Type TypeDest { get; private set; }

        /// <summary>
        /// Gets the member to map.
        /// </summary>
        public List<MemberAssignment> MemberToMap { get; protected set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfigurationBase"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public MapperConfigurationBase(Type source, Type destination)
        {
            TypeDest = destination;
            TypeSource = source;
            paramClassSource = Expression.Parameter(source, "source");
            MemberToMap = new List<MemberAssignment>();
        }
      
        #endregion

        #region Méthodes publiques

       

        /// <summary>
        /// Gets the delegate of mapping.
        /// </summary>
        /// <returns></returns>
        public Delegate GetDelegate()
        {
            if (!isInitialized)
            {
                throw new MapperNotInitializedException(TypeSource, TypeDest);
            }
            //Le fait de stocker le delegate réduit considérablement le temps de traitement 
            //Super Perf!!! 
            //(pas de compile de l'expression à chaque appel qui est très consommateur)
            if (delegateCall == null)
            {
                MemberInitExpression exp = GetMemberInitExpression();

                delegateCall = Expression.Lambda(exp, paramClassSource).Compile();
            }
            return delegateCall;
        }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        /// <returns></returns>
        public Type GetDestinationType()
        {
            if (UseServiceLocator)
                return constructorFunc(TypeDest).GetType();
            return TypeDest;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Changes the original source.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="paramSource">The parameter source.</param>
        /// <returns></returns>
        protected List<MemberAssignment> ChangeSource(PropertyInfo property, ParameterExpression paramSource)
        {
            if (!isInitialized)
            {
                CreateMappingExpression(constructorFunc);
            }
            List<MemberAssignment> membersTransformed = new List<MemberAssignment>();
            MemberToMap.ForEach((m) =>
            {
                //Propriété de la classe source référençant la classe enfant
                Expression innerProperty = Expression.Property(paramSource, property);
                //Propriété de la class enfant
                if (m.Expression is MemberExpression)
                {
                    Expression outerProperty = Expression.Property(innerProperty, (m.Expression as MemberExpression).Member as PropertyInfo);
                    //Affectation de la propriété de destination à la propriété de la class enfant
                    MemberAssignment expBind = Expression.Bind(m.Member, outerProperty);

                    membersTransformed.Add(expBind);
                }

            });
            return membersTransformed;
        }
        protected MapperConfigurationBase GetMapper(Type tSource, Type tDest, bool throwExceptionOnNoFound = true)
        {
            MapperConfigurationBase mapperExterne = null;

            mapperExterne = MapperConfigurationContainer.Instance.Find(tSource, tDest);
            //on levé une exception si rien n'est trouvé
            if (mapperExterne == null && throwExceptionOnNoFound)
                throw new NoFoundMapperException(tSource, tDest);

            return mapperExterne;
        }

        protected  void CreateCommonMember()
        {
            PropertyInfo[] propertiesSource = TypeSource.GetProperties();
            ParameterExpression paramDest = Expression.Parameter(TypeDest, "d");
         
            foreach (PropertyInfo propSource in propertiesSource)
            {
                PropertyInfo propDest = TypeDest.GetProperty(propSource.Name);
                if (propDest != null)
                {
                    bool ignorePropDest = propertiesToIgnore.Exists(x => x.Name == propDest.Name);
                    if (propDest.CanWrite && !ignorePropDest && propDest.PropertyType == propSource.PropertyType)
                    {
                        LambdaExpression expSource = Expression.Lambda(Expression.MakeMemberAccess(paramClassSource, propSource), paramClassSource);
                        LambdaExpression expDest = Expression.Lambda(Expression.MakeMemberAccess(paramDest, propDest), paramDest);
                        propertiesMapping.Add(Tuple.Create(expSource, expDest, false));
                    }
                }
            }
        }

        protected void CreateMemberAssignement(PropertyInfo memberSource, PropertyInfo memberDest)
        {

            //On supprime l'ancien (si appel plusieurs fois de la méthode)
            MemberToMap.RemoveAll(m => m.Member.Name == memberSource.Name);

            if (!memberDest.CanWrite)
            {
                throw new ReadOnlyPropertyException(memberDest);
            }
            //On regarde si la propriété est une liste
            bool isList = CheckAndConfigureTypeOfList(memberSource, memberDest);
            //Si pas une liste
            if (!isList)
            {

                CheckAndConfigureMembersMapping(memberSource, memberDest);
            }
        }

        protected void CheckAndConfigureMembersMapping(PropertyInfo memberSource, PropertyInfo memberDest)
        {
            MapperConfigurationBase mapperExterne = null;
            //On regarde si la propriété source existe dans la source 
            //(on fait cela dans le cas où la propriété de l'expression n'est pas sur le même objet que celui de base)
            //Exemple
            //On veut mapper
            //destination.LaPropriete = source.SubClass.LaPropriete
            PropertyInfo property = TypeSource.GetProperty(memberSource.Name);
            if (property != null && memberSource.DeclaringType == TypeSource)
            {
                //Suppression du mapping d'origine de destination pour cette propriété
                CheckAndRemoveMemberDest(memberDest.Name);
                //Suppression du mapping d'origine pour cette propriété
                CheckAndRemoveMemberSource(memberSource.Name);
                if (memberDest.PropertyType == property.PropertyType)
                {
                    //Si la propriété correspond bien au type source
                    if (property.ReflectedType == TypeSource)
                    {
                        MemberExpression memberClassSource = Expression.Property(paramClassSource, property);

                        MemberAssignment expBind = Expression.Bind(memberDest, memberClassSource);
                        MemberToMap.Add(expBind);
                    }
                }
                else
                {
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, false);
                    if (mapperExterne != null)
                    {
                        CreateCheckIfNull(property, memberDest, mapperExterne);
                    }
                    else //On lève un exception
                    {
                        throw new NotSameTypePropertyException(memberSource.PropertyType, memberDest.PropertyType);
                    }
                }
            }
            else
            {
                //Si le type est différent on va regarde si l'on n'a pas un mappeur
                if (memberSource.PropertyType != memberDest.PropertyType)
                {
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, false);
                    if (mapperExterne != null)
                    {
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne);
                    }
                    else //On lève un exception
                    {
                        throw new NotSameTypePropertyException(memberSource.PropertyType, memberDest.PropertyType);
                    }
                }
                //Si on vient de la class de base
                else if (memberSource.ReflectedType == TypeSource)
                {
                    MemberExpression memberClassSource = Expression.Property(paramClassSource, memberSource);
                    MemberAssignment expBind = Expression.Bind(memberDest, memberClassSource);
                    MemberToMap.Add(expBind);
                }
                //cas :
                //destination.LaPropriete = source.SubClass.LaPropriete
                else
                {
                    if (mapperExterne != null)
                    {
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne);
                    }
                    else
                    {
                        //On va récupérer l'expression source
                        Tuple<LambdaExpression, LambdaExpression, bool> expMember = propertiesMapping.Find(s => GetPropertyInfo(s.Item1).Name == memberSource.Name);
                        if (expMember != null)
                        {
                            //On crée la nouvelle expression
                            Expression memberAccess = CreateMemberAssign(expMember.Item1, expMember.Item3);
                            //Et on assigne (cool)
                            MemberAssignment expBind = Expression.Bind(memberDest, memberAccess);
                            MemberToMap.Add(expBind);
                        }
                    }
                }
            }
            
        }

        protected bool CheckAndConfigureTypeOfList(PropertyInfo memberSource, PropertyInfo memberDest)
        {


            if (memberSource.PropertyType.GetInterfaces().Count(t => t == typeof(IList)) > 0)
            {
                MapperConfigurationBase mapperExterne = null;

                //Type de la liste source
                Type sourceTypeList = memberSource.PropertyType.GetGenericArguments()[0];
                // Type de la liste de destination
                Type destTypeList = memberDest.PropertyType.GetGenericArguments()[0];
                mapperExterne = GetMapper(sourceTypeList, destTypeList);
                //Pour initialisé le mappeur
                mapperExterne.CreateMappingExpression(constructorFunc);
                //Suppression du mapping d'origine pour cette propriété
                CheckAndRemoveMemberSource(memberSource.Name);
                CheckAndRemoveMemberDest(memberDest.Name);
                //Appel de la méthode pour récupère l'expression lambda pour le select
                MethodInfo methodeGetExpression = mapperExterne.GetType().GetMethod("GetLambdaExpression");

                Expression expMappeur = methodeGetExpression.Invoke(mapperExterne, null) as Expression;
                //On cherche la méthode select
                MethodInfo selectMethod = null;
                IEnumerable<MethodInfo> selectsMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "Select");
                //Pour prendre la bonne (y en a 2)
                foreach (MethodInfo m in selectsMethod)
                {
                    IEnumerable<ParameterInfo> parameters = m.GetParameters().Where(p => p.Name.Equals("selector"));
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
                Expression select = Expression.Call(selectMethod.MakeGenericMethod(sourceTypeList, destTypeList),
                    new Expression[] 
                    { 
                        Expression.Property(paramClassSource, memberSource), expMappeur 
                    });
                //On crée l'appel à la méthode  ToList
                Expression toList = Expression.Call(typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(destTypeList), select);
                Expression asExp = Expression.TypeAs(toList, memberDest.PropertyType);
                //test si la propriété source est nul
                Expression checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, memberSource), Expression.Constant(null));
                Expression expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(null, memberDest.PropertyType));
                //Affectation de la propriétés de destination
                MemberAssignment expBind = Expression.Bind(memberDest, expCondition);
                MemberToMap.Add(expBind);
                return true;
            }
            return false;
        }

        protected void CreateCheckIfNull(PropertyInfo memberSource, PropertyInfo memberDest, MapperConfigurationBase mapperExterne)
        {
            Expression checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, memberSource), Expression.Constant(null, memberSource.PropertyType));

            //Création de la nouvelle class de destination
            NewExpression newClassDest  = Expression.New(mapperExterne.GetDestinationType());

            //On crée la nouvelle affectation des propriétés avec la source
            List<MemberAssignment> newMembers = mapperExterne.ChangeSource(memberSource, paramClassSource);

            //Initialisation de l'affectation des propriétés de l'objet de destination
            Expression exp = Expression.MemberInit(newClassDest, newMembers);

            //Création de la condition de test
            Expression expCondition = Expression.Condition(checkIfNull, exp, Expression.Constant(null, memberDest.PropertyType));

            //Affectation de la propriétés de destination
            MemberAssignment expBind = Expression.Bind(memberDest, expCondition);
            MemberToMap.Add(expBind);
        }

        protected void CheckAndRemoveMemberDest(string properyName)
        {
            Predicate<MemberAssignment> exp = m => m.Member.Name == properyName;
            if (MemberToMap.Exists(exp))
            {
                MemberToMap.RemoveAll(exp);
            }

        }

        protected void CheckAndRemoveMemberSource(string properyName)
        {
            Predicate<MemberAssignment> exp = m => m.Expression is MemberExpression
                && (m.Expression as MemberExpression).Member.Name == properyName;
            if (MemberToMap.Exists(exp))
            {
                MemberToMap.RemoveAll(exp);
            }
        }

        protected MemberInitExpression GetMemberInitExpression()
        {
            Type typeDest = GetDestinationType();

            NewExpression newClassDest = Expression.New(typeDest);

            //new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            MemberInitExpression exp = Expression.MemberInit(newClassDest, MemberToMap);
            return exp;
        }

        protected Expression CreateMemberAssign(Expression propertyExpression, bool checkIfNull)
        {
            MapperExpressionVisitor visitor = new MapperExpressionVisitor(checkIfNull, paramClassSource);
            //Visite l'expression pour sa transformation
            Expression result = visitor.Visit(propertyExpression);

            if (result.NodeType == ExpressionType.Lambda)
                return (result as LambdaExpression).Body;
            return result;
        }

        protected MapperConfigurationBase ForMember(LambdaExpression getPropertySource, LambdaExpression getPropertyDest, bool checkIfNull = false)
        {

            //Ajout dans la liste pour le traitement ultérieur
            propertiesMapping.Add(Tuple.Create<LambdaExpression, LambdaExpression, bool>(getPropertySource, getPropertyDest, checkIfNull));
            return this;
        }

        protected PropertyInfo GetPropertyInfo(LambdaExpression propertyExpression)
        {
            switch (propertyExpression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    Expression operand = (propertyExpression.Body as UnaryExpression).Operand;
                    if (operand.NodeType == ExpressionType.MemberAccess)
                    {
                        return (operand as MemberExpression).Member as PropertyInfo;
                    }
                    else
                    {
                        throw new NotImplementedException("Ce type d'expression n'est pas prit en charge");
                    }
                case ExpressionType.MemberAccess:
                    return (propertyExpression.Body as MemberExpression).Member as PropertyInfo;
                default:
                    throw new NotImplementedException("Ce type d'expression n'est pas valide");
            }
        }

        internal virtual void CreateMappingExpression(Func<Type, object> constructor)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                if (UseServiceLocator)
                    constructorFunc = constructor;
                CreateCommonMember();
                GetDelegate();
            }
        }

        #endregion       
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exception;

namespace MapperExpression.Core
{
    /// <summary>
    /// Basic Class for managing the mapping
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

        #region Properties

        /// <summary>
        /// Indicates whether one uses the service injection
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

        #region Constructor

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

        #region Publics methods

       

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
            //Storing the delegate significantly reduces the processing time 
            //Super Perf!!! 
            //(no expression compiles every call which is very consumer)
            if (delegateCall == null)
            {
                MemberInitExpression exp = GetMemberInitExpression();

                delegateCall = Expression.Lambda(exp, paramClassSource).Compile();
            }
            return delegateCall;
        }

        /// <summary>
        /// Gets the real type of the destination.
        /// </summary>
        /// <returns></returns>
        public Type GetDestinationType()
        {
            if (UseServiceLocator)
                return constructorFunc(TypeDest).GetType();
            return TypeDest;
        }

        #endregion

        #region Privates methods

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
                //Property of the source class referencing the child class
                Expression innerProperty = Expression.Property(paramSource, property);
                //Property of the child class
                if (m.Expression is MemberExpression)
                {
                    Expression outerProperty = Expression.Property(innerProperty, (m.Expression as MemberExpression).Member as PropertyInfo);
                    //Assigning the destination property to the property of the child class
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
            //we threw an exception if nothing is found
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

            //It removes the old (if repeatedly call the method)
            MemberToMap.RemoveAll(m => m.Member.Name == memberSource.Name);

            if (!memberDest.CanWrite)
            {
                throw new ReadOnlyPropertyException(memberDest);
            }
            //We check whether the property is a list
            bool isList = CheckAndConfigureTypeOfList(memberSource, memberDest);
            //If not a list
            if (!isList)
            {

                CheckAndConfigureMembersMapping(memberSource, memberDest);
            }
        }

        protected void CheckAndConfigureMembersMapping(PropertyInfo memberSource, PropertyInfo memberDest)
        {
            MapperConfigurationBase mapperExterne = null;
            //You look if the source property exists in the source             
            // (this is done in case the property of the expression is not the same object as the base)             
            // Example             
            // We want to map            
            //destination.TheProperty = source.SubClass.TheProperty
            PropertyInfo property = TypeSource.GetProperty(memberSource.Name);
            if (property != null && memberSource.DeclaringType == TypeSource)
            {
                //Deleting the original destination mapping for this property
                CheckAndRemoveMemberDest(memberDest.Name);
                //Removing the original mapping for this property
                CheckAndRemoveMemberSource(memberSource.Name);
                if (memberDest.PropertyType == property.PropertyType)
                {
                    //If the property matches the type of source
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
                    else //It raises an exception
                    {
                        throw new NotSameTypePropertyException(memberSource.PropertyType, memberDest.PropertyType);
                    }
                }
            }
            else
            {
                //If the type is different we will look if we did not Mapper
                if (memberSource.PropertyType != memberDest.PropertyType)
                {
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, false);
                    if (mapperExterne != null)
                    {
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne);
                    }
                    else //It raises an exception
                    {
                        throw new NotSameTypePropertyException(memberSource.PropertyType, memberDest.PropertyType);
                    }
                }
                //If it belongs to the base class
                else if (memberSource.ReflectedType == TypeSource)
                {
                    MemberExpression memberClassSource = Expression.Property(paramClassSource, memberSource);
                    MemberAssignment expBind = Expression.Bind(memberDest, memberClassSource);
                    MemberToMap.Add(expBind);
                }
                //case :
                //destination.TheProperty = source.SubClass.TheProperty
                else
                {
                    if (mapperExterne != null)
                    {
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne);
                    }
                    else
                    {
                        // We will recover the source expression
                        Tuple<LambdaExpression, LambdaExpression, bool> expMember = propertiesMapping.Find(s => GetPropertyInfo(s.Item1).Name == memberSource.Name);
                        if (expMember != null)
                        {
                            //The new expression is created
                            Expression memberAccess = CreateMemberAssign(expMember.Item1, expMember.Item3);
                            //And assigned (cool)
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

                //Type in the source list
                Type sourceTypeList = memberSource.PropertyType.GetGenericArguments()[0];
                //Type in the destination list 
                Type destTypeList = memberDest.PropertyType.GetGenericArguments()[0];
                mapperExterne = GetMapper(sourceTypeList, destTypeList);
                //To initialize the mapper
                mapperExterne.CreateMappingExpression(constructorFunc);
                //Removing the original mapping for this property
                CheckAndRemoveMemberSource(memberSource.Name);
                CheckAndRemoveMemberDest(memberDest.Name);
                //Calling the method to recover the lambda expression to the select
                MethodInfo methodeGetExpression = mapperExterne.GetType().GetMethod("GetLambdaExpression");

                Expression expMappeur = methodeGetExpression.Invoke(mapperExterne, null) as Expression;
                //We seek the select method
                MethodInfo selectMethod = null;
                IEnumerable<MethodInfo> selectsMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "Select");
                //To the right (there are two)
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
                //We create the call to the Select method
                Expression select = Expression.Call(selectMethod.MakeGenericMethod(sourceTypeList, destTypeList),
                    new Expression[] 
                    { 
                        Expression.Property(paramClassSource, memberSource), expMappeur 
                    });
                //We create the call to ToList method
                Expression toList = Expression.Call(typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(destTypeList), select);
                Expression asExp = Expression.TypeAs(toList, memberDest.PropertyType);
                //test if the source property is null
                Expression checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, memberSource), Expression.Constant(null));
                Expression expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(null, memberDest.PropertyType));
                //Assigning the destination properties
                MemberAssignment expBind = Expression.Bind(memberDest, expCondition);
                MemberToMap.Add(expBind);
                return true;
            }
            return false;
        }

        protected void CreateCheckIfNull(PropertyInfo memberSource, PropertyInfo memberDest, MapperConfigurationBase mapperExterne)
        {
            Expression checkIfNull = Expression.NotEqual(Expression.Property(paramClassSource, memberSource), Expression.Constant(null, memberSource.PropertyType));

            //Creation of the new class destination
            NewExpression newClassDest  = Expression.New(mapperExterne.GetDestinationType());

            //We create the new assignment of the properties with the source
            List<MemberAssignment> newMembers = mapperExterne.ChangeSource(memberSource, paramClassSource);

            //Initialize the allocation of properties of the destination object
            Expression exp = Expression.MemberInit(newClassDest, newMembers);

            //Creating the test condition
            Expression expCondition = Expression.Condition(checkIfNull, exp, Expression.Constant(null, memberDest.PropertyType));

            //Assigned members
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
            //Visit the expression for its transformation
            Expression result = visitor.Visit(propertyExpression);

            if (result.NodeType == ExpressionType.Lambda)
                return (result as LambdaExpression).Body;
            return result;
        }

        protected MapperConfigurationBase ForMember(LambdaExpression getPropertySource, LambdaExpression getPropertyDest, bool checkIfNull = false)
        {

            //Adding in the list for further processing
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
                        throw new NotImplementedException("This type of expression is not assumed responsibility");
                    }
                case ExpressionType.MemberAccess:
                    return (propertyExpression.Body as MemberExpression).Member as PropertyInfo;
                default:
                    throw new NotImplementedException("This type of expression is not valid");
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

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exception;
using System.Diagnostics.Contracts;
using MapperExpression.Helper;
using System.Diagnostics;

namespace MapperExpression.Core
{
    /// <summary>
    /// Basic Class for managing the mapping
    /// </summary>
    [DebuggerDisplay("Mapper for {SourceType.Name} To {TargetType.Name}")]
    public abstract class MapperConfigurationBase
    {
        #region Variables        

        internal ParameterExpression paramClassSource;
        /// <summary>
        /// The delegate call for th new instance of TDest
        /// </summary>
        protected Delegate delegateCallForNew;

        /// <summary>
        /// The delegate call for map with a existing object of TDest
        /// </summary>
        protected Delegate delegateCallForExisting;
        /// <summary>
        /// The constructor function
        /// </summary>
        protected Func<Type, object> constructorFunc;
        /// <summary>
        /// The is initialized
        /// </summary>
        protected bool isInitialized = false;
        /// <summary>
        /// The properties mapping
        /// </summary>
        protected List<Tuple<LambdaExpression, LambdaExpression, bool>> propertiesMapping;
        /// <summary>
        /// The properties to ignore
        /// </summary>
        protected List<PropertyInfo> propertiesToIgnore;

        /// <summary>
        /// The expression for existing mapping
        /// </summary>
        protected LambdaExpression expressionForExisting;
        /// <summary>
        /// The visitor mapper
        /// </summary>
        internal MapperExpressionVisitor visitorMapper;
        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether one uses the service injection
        /// </summary>
        public bool UseServiceLocator { get; protected set; }

        /// <summary>
        /// Gets the type source.
        /// </summary>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Gets the type dest.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets the members list to map.
        /// </summary>
        public List<MemberAssignment> MemberToMapForNew { get; protected set; }
        /// <summary>
        /// Name of the mapper.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfigurationBase" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="name">The name.</param>
        public MapperConfigurationBase(Type source, Type destination, string paramName, string name = null)
        {
            TargetType = destination;
            SourceType = source;
            paramClassSource = Expression.Parameter(source, paramName);
            Name = string.IsNullOrEmpty(name) ? paramName : name;

            visitorMapper = new MapperExpressionVisitor(paramClassSource);
            MemberToMapForNew = new List<MemberAssignment>();
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
                throw new MapperNotInitializedException(SourceType, TargetType);
            }
            // It's here that we have the mapper's perfomances
            // Storing the delegate significantly reduces the processing time 
            // Super Perf!!! 
            // (no expression compiles every call which is very consumer)
            if (delegateCallForNew == null)
            {
                MemberInitExpression exp = GetMemberInitExpression();

                delegateCallForNew = Expression.Lambda(exp, paramClassSource).Compile();

            }
            return delegateCallForNew;
        }
        /// <summary>
        /// Gets the delegate for existing target.
        /// </summary>
        /// <exception cref="MapperNotInitializedException"></exception>
        public Delegate GetDelegateForExistingTarget()
        {
            if (!isInitialized)
            {
                throw new MapperNotInitializedException(SourceType, TargetType);
            }
            // It's here that we have the mapper's perfomances
            // Storing the delegate significantly reduces the processing time 
            // Super Perf!!! 
            // (no expression compiles every call which is very consumer)
            if (delegateCallForExisting == null)
            {
                CreateMemberAssignementForExistingTarget();
            }
            return delegateCallForExisting;
        }
        /// <summary>
        /// Gets the generic lambda expression.
        /// </summary>
        public LambdaExpression GetGenericLambdaExpression()
        {
            MemberInitExpression exp = GetMemberInitExpression();
            return Expression.Lambda(exp, paramClassSource);
        }
        /// <summary>
        /// Gets the real type of the destination.
        /// </summary>
        /// <returns></returns>
        public Type GetDestinationType()
        {
            return GetRealType(TargetType); ;
        }
        #endregion

        #region Privates methods      

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <param name="typeOfSource">The t source.</param>
        /// <param name="typeOfTarget">The t dest.</param>
        /// <param name="throwExceptionOnNoFound">if set to <c>true</c> [throw exception on no found].</param>
        /// <returns></returns>
        /// <exception cref="NoFoundMapperException"></exception>
        protected MapperConfigurationBase GetMapper(Type typeOfSource, Type typeOfTarget, bool throwExceptionOnNoFound = true)
        {
            MapperConfigurationBase mapperExterne = null;

            mapperExterne = MapperConfigurationContainer.Instance.Find(typeOfSource, typeOfTarget);
            // we raise an exception if there is nothing and configured
            if (mapperExterne == null && throwExceptionOnNoFound)
                throw new NoFoundMapperException(typeOfSource, typeOfTarget);

            return mapperExterne;
        }

        /// <summary>
        /// Creates the common member.
        /// </summary>
        protected void CreateCommonMember()
        {

            PropertyInfo[] propertiesSource = SourceType.GetProperties();
            ParameterExpression paramDest = Expression.Parameter(TargetType, "d");

            foreach (PropertyInfo propSource in propertiesSource)
            {
                PropertyInfo propDest = TargetType.GetProperty(propSource.Name);
                if (propDest != null)
                {
                    // We check if already exist or ignored.
                    bool ignorePropDest = propertiesToIgnore.Exists(x => x.Name == propDest.Name) ||
                        propertiesMapping.Exists(x => GetPropertyInfo(x.Item2).Name == propDest.Name);

                    if (propDest.CanWrite && !ignorePropDest)
                    {
                        Type sourceType = propSource.PropertyType;
                        Type destType = propDest.PropertyType;
                        bool isList = IsListOf(destType);
                        if (isList)
                        {
                            sourceType = propSource.PropertyType.GetGenericArguments().First();
                            destType = propDest.PropertyType.GetGenericArguments().First();
                        }
                        bool createConfig = CreateConfig(sourceType, destType);
                        if (createConfig)
                        {
                            //We create only the relation for the moment 
                            LambdaExpression expSource = Expression.Lambda(Expression.MakeMemberAccess(paramClassSource, propSource), paramClassSource);
                            LambdaExpression expDest = Expression.Lambda(Expression.MakeMemberAccess(paramDest, propDest), paramDest);
                            propertiesMapping.Add(Tuple.Create(expSource, expDest, false));
                        }
                    }
                }
            }

        }

        private bool CreateConfig(Type typeSource, Type typeTarget)
        {
            bool result = false;
            result = typeSource == typeTarget;
            //Is not the same type
            if (!result)
            {
                //Find if a mapper exist
                result = GetMapper(typeSource, typeTarget, false) != null;
            }

            return result;
        }

        /// <summary>
        /// Checks the and configure mapping.
        /// </summary>
        /// <param name="configExpression">The configuration expression.</param>
        /// <exception cref="NotSameTypePropertyException">
        /// </exception>
        /// <exception cref="ReadOnlyPropertyException"></exception>
        protected void CheckAndConfigureMapping(Tuple<LambdaExpression, LambdaExpression, bool> configExpression)
        {

            Type typeSource = configExpression.Item1.Body.Type;
            Type typeTarget = configExpression.Item2.Body.Type;
            MemberAssignment bindExpression = null;
            // Normaly the target expression is a MemberExpression
            PropertyInfo propTarget = GetPropertyInfo(configExpression.Item2);

            if (propTarget.CanWrite)
            {
                CheckAndRemoveMemberDest(propTarget.Name);
                if (!IsListOf(typeTarget))
                {
                    // no specific action.
                    if (typeSource == typeTarget)
                    {

                        // We create the binding.
                        CreateMemberBinding(configExpression.Item1.Body, propTarget, configExpression.Item3);

                    }
                    else
                    {
                        // Try to find a mapper.
                        var externalMapper = GetMapper(typeSource, typeTarget, false);
                        if (externalMapper != null)
                        {
                            Expression mapExpression = externalMapper.GetMemberInitExpression();
                            Expression defaultExpression = Expression.Constant(MapperHelper.GetDefaultValue(configExpression.Item1.Body.Type), configExpression.Item1.Body.Type);
                            // To change the parameter.
                            Expression expSource = visitorMapper.Visit(configExpression.Item1.Body, false);
                            ChangParameterExpressionVisitor changeParamaterVisitor = new ChangParameterExpressionVisitor(expSource);
                            mapExpression = changeParamaterVisitor.Visit(mapExpression);
                            Expression checkIfNull = Expression.NotEqual(expSource, defaultExpression);
                            // Create condition.
                            var checkExpression = Expression.Condition(checkIfNull, mapExpression,
                                Expression.Constant(MapperHelper.GetDefaultValue(mapExpression.Type),
                                mapExpression.Type));
                            bindExpression = Expression.Bind(propTarget, checkExpression);
                            MemberToMapForNew.Add(bindExpression);
                        }
                        else
                        {
                            throw new NotSameTypePropertyException(typeSource, typeTarget);
                        }
                    }

                }
                // IEnumerable<>,IList,ICollection.
                else
                {
                    Type sourceTypeList = typeSource.GetGenericArguments().First();
                    Type destTypeList = typeTarget.GetGenericArguments().First();
                    //No change it's easy
                    if (sourceTypeList == destTypeList)
                    {
                        if (configExpression.Item2.Body.NodeType == ExpressionType.MemberAccess)
                        {
                            CreateMemberBinding(configExpression.Item1.Body, propTarget, configExpression.Item3);
                        }
                    }
                    // Use the extention Enumerable to change type.
                    else
                    {
                        bool isIQuerable = typeSource is IQueryable;
                        var externalMapper = GetMapper(sourceTypeList, destTypeList, false);
                        if (externalMapper != null)
                        {
                            externalMapper.CreateMappingExpression(constructorFunc);

                            Type extentionType = typeof(Enumerable);
                            // We seek the good select method.
                            var goodSelectMethod = extentionType.GetMethods()
                                .Where(m => m.Name == "Select")
                                .Select(x => x.GetParameters().First(p => p.Name.Equals("selector") &&
                                                                     p.ParameterType.GetGenericArguments().Count() == 2))
                                .First().Member as MethodInfo;
                            MemberAssignment expBind = null;
                            ChangParameterExpressionVisitor changeVisitor = new ChangParameterExpressionVisitor(paramClassSource);
                            Expression expSource = changeVisitor.Visit(configExpression.Item1.Body);
                            //For compatibility with EF/LINQ2SQL.
                            LambdaExpression expMappeur = externalMapper.GetGenericLambdaExpression();
                            // We create the call to the Select method.
                            Expression select = Expression.Call(goodSelectMethod.MakeGenericMethod(sourceTypeList, destTypeList),
                                new Expression[] { expSource, expMappeur });
                            // We create the call to ToList method
                            Expression toList = Expression.Call(extentionType.GetMethod("ToList").MakeGenericMethod(destTypeList), select);

                            if (!isIQuerable// Normaly if isIQuerable we don't need to check the nullity.
                                && configExpression.Item3)// But if you want
                            {
                                // test if the source property is null.
                                Expression checkIfNull = Expression.NotEqual(expSource, Expression.Constant(MapperHelper.GetDefaultValue(expSource.Type), expSource.Type));
                                Expression expCondition = null;
                                // For boxing some time the ToList method not working.
                                Expression asExp = Expression.TypeAs(toList, propTarget.PropertyType);
                                // Create condition
                                expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(MapperHelper.GetDefaultValue(typeTarget), typeTarget));

                                // Assigning to the destination propery.
                                expBind = Expression.Bind(propTarget, expCondition);
                            }
                            else
                            {
                                // Assigning to the destination propery.
                                expBind = Expression.Bind(propTarget, toList);
                            }

                            MemberToMapForNew.Add(expBind);
                        }
                        else
                        {
                            throw new NotSameTypePropertyException(typeSource, typeTarget);
                        }
                    }
                }
            }
            else
            {
                throw new ReadOnlyPropertyException(propTarget);
            }

        }

        /// <summary>
        /// Checks  and remove the member dest.
        /// </summary>
        /// <param name="properyName">Name of the propery.</param>
        protected void CheckAndRemoveMemberDest(string properyName)
        {
            Predicate<MemberAssignment> exp = m => m.Member.Name == properyName;
            if (MemberToMapForNew.Exists(exp))
            {
                MemberToMapForNew.RemoveAll(exp);
            }

        }

        /// <summary>
        /// Gets the member initialize expression.
        /// </summary>
        /// <returns></returns>
        protected MemberInitExpression GetMemberInitExpression()
        {
            Type typeDest = GetDestinationType();

            NewExpression newClassDest = Expression.New(typeDest);

            // new ClassDestination() { Test1 = source.Test1, Test2 = source.Test2 };
            MemberInitExpression exp = Expression.MemberInit(newClassDest, MemberToMapForNew);
            return exp;
        }

        /// <summary>
        /// Creates the member binding.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="propertyTarget">The property target.</param>
        /// <param name="checkIfNull">if set to <c>true</c> [check if null].</param>
        protected void CreateMemberBinding(Expression propertyExpression, PropertyInfo propertyTarget, bool checkIfNull)
        {
            // Visit the expression for its transformation.
            Expression result = visitorMapper.Visit(propertyExpression, checkIfNull);
            MemberAssignment bind = Expression.Bind(propertyTarget, result);
            MemberToMapForNew.Add(bind);

        }

        /// <summary>
        /// Assign the mapping for the expression source to the property destination.
        /// </summary>
        /// <param name="getPropertySource">The expression of property source.</param>
        /// <param name="getPropertyDest">The expression of property dest.</param>
        /// <param name="checkIfNull">if set to <c>true</c> [check if null].</param>
        /// <returns></returns>
        protected MapperConfigurationBase ForMember(LambdaExpression getPropertySource, LambdaExpression getPropertyDest, bool checkIfNull = false)
        {
            // Adding in the list for further processing.
            propertiesMapping.Add(Tuple.Create(getPropertySource, getPropertyDest, checkIfNull));


            return this;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">
        /// This type of expression is not assumed responsibility
        /// or
        /// This type of expression is not valid
        /// </exception>
        protected PropertyInfo GetPropertyInfo(LambdaExpression propertyExpression)
        {
            switch (propertyExpression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    Expression operand = (propertyExpression.Body as UnaryExpression).Operand;

                    switch (operand.NodeType)
                    {
                        case ExpressionType.MemberAccess:
                            return (operand as MemberExpression).Member as PropertyInfo;
                        default:
                            throw new NotImplementedException("This type of expression is not assumed responsibility");

                    }
                case ExpressionType.MemberAccess:
                    return (propertyExpression.Body as MemberExpression).Member as PropertyInfo;
                default:
                    throw new NotImplementedException("This type of expression is not assumed responsibility");
            }
        }

        internal virtual void CreateMemberAssignementForExistingTarget()
        {
            if (delegateCallForExisting == null)
            {
                // For change the parameter of the original expression.
                MapperExpressionVisitor visitSource = new MapperExpressionVisitor(paramClassSource);
                MapperExpressionVisitor visitDest = new MapperExpressionVisitor(Expression.Parameter(TargetType, paramClassSource.Name.Replace("s","t")));

                IEnumerable<Expression> finalAssign = CreateExpressions(propertiesMapping, visitSource, visitDest);
                expressionForExisting = Expression.Lambda(Expression.Block(finalAssign), visitSource.Parameter, visitDest.Parameter);
                // Assign the delegate
                delegateCallForExisting = expressionForExisting.Compile();
            }
        }
        internal Expression GetLambdaDest(string propertyName)
        {
            var exp = propertiesMapping.Find(x => GetPropertyInfo(x.Item1).Name == propertyName);
            if (exp != null)
            {
                var final = exp.Item2.Body;
                if (final.NodeType == ExpressionType.Convert)
                {
                    final = (final as UnaryExpression).Operand;
                }
                return final;
            }
            return null;
        }

        internal virtual void CreateMappingExpression(Func<Type, object> constructor)
        {
            if (!isInitialized)
            {
                // it is put before treatment to avoid recursive loops.
                isInitialized = true;
                constructorFunc = constructor;
                CreateCommonMember();

                for (int i = 0; i < propertiesMapping.Count; i++)
                {
                    CheckAndConfigureMapping(propertiesMapping[i]);
                }
                // creation of delegate.
                GetDelegate();
            }
        }

        internal IEnumerable<Expression> CreateExpressions(List<Tuple<LambdaExpression, LambdaExpression, bool>> propertiesMapping,
            MapperExpressionVisitor expressionVisitorSource,
            MapperExpressionVisitor expressionVisitorTarget)
        {

            for (int i = 0; i < propertiesMapping.Count; i++)
            {
                var item = propertiesMapping[i];

                // For only change the source parameter of expression.
                var expSource = expressionVisitorSource.Visit(item.Item1, item.Item3);
                var expDest = expressionVisitorTarget.Visit(item.Item2);
                if (!expSource.Type.IsValueType && expSource.Type != expDest.Type)
                {
                    // Try to find a mapper.
                    var mapperExterne = GetMapper(expSource.Type, expDest.Type, false);
                    if (mapperExterne != null)
                    {

                        // Create the delegate.

                        mapperExterne.CreateMemberAssignementForExistingTarget();
                        var exps = (mapperExterne.expressionForExisting.Body as BlockExpression).Expressions;
                        // Check If Null
                        var ifExpression = Expression.NotEqual(expSource, Expression.Constant(null, expSource.Type));

                        var check = Expression.IfThen(ifExpression, mapperExterne.expressionForExisting);
                        //  yield return check;
                    }
                }
                else
                {

                    yield return Expression.Assign(expDest, expSource);
                }

            }
        }

        internal Type GetRealType(Type typeToFind)
        {
            if (UseServiceLocator)
                return constructorFunc(typeToFind).GetType();
            return typeToFind;
        }
        #endregion

        private bool IsListOf(Type typeTarget)
        {
            // Special case string is a list of char.
            if (typeTarget.IsAssignableFrom(typeof(string)))
            {
                return false;
            }
            Func<Type, bool> test = t => t.IsAssignableFrom(typeof(IEnumerable));

            return test(typeTarget) || typeTarget.GetInterfaces().Any(test);
        }
    }
}

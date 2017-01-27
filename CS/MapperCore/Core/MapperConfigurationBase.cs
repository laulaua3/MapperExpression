using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MapperExpression.Exceptions;
using MapperExpression.Helper;
using System.Diagnostics;
using MapperExpression.Core.Visitor;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace MapperExpression.Core
{
    /// <summary>
    /// Basic class for managing the mapping
    /// </summary>
    [DebuggerDisplay("Mapper for {SourceType.Name} To {TargetType.Name} ({Name})")]
    public abstract class MapperConfigurationBase
    {

        #region Private's variables

        private Delegate delegateCallForNew;

        private Delegate delegateCallForExisting;

        private Func<Type, object> constructorFunc;

        private bool isInitialized = false;

        //Item1 : Source Expression 
        //Item2 : Target Expression
        //Item3 : Check Nullity
        //Item4 : Name of the mapper
        private List<Tuple<Expression, Expression, bool, string>> propertiesMapping;

        readonly MethodInfo selectMethod;

        readonly MethodInfo toListMethod;

        readonly List<PropertyInfo> propertiesToIgnore;

        #endregion

        #region Internal's variables

        internal ParameterExpression paramClassSource;
        internal MapperExpressionVisitor visitorMapper;
        internal List<MemberAssignment> memberForNew;
        internal LambdaExpression expressionForExisting;
        #endregion

        #region protected's variables

        /// <summary>
        /// The properties mapping
        /// </summary>
        protected List<Tuple<Expression, Expression, bool, string>> PropertiesMapping
        {
            get { return propertiesMapping; }
        }


        /// <summary>
        /// The properties to ignore
        /// </summary>
        protected ReadOnlyCollection<PropertyInfo> PropertiesToIgnore
        {
            get
            {
                return propertiesToIgnore.AsReadOnly();
            }
        }

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
        public ReadOnlyCollection<MemberAssignment> MemberToMapForNew
        {
            get
            {
                return new ReadOnlyCollection<MemberAssignment>(memberForNew);
            }
        }


        private string name;
        /// <summary>
        /// Name of the mapper.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            protected set
            {
                name = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfigurationBase" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="name">The name.</param>
        protected MapperConfigurationBase(Type source, Type destination, string paramName, string name = null)
        {
            TargetType = destination;
            SourceType = source;
            paramClassSource = Expression.Parameter(source, paramName);
            Name = string.IsNullOrEmpty(name) ? paramName : name;
            propertiesToIgnore = new List<PropertyInfo>();
            propertiesMapping = new List<Tuple<Expression, Expression, bool, string>>();
            visitorMapper = new MapperExpressionVisitor(paramClassSource);
            memberForNew = new List<MemberAssignment>();
            selectMethod = typeof(Enumerable).GetMethods()
                                .Where(m => m.Name == "Select")
                                .Select(x => x.GetParameters().First(p => p.Name.Equals("selector") &&
                                                                     p.ParameterType.GetGenericArguments().Count() == 2))
                                .First().Member as MethodInfo;
            toListMethod = typeof(Enumerable).GetMethod("ToList");
        }

        #endregion

        #region Public's methods

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
        public Type GetDestinationType()
        {
            return GetRealType(TargetType);
        }

        /// <summary>
        /// Ignores the specified property dest.
        /// </summary>
        /// <typeparam name="TDest">The type of the dest.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyDest">The property dest.</param>
        /// <returns></returns>
        protected MapperConfigurationBase IgnoreBase<TDest, TProperty>(Expression<Func<TDest, TProperty>> propertyDest)
        {
            // Adding in the list for further processing
            propertiesToIgnore.Add(GetPropertyInfo(propertyDest));
            return this;
        }
        #endregion

        #region Protected's methods      

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <param name="typeOfSource">The t source.</param>
        /// <param name="typeOfTarget">The t dest.</param>
        /// <param name="throwExceptionOnNoFound">if set to <c>true</c> [throw exception on no found].</param>
        /// <param name="name">The name of mapper.</param>
        /// <returns></returns>
        /// <exception cref="NoFoundMapperException"></exception>
        protected static MapperConfigurationBase GetMapper(Type typeOfSource, Type typeOfTarget, bool throwExceptionOnNoFound, string name = null)
        {
            MapperConfigurationBase mapperExterne ;

            mapperExterne = MapperConfigurationCollectionContainer.Instance.Find(typeOfSource, typeOfTarget, name);
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
                            sourceType = TypeSystem.GetElementType(propSource.PropertyType);
                            destType = TypeSystem.GetElementType(propDest.PropertyType);
                        }
                        var canCreateConfig = CanCreateConfig(sourceType, destType);
                        if (canCreateConfig.CanCreate)
                        {
                            //We create only the relation for the moment 
                            Expression expSource = Expression.MakeMemberAccess(paramClassSource, propSource);
                            ParameterExpression paramDest = Expression.Parameter(TargetType, "t");
                            Expression expDest = Expression.MakeMemberAccess(paramDest, propDest);
                            propertiesMapping.Add(Tuple.Create(expSource, expDest, false, canCreateConfig.MapperName));
                        }
                    }
                }
            }

        }

        private static CreateConfig CanCreateConfig(Type typeSource, Type typeTarget)
        {
            CreateConfig result = new CreateConfig();
            result.CanCreate = typeSource == typeTarget;
            //Is not the same type
            if (!result.CanCreate)
            {
                //Find if a mapper exist
                var mapper = MapperConfigurationCollectionContainer.Instance.Find(typeSource, typeTarget);
                if (mapper != null)
                {
                    result.MapperName = mapper.Name;
                    result.CanCreate = true;
                }
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
        protected void CheckAndConfigureMapping(ref Tuple<Expression, Expression, bool, string> configExpression)
        {
            Contract.Requires(configExpression != null);
            Type typeSource = configExpression.Item1.Type;
            Type typeTarget = configExpression.Item2.Type;

            // Normaly the target expression is a MemberExpression
            PropertyInfo propTarget = GetPropertyInfo(configExpression.Item2);

            if (propTarget.CanWrite)
            {
                CheckAndRemoveMemberDest(propTarget.Name);
                if (!IsListOf(typeTarget))
                {
                    CreatBindingFromSimple(ref configExpression, typeSource, typeTarget, propTarget);
                }
                else
                {
                    CreateBindingFromList(ref configExpression, typeSource, typeTarget, propTarget);
                }
            }
            else
            {
                throw new ReadOnlyPropertyException(propTarget);
            }

        }

        /// <summary>
        /// Checks and remove the member dest.
        /// </summary>
        /// <param name="properyName">Name of the propery.</param>
        protected void CheckAndRemoveMemberDest(string properyName)
        {
            Predicate<MemberAssignment> exp = m => m.Member.Name == properyName;
            if (memberForNew.Exists(exp))
            {
                memberForNew.RemoveAll(exp);
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
        protected void CreateMemberBinding(Expression propertyExpression, MemberInfo propertyTarget, bool checkIfNull)
        {
            // Visit the expression for its transformation.
            Expression result = visitorMapper.Visit(propertyExpression, checkIfNull);
            MemberAssignment bind = Expression.Bind(propertyTarget, result);
            memberForNew.Add(bind);

        }

        /// <summary>
        /// Assign the mapping for the expression source to the property destination.
        /// </summary>
        /// <param name="getPropertySource">The expression of property source.</param>
        /// <param name="getPropertyDest">The expression of property dest.</param>
        /// <param name="checkIfNull">if set to <c>true</c> [check if null].</param>
        /// <param name="name">Name of the mapper to use </param>
        internal MapperConfigurationBase ForMemberBase(Expression getPropertySource, Expression getPropertyDest, bool checkIfNull, string name = null)
        {
            // Adding in the list for further processing.
            propertiesMapping.Add(Tuple.Create(getPropertySource, getPropertyDest, checkIfNull, name));
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
        protected static PropertyInfo GetPropertyInfo(Expression propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            var expressionToAnalyse = propertyExpression.NodeType == ExpressionType.Lambda ? (propertyExpression as LambdaExpression).Body : propertyExpression;
            switch (expressionToAnalyse.NodeType)
            {
                case ExpressionType.Convert:
                    Expression operand = (expressionToAnalyse as UnaryExpression).Operand;

                    switch (operand.NodeType)
                    {
                        case ExpressionType.MemberAccess:
                            return (operand as MemberExpression).Member as PropertyInfo;
                        default:
                            throw new NotImplementedException("This type of expression is not assumed responsibility");

                    }
                case ExpressionType.MemberAccess:
                    return (expressionToAnalyse as MemberExpression).Member as PropertyInfo;
                default:
                    throw new NotImplementedException("This type of expression is not assumed responsibility");
            }
        }
        #endregion

        #region Internal's methods

        internal void Initialize(Func<Type, object> constructor)
        {

            CreateMappingExpression(constructor);
            CreateMemberAssignementForExistingTarget();
        }
        internal virtual void CreateMemberAssignementForExistingTarget()
        {
            if (propertiesMapping.Count > 0)
            {
                // For change the parameter of the original expression.
                var paramTarget = Expression.Parameter(TargetType, paramClassSource.Name.Replace("s", "t"));
                ChangParameterExpressionVisitor visitSource = new ChangParameterExpressionVisitor(paramClassSource);
                ChangParameterExpressionVisitor visitTarget = new ChangParameterExpressionVisitor(paramTarget);

                List<Expression> finalAssign = new List<Expression>();

                for (int i = 0; i < propertiesMapping.Count; i++)
                {
                    var item = propertiesMapping[i];
                    var propToAssign = visitTarget.Visit(item.Item2);
                    Expression assignExpression ;
                    assignExpression = visitSource.Visit(item.Item1);
                    Type sourceType = TypeSystem.GetElementType(item.Item2.Type);
                    Type targetType = TypeSystem.GetElementType(item.Item1.Type);
                    if (string.IsNullOrEmpty(item.Item4))
                    {
                        object defaultValue = MapperHelper.GetDefaultValue(item.Item2.Type);
                        Expression defaultExpression = Expression.Constant(defaultValue, item.Item2.Type);
                        Expression checkIfNull = Expression.NotEqual(assignExpression, defaultExpression);
                        if (item.Item3)
                        {
                            // Create check if null.
                            Expression setIf = Expression.IfThen(checkIfNull, Expression.Assign(propToAssign, assignExpression));
                            finalAssign.Add(setIf);
                        }
                        else
                        {
                            if (!IsListOf(propToAssign.Type))
                            {
                                finalAssign.Add(Expression.Assign(propToAssign, assignExpression));
                            }
                            else
                            {
                                // NOT TESTED !!!!!
                                if (sourceType == targetType)
                                {
                                    Expression toListExp = Expression.Call(toListMethod.MakeGenericMethod(sourceType), assignExpression);
                                    Expression setIf = Expression.IfThen(checkIfNull, Expression.Assign(propToAssign, toListExp));
                                    finalAssign.Add(setIf);
                                }
                            }
                        }

                    }
                    else   // Come from a other mapper.
                    {

                        var mapper = GetMapper(sourceType, targetType, false, item.Item4);
                        if (mapper != null)
                        {
                            mapper.Initialize(constructorFunc);

                            Expression defaultExpression = Expression.Constant(MapperHelper.GetDefaultValue(item.Item2.Type), item.Item2.Type);
                            if (!IsListOf(propToAssign.Type))
                            {
                                ChangParameterExpressionVisitor changeVisitor = new ChangParameterExpressionVisitor(propToAssign, assignExpression);

                                Expression modifiedExpression = changeVisitor.Visit(mapper.expressionForExisting.Body);
                                Expression checkIfNull = Expression.NotEqual(propToAssign, defaultExpression);
                                Expression setIf = Expression.IfThen(checkIfNull, modifiedExpression);
                                assignExpression = setIf;
                            }
                            else
                            {
                                // Use Select of Enumerable NOT TESTED !!!!!
                                Expression selectExp = Expression.Call(selectMethod.MakeGenericMethod(sourceType), Expression.Constant(mapper.GetDelegate()));
                                Expression checkIfNull = Expression.NotEqual(propToAssign, defaultExpression);
                                Expression setIf = Expression.IfThen(checkIfNull, Expression.Assign(propToAssign, selectExp));
                                assignExpression = setIf;
                            }
                            finalAssign.Add(assignExpression);
                        }
                    }

                }
                if (finalAssign.Count > 0 && delegateCallForExisting == null)
                {
                    expressionForExisting = Expression.Lambda(Expression.Block(typeof(void), finalAssign), paramClassSource, paramTarget);
                    // Assign the delegate.
                    delegateCallForExisting = expressionForExisting.Compile();
                }
            }

        }
        internal Expression GetLambdaDest(string propertyName)
        {
            var exp = propertiesMapping.Find(x => GetPropertyInfo(x.Item1).Name == propertyName);
            if (exp != null)
            {
                var final = exp.Item2;
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
                var propsToAnalyse = propertiesMapping.ToList();// Clone the list because we can change.
                for (int i = 0; i < propsToAnalyse.Count; i++)
                {
                    var propToAnalyse = propsToAnalyse[i];
                    CheckAndConfigureMapping(ref propToAnalyse);
                    propsToAnalyse[i] = propToAnalyse;
                }
                propertiesMapping = propsToAnalyse;
                // Creation of delegate.
                GetDelegate();
            }
        }
        internal Type GetRealType(Type typeToFind)
        {
            if (UseServiceLocator)
                return constructorFunc(typeToFind).GetType();
            return typeToFind;
        }

        internal PropertiesNotMapped GetPropertiesNotMapped()
        {
            PropertiesNotMapped result = new PropertiesNotMapped();
            // Get Propeties0.
            List<PropertyInfo> sourceProperties = SourceType.GetProperties().ToList();
            List<PropertyInfo> targetProperties = TargetType.GetProperties().ToList();

            PropertiesVisitor visitor = new PropertiesVisitor(TargetType);
            for (int i = 0; i < memberForNew.Count; i++)
            {
                var members = memberForNew[i];
                // Simple remove.
                sourceProperties.RemoveAll((p) => members.Member.Name == p.Name);
                targetProperties.RemoveAll((p) => visitor.GetProperties(members.Expression).Contains(p));
            }
            // Check the ignored properties.
            sourceProperties.RemoveAll((p) => propertiesToIgnore.Contains(p));
            result.sourceProperties = sourceProperties;
            result.targetProperties = targetProperties;

            return result;
        }

        internal LambdaExpression GetSortedExpression(string propertySource)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertySource));
            Expression result ;
            var exp = PropertiesMapping.Find(x => GetPropertyInfo(x.Item2).Name == propertySource);
            if (exp == null)
            {
                throw new PropertyNoExistException(propertySource, TargetType);
            }
            // To change the parameter.
            var visitor = new MapperExpressionVisitor(paramClassSource);
            result = visitor.Visit(exp.Item1);
            return Expression.Lambda(result, paramClassSource);

        }

        #endregion

        #region Private's methods

        private static bool IsListOf(Type typeTarget)
        {
            // Special case string is a list of char.
            if (typeTarget == typeof(string))
            {
                return false;
            }
            Func<Type, bool> test = t => t.IsAssignableFrom(typeof(IEnumerable));

            return test(typeTarget) || typeTarget.GetInterfaces().Any(test);
        }

        private MapperConfigurationBase GetAndCheckMapper(Type typeOfSource, Type typeOfTarget, string name)
        {
            var externalMapper = GetMapper(typeOfSource, typeOfTarget, false, name);
            if (externalMapper != null)
            {
                return externalMapper;
            }
            //If the mapper with a name was no found
            else if (!string.IsNullOrEmpty(name))
            {
                throw new NoFoundMapperException(name);
            }
            else
            {
                throw new NotSameTypePropertyException(typeOfSource, typeOfTarget);
            }
        }

        private void CreatBindingFromSimple(ref Tuple<Expression, Expression, bool, string> configExpression, Type typeSource, Type typeTarget, PropertyInfo propTarget)
        {
            // No specific action.
            if (typeSource == typeTarget)
            {
                // We create the binding.
                CreateMemberBinding(configExpression.Item1, propTarget, configExpression.Item3);
            }
            else
            {
                // Try to find a mapper.
                MapperConfigurationBase externalMapper = GetAndCheckMapper(typeSource, typeTarget, configExpression.Item4);
                // If the mapper is not initialized at this moment.
                externalMapper.CreateMappingExpression(constructorFunc);
                // By default, check the nullity of the object.
                Expression mapExpression = externalMapper.GetMemberInitExpression();
                Expression defaultExpression = Expression.Constant(MapperHelper.GetDefaultValue(configExpression.Item1.Type), configExpression.Item1.Type);
                // To change the parameter.
                Expression expSource = visitorMapper.Visit(configExpression.Item1, false);
                ChangParameterExpressionVisitor changeParamaterVisitor = new ChangParameterExpressionVisitor(expSource);
                mapExpression = changeParamaterVisitor.Visit(mapExpression);
                // Now we can create the check with the good parameters.
                Expression checkIfNull = Expression.NotEqual(expSource, defaultExpression);
                // Create condition.
                var checkExpression = Expression.Condition(checkIfNull, mapExpression,
                     Expression.Constant(MapperHelper.GetDefaultValue(mapExpression.Type), mapExpression.Type),
                    mapExpression.Type);
                MemberAssignment bindExpression = Expression.Bind(propTarget, checkExpression);
                // We find the mapper and not configured.
                if (string.IsNullOrEmpty(configExpression.Item4))
                {
                    configExpression = Tuple.Create(configExpression.Item1, configExpression.Item2, configExpression.Item3, externalMapper.Name);
                }
                memberForNew.Add(bindExpression);
            }
        }

        private void CreateBindingFromList(ref Tuple<Expression, Expression, bool, string> configExpression, Type typeSource, Type typeTarget, PropertyInfo propTarget)
        {
            Type sourceTypeList = TypeSystem.GetElementType(typeSource);
            Type destTypeList = TypeSystem.GetElementType(typeTarget);
            // No change it's easy.
            if (sourceTypeList == destTypeList)
            {
                if (configExpression.Item2.NodeType == ExpressionType.MemberAccess)
                {
                    CreateMemberBinding(configExpression.Item1, propTarget, configExpression.Item3);
                }
            }
            // Using "Select" of Enumerable class to change type.
            else
            {
                var externalMapper = GetAndCheckMapper(sourceTypeList, destTypeList, configExpression.Item4);
                externalMapper.CreateMappingExpression(constructorFunc);
                MemberAssignment expBind ;
                Expression expSource = configExpression.Item1;

                ChangParameterExpressionVisitor visitor = new ChangParameterExpressionVisitor(paramClassSource);
                expSource = visitor.Visit(expSource);

                //For compatibility with EF/LINQ2SQL.
                LambdaExpression expMappeur = externalMapper.GetGenericLambdaExpression();
                /* We create the call to the Select method and
                   we insert a lambda expression in select of Enumerable(the parameter is a delegate),
                   normally, that's impossible but (we think) that the compiler creating like this and that LINQ2SQL/EF can make the sql query.*/
                Expression select = Expression.Call(selectMethod.MakeGenericMethod(sourceTypeList, destTypeList),
                    new Expression[] { expSource, expMappeur });
                // We create the call to ToList method.
                Expression toList = Expression.Call(toListMethod.MakeGenericMethod(destTypeList), select);

                if (configExpression.Item3) // If you want check the nullity(with EF/LinqTosql, you don't need).
                {

                    // Test if the source property is null.
                    Expression checkIfNull = Expression.NotEqual(expSource, Expression.Constant(MapperHelper.GetDefaultValue(expSource.Type), expSource.Type));
                    Expression expCondition ;
                    // For boxing some time the ToList method not working.
                    // With a class that implement a list that does not work.
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
                // We find the mapper.
                if (string.IsNullOrEmpty(configExpression.Item4))
                {
                    configExpression = Tuple.Create(configExpression.Item1, configExpression.Item2, configExpression.Item3, externalMapper.Name);
                }
                memberForNew.Add(expBind);
            }
        }

        #endregion
    }
}
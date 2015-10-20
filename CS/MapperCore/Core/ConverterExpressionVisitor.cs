using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MapperExpression.Core
{
    /// <summary>
    /// Visitor expression that converts an expression from source to target
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDest">The type of the dest.</typeparam>
    internal class ConverterExpressionVisitor<TSource, TDest> : ExpressionVisitor
    {
        private ParameterExpression paramClassSource;
        private MapperConfiguration<TSource, TDest> mapper;

        /// <summary>
        /// Gets the new parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        internal ParameterExpression Parameter
        {
            get
            {
                return paramClassSource;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterExpressionVisitor{TSource, TDest}"/> class.
        /// </summary>
        internal ConverterExpressionVisitor()
        {
            mapper = Mapper.GetMapper(typeof(TSource), typeof(TDest)) as MapperConfiguration<TSource, TDest>;
            paramClassSource = Expression.Parameter(typeof(TDest), "d");
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return paramClassSource;
        }

        public override Expression Visit(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:

                    return base.Visit((node as LambdaExpression).Body);
                default:

                    return base.Visit(node);
            }

        }
        protected override Expression VisitMember(MemberExpression node)
        {
            Expression exp = base.Visit(node.Expression);
            //For children class
            if (exp.NodeType == ExpressionType.MemberAccess && (exp as MemberExpression).Member.DeclaringType.IsClass)
            {

                var subMapper = Mapper.GetMapper(node.Member.ReflectedType, exp.Type);
                //Need to call dynamicaly the method because it inside the MapperConfiguration<TSource,TDest> class 
                MethodInfo methodGetPropertyInfoDest = subMapper.GetType()
                                                                .GetMethod("GetPropertyInfoDest", BindingFlags.NonPublic | BindingFlags.Instance);
                //For the performances
                Func<string, PropertyInfo> executeDelegate = (Func<string, PropertyInfo>)Delegate.CreateDelegate(typeof(Func<string, PropertyInfo>),
                                                                                                                 subMapper,
                                                                                                                 methodGetPropertyInfoDest);
                PropertyInfo property = executeDelegate(node.Member.Name);
                //Not need test the PropertyInfo because the method 'GetPropertyInfoDest' throw a exception if the property is not found
                return Expression.MakeMemberAccess(exp, property);
            }
            else
            {
                var property = mapper.GetPropertyInfoDest(node.Member.Name);
                //Not need test the PropertyInfo because the method 'GetPropertyInfoDest' throw a exception if the property is not found
                return Expression.MakeMemberAccess(paramClassSource, property);
            }

        }
    }
}

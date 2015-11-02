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

        internal ParameterExpression Parameter
        {
            get
            {
                return paramClassSource;
            }
        }
   
        internal ConverterExpressionVisitor()
        {
            mapper = Mapper.GetMapper(typeof(TSource), typeof(TDest)) as MapperConfiguration<TSource, TDest>;
            paramClassSource = Expression.Parameter(typeof(TDest), "d");
        }

        /// <summary>
        /// Visit <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return paramClassSource;
        }

        /// <summary>
        /// Distributes the expression one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                switch (node.NodeType)
                {
                    //to remove validation of the lambda expression
                    case ExpressionType.Lambda:
                        return base.Visit((node as LambdaExpression).Body);
                    default:
                        return base.Visit(node);
                }
            }
            return node;
        }

        /// <summary>
        /// Visit the children of <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            Expression exp = base.Visit(node.Expression);
            Expression result = null;
            //this is use to only for change the parameter of the result expression :(
            MapperExpressionVisitor changeParameterVisitor = null;
            //For children class
            if (exp.NodeType == ExpressionType.MemberAccess && (exp as MemberExpression).Member.DeclaringType.IsClass)
            {

                var subMapper = Mapper.GetMapper(node.Member.ReflectedType, exp.Type);
                result = subMapper.GetLambdaDest(node.Member.Name);
                changeParameterVisitor = new MapperExpressionVisitor(false, Expression.Parameter(subMapper.TypeDest, "d"));
            }
            else
            {
                result = mapper.GetLambdaDest(node.Member.Name);
                changeParameterVisitor = new MapperExpressionVisitor(false, paramClassSource);
            }

            if (result != null)
            {
                //Because the expression of the member is Expression<Func<T,object>> the compiler use a convert expression to cast the type of the property from object
                //we don't need to have it
                result = result.NodeType == ExpressionType.Convert ? (result as UnaryExpression).Operand : result;

                result = changeParameterVisitor.Visit(result);
                return result;
            }
            return node;
        }
    }
}

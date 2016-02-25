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
    internal class ConverterExpressionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<Expression, Expression> parameterMap;


        internal ConverterExpressionVisitor(
            Dictionary<Expression, Expression> parameterMap, Type typeDestination)
        {
            this.parameterMap = parameterMap;
            this.destinationType = typeDestination;
        }
        private Type destinationType;
        private MapperConfigurationBase mapper;

        /// <summary>
        /// Visit <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            //  re-map the parameter
            Expression found;
            if (!parameterMap.TryGetValue(node, out found))
                found = Expression.Parameter(destinationType, "dest");
            return found;
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
                Expression expression = null;
                switch (node.NodeType)
                {
                    // to remove validation of the lambda expression.
                    case ExpressionType.Lambda:

                        expression = base.Visit((node as LambdaExpression).Body);
                        break;
                    default:
                        expression = base.Visit(node);
                        break;
                }
                return expression;
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
            var expr = Visit(node.Expression);
            if (expr != null && expr.Type != node.Type)
            {
                if (mapper == null)
                {
                    mapper = Mapper.GetMapper(node.Member.DeclaringType, destinationType);
                }
                Expression expDest = null;
                // We consider that the primitive class is the simple property(not a sub's object).
                if (!expr.Type.IsValueType && expr.Type != typeof(string) && 
                    expr.NodeType != ExpressionType.Parameter && expr.NodeType != ExpressionType.Constant)
                {
                    var subExp = Mapper.GetMapper(node.Member.DeclaringType, expr.Type);
                    expDest = subExp.GetLambdaDest(node.Member.Name);
                    return AnalyseDestExpression(expr, expDest);
                }
                else
                {
                    expDest = mapper.GetLambdaDest(node.Member.Name);
                    if (expDest != null)
                    {
                        return AnalyseDestExpression(expr, expDest);
                    }
                }
            }
                return base.VisitMember(node); 
        }

        private Expression AnalyseDestExpression(Expression expr, Expression expDest)
        {
            if (expDest.NodeType == ExpressionType.MemberAccess)
            {
                return Expression.MakeMemberAccess(expr, (expDest as MemberExpression).Member);
            }
            else // Special case like Count method.
            {
                return base.Visit(expDest);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(node.Object != null && node.Object.NodeType == ExpressionType.MemberAccess)
            {
                VisitMember(node.Object as MemberExpression);
            }
            return base.VisitMethodCall(node);
        }


    }
}

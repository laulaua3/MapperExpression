using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using MapperExpression.Helper;

namespace MapperExpression.Core
{
    internal class MapperExpressionVisitor : ExpressionVisitor
    {
        #region Variables


        private bool checkNull;

        private readonly ParameterExpression parameter;

        private readonly Stack<MemberExpression> membersToCheck;

        internal ParameterExpression Parameter
        {
            get
            {
                return parameter;
            }
        }
        #endregion

        #region Constructor


        internal MapperExpressionVisitor(ParameterExpression paramClassSource)
        {

            parameter = paramClassSource;
            membersToCheck = new Stack<MemberExpression>();
        }

        #endregion

        #region Overloaded methods


        /// <summary>
        /// Distributes the expression one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <param name="checkIfNullity">Check</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        public Expression Visit(Expression node, bool checkIfNullity = false)
        {
            checkNull = checkIfNullity;
            Expression result = null;
            if (node == null)
                return node;
            if (checkNull)
            {
                // Treated our case
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        result = VisitMember(node as MemberExpression);
                        break;
                    case ExpressionType.Parameter:
                        result = VisitParameter(node as ParameterExpression);
                        break;
                    case ExpressionType.Convert:
                        result = VisitMember((node as UnaryExpression).Operand as MemberExpression);
                        break;
                    case ExpressionType.Lambda:
                        // to remove validation of the lambda expression
                        result = base.Visit((node as LambdaExpression).Body);
                        break;
                    default:
                        result = base.Visit(node);
                        break;

                }
                bool isFirst = true;
                Expression previousExpression = null;
                if (membersToCheck.Count > 1)
                {
                    // We want to test all the sub objects before assigning the value
                    // Ex: source.SubClass.SubClass2.MyProperty
                    // Which will give
                    // source.SubClass != null ? source.SubClass.SubClass2 != null ? source.SubClass.SubClass2.MyProperty :DefaultValueOfProperty :DefaultValueOfProperty
                    foreach (MemberExpression item in membersToCheck)
                    {

                        if (!isFirst) // Not to test the value of the property back
                        {
                            object defaultValue = MapperHelper.GetDefaultValue(item.Type);

                            // Creating verification of default value
                            Expression notDefaultValue = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type));
                            Expression conditional = null;
                            // It creates a condition that includes the above condition
                            if (previousExpression != null)
                            {
                                object defaultPreviousValue = MapperHelper.GetDefaultValue(previousExpression.Type);
                                conditional = Expression.Condition(notDefaultValue, previousExpression, Expression.Constant(defaultPreviousValue, previousExpression.Type));
                            }
                            // It affects the newly created conditions that will become the previous
                            previousExpression = conditional;
                        }
                        else // here the requested property
                        {
                            previousExpression = item;
                            isFirst = false;
                        }
                    }

                    return previousExpression;
                }
                // For one element don't need recusrive
                else if (membersToCheck.Count == 1)
                {
                    var item = membersToCheck.Peek();
                    object defaultValue = MapperHelper.GetDefaultValue(item.Type);
                    // Creating verification of default value
                    Expression notDefaultValue = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type));
                    Expression conditional = null;
                    conditional = Expression.Condition(notDefaultValue, item, Expression.Constant(defaultValue, item.Type));

                    return conditional;
                }
                return result;

            }
            else
            {
                // return by default (with change of the parameter)
                // to remove validation of the lambda expression
                if ((node.NodeType == ExpressionType.Lambda))
                {
                    return base.Visit(((LambdaExpression)node).Body);
                }
                else
                {
                    return base.Visit(node);
                }
            }
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
            // To change parameter
            return parameter;
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
            if (node == null)
            {
                return node;
            }
            MemberExpression memberAccessExpression = (MemberExpression)base.VisitMember(node);

            // To treat later
            if (memberAccessExpression != null && checkNull)
            {
                // Knowing that the last member is in the first visit and we go back every time
                // our current insert member is in the first line of the list to change the order
                membersToCheck.Push(memberAccessExpression);
            }
            return memberAccessExpression;
        }

        /// <summary>
        ///  Visit the children of <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node != null)
            {
                if (node.Operand.NodeType == ExpressionType.MemberAccess)
                {
                    return VisitMember(node.Operand as MemberExpression);
                }
                if (node.NodeType == ExpressionType.Convert)
                {
                    return Visit(node.Operand);
                }
            }
            return node;
        }

        #endregion


    }
}
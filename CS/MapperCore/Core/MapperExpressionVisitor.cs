using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace MapperExpression.Core
{
    internal class MapperExpressionVisitor : ExpressionVisitor
    {
        #region Variables

        private Expression previousExpression;

        private readonly bool checkNull;

        private readonly ParameterExpression paramSource;

        private readonly List<MemberExpression> membersToCheck;

        #endregion

        #region Constructor

       
        internal MapperExpressionVisitor(bool checkIfNull, ParameterExpression paramClassSource)
        {
            checkNull = checkIfNull;
            paramSource = paramClassSource;
            membersToCheck = new List<MemberExpression>();
        }

        #endregion

        #region Overloaded methods

        /// <summary>
        /// Distributes the expression one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">Expression to visit.</param>
        /// <returns>
        /// Altered expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;
            if (checkNull)
            {
                //Treated our case
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return VisitMember(node as MemberExpression);
                    case ExpressionType.Parameter:
                        return VisitParameter(node as ParameterExpression);
                    case ExpressionType.Convert:
                        return VisitMember((node as UnaryExpression).Operand as MemberExpression);
                    case ExpressionType.Lambda:
                        //to remove validation of the lambda expression
                        base.Visit((node as LambdaExpression).Body);
                        break;
                    default:
                        base.Visit(node);
                        break;

                }
                bool isFirst = true;
                //We want to test all the sub objects before assigning the value
                //Ex: source.SubClass.SubClass2.MyProperty
                //Which will give
                //source.SubClass != null ? source.SubClass.SubClass2 != null ? source.SubClass.SubClass2.MyProperty :DefaultValueOfProperty :DefaultValueOfProperty
                foreach (MemberExpression item in membersToCheck)
                {

                    if (!isFirst) //Not to test the value of the property back
                    {
                        object defaultValue = GetDefaultValue(item.Type);

                        //Creating verification of nullity
                        Expression notNull = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type));
                        Expression conditional = null;
                        //It creates a condition that includes the above condition
                        if (previousExpression != null)
                        {
                            object defaultPreviousValue = GetDefaultValue(previousExpression.Type);
                            conditional = Expression.Condition(notNull, previousExpression, Expression.Constant(defaultPreviousValue, previousExpression.Type));
                        }
                        //It affects the newly created conditions that will become the previous
                        previousExpression = conditional;
                    }
                    else //here the requested property
                    {
                        previousExpression = item;
                        isFirst = false;
                    }
                }
                return previousExpression;
            }
            else
            {
                //return by default (with change of the parameter)
                //to remove validation of the lambda expression
                if ((node.NodeType == ExpressionType.Lambda))
                {
                    return base.Visit((node as LambdaExpression).Body);
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
            //To change parameter
            return paramSource;
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
            MemberExpression memberAccessExpression = (MemberExpression)base.VisitMember(node);

            //To treat later
            if (memberAccessExpression != null && checkNull)
            {
                //Knowing that the first member is in the first visit and as we descend each time 
                //our current insert member is in the first line of the list to change the order
                //exemple :
                //source.SubClass.SubClass2.MyProperty
                //the list would be:
                //MyList[0] = SubClass
                //MyList[1] = SubClass2
                //MyList[2] = MyProperty
                //
                //but we want
                //MyList[0] = MyProperty
                //MyList[1] = SubClass2
                //MyList[2] = SubClass
                membersToCheck.Insert(0, memberAccessExpression);
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
            return VisitMember(node.Operand as MemberExpression);
        }

        private object GetDefaultValue(Type typeObject)
        {
            object defaultValue = null;
            //In the case of value types (eg Integer), you must instantiate the object to have its default value (default(T) not working for some case)
            if (typeObject.IsValueType)
            {
                NewExpression exp = Expression.New(typeObject);
                LambdaExpression lambda = Expression.Lambda(exp);
                Delegate constructor = lambda.Compile();
                defaultValue = constructor.DynamicInvoke();
            }
            return defaultValue;
        }

        #endregion
    }
}
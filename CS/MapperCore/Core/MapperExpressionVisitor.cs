using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace MapperExpression.Core
{
    internal class MapperExpressionVisitor : ExpressionVisitor
    {
        #region Variables

        private Expression previousMember;

        private readonly bool checkNull;

        private readonly ParameterExpression paramSource;

        private readonly List<MemberExpression> membersTocheck;

        #endregion

        #region Constructor

        // <summary>
        // Initialise une nouvelle instance de  <see cref="MapperExpressionVisitor"/> classe.
        // </summary>
        // <param name="checkIfNull">Indique si l'on tester la nullité des objets (récursive)</param>
        // <param name="paramClassSource">paramètre de la source</param>
        internal MapperExpressionVisitor(bool checkIfNull, ParameterExpression paramClassSource)
        {
            checkNull = checkIfNull;
            paramSource = paramClassSource;
            membersTocheck = new List<MemberExpression>();
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
                //source.SubClass != null ? source.SubClass.SubClass2 != null ? source.SubClass.SubClass2.MyProperty :null :null
                foreach (MemberExpression item in membersTocheck)
                {

                    if (!isFirst) //Not to test the value of the property back
                    {
                        object defaultValue = GetDefaultValue(item.Type);

                        //Creating verification of nullity
                        Expression notNull = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type));
                        Expression conditional = null;
                        //It creates a condition that includes the above condition
                        if (previousMember != null)
                        {
                            object defaultPreviousValue = GetDefaultValue(previousMember.Type);
                            conditional = Expression.Condition(notNull, previousMember, Expression.Constant(defaultPreviousValue, previousMember.Type));
                        }
                        //It affects the newly created conditions that will become the previous
                        previousMember = conditional;
                    }
                    else //here the requested property
                    {
                        previousMember = item;
                        isFirst = false;
                    }
                }
                return previousMember;
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

            //Pour traiter plus tard
            if (memberAccessExpression != null && checkNull)
            {
                // Knowing that the first member is in the first visit and as we descend each time 
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
                membersTocheck.Insert(0, memberAccessExpression);
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
            //In the case of value types (eg Integer), you must instantiate the object to have its default value
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
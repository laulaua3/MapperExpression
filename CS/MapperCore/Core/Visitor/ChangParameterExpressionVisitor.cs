using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Core.Visitor
{
    internal class ChangParameterExpressionVisitor : ExpressionVisitor
    {
        private Expression _parameter;
        private ParameterExpression getParameterExpression;
        private bool getParameter;
        internal ChangParameterExpressionVisitor(Expression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != null)
            {
                if (!getParameter)
                {
                    if (node.Type == _parameter.Type)
                        return _parameter;
                    return base.VisitParameter(node);
                }
                else
                {
                    getParameterExpression = node;
                }
                return node;
                
            }
            return node;
        }


        internal ParameterExpression GetParameter()
        {
            getParameter = true;

            Visit(_parameter);
            getParameter = false;
            return getParameterExpression;
        }
        public Expression Parameter
        {
            get
            {
                return _parameter;
            }
        }
    }
}

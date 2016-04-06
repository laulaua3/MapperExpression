using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Core.Visitor
{
    internal class ChangParameterExpressionVisitor : ExpressionVisitor
    {
        private Expression _parameter;
        
        internal ChangParameterExpressionVisitor(Expression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != null)
            {
                if (node.Type == _parameter.Type)
                    return _parameter;
                return base.VisitParameter(node);
            }
            return node;
        }
              
    }
}

using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Core
{
    internal class ChangParameterExpressionVisitor : ExpressionVisitor
    {
        private Expression _parameter;
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangParameterExpressionVisitor"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public ChangParameterExpressionVisitor(Expression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _parameter.Type)
                return _parameter;
            return base.VisitParameter(node);
        }
              
    }
}

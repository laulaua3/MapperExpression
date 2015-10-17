using MapperExpression.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MapperExpression.Extensions
{
    public static class MapperExtentions
    {

        public static Expression<Func<TDest, bool>> ConvertTo<TSource, TDest>(this Expression<Func<TSource, bool>> expression)
        {
            Contract.Requires(expression != null);
            ConverterExpressionVisitor<TSource, TDest> visitor = new ConverterExpressionVisitor<TSource, TDest>();

            return Expression.Lambda<Func<TDest, bool>>(visitor.Visit(expression), visitor.Parameter);
        }
    }
}

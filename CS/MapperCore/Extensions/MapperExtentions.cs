using MapperExpression.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MapperExpression.Extensions
{
    /// <summary>
    /// Extentions of the mapper
    /// </summary>
    public static class MapperExtentions
    {
        /// <summary>
        /// Convert a expression source to same expression target
        /// </summary>
        /// <typeparam name="TSource">type of source</typeparam>
        /// <typeparam name="TDest">type of target</typeparam>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
        public static Expression<Func<TDest, bool>> ConvertTo<TSource, TDest>(this Expression<Func<TSource, bool>> expression)
        {
            Contract.Requires(expression != null);

            ConverterExpressionVisitor<TSource, TDest> visitor = new ConverterExpressionVisitor<TSource, TDest>();
            return Expression.Lambda<Func<TDest, bool>>(visitor.Visit(expression), visitor.Parameter);
        }
    }
}

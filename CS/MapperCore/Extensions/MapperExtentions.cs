using MapperExpression.Core;
using MapperExpression.Core.Visitor;
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
        /// Converts to.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <param name="from">From.</param>
        /// <param name="toType">To type.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression ConvertTo<TFrom>(
            this Expression<Func<TFrom, object>> from, Type toType)
        {
            return ConvertImpl(from, toType);
        }
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <param name="from">From.</param>
        /// <param name="toType">To type.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression ConvertTo<TFrom>(this Expression<Func<TFrom, bool>> from, Type toType)
        {
            return ConvertImpl(from, toType);
        }
        /// <summary>
        /// Converts a lambda expression type <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>
        /// </summary>
        /// <typeparam name="TFrom">The type of original.</typeparam>
        /// <typeparam name="TTo">The type of target.</typeparam>
        /// <param name="from">From.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression ConvertTo<TFrom, TTo>(this Expression<Func<TFrom, object>> from)
        {
            return ConvertImpl(from, typeof(TTo));
        }
        /// <summary>
        /// Converts expression of <typeparamref name="TFrom"/> to expression of <typeparamref name="TTo"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type of original expression.</typeparam>
        /// <typeparam name="TTo">The type of converted expression.</typeparam>
        /// <param name="from">original expression.</param>
        /// <returns>expression converted or if no mapping is found the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<TTo, bool>> ConvertTo<TFrom, TTo>(this Expression<Func<TFrom, bool>> from)
        {
            return (Expression<Func<TTo, bool>>)ConvertImpl(from, typeof(TTo));
        }
        private static Expression ConvertImpl<TFrom>(Expression<TFrom> from, Type toType)
           where TFrom : class
        {
            //  re-map all parameters that involve different types
            Dictionary<Expression, Expression> parameterMap
                = new Dictionary<Expression, Expression>();
            ParameterExpression[] newParams =
                new ParameterExpression[from.Parameters.Count];
            for (int i = 0; i < newParams.Length; i++)
            {
                newParams[i] = Expression.Parameter(toType, from.Parameters[i].Name);
                parameterMap[from.Parameters[i]] = newParams[i];
            }

            //  rebuild the lambda
            var body = new ConverterExpressionVisitor(parameterMap, toType).Visit(from.Body);
            return Expression.Lambda(body, newParams);
        }
    }
}

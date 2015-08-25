using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperCore
{
    public static class MapperExtentions
    {
        public static IEnumerable<TResult> MapTo<TSource, TResult>(this IQueryable<TSource> source)
            where TSource : class
            where TResult : class
        {
            return source.Select(Mapper.GetQueryExpression<TSource, TResult>());

        }
    }
}

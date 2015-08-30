using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapperExpression.Tests.Units.ClassTests
{
    public class QueryableImplTest<T> : IQueryable<T>, IOrderedQueryable<T>
    {
        private Expression _expression;

        private IQueryProvider _provider;

        public QueryableImplTest()
        {
            _provider = new QueryProviderImplTest();
            _expression = Expression.Constant(this);
        }

        public QueryableImplTest(Expression expression)
        : this()
        {
            _expression = expression;
        }
        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.CreateQuery<T>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.CreateQuery<T>(Expression).GetEnumerator();
        }
    }
    public class QueryProviderImplTest : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return null;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new QueryableImplTest<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return null;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default(TResult);
        }
    }
}

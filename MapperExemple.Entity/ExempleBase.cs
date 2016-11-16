using MapperExemple.Entity.EF;
using MapperExpression.Extensions;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MapperExemple.Entity
{
    public class ExempleBase :IDisposable

    {
        // Flag: Has Dispose already been called?
        bool disposed = false;

        protected IExempleDbContext _context;

       
        public ExempleBase()
            :this(new ExempleDbContext())
        { }
        public ExempleBase(IExempleDbContext context)
        {
            _context = context;
            //For see the sql request
            (_context as ExempleDbContext).Database.Log = x => Debug.WriteLine(x);
        }

        /// <summary>
        /// Exemple to make a generic method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selectQuery"></param>
        /// <returns></returns>
        protected IQueryable<TResult> GetEntities<TEntity, TResult>(Expression<Func<TEntity, TResult>> selectQuery)
            where TEntity : class
        {
            IQueryable<TResult> result = null;

            result = (_context as DbContext).Set<TEntity>().Select(selectQuery);

            return result;
        }
        /// <summary>
        /// Exemple to make a generic method with criterias
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TCriterias"></typeparam>
        /// <param name="criterias"></param>
        /// <param name="selectQuery"></param>
        /// <returns></returns>
        protected IQueryable<TResult> GetEntities<TEntity, TCriterias, TResult>(Expression<Func<TCriterias, bool>> criterias, Expression<Func<TEntity, TResult>> selectQuery)
            where TEntity : class
        {
            IQueryable<TResult> result = null;
            result = (_context as DbContext).Set<TEntity>()
                .Where(criterias)
                .Select(selectQuery);
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);

        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

    }
}

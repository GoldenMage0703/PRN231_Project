using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetLastAsync<TKey>(Expression<Func<T, TKey>> keySelector);  
        Task<IEnumerable<T>> FindIncludeAsync<TKey>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdIncludeAsync<TKey>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(ICollection<T> list);
        Task UpdateRangeAsync(ICollection<T> list);

        // New method for custom queries
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}

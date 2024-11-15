﻿using Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly PRN231_ProjectContext _context;

        public Repository(PRN231_ProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(ICollection<T> entityList)
        {
           await _context.Set<T>().AddRangeAsync(entityList);
           await _context.SaveChangesAsync();   
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Implement the dynamic query
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetLastAsync<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return await _context.Set<T>().OrderBy(keySelector).LastOrDefaultAsync();
        }



        public async Task<IEnumerable<T>> FindIncludeAsync<TKey>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                                 .Include(keySelector)  // Include the related entity
                                 .Where(predicate)      // Apply filtering
                                 .ToListAsync();
        }

        public async Task<T> GetByIdIncludeAsync<TKey>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                                 .Include(keySelector)  // Include the related entity
                                 .FirstOrDefaultAsync(predicate);      // Apply filtering
                                 
        }

        public async Task DeleteRangeAsync(ICollection<T> list)
        {
            _context.Set<T>().RemoveRange(list);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(ICollection<T> list)
        {
            _context.Set<T>().UpdateRange(list);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

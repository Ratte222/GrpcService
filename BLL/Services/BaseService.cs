using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity<long>
    {
        protected AppDBContext _context;
        public BaseService(AppDBContext appDBContext)
        {
            _context = appDBContext;
        }

        public virtual IQueryable<T> GetAll_Queryable()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual IEnumerable<T> GetAll_Enumerable()
        {
            return _context.Set<T>().AsNoTracking().AsEnumerable();
        }

        public virtual T Get(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual void Create(T model)
        {
            _context.Set<T>().Add(model);
            _context.SaveChanges();
        }

        public virtual void CreateRange(IEnumerable<T> items)
        {
            _context.Set<T>().AddRange(items);
            _context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            _context.Set<T>().Update(item);
            _context.SaveChanges();
        }

        public virtual void Delete(T item)
        {
            _context.Set<T>().Remove(item);
            _context.SaveChanges();
        }


    }
}

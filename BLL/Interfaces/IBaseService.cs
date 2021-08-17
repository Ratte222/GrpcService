using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBaseService<T> where T : BaseEntity<long>
    {
        IEnumerable<T> GetAll_Enumerable();
        IQueryable<T> GetAll_Queryable();
        IQueryable<T> GetAll();
        T Get(long id);
        Task<T> GetAsync(long id);
        void Create(T item);
        //void CreateRange(IEnumerable<T> items);
        void Update(T item);
        void Delete(T item);
    }
}

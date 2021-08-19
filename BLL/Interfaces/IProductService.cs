using BLL.DTO.Product;
using BLL.Filters;
using BLL.Helpers;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        IQueryable<Product> GetAll();
        Product Get(long id);
        IEnumerable<Product> GetProducts(PageResponse<Product> pageResponse, ProductFilter productFilter);
        void Create(Product item);
        void Update(Product item);
        void Delete(long id);


        
    }
}

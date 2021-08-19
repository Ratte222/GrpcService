using BLL.DTO.Product;
using BLL.Extensions;
using BLL.Filters;
using BLL.Helpers;
using BLL.Infrastructure;
using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly AppSettings _appSettings;
        private readonly AppDBContext _context;
        public ProductService(AppDBContext appDBContext, IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            _context = appDBContext;
        }

        public virtual Product Get(long id)
        {
            Product item = new Product();
            string tableName = nameof(Product) + 's';
            string query = $"SELECT * FROM {tableName} WHERE {nameof(item.Id)} = '{id}'";
            return _context.Set<Product>().FromSqlRaw(query)
                .FirstOrDefault();
        }

        public virtual IQueryable<Product> GetAll()
        {
            string tableName = nameof(Product) + 's';
            string query = $"SELECT * FROM {tableName}";
            return _context.Set<Product>().FromSqlRaw(query);
        }

        public void Create(Product item)
        {
            string query = $"INSERT INTO {nameof(Product)}s ({nameof(item.Name)}, {nameof(item.Description)}," +
                $" {nameof(item.Cost)}) VALUES ('{item.Name}'," +
                $" '{item.Description}', '{item.Cost}')";           
            _context.Database.ExecuteSqlRaw(query);
        }

        public void Update(Product item)
        {
            string query = $"UPDATE {nameof(Product)}s SET {nameof(item.Name)} = '{item.Name}', " +
                $"{nameof(item.Description)} = '{item.Description}', {nameof(item.Cost)} = '{item.Cost}' " +
                $"WHERE {nameof(item.Id)} = '{item.Id}'";
            _context.Database.ExecuteSqlRaw(query);
        }
            
        public void Delete(long id)
        {
            Product item = new Product();
            string query = $"DELETE FROM {nameof(Product)}s " +
                $"WHERE {nameof(item.Id)} = '{id}'";
            _context.Database.ExecuteSqlRaw(query);
        }

        public IEnumerable<Product> GetProducts(PageResponse<Product> pageResponse, ProductFilter productFilter)
        {
            var query = GetAll();
            if (String.IsNullOrEmpty(productFilter.FieldOrderBy))
            {
                Product product = new Product();
                query = query.OrderBy(nameof(product.Name), productFilter.OrderByDescending);
            }
            else
                query = query.OrderBy(productFilter.FieldOrderBy, productFilter.OrderByDescending);
            return query.Skip(pageResponse.Skip).Take(pageResponse.Take);
        }

        

        
    }
}

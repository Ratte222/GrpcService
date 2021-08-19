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

        public void Serialize(ProductDTO product)
        {
            string _path = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.DirectoryForSerializeDeserialize);
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            _path = Path.Combine(_path, _appSettings.FileName);
            #region Binary
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // To serialize the hashtable and its key/value pairs,
            // you must first open a stream for writing.
            // In this case, use a file stream.
            using (FileStream fs = new FileStream(_path + ".dat", FileMode.OpenOrCreate))
            {
                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, product);
            }
            #endregion
            #region xml
            
            using (FileStream fs = new FileStream(_path + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(ProductDTO));
                xmlFormatter.Serialize(fs, product);
            }
            #endregion
            #region json
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(_path+".json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, product);                
            }

            #endregion
        }

        public List<ProductDTO> Deserialize()
        {
            List<ProductDTO> productDTOs = new List<ProductDTO>();
            string _path = Path.Combine(Directory.GetCurrentDirectory(), 
                _appSettings.DirectoryForSerializeDeserialize, _appSettings.FileName);

            #region binary
            using (FileStream fs = new FileStream(_path + ".dat", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and
                // assign the reference to the local variable.
                productDTOs.Add((ProductDTO)formatter.Deserialize(fs));
            }                
            #endregion

            #region xml
            XmlSerializer xmlFrmatter = new XmlSerializer(typeof(ProductDTO));            
            using (FileStream fs = new FileStream(_path + ".xml", FileMode.Open))
            {
                productDTOs.Add((ProductDTO)xmlFrmatter.Deserialize(fs));
            }
            #endregion
            #region Json
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamReader sr = new StreamReader(_path + ".json"))
            using (JsonReader writer = new JsonTextReader(sr))
            {
                productDTOs.Add((ProductDTO)serializer.Deserialize(sr, typeof(ProductDTO)));
            }
            #endregion

            return productDTOs;
        }
        //https://www.youtube.com/watch?v=lfEgW53RDkc&list=PLrb70iTVZjZPEbhCh85VQIpRbQos2Qx3i&index=6
        public async Task InsertNewProductToFireBaseAsync(List<ProductFbDTO> products)
        {
            FirestoreDb firestoreDb = FirestoreDb.Create(_appSettings.FireBaseConfig["ProjectName"]);
            //DocumentReference doc = firestoreDb.Collection("Add_Document_With_CustomID").Document("People");
            //Dictionary<string, object> data1 = new Dictionary<string, object>()
            //{
            //    { "FirstName","Artur"},
            //    { "LsatName", "Nesterenko" }
            //};
            //doc.SetAsync(data1).GetAwaiter();
            //DocumentReference doc2 = firestoreDb.Collection("Add_Document_With_CustomID").Document("Products");
            //CollectionReference doc2 = firestoreDb.Collection("Products");
            Query query = firestoreDb.Collection("Products");
            QuerySnapshot querySnapshots = await query.GetSnapshotAsync();
            var keyValuePairs = products.ToDictionary(i=>i.Id);
            Dictionary<string, object> MainData = new Dictionary<string, object>();
            foreach(var product in products)
            {
                if (querySnapshots.Documents.Any(i => Convert.ToInt64(i.Id) == product.Id))
                    continue;
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add(nameof(product.Id), product.Id);
                data.Add(nameof(product.Name), product.Name);
                data.Add(nameof(product.Description), product.Description);
                data.Add(nameof(product.Cost), product.Cost);
                MainData.Add(product.Id.ToString(), data);
                DocumentReference doc3 = firestoreDb.Collection("Products").Document(product.Id.ToString());
                await doc3.CreateAsync(data);//not optimized 
            }
            //var result = await doc2.CreateAsync(MainData);            
        }


        //public async Task<List<object>> GetDataFromFireBase()
        //{
        //    FirestoreDb firestoreDb = FirestoreDb.Create(_appSettings.FireBase["ProjectName"]);
        //    DocumentReference docref = firestoreDb.Collection("Add_Document_With_CustomID").Document("Products");
        //    DocumentSnapshot snap = await docref.GetSnapshotAsync();
        //    List<object> list =
        //            new List<object>();
        //    if (snap.Exists)
        //    {
        //        var dict = snap.ToDictionary();

        //        foreach (var value in dict)
        //        {                    
        //            list.Add(value.Value);
        //        }

        //    }
        //    return list;
        //}

        public async Task<List<ProductFbDTO>> GetDataFromFireBase(
            PageResponse<ProductFbDTO> pageResponse, ProductFilter productFilter)
        {
            FirestoreDb firestoreDb = FirestoreDb.Create(_appSettings.FireBaseConfig["ProjectName"]);
            Query query = null;
            if (String.IsNullOrEmpty(productFilter.FieldOrderBy) && productFilter.OrderByDescending)
            {
                Product product = new Product();
                query = firestoreDb.Collection("Products").OrderByDescending(nameof(product.Name))
                    .Offset(pageResponse.Skip).Limit(pageResponse.Take);
            }
            else if (String.IsNullOrEmpty(productFilter.FieldOrderBy) && !productFilter.OrderByDescending)
            {
                Product product = new Product();
                query = firestoreDb.Collection("Products").OrderBy(nameof(product.Name))
                    .Offset(pageResponse.Skip).Limit(pageResponse.Take);
            }
            else if (!String.IsNullOrEmpty(productFilter.FieldOrderBy) && productFilter.OrderByDescending)
            {
                query = firestoreDb.Collection("Products").OrderByDescending(productFilter.FieldOrderBy)
                    .Offset(pageResponse.Skip).Limit(pageResponse.Take);
            }
            else if (!String.IsNullOrEmpty(productFilter.FieldOrderBy) && !productFilter.OrderByDescending)
            {
                query = firestoreDb.Collection("Products").OrderBy(productFilter.FieldOrderBy)
                    .Offset(pageResponse.Skip).Limit(pageResponse.Take);
            }
            QuerySnapshot querySnapshots = await query.GetSnapshotAsync();
            List<ProductFbDTO> products = new List<ProductFbDTO>();
            foreach(DocumentSnapshot value in querySnapshots)
            {
                products.Add(value.ConvertTo<ProductFbDTO>());
            }
            pageResponse.TotalItems = -1;//did it on purpose so that there was an example of a sample from a google database
            //and did not want to make another request in order to take the total number of elements 
            return products;
        }

        
    }
}

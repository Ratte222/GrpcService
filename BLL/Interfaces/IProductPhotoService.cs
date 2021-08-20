using DAL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProductPhotoService:IBaseService<ProductPhoto>
    {
        string GetFileName(ProductPhoto entity);
        bool ContentExists(ProductPhoto entity);
        Stream GetContent(ProductPhoto entity);
        Task SetContentAsync(ProductPhoto entity, Stream content);
        Task<ProductPhoto> GetById(long id);
        Task SaveProductPhoto(List<ProductPhoto> productPhotos);
        Task<List<ProductPhoto>> GetAllFilesName();
    }
}

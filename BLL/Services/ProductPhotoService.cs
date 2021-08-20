using BLL.Helpers;
using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AuxiliaryLib.Extensions;
namespace BLL.Services
{
    public class ProductPhotoService : BaseService<ProductPhoto>, IProductPhotoService
    {
        //private readonly AppDBContext _dbContext;
        //private SaveUserFiles.Options.Options _options = new Options.Options()
        //{
        //    Path = System.IO.Directory.GetCurrentDirectory(),
        //    PathSliceDepth = 2
        //};
        private readonly AppSettings _appSettings;
        public ProductPhotoService(AppDBContext appDBContext, AppSettings appSettings) : base(appDBContext)
        {
            _appSettings = appSettings;
        }

        public string GetFileName(ProductPhoto entity)
        {
            var id = entity.LocalId.ToNumericString();
            if (_appSettings.PathSliceDepth > 0)
            {
                var pathParts = new string[_appSettings.PathSliceDepth + 2];
                pathParts[0] = _appSettings.PathSaveProductPhoto;
                int j = 1;
                for (int i = 0; i < _appSettings.PathSliceDepth; i++)
                {
                    pathParts[j++] = id[i].ToString();
                }
                pathParts[j] = id;
                return Path.Combine(pathParts);
            }
            else
            {
                return Path.Combine(_appSettings.PathSaveProductPhoto, id);
            }
        }

        public bool ContentExists(ProductPhoto entity)
        {
            return File.Exists(GetFileName(entity));
        }

        public Stream GetContent(ProductPhoto entity)
        {
            return new FileStream(GetFileName(entity), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public async Task SetContentAsync(ProductPhoto entity, Stream content)
        {
            var filename = GetFileName(entity);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await content.CopyToAsync(stream);
            }
        }

        public async Task<ProductPhoto> GetById(long id)
        {
            return await GetAsync(id);
        }

        public async Task SaveProductPhoto(List<ProductPhoto> productPhotos)
        {
            await CreateRangeAsync(productPhotos);
        }

        public async Task<List<ProductPhoto>> GetAllFilesName()
        {
            return await GetAll().ToListAsync();
        }
    }
}

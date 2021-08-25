using BLL.Helpers;
using BLL.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using model = DAL.Model;

namespace GrpcService.Services
{
    public class ProductPhotoService : ProductPhotoProtoService.ProductPhotoProtoServiceBase
    {
        private readonly IProductPhotoService _productPhotoService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ProductPhotoService> _logger;
        public ProductPhotoService(IProductPhotoService productPhotoService, AppSettings appSettings,
            ILogger<ProductPhotoService> logger)
        {
            _productPhotoService = productPhotoService;
            _appSettings = appSettings;
            _logger = logger;
        }

        public override async Task GetPhoto(GetPhotoRequest request, IServerStreamWriter<ChunkMsg> responseStream, ServerCallContext context)
        {
            var file = await _productPhotoService.GetById(request.PhotoId);
            if (file == null || file.IsPublic == false)
            {
                return;
            }
            string _file_path = Path.Combine(_productPhotoService.GetFileName(file), file.Name);
            if (File.Exists(_file_path))
            {
                var _file_info = new FileInfo(_file_path);

                var _chunk = new ChunkMsg
                {
                    FileName = Path.GetFileName(_file_path),
                    FileSize = _file_info.Length
                };

                var _chunk_size = 64 * 1024;

                var _file_bytes = File.ReadAllBytes(_file_path);
                var _file_chunk = new byte[_chunk_size];

                var _offset = 0;

                while (_offset < _file_bytes.Length)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                        break;

                    var _length = Math.Min(_chunk_size, _file_bytes.Length - _offset);
                    Buffer.BlockCopy(_file_bytes, _offset, _file_chunk, 0, _length);

                    _offset += _length;

                    _chunk.ChunkSize = _length;
                    _chunk.Chunk = ByteString.CopyFrom(_file_chunk);

                    await responseStream.WriteAsync(_chunk).ConfigureAwait(false);
                }
            }
        }



        public override async Task<Google.Rpc.Status> AddPhoto(IAsyncStreamReader<AddPhotoRequest> requestStream, ServerCallContext context)
        {
            Google.Rpc.Status stringReply = new Google.Rpc.Status() { Message = "File saved successfully"};
            stringReply.Code = (int)Grpc.Core.StatusCode.OK;
            try
            { 
                _productPhotoService.Create(await SaveFile(requestStream)); 
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"{ex.Message} \r\n {ex?.InnerException}");
                stringReply.Message = "File could not be saved ";
                stringReply.Code = (int)Grpc.Core.StatusCode.Internal;
            }
            return stringReply;
        }

        
        private async Task<model.ProductPhoto> SaveFile(IAsyncStreamReader<AddPhotoRequest> requestStream)
        {
            model.ProductPhoto productProto = new model.ProductPhoto()
            {
                DateTime = DateTime.Now,
                IsPublic = true,                
            };
            //Assigning Unique Filename (Guid)
            //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
            var myUniqueFileName = Convert.ToString(productProto.LocalId);

            
            
            var _temp_file = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.PathTemp,
                $"temp_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}.tmp");
            var _final_file = _temp_file;
            await using (var _fs = File.OpenWrite(_temp_file))
            {
                bool fileInfoFilled = false;
                await foreach (var _chunk in requestStream.ReadAllAsync().ConfigureAwait(false))
                {
                    var _total_size = _chunk.ChungMsg.FileSize;

                    if (!String.IsNullOrEmpty(_chunk.ChungMsg.FileName) && !fileInfoFilled)
                    {
                        
                        //Getting file Extension
                        var FileExtension = Path.GetExtension(_chunk.ChungMsg.FileName);

                        // concating  FileName + FileExtension
                        productProto.Name = _chunk.ChungMsg.FileName;
                        productProto.MimeType = $"application/{FileExtension}";
                        productProto.Size = _chunk.ChungMsg.FileSize;
                        productProto.ProductId = _chunk.ProductId;

                        _final_file = Path.Combine(
                            _productPhotoService.GetFileName(productProto), productProto.Name);
                        fileInfoFilled = true;
                    }

                    if (_chunk.ChungMsg.Chunk.Length == _chunk.ChungMsg.ChunkSize)
                        _fs.Write(_chunk.ChungMsg.Chunk.ToByteArray());
                    else
                    {
                        _fs.Write(_chunk.ChungMsg.Chunk.ToByteArray(), 0, _chunk.ChungMsg.ChunkSize);
                        Console.WriteLine($"final chunk size: {_chunk.ChungMsg.ChunkSize}");
                    }
                }
            }
            try
            {
                if (_final_file != _temp_file)
                {
                    string targetDir = Path.GetDirectoryName(_final_file);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
                    File.Move(_temp_file, _final_file);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                File.Delete(_temp_file);
            }
            
            
            return productProto;
        }
    }
}

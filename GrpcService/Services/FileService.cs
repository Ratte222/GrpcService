using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GrpcService.Services
{
    public class FileService : GrpcService.FileTransferService.FileTransferServiceBase
    {
        private readonly ILogger<FileService> _logger;
        private readonly IMapper _mapper;
        public FileService(ILogger<FileService> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task DownloadFile(FileRequest request, IServerStreamWriter<ChunkMsg> responseStream, ServerCallContext context)
        {
            string _file_path = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "Free_Proxy_List.json");//request.FilePath;

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
    }
}

using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService//.Services
{
    public class ProductService : Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;
        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }

        public override Task<ProductReply> GetProduct(ProductRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ProductReply
            {
                Message = "First product"
            });
        }
    }
}

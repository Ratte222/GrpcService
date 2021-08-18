using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using AutoMapper;
using model = DAL.Model;
using BLL.Filters;
using BLL.Helpers;
using BLL.DTO.Product;

namespace GrpcService//.Services
{
    public class ProductService : Product.ProductBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductService(ILogger<ProductService> logger, IProductService productService,
            IMapper mapper)
        {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
        }

        public override Task<ProductReply> GetProduct(ProductRequest request, ServerCallContext context)
        {
            model.Product product = _productService.Get(request.ProductId);
            //var result = new ProductReply
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    Cost = product.Cost
            //};
            //return Task.FromResult(result);
            return Task.FromResult(_mapper.Map<model.Product, ProductReply>(product));
        }

        public override Task<ProductsReply> GetProducts(ProductsRequest request, ServerCallContext context)
        {
            PageResponse<ProductDTO> pageResponse = new PageResponse<ProductDTO>(
                request.PageLength, request.PageNumber);
            ProductFilter productFilter = new ProductFilter()
            {
                FieldOrderBy = request.FieldOrderBy,
                OrderByDescending = request.OrderByDescending == null ? false : true
            };
            List<ProductReply> list = _mapper.ProjectTo<ProductReply>(_productService.GetAll()).ToList();
            ProductsReply reply = new ProductsReply();
            reply.Products.AddRange(list);
            return Task.FromResult(reply);
        }
    }
}

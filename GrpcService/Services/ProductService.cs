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
using Microsoft.AspNetCore.Authorization;

namespace GrpcService.Services
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

        public override Task<ProductProto> GetProduct(ProductRequest request, ServerCallContext context)
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
            return Task.FromResult(_mapper.Map<model.Product, ProductProto>(product));
        }

        [Authorize]
        public override Task<ProductsReply> GetProducts(ProductsRequest request, ServerCallContext context)
        {
            PageResponse<model.Product> pageResponse = new PageResponse<model.Product>(
                request.PageLength, request.PageNumber);
            ProductFilter productFilter = new ProductFilter()
            {
                FieldOrderBy = request.FieldOrderBy,
                OrderByDescending = request.OrderByDescending == null ? false : true
            };
            List<ProductProto> list = _mapper.ProjectTo<ProductProto>(_productService.GetAll()).ToList();
            ProductsReply reply = new ProductsReply();
            reply.PageResponse = _mapper.Map<PageResponse<model.Product>, PageResponse>(pageResponse);
            reply.Products.AddRange(list);
            return Task.FromResult(reply);
        }

        public override Task<Google.Rpc.Status> CreateProduct(NewProductProto request, ServerCallContext context)
        {
            _productService.Create(_mapper.Map<NewProductProto, model.Product>(request));
            return Task.FromResult(new Google.Rpc.Status { Message = "Product created successfully",
            Code = (int)StatusCode.OK});
        }

        public override Task<Google.Rpc.Status> EditProduct(ProductProto request, ServerCallContext context)
        {
            _productService.Update(_mapper.Map<ProductProto, model.Product>(request));
            return Task.FromResult(new Google.Rpc.Status { Message = "Product updated successfully",
                Code = (int)StatusCode.OK
            });
        }

        public override Task<Google.Rpc.Status> DeleteProduct(ProductRequest request, ServerCallContext context)
        {
            _productService.Delete(request.ProductId);
            return Task.FromResult(new Google.Rpc.Status { Message = "Product removed successfully",
                Code = (int)StatusCode.OK
            });
        }
    }
}

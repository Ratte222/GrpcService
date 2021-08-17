using BLL.DTO.OrderLine;
using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class OrderLineService: BaseService<OrderLine>, IOrderLineService
    {
        public readonly IProductService _productService;

        public OrderLineService(AppDBContext appDBContext, IProductService productService):base(appDBContext)
        {
            _productService = productService;
        }

    }
}

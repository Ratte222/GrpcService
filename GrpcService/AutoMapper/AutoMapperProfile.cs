using AutoMapper;
using BLL.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model = DAL.Model;
using BLL.Helpers;
namespace GrpcService.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<model.Product, ProductProto>()
                .ForMember(dest => dest.ProductPhotos, opt => opt.MapFrom(scr => scr.ProductPhotos));
            CreateMap<ProductPhotosProto, model.ProductPhoto>();
            CreateMap<model.ProductPhoto, ProductPhotosProto>();
            CreateMap<ProductProto, model.Product>();
            CreateMap<NewProductProto, model.Product>();
            CreateMap<PageResponse<model.Product>, PageResponse>();
        }
    }
}

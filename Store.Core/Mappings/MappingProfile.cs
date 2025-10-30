using AutoMapper;
using Store.Core.DTOs;
using Store.Core.Entities;

namespace Store.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}

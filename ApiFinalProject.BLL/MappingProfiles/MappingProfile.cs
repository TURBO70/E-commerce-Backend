using AutoMapper;
using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.BLL.DTOs.Categories;
using ApiFinalProject.BLL.DTOs.Products;
using ApiFinalProject.BLL.DTOs.Cart;
using ApiFinalProject.BLL.DTOs.Orders;

namespace ApiFinalProject.BLL.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category Mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

        // Cart Mappings
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * (i.Product != null ? i.Product.Price : 0))));

        // Order Mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
        
        CreateMap<Order, OrderDto>();
    }
}

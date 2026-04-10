using ApiFinalProject.DAL.UnitOfWork;
using ApiFinalProject.BLL.DTOs.Products;
using ApiFinalProject.Common.GeneralResult;
using ApiFinalProject.Common.Pagination;
using ApiFinalProject.Common.Filtering;
using ApiFinalProject.DAL.Data.Models;
using AutoMapper;

namespace ApiFinalProject.BLL.Managers;

public interface IProductManager
{
    Task<Result<PagedResult<ProductDto>>> GetProductsAsync(ProductFilterParameters filter);
    Task<Result<ProductDto>> GetProductByIdAsync(int id);
    Task<Result<ProductDto>> CreateProductAsync(ProductCreateDto dto);
    Task<Result<bool>> UpdateProductAsync(int id, ProductUpdateDto dto);
    Task<Result<bool>> DeleteProductAsync(int id);
    Task<Result<bool>> UpdateProductImageAsync(int id, string imageUrl);
}

public class ProductManager : IProductManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ProductDto>>> GetProductsAsync(ProductFilterParameters filter)
    {
        var (items, totalCount) = await _unitOfWork.Products.GetFilteredProductsAsync(filter);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(items);

        var pagedResult = new PagedResult<ProductDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
        return Result<PagedResult<ProductDto>>.Success(pagedResult);
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            return Result<ProductDto>.Failure("Product not found.");
        }

        var dto = _mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(dto);
    }

    public async Task<Result<ProductDto>> CreateProductAsync(ProductCreateDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        var createdDto = _mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(createdDto, "Product created successfully.");
    }

    public async Task<Result<bool>> UpdateProductAsync(int id, ProductUpdateDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return Result<bool>.Failure("Product not found.");

        _mapper.Map(dto, product);
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Product updated successfully.");
    }

    public async Task<Result<bool>> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return Result<bool>.Failure("Product not found.");

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Product deleted successfully.");
    }

    public async Task<Result<bool>> UpdateProductImageAsync(int id, string imageUrl)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return Result<bool>.Failure("Product not found.");

        product.ImageUrl = imageUrl;
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Product image updated successfully.");
    }
}

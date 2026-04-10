using ApiFinalProject.BLL.DTOs.Categories;
using ApiFinalProject.Common.GeneralResult;
using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.DAL.UnitOfWork;
using AutoMapper;

namespace ApiFinalProject.BLL.Managers;

public interface ICategoryManager
{
    Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryDto>> CreateCategoryAsync(CategoryCreateDto dto);
    Task<Result<bool>> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
    Task<Result<bool>> DeleteCategoryAsync(int id);
    Task<Result<bool>> UpdateCategoryImageAsync(int id, string imageUrl);
}

public class CategoryManager : ICategoryManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return Result<IEnumerable<CategoryDto>>.Success(dtos);
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return Result<CategoryDto>.Failure("Category not found.");

        var dto = _mapper.Map<CategoryDto>(category);
        return Result<CategoryDto>.Success(dto);
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CategoryCreateDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        var createdDto = _mapper.Map<CategoryDto>(category);
        return Result<CategoryDto>.Success(createdDto, "Category created successfully.");
    }

    public async Task<Result<bool>> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return Result<bool>.Failure("Category not found.");

        _mapper.Map(dto, category);
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Category updated successfully.");
    }

    public async Task<Result<bool>> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return Result<bool>.Failure("Category not found.");

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Category deleted successfully.");
    }

    public async Task<Result<bool>> UpdateCategoryImageAsync(int id, string imageUrl)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return Result<bool>.Failure("Category not found.");

        category.ImageUrl = imageUrl;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Category image updated successfully.");
    }
}

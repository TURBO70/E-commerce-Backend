using ApiFinalProject.BLL.DTOs.Categories;
using ApiFinalProject.BLL.Managers;
using ApiFinalProject.Common.GeneralResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinalProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryManager _categoryManager;
    private readonly IWebHostEnvironment _env;

    public CategoriesController(ICategoryManager categoryManager, IWebHostEnvironment env)
    {
        _categoryManager = categoryManager;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _categoryManager.GetAllCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var result = await _categoryManager.GetCategoryByIdAsync(id);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
    {
        var result = await _categoryManager.CreateCategoryAsync(dto);
        if (!result.IsSuccess) return BadRequest(result);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        var result = await _categoryManager.UpdateCategoryAsync(id, dto);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryManager.DeleteCategoryAsync(id);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{id}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(Result<string>.Failure("No file uploaded."));

        var result = await _categoryManager.GetCategoryByIdAsync(id);
        if (!result.IsSuccess) return NotFound(result);

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var pathForDb = $"/images/{uniqueFileName}";
        var updateResult = await _categoryManager.UpdateCategoryImageAsync(id, pathForDb);

        if (!updateResult.IsSuccess) return BadRequest(updateResult);

        return Ok(Result<string>.Success(pathForDb, "Category image uploaded securely."));
    }
}

using Microsoft.AspNetCore.Mvc;
using ApiFinalProject.BLL.Managers;
using ApiFinalProject.BLL.DTOs.Products;
using ApiFinalProject.Common.Filtering;
using ApiFinalProject.Common.GeneralResult;
using Microsoft.AspNetCore.Authorization;

namespace ApiFinalProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductManager _productManager;
    private readonly IWebHostEnvironment _env;

    public ProductsController(IProductManager productManager, IWebHostEnvironment env)
    {
        _productManager = productManager;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterParameters filter)
    {
        var result = await _productManager.GetProductsAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productManager.GetProductByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
    {
        var result = await _productManager.CreateProductAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
    {
        var result = await _productManager.UpdateProductAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productManager.DeleteProductAsync(id);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(Result<string>.Failure("No file uploaded."));

        var result = await _productManager.GetProductByIdAsync(id);
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
        var updateResult = await _productManager.UpdateProductImageAsync(id, pathForDb);

        if (!updateResult.IsSuccess) return BadRequest(updateResult);

        return Ok(Result<string>.Success(pathForDb, "Product image uploaded securely."));
    }
}

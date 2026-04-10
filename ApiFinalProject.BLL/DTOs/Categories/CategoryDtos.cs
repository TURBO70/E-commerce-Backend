namespace ApiFinalProject.BLL.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class CategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
}

public class CategoryUpdateDto
{
    public string Name { get; set; } = string.Empty;
}

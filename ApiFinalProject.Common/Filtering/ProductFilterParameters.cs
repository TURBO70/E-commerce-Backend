namespace ApiFinalProject.Common.Filtering;

public class ProductFilterParameters
{
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
    
    private const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    
    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}

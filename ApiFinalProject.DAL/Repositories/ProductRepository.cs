using ApiFinalProject.DAL.Data.Context;
using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.Common.Filtering;
using Microsoft.EntityFrameworkCore;

namespace ApiFinalProject.DAL.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredProductsAsync(ProductFilterParameters filter)
    {
        var query = _dbSet.Include(p => p.Category).AsQueryable();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(p => p.Name.Contains(filter.Name));
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.Common.Filtering;

namespace ApiFinalProject.DAL.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredProductsAsync(ProductFilterParameters filter);
}

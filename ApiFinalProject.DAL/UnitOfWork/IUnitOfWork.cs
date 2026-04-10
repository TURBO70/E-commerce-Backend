namespace ApiFinalProject.DAL.UnitOfWork;

using ApiFinalProject.DAL.Repositories;
using ApiFinalProject.DAL.Data.Models;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<Cart> Carts { get; }
    IGenericRepository<CartItem> CartItems { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }
    
    Task<int> SaveChangesAsync();
}

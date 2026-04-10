using ApiFinalProject.DAL.Data.Context;
using ApiFinalProject.DAL.Repositories;
using ApiFinalProject.DAL.Data.Models;

namespace ApiFinalProject.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IProductRepository Products { get; private set; }
    public IGenericRepository<Category> Categories { get; private set; }
    public IGenericRepository<Cart> Carts { get; private set; }
    public IGenericRepository<CartItem> CartItems { get; private set; }
    public IGenericRepository<Order> Orders { get; private set; }
    public IGenericRepository<OrderItem> OrderItems { get; private set; }

    public UnitOfWork(AppDbContext context, 
                      IProductRepository products,
                      IGenericRepository<Category> categories,
                      IGenericRepository<Cart> carts,
                      IGenericRepository<CartItem> cartItems,
                      IGenericRepository<Order> orders,
                      IGenericRepository<OrderItem> orderItems)
    {
        _context = context;
        Products = products;
        Categories = categories;
        Carts = carts;
        CartItems = cartItems;
        Orders = orders;
        OrderItems = orderItems;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

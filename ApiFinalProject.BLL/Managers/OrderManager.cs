using ApiFinalProject.BLL.DTOs.Orders;
using ApiFinalProject.Common.GeneralResult;
using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.DAL.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiFinalProject.BLL.Managers;

public interface IOrderManager
{
    Task<Result<OrderDto>> PlaceOrderAsync(string userId);
    Task<Result<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId);
    Task<Result<OrderDto>> GetOrderDetailsAsync(string userId, int orderId);
}

public class OrderManager : IOrderManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OrderDto>> PlaceOrderAsync(string userId)
    {
        var cart = await _unitOfWork.Carts.GetQueryable()
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (cart == null || !cart.Items.Any())
            return Result<OrderDto>.Failure("Cart is empty.");

        var order = new Order
        {
            ApplicationUserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = cart.Items.Sum(i => i.Quantity * i.Product.Price),
            Items = cart.Items.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.Price
            }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);
        
        // Clear Cart
        foreach (var item in cart.Items.ToList())
        {
            _unitOfWork.CartItems.Delete(item);
        }

        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<OrderDto>(order);
        return Result<OrderDto>.Success(dto, "Order placed successfully.");
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.Orders.GetQueryable()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ApplicationUserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
        return Result<IEnumerable<OrderDto>>.Success(dtos);
    }

    public async Task<Result<OrderDto>> GetOrderDetailsAsync(string userId, int orderId)
    {
        var order = await _unitOfWork.Orders.GetQueryable()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ApplicationUserId == userId);

        if (order == null)
            return Result<OrderDto>.Failure("Order not found or access denied.");

        var dto = _mapper.Map<OrderDto>(order);
        return Result<OrderDto>.Success(dto);
    }
}

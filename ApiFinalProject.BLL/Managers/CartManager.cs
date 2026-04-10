using ApiFinalProject.BLL.DTOs.Cart;
using ApiFinalProject.Common.GeneralResult;
using ApiFinalProject.DAL.Data.Models;
using ApiFinalProject.DAL.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiFinalProject.BLL.Managers;

public interface ICartManager
{
    Task<Result<CartDto>> GetCartAsync(string userId);
    Task<Result<bool>> AddToCartAsync(string userId, AddToCartDto dto);
    Task<Result<bool>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemDto dto);
    Task<Result<bool>> RemoveFromCartAsync(string userId, int productId);
}

public class CartManager : ICartManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CartDto>> GetCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var dto = _mapper.Map<CartDto>(cart);
        return Result<CartDto>.Success(dto);
    }

    public async Task<Result<bool>> AddToCartAsync(string userId, AddToCartDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        if (product == null) return Result<bool>.Failure("Product not found.");

        var cart = await GetOrCreateCartAsync(userId);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            _unitOfWork.CartItems.Update(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            await _unitOfWork.CartItems.AddAsync(cartItem);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true, "Item added to cart successfully.");
    }

    public async Task<Result<bool>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemDto dto)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (item == null) return Result<bool>.Failure("Item not found in cart.");

        if (dto.Quantity <= 0)
        {
            _unitOfWork.CartItems.Delete(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
            _unitOfWork.CartItems.Update(item);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true, "Cart item updated successfully.");
    }

    public async Task<Result<bool>> RemoveFromCartAsync(string userId, int productId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null) return Result<bool>.Failure("Item not found in cart.");

        _unitOfWork.CartItems.Delete(item);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Item removed from cart successfully.");
    }

    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _unitOfWork.Carts.GetQueryable()
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (cart == null)
        {
            cart = new Cart { ApplicationUserId = userId };
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        return cart;
    }
}

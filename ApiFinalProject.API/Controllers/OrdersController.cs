using ApiFinalProject.BLL.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiFinalProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderManager _orderManager;

    public OrdersController(IOrderManager orderManager)
    {
        _orderManager = orderManager;
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var userId = GetUserId();
        var result = await _orderManager.PlaceOrderAsync(userId);
        if (!result.IsSuccess) return BadRequest(result);
        return CreatedAtAction(nameof(GetOrderDetails), new { id = result.Data!.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersHistory()
    {
        var userId = GetUserId();
        var result = await _orderManager.GetUserOrdersAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        var userId = GetUserId();
        var result = await _orderManager.GetOrderDetailsAsync(userId, id);
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }
}

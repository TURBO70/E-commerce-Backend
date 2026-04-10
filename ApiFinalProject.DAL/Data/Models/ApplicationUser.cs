using Microsoft.AspNetCore.Identity;

namespace ApiFinalProject.DAL.Data.Models;

public class ApplicationUser : IdentityUser
{
    // Extensible properties for users
    
    // Navigation properties
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

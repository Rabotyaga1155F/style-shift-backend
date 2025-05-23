using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Data;

public class DataContext:IdentityDbContext<CustomUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    
    public DbSet<ProductSize> ProductSizes { get; set; }
    
    public DbSet<StyleCard> StyleCards { get; set; }
    
    public DbSet<StyleCardStatuses> StyleCardStatuses { get; set; }
    
    public DbSet<SupportRequest> SupportRequests { get; set; }
    
    public DbSet<SupportRequestStatus> SupportRequestStatuses { get; set; }
    public DbSet<PickupPoint> PickupPoints { get; set; }
    public DbSet<City> Cities { get; set; }
    

}


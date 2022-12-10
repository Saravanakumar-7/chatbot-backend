using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        {

        }
        public DbSet<BTODeliveryOrder> bTODeliveryOrder { get; set; }
        public DbSet<DeliveryOrder> deliveryOrder { get; set; }

    }
}

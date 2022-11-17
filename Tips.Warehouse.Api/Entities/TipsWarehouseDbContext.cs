using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        {

        }

    }
}

using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities
{
    public class TipsWarehouseDbContext : DbContext
    {

        public TipsWarehouseDbContext(DbContextOptions<TipsWarehouseDbContext> options) : base(options)
        {

        }

        public DbSet<Invoice> invoices { get; set; }
        public DbSet<InvoiceChildItem> invoiceChildItems { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class TipsSalesServiceDbContext : DbContext
    {
        
        public TipsSalesServiceDbContext(DbContextOptions<TipsSalesServiceDbContext> options) : base(options)
        {

        }

        public DbSet<Rfq> rfqs { get; set; }

        public DbSet<RfqNumber> rfqNumbers { get ;set;}
        public DbSet<RfqCustomerSupport> rfqCustomerSupports { get; set; }

        public DbSet<RfqCSDeliverySchedule> rfqCSDeliverySchedules { get; set; }

        public DbSet<RfqCustomerSupportItems> rfqCustomerSupportItems { get; set; }

        public DbSet<RfqCustomerSupportNotes> rfqCustomerSupportNotes { get; set; }

        public DbSet<RfqCustomGroup> rfqCustomGroups { get; set; }

        public DbSet<SourcingVendor> sourcingVendors { get; set; }




    }
}

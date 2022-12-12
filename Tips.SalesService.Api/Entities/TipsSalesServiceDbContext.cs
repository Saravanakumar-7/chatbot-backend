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

        public DbSet<RfqSourcing> RfqSourcings { get; set; }
        public DbSet<RfqSourcingItems> RfqSourcingItems { get; set; }

        public DbSet<RfqSourcingVendor> RfqSourcingVendors { get; set; }

        public DbSet<RfqEngg> RfqEnggs { get; set; }

        public DbSet<RfqEnggItem> RfqEnggItems { get; set; }

        public DbSet<RfqEnggRiskIdentification> RfqEnggRiskIdentifications { get; set; }

        public DbSet<RfqLPCosting> RfqLPCostings { get; set; }

        public DbSet<SalesOrder> salesOrders { get; set; }

        public DbSet<Quote> quotes { get; set; }

        



        public DbSet<RfqLPCostingItem> RfqLPCostingItems { get; set; }

        public DbSet<RfqLPCostingProcess> RfqLPCostingProcesses { get; set; }
        public DbSet<RfqLPCostingNREConsumable> RfqLPCostingNREConsumables { get; set; }
        public DbSet<RfqLPCostingOtherCharges> RfqLPCostingOtherCharges { get; set; }
        public DbSet<FgOqc> FgOqcs { get; set; }
        public DbSet<SaOqc> SaOqcs { get; set; }


    }
}

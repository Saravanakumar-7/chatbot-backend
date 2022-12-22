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

        public DbSet<RfqCustomGroup> RfqCustomGroups { get; set; }

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

        public DbSet<ForeCast> ForeCasts { get; set; }
        public DbSet<ForeCastCustomerSupport> ForeCastCustomerSupports { get; set; }
        public DbSet<ForeCastCustomerSupportItem> foreCastCustomerSupportItems { get; set; }
        public DbSet<ForeCastCSDeliverySchedule> ForeCastCSDeliverySchedules { get; set; }
        public DbSet<ForeCastCustomerSupportNotes> ForeCastCustomerSupportNotes { get; set; }
        public DbSet<ForeCastEngg> ForeCastEnggs { get; set; }
        public DbSet<ForeCastEnggItems> ForeCastEnggItems { get; set; }
        public DbSet<ForeCastEnggRiskIdentification> ForeCastEnggRiskIdentifications { get; set; }
        public DbSet<ForecastSourcing> ForecastSourcings { get; set; }
        public DbSet<ForecastSourcingItems> ForecastSourcingItems { get; set; }
        public DbSet<ForecastSourcingVendor> ForecastSourcingVendors { get; set; }

        public DbSet<RfqLPCostingItem> RfqLPCostingItems { get; set; }

        public DbSet<RfqLPCostingProcess> RfqLPCostingProcesses { get; set; }
        public DbSet<RfqLPCostingNREConsumable> RfqLPCostingNREConsumables { get; set; }
        public DbSet<RfqLPCostingOtherCharges> RfqLPCostingOtherCharges { get; set; }
        public DbSet<FgOqc> FgOqcs { get; set; }
        public DbSet<SaOqc> SaOqcs { get; set; }
        public DbSet<ForecastLpCosting> ForecastLpCostings { get; set; }
        public DbSet<ForecastLpCostingItem> ForecastLpCostingItems { get; set; }
        public DbSet<ForecastLpCostingProcess> ForecastLpCostingProcesses { get; set; }
        public DbSet<ForecastLPCostingNREConsumable> ForecastLPCostingNREConsumables { get; set; }
        public DbSet<ForecastLpCostingOtherCharges> ForecastLpCostingOtherCharges { get; set; }
        public DbSet<RfqCustomField> RfqCustomFields { get; set; }
        public DbSet<ForeCastCustomGroup> ForeCastCustomGroups { get; set; }
        public DbSet<ForeCastCustomField> ForeCastCustomFields { get; set; }

        public DbSet<MaterialRequest> materialRequests { get; set; }
        public DbSet<MaterialRequestItem> materialRequestItems { get; set; }
        public DbSet<MaterialTransactionNote> materialTransactionNotes { get; set; }
        public DbSet<MaterialTransactionNoteItem> materialTransactionNoteItems { get; set; }
        public DbSet<LocationTransfer> locationTransfers { get; set; }
        public DbSet<ReleaseLp> ReleaseLps { get; set; }

    }
}

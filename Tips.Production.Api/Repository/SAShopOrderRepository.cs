using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class SAShopOrderRepository : RepositoryBase<SAShopOrder>, ISAShopOrderRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;



        public SAShopOrderRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateSAShopOrder(SAShopOrder SAshopOrder)
        {
            SAshopOrder.LastModifiedBy = "Admin";
            SAshopOrder.LastModifiedOn = DateTime.Now;
            SAshopOrder.CreatedBy = "Admin";
            SAshopOrder.CreatedOn = DateTime.Now;
            SAshopOrder.Unit = "Bangalore";
            var result = await Create(SAshopOrder);
            return result.Id;
        }



        public async Task<IEnumerable<SAShopOrder>> GetAllSAShopOrders()
        {
            var SAshopOrderList = await _tipsProductionDbContext.SAshopOrders.ToListAsync();

            return (SAshopOrderList);

        }

        public async Task<SAShopOrder> GetSAShopOrderById(int id)
        {
            var SAshopOrderList = await _tipsProductionDbContext.SAshopOrders
                            .Where(x => x.Id == id)
                                .FirstOrDefaultAsync();
            return SAshopOrderList;
        }

        public async Task<string> UpdateSAShopOrder(SAShopOrder SAshopOrders)
        {
            SAshopOrders.LastModifiedBy = "Admin";
            SAshopOrders.LastModifiedOn = DateTime.Now;
            Update(SAshopOrders);
            string result = $"LeadTime details of {SAshopOrders.Id} is updated successfully!";
            return result;
        }

        public async Task<SAShopOrder> GetSAShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var sashopOrderList = await FindByCondition(x => x.SalesOrderNo == salesOrderNo)

                                .FirstOrDefaultAsync();
            return sashopOrderList;
        }

        public async Task<SAShopOrder> GetSAShopOrderShopOrderNo(string SAShopOrderNo)
        {
            var sashopOrderList = await
                            FindByCondition(x => x.SAShopOrderNo == SAShopOrderNo)
                                .FirstOrDefaultAsync();
            return sashopOrderList;
        }

        public async Task<IEnumerable<SAShopOrder>> GetAllOpenSAShopOrders()
        {
            var sashopOrderList = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (sashopOrderList);

        }
    }
}

    


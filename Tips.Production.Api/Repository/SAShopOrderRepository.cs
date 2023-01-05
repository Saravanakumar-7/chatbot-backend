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
            var sAshopOrderDetials = await _tipsProductionDbContext.SAshopOrders.ToListAsync();

            return (sAshopOrderDetials);

        }

        public async Task<SAShopOrder> GetSAShopOrderById(int id)
        {
            var sAShopOrder = await _tipsProductionDbContext.SAshopOrders
                            .Where(x => x.Id == id)
                                .FirstOrDefaultAsync();
            return sAShopOrder;
        }

        public async Task<string> UpdateSAShopOrder(SAShopOrder SAshopOrders)
        {
            SAshopOrders.LastModifiedBy = "Admin";
            SAshopOrders.LastModifiedOn = DateTime.Now;
            Update(SAshopOrders);
            string result = $"SAShopOrder details of {SAshopOrders.Id} is updated successfully!";
            return result;
        }

        public async Task<SAShopOrder> GetSAShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var sAShopOrderList = await FindByCondition(x => x.SalesOrderNumber == salesOrderNo)

                                .FirstOrDefaultAsync();
            return sAShopOrderList;
        }

        public async Task<SAShopOrder> GetSAShopOrderBySAShopOrderNo(string SAShopOrderNo)
        {
            var sAShopOrderList = await
                            FindByCondition(x => x.SAShopOrderNumber == SAShopOrderNo)
                                .FirstOrDefaultAsync();
            return sAShopOrderList;
        }

        public async Task<IEnumerable<SAShopOrder>> GetAllOpenSAShopOrders()
        {
            var sAShopOrderList = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (sAShopOrderList);

        }
    }
}

    


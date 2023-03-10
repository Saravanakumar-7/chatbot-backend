using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Repository
{
    public class SAShopOrderRepository : RepositoryBase<SAShopOrder>, ISAShopOrderRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;



        public SAShopOrderRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateSAShopOrder(SAShopOrder sAShopOrder)
        {
            sAShopOrder.LastModifiedBy = "Admin";
            sAShopOrder.LastModifiedOn = DateTime.Now;
            sAShopOrder.CreatedBy = "Admin";
            sAShopOrder.CreatedOn = DateTime.Now;
            sAShopOrder.Unit = "Bangalore";
            var result = await Create(sAShopOrder);
            return result.Id;
        }



        public async Task<IEnumerable<SAShopOrder>> GetAllSAShopOrders()
        {
            var sAShopOrderDetials = await _tipsProductionDbContext.SAShopOrders.ToListAsync();

            return (sAShopOrderDetials);

        }

        public async Task<SAShopOrder> GetSAShopOrderById(int id)
        {
            var sAShopOrderDetailById = await _tipsProductionDbContext.SAShopOrders
                            .Where(x => x.Id == id)
                                .FirstOrDefaultAsync();
            return sAShopOrderDetailById;
        }

        public async Task<string> UpdateSAShopOrder(SAShopOrder sAShopOrders)
        {
            sAShopOrders.LastModifiedBy = "Admin";
            sAShopOrders.LastModifiedOn = DateTime.Now;
            Update(sAShopOrders);
            string result = $"SAShopOrder details of {sAShopOrders.Id} is updated successfully!";
            return result;
        }

        public async Task<SAShopOrder> GetSAShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var sAShopOrderBySalesOrderNoList = await FindByCondition(x => x.SalesOrderNumber == salesOrderNo)

                                .FirstOrDefaultAsync();
            return sAShopOrderBySalesOrderNoList;
        }

        public async Task<SAShopOrder> GetSAShopOrderBySAShopOrderNo(string sAShopOrderNo)
        {
            var sAShopOrderBySAShopOrderNoList = await
                            FindByCondition(x => x.SAShopOrderNumber == sAShopOrderNo)
                                .FirstOrDefaultAsync();
            return sAShopOrderBySAShopOrderNoList;
        }

        public async Task<IEnumerable<SAShopOrder>> GetAllOpenSAShopOrders()
        {
            var openSAShopOrderList = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (openSAShopOrderList);

        }
    }
}

    


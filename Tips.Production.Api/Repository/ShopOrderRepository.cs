using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;



namespace Tips.Production.Api.Repository
{
    public class ShopOrderRepository : RepositoryBase<ShopOrder>, IShopOrderRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        

        public ShopOrderRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.LastModifiedBy = "Admin";
            shopOrder.LastModifiedOn = DateTime.Now;
            shopOrder.CreatedBy = "Admin";
            shopOrder.CreatedOn = DateTime.Now;
            var result = await Create(shopOrder);
            return result.Id;
        }

        public async Task<IEnumerable<ShopOrder>> GetAllShopOrders()
        {
            var getAllShopOrderDetails = await _tipsProductionDbContext.shopOrders.ToListAsync();

            return (getAllShopOrderDetails);

        }

        public async Task<ShopOrder> GetShopOrderById(int id)
        {
            var getShopOrder = await _tipsProductionDbContext.shopOrders
                            .Where(x => x.Id == id)
                             .FirstOrDefaultAsync();
            return getShopOrder;
        }

        public async Task<string> UpdateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.LastModifiedBy = "Admin";
            shopOrder.LastModifiedOn = DateTime.Now;
            Update(shopOrder);
            string result = $"ShopOrder details of {shopOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var getShopOrderBySalesOrderNoList = await FindByCondition(x => x.SalesOrderNumber == salesOrderNo)
                
                             .FirstOrDefaultAsync();
            return getShopOrderBySalesOrderNoList;
        }

        public async Task<ShopOrder> GetShopOrderByShopOrderNo(string ShopOrderNo)
        {
            var getShopOrderByShopOrderNoList = await
                            FindByCondition(x => x.ShopOrderNumber == ShopOrderNo)
                             .FirstOrDefaultAsync();
            return getShopOrderByShopOrderNoList;
        }

        public async Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders()
        {
            var getAllOpenShopOrderList = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2 )
                .ToListAsync();

            return (getAllOpenShopOrderList);

        }
    }
}

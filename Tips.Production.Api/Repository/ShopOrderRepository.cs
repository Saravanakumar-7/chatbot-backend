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
            var shopOrderList = await _tipsProductionDbContext.shopOrders.ToListAsync();

            return (shopOrderList);

        }

        public async Task<ShopOrder> GetShopOrderById(int id)
        {
            var shopOrderList = await _tipsProductionDbContext.shopOrders
                            .Where(x => x.Id == id)
                             .FirstOrDefaultAsync();
            return shopOrderList;
        }

        public async Task<string> UpdateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.LastModifiedBy = "Admin";
            shopOrder.LastModifiedOn = DateTime.Now;
            Update(shopOrder);
            string result = $"LeadTime details of {shopOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var shopOrderList = await FindByCondition(x => x.SalesOrderNo == salesOrderNo)
                
                             .FirstOrDefaultAsync();
            return shopOrderList;
        }

        public async Task<ShopOrder> GetShopOrderShopOrderNo(string ShopOrderNo)
        {
            var shopOrderList = await
                            FindByCondition(x => x.ShopOrderNo == ShopOrderNo)
                             .FirstOrDefaultAsync();
            return shopOrderList;
        }

        public async Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders()
        {
            var shopOrderList = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2 )
                .ToListAsync();

            return (shopOrderList);

        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Entities;
using Entities.Helper;



namespace Tips.Production.Api.Repository
{
    public class ShopOrderRepository : RepositoryBase<ShopOrder>, IShopOrderRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        

        public ShopOrderRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        
        public async Task<int?> CreateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.CreatedBy = "Admin";
            shopOrder.CreatedOn = DateTime.Now;
            Guid shopOrderNumber = Guid.NewGuid();
            shopOrder.ShopOrderNumber = "SH-" + shopOrderNumber.ToString();
            shopOrder.Unit = "Bangalore";
            var result = await Create(shopOrder);
            return result.Id;
        }


        public async Task<PagedList<ShopOrder>> GetAllShopOrders(PagingParameter pagingParameter)
        {
            var getAllShopOrders =  PagedList<ShopOrder>.ToPagedList(FindAll()
            .Include(t => t.ShopOrderItems)
            .OrderBy(on => on.Id),pagingParameter.PageNumber,pagingParameter.PageSize);
            return getAllShopOrders;

        }

        public async Task<ShopOrder> GetShopOrderById(int id)
        {
            var shopOrderById = await _tipsProductionDbContext.ShopOrders
                            .Where(x => x.Id == id)
                            .Include(y => y.ShopOrderItems)
                             .FirstOrDefaultAsync();
            return shopOrderById;
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
            var getShopOrderBySalesOrderNo = await _tipsProductionDbContext.ShopOrders
                .Include (x => x.ShopOrderItems)
                .Where (z => z.SalesOrderNumber == salesOrderNo)
                             .FirstOrDefaultAsync();
            return getShopOrderBySalesOrderNo;
        }

        public async Task<ShopOrder> GetShopOrderByShopOrderNo(string ShopOrderNo)
        {
            var getShopOrderByShopOrderNo = await _tipsProductionDbContext.ShopOrders
                            .Include(x => x.ShopOrderItems)
                            .Where(x => x.ShopOrderNumber == ShopOrderNo)
                             .FirstOrDefaultAsync();
            return getShopOrderByShopOrderNo;
        }

        public async Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders()
        {
            var getAllShopOrder = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (getAllShopOrder);

        }

        
    }
}

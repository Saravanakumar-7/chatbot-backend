using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Entities;
using Entities.Helper;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;

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
            var shopOrderDetails =  PagedList<ShopOrder>.ToPagedList(FindAll()
            .Include(t => t.ShopOrderItems)
            .OrderByDescending(on => on.Id),pagingParameter.PageNumber,pagingParameter.PageSize);
            return shopOrderDetails;

        }

        public async Task<ShopOrder> GetShopOrderById(int id)
        {
            var shopOrderDetailById = await _tipsProductionDbContext.ShopOrders
                            .Where(x => x.Id == id)
                            .Include(y => y.ShopOrderItems)
                             .FirstOrDefaultAsync();
            return shopOrderDetailById;
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
            var shopOrderBySalesOrderNo = await _tipsProductionDbContext.ShopOrders
                .Include (x => x.ShopOrderItems)
                .Where (z => z.ShopOrderItems.FirstOrDefault().SalesOrderNumber == salesOrderNo)
                             .FirstOrDefaultAsync();
            return shopOrderBySalesOrderNo;
        }

        public async Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo)
        {
            var shopOrderByShopOrderNo = await _tipsProductionDbContext.ShopOrders
                            .Include(x => x.ShopOrderItems)
                            .Where(x => x.ShopOrderNumber == shopOrderNo)
                             .FirstOrDefaultAsync();
            return shopOrderByShopOrderNo;
        }

        public async Task<ShopOrder> GetShopOrderDetailsByShopOrderNo(string shopOrderNo)
        {
            var shopOrderById = await _tipsProductionDbContext.ShopOrders.Where(x => x.ShopOrderNumber == shopOrderNo)

                          .FirstOrDefaultAsync();

            return shopOrderById;
        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByItemType(string itemType)
        {
            IEnumerable<ListOfShopOrderDto> shopOrderByItemType = await _tipsProductionDbContext.ShopOrders
                           .Where(x => x.ItemType == (PartType)Enum.Parse(typeof(PartType),itemType)).Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                           }).ToListAsync();


            return shopOrderByItemType;

        }
        public async Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNo(string fGNumber)
        {
            IEnumerable<ListOfShopOrderDto> shopOrderByFGNo = await _tipsProductionDbContext.ShopOrders
                           .Where(x => x.ItemNumber == fGNumber && x.ItemType == PartType.FG).Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                           }).ToListAsync();


            return shopOrderByFGNo;

        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNoAndSANo(string fGNumber, string sANumber)
        {
            IEnumerable<ListOfShopOrderDto> shopOrderByFGNoAndSANo = await _tipsProductionDbContext.ShopOrders
                           .Where(x => x.ItemNumber == fGNumber && x.ItemNumber == sANumber && x.ItemType == PartType.SA).Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                           }).ToListAsync();


            return shopOrderByFGNoAndSANo;

        }

        public async Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders()
        {
            var getAllShopOrder = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (getAllShopOrder);

        }     
    }
}

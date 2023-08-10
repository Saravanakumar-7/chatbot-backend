using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Entities;
using Entities.Helper;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Entities.DTOs;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tips.Production.Api.Repository
{
    public class ShopOrderRepository : RepositoryBase<ShopOrder>, IShopOrderRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public ShopOrderRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }


        public async Task<int?> CreateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.CreatedBy = _createdBy;
            shopOrder.CreatedOn = DateTime.Now;            
            shopOrder.Unit = _unitname;
            var result = await Create(shopOrder);
            return result.Id;
        }   
        public async Task<string> GenerateSONumber()
        {
            using var transaction = await _tipsProductionDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsProductionDbContext.SONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsProductionDbContext.Update(poNumberEntity);
                await _tipsProductionDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"WO-{poNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<PagedList<ShopOrder>> GetAllShopOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            var allShopOrderDetails = FindAll().OrderByDescending(x => x.Id)
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || inv.ShopOrderNumber.Contains(searchParamess.SearchValue) ||
                      inv.ProjectType.Contains(searchParamess.SearchValue) || inv.ItemType.Equals(int.Parse(searchParamess.SearchValue)) || inv.TotalSOReleaseQty.Equals(int.Parse(searchParamess.SearchValue)))))
                     .Include(t => t.ShopOrderItems);

            return PagedList<ShopOrder>.ToPagedList(allShopOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllFGShopOrderNoList()
        {
            IEnumerable<ListOfShopOrderDto> fGShopOrderNoList = await _tipsProductionDbContext.ShopOrders
                                .Where(x => x.ItemType == PartType.FG && x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                                .Select(c => new ListOfShopOrderDto()
                                {
                                    Id = c.Id,
                                    ShopOrderNumber = c.ShopOrderNumber,
                                })
                              .ToListAsync();

            return fGShopOrderNoList;
        }
        public async Task<IEnumerable<ShopOrder>> GetAllShopOrderWithItems(ShopOrderSearchDto shopOrderSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.ShopOrders.Include("ShopOrderItems");
                if (shopOrderSearch != null || (shopOrderSearch.FGItemNumber.Any())
               && shopOrderSearch.ShopOrderNumber.Any() && shopOrderSearch.SAItemNumber.Any() && shopOrderSearch.TotalSOReleaseQty.Any())
                {
                    query = query.Where
                  (po => (shopOrderSearch.ShopOrderNumber.Any() ? shopOrderSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true)
                  // && (shopOrderSearch.SAItemNumber.Any() ? shopOrderSearch.SAItemNumber.Contains(po.SAItemNumber) : true)
                  // && (shopOrderSearch.FGItemNumber.Any() ? shopOrderSearch.FGItemNumber.Contains(po.FGItemNumber) : true)
                   && (shopOrderSearch.TotalSOReleaseQty.Any() ? shopOrderSearch.TotalSOReleaseQty.Contains(po.TotalSOReleaseQty) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<ShopOrder>> SearchShopOrder([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.ShopOrders.Include("ShopOrderItems");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.ShopOrderNumber.Contains(searchParammes.SearchValue)
                    || po.TotalSOReleaseQty.ToString().Contains(searchParammes.SearchValue)
                    || po.ShopOrderNumber.Contains(searchParammes.SearchValue)
                    || po.ProjectType.Contains(searchParammes.SearchValue)
                    || po.Description.Contains(searchParammes.SearchValue)
                    || po.SOCloseDate.ToString().Contains(searchParammes.SearchValue)
                    || po.ItemNumber.Contains(searchParammes.SearchValue)
                    || po.ShopOrderItems.Any(s => s.FGItemNumber.Contains(searchParammes.SearchValue) ||
                    s.Description.Contains(searchParammes.SearchValue)
                    || s.ProjectNumber.Contains(searchParammes.SearchValue)
                    || s.SalesOrderNumber.Contains(searchParammes.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<ShopOrder>> SearchShopOrderDate([FromQuery] SearchDateparames searchDatesParams)
        {
            var shopOrderDetails = _tipsProductionDbContext.ShopOrders
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.ShopOrderItems)
            .ToList();
            return shopOrderDetails;
        }
        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllSAShopOrderNoList()
        {
            IEnumerable<ListOfShopOrderDto> sAShopOrderNoList = await _tipsProductionDbContext.ShopOrders
                                .Where(x => x.ItemType == PartType.SA && x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                                .Select(c => new ListOfShopOrderDto()
                                {
                                    Id = c.Id,
                                    ShopOrderNumber = c.ShopOrderNumber,
                                })
                              .ToListAsync();

            return sAShopOrderNoList;
        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllActiveShopOrderNoList()
        {
            IEnumerable<ListOfShopOrderDto> shopOrderNoList = await _tipsProductionDbContext.ShopOrders
                           .Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                           }).ToListAsync();


            return shopOrderNoList;

        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllShopOrderIdNameList()
        {
            IEnumerable<ListOfShopOrderDto> shopOrderNoList = await _tipsProductionDbContext.ShopOrders
                           .Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                               TotalSOReleaseQty = x.TotalSOReleaseQty
                           }).ToListAsync();


            return shopOrderNoList;

        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllActiveShopOrderNoListByProjectNo(string projectNo, PartType partType)
        {
            var shopOrderItemListByProjectNo = await _tipsProductionDbContext.ShopOrderItems
                           .Where(x => x.ProjectNumber == projectNo )
                           .Select(x => x.ShopOrderId)
                           .Distinct().ToListAsync();


            var shopOrderNoListByProjectNo = await _tipsProductionDbContext.ShopOrders
                                .Where(x => shopOrderItemListByProjectNo.Contains(x.Id) 
                                && x.ItemType == partType && x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                                .Select(s => new ListOfShopOrderDto()
                                {
                                    Id = s.Id,
                                    ShopOrderNumber = s.ShopOrderNumber,
                                }).Distinct().ToListAsync();

            return shopOrderNoListByProjectNo;

        }

        public async Task<ShopOrder> GetShopOrderById(int id)
        {
            var shopOrderDetailById = await _tipsProductionDbContext.ShopOrders
                            .Where(x => x.Id == id)
                            .Include(y => y.ShopOrderItems)
                             .FirstOrDefaultAsync();
            return shopOrderDetailById;
        }
        //Get shop order item by 

        public async Task<string> UpdateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.LastModifiedBy = _createdBy;
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

        public async Task<List<string>> GetShopOrderNoListBySalesOrderNo(string salesOrderNo,string itemNumber)
        {
            var shopOrderNoList = await _tipsProductionDbContext.ShopOrderItems
                .Where(x => x.SalesOrderNumber == salesOrderNo && x.FGItemNumber == itemNumber)
                .Select(i => i.ShopOrder.ShopOrderNumber)
                .ToListAsync();
            return shopOrderNoList;
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

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
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Production.Api.Repository
{
    public class ShopOrderRepository : RepositoryBase<ShopOrder>, IShopOrderRepository
    {
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public ShopOrderRepository(TipsProductionDbContext repositoryContext,AdvitaTipsProductionDbContext advitaTipsProductionDbContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext, advitaTipsProductionDbContext)
        {
            _advitaTipsProductionDbContext = advitaTipsProductionDbContext;
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<List<PickListDTO?>> GetPickListReport(string? ShopOrderNumber)
        {
            var result = _tipsProductionDbContext
            .Set<PickListDTO>()
            .FromSqlInterpolated($"CALL Piclist({ShopOrderNumber})")
            .ToList();

            return result;
        }

        public async Task<int?> CreateShopOrder(ShopOrder shopOrder)
        {
            shopOrder.CreatedBy = _createdBy;
            shopOrder.CreatedOn = DateTime.Now;
            shopOrder.Unit = _unitname;
            var result = await Create(shopOrder);
            return result.Id;
        }
        public async Task<IEnumerable<ShopOrderNumberSPReport>> ShopOrderNumberSPReport()
        {
            var results = _tipsProductionDbContext.Set<ShopOrderNumberSPReport>()
                        .FromSqlInterpolated($"CALL Shop_Order_Report")
                        .ToList();

            return results;
        }
        public async Task<PagedList<ShopOrderSPReportForTrans>> GetShopOrderNumberSPReportForTrans(PagingParameter pagingParameter)
        {
            var results = _tipsProductionDbContext.Set<ShopOrderSPReportForTrans>()
                        .FromSqlInterpolated($"CALL Shop_Order_Report_tras")
                        .ToList();

            return PagedList<ShopOrderSPReportForTrans>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize); 
        }
        public async Task<IEnumerable<ShopOrderNumberSPReport>> GetShopOrderSPReportWithParam(string? shopOrderNo, string? projectType,
                                                                                                  string? projectNo,string? salesOrderNo, string? KPN, string? MPN)
        {
            var result = _tipsProductionDbContext
            .Set<ShopOrderNumberSPReport>()
            .FromSqlInterpolated($"Shop_Order_Report_withparameter({shopOrderNo},{projectType},{projectNo},{salesOrderNo},{KPN},{MPN})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<ShopOrderSPReportForTrans>> GetShopOrderSPReportWithParamForTrans(string? WorkOrderNumber, string? projectType,
                                                                                                 string? projectNo, string? salesOrderNo, string? KPN, string? MPN)
        {
            var result = _tipsProductionDbContext
            .Set<ShopOrderSPReportForTrans>()
            .FromSqlInterpolated($"Shop_Order_Report_withparameter_tras({WorkOrderNumber},{projectType},{projectNo},{salesOrderNo},{KPN},{MPN})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<ShopOrderNumberSPReportForAvi>> GetShopOrderSPReportWithParamForAvi(string? shopOrderNo, string? projectType,
                                                                                                 string? projectNo, string? salesOrderNo, string? KPN, string? MPN)
        {
            var result = _tipsProductionDbContext
            .Set<ShopOrderNumberSPReportForAvi>()
            .FromSqlInterpolated($"Shop_Order_Report_withparameter_new({shopOrderNo},{projectType},{projectNo},{salesOrderNo},{KPN},{MPN})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<ShopOrderNumberSPReport>> GetShopOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsProductionDbContext.Set<ShopOrderNumberSPReport>()
                      .FromSqlInterpolated($"Shop_Order_Report_withparameter_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<ShopOrderNumberSPReportForAvi>> GetShopOrderSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsProductionDbContext.Set<ShopOrderNumberSPReportForAvi>()
                      .FromSqlInterpolated($"Shop_Order_Report_withparameter_withdate_new({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<ShopOrderSPReportForTrans>> GetShopOrderSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsProductionDbContext.Set<ShopOrderSPReportForTrans>()
                      .FromSqlInterpolated($"Shop_Order_Report_withparameter_withdate_tras({FromDate},{ToDate})")
                      .ToList();

            return results;
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
        public async Task<string> GenerateSONumberForKeus()
        {
            using var transaction = await _tipsProductionDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsProductionDbContext.SONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsProductionDbContext.Update(poNumberEntity);
                await _tipsProductionDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"SHO-{poNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> GenerateSONumberForAvision()
        {
            using var transaction = await _tipsProductionDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsProductionDbContext.SONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsProductionDbContext.Update(poNumberEntity);
                await _tipsProductionDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                //int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) // Check if the current date is after or equal to April
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100; // Get the last two digits of the current finance year
                int nextYear = (financeYearStart.Year + 1) % 100; // Get the last two digits of the next finance year

                return $"ASPL|PPC|SO|{currentYear:D2}-{nextYear:D2}|{poNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<PagedList<ShopOrder>> GetAllShopOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            PartType? check;
            if (Enum.TryParse<PartType>(searchParamess.SearchValue, out PartType result))
            {
                check = result;
            }
            else
            {
                check = null;
            }
            var allShopOrderDetails = FindAll().OrderByDescending(x => x.Id)
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue)
                      || inv.ShopOrderNumber.Contains(searchParamess.SearchValue)
                      || inv.ProjectType.Contains(searchParamess.SearchValue)
                      || inv.ItemType.Equals(check)
                      )))
                     .Include(t => t.ShopOrderItems);

            return PagedList<ShopOrder>.ToPagedList(allShopOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //var allShopOrderDetails = FindAll().OrderByDescending(x => x.Id)
            //          .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || inv.ShopOrderNumber.Contains(searchParamess.SearchValue) ||
            //          inv.ProjectType.Contains(searchParamess.SearchValue) || inv.ItemType.Equals(int.Parse(searchParamess.SearchValue)) || inv.TotalSOReleaseQty.Equals(int.Parse(searchParamess.SearchValue)))))
            //         .Include(t => t.ShopOrderItems);

            //return PagedList<ShopOrder>.ToPagedList(allShopOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
                           .Where(x => x.ProjectNumber == projectNo)
                           .Select(x => x.ShopOrderId)
                           .Distinct().ToListAsync();


            var shopOrderNoListByProjectNo = await _tipsProductionDbContext.ShopOrders
                                .Where(x => shopOrderItemListByProjectNo.Contains(x.Id)
                                && x.ItemType == partType && x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2 && x.Status != (OrderStatus)3)
                                .Select(s => new ListOfShopOrderDto()
                                {
                                    Id = s.Id,
                                    ShopOrderNumber = s.ShopOrderNumber,
                                }).Distinct().ToListAsync();

            return shopOrderNoListByProjectNo;

        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetAllShopOrderNoListByProjectNoForMRN(string projectNo, PartType partType)
        {
            var shopOrderItemListByProjectNo = await _tipsProductionDbContext.ShopOrderItems
                           .Where(x => x.ProjectNumber == projectNo)
                           .Select(x => x.ShopOrderId)
                           .Distinct().ToListAsync();


            var shopOrderNoListByProjectNo = await _tipsProductionDbContext.ShopOrders
                                .Where(x => shopOrderItemListByProjectNo.Contains(x.Id)
                                && x.ItemType == partType && x.IsDeleted == false)
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

        public async Task<string> UpdateShopOrderForApproval(ShopOrder shopOrder)
        {
            Update(shopOrder);
            string result = $"ShopOrderApproval details of {shopOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo)
        {
            var shopOrderBySalesOrderNo = await _tipsProductionDbContext.ShopOrders
                .Include(x => x.ShopOrderItems)
                .Where(z => z.ShopOrderItems.FirstOrDefault().SalesOrderNumber == salesOrderNo)
                             .FirstOrDefaultAsync();
            return shopOrderBySalesOrderNo;
        }

        public async Task<List<string>> GetShopOrderNoListBySalesOrderNo(string salesOrderNo, string itemNumber)
        {
            var shopOrderNoList = await _tipsProductionDbContext.ShopOrderItems
                .Where(x => x.SalesOrderNumber == salesOrderNo && x.FGItemNumber == itemNumber)
                .Select(i => i.ShopOrder.ShopOrderNumber)
                .ToListAsync();
            return shopOrderNoList;
        }

        public async Task<List<ShopOrderComsumpDetailsDto>> GetShopOrderComsumptionDetialsBySaleOrderNos(List<string> lotNoListString)
        {
            var poStatus = new List<OrderStatus> { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var result = await _tipsProductionDbContext.ShopOrders
                .Where(x => lotNoListString.Contains(x.ShopOrderNumber) && poStatus.Contains(x.Status))
                .Select(s=> new ShopOrderComsumpDetailsDto
                      {
                          ShopOrderNumber = s.ShopOrderNumber,
                          ItemNumber = s.ItemNumber,
                          ReleaseQty = s.TotalSOReleaseQty,
                          WipQty = s.WipQty 
                      })
                .ToListAsync();

            return result;
        }
        public async Task<string> GetShopOrderComsumptionDetialsBySaItemNo(string saItemNo, string fgItemNumber)
        {
            var poStatus = new List<OrderStatus> { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var result = await (from shopOrder in _tipsProductionDbContext.ShopOrders
                                join item in _tipsProductionDbContext.ShopOrderItems
                                on shopOrder.Id equals item.ShopOrderId
                                where shopOrder.ItemNumber == saItemNo
                                && item.FGItemNumber == fgItemNumber
                                && poStatus.Contains(shopOrder.Status)
                                select shopOrder.ShopOrderNumber)
                               .FirstOrDefaultAsync();

            return result;
        }


        public async Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo)
        {
            var shopOrderByShopOrderNo = await _tipsProductionDbContext.ShopOrders
                            .Include(x => x.ShopOrderItems)
                            .Where(x => x.ShopOrderNumber == shopOrderNo)
                             .FirstOrDefaultAsync();
            return shopOrderByShopOrderNo;
        }

        public async Task<ShopOrder> GetShopOrderApprovalStatusByShopOrderNo(string shopOrderNo)
        {
            var shopOrderByShopOrderNo = await _tipsProductionDbContext.ShopOrders
                            .Include(x => x.ShopOrderItems)
                            .Where(x => x.ShopOrderNumber == shopOrderNo && x.ShopOrderApproval == true)
                             .FirstOrDefaultAsync();
            return shopOrderByShopOrderNo;
        }

        public async Task<ShopOrder> GetShopOrderDetailsByShopOrderNo(string shopOrderNo)
        {
            var shopOrderById = await _tipsProductionDbContext.ShopOrders.Where(x => x.ShopOrderNumber == shopOrderNo).Include(x=>x.ShopOrderItems)

                          .FirstOrDefaultAsync();

            return shopOrderById;
        }

        public async Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByItemType(string itemType)
        {
            IEnumerable<ListOfShopOrderDto> shopOrderByItemType = await _tipsProductionDbContext.ShopOrders
                           .Where(x => x.ItemType == (PartType)Enum.Parse(typeof(PartType), itemType)).Select(x => new ListOfShopOrderDto()
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

        public async Task<IEnumerable<ListOfShopOrderDto>> GetApprovedShopOrderNumberList()
        {
            IEnumerable<ListOfShopOrderDto> shopOrderByItemType = await _tipsProductionDbContext.ShopOrders
                           .Where(x =>  x.ShopOrderApproval == true && x.Status != OrderStatus.ShortClose)
                           .Select(x => new ListOfShopOrderDto()
                           {
                               Id = x.Id,
                               ShopOrderNumber = x.ShopOrderNumber,
                           }).ToListAsync();


            return shopOrderByItemType;

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
        public async Task<IEnumerable<ShopOrderWipQtyDto>> GetShopOrderWipQtyByProjectNo(List<string> itemNumberList,string projectNo)
        {
            var shopOrderByIds = await _tipsProductionDbContext.ShopOrderItems
                            .Where(x => x.ProjectNumber == projectNo).Select(x => x.ShopOrderId)
                            .ToListAsync();

            List<ShopOrderWipQtyDto> shopOrderConWipQtyList = await _tipsProductionDbContext.ShopOrders
                .Where(x => itemNumberList.Contains(x.ItemNumber) && shopOrderByIds.Contains(x.Id))
                .GroupBy(x => new { x.ItemNumber })
                .Select(gr => new ShopOrderWipQtyDto
                {
                    ItemNumber = gr.Key.ItemNumber,
                    WipQuantity = gr.Sum(x => x.WipQty - x.OqcQty)
                })
                .ToListAsync();

            return shopOrderConWipQtyList;
        }
        public async Task<ShopOrderWipQtyDto> GetSAShopOrderWipQtyByProjectNo(string itemNumber, string projectNo)
        {
            var shopOrderByIds = await _tipsProductionDbContext.ShopOrderItems
                            .Where(x => x.ProjectNumber == projectNo).Select(x => x.ShopOrderId)
                            .ToListAsync();

            var shopOrderConWipQtyList = await _tipsProductionDbContext.ShopOrders
                .Where(x => x.ItemNumber == itemNumber && shopOrderByIds.Contains(x.Id))
                .GroupBy(x => new { x.ItemNumber })
                .Select(gr => new ShopOrderWipQtyDto
                {
                    ItemNumber = gr.Key.ItemNumber,
                    WipQuantity = gr.Sum(x => x.WipQty - x.OqcQty)
                })
                .FirstOrDefaultAsync();

            return shopOrderConWipQtyList;
        }
        public async Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders()
        {
            var getAllShopOrder = await FindByCondition(x => x.IsDeleted == false && x.IsShortClosed == false && x.Status != (OrderStatus)2)
                .ToListAsync();

            return (getAllShopOrder);

        }
    }
}

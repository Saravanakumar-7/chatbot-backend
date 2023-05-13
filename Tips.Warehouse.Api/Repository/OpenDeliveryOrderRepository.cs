using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class OpenDeliveryOrderRepository : RepositoryBase<OpenDeliveryOrder>, IOpenDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public OpenDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            var date = DateTime.Now;
            openDeliveryOrder.CreatedBy = "Admin";
            openDeliveryOrder.CreatedOn = date.Date;          
          
            openDeliveryOrder.Unit = "Bangalore";
            var result = await Create(openDeliveryOrder);
           
            return result.Id;
        }        

        public async Task<int?> GetODONumberAutoIncrementCount(DateTime date)
        {
            var getOpenDeliveryOrderDetailsByIds = _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.CreatedOn == date.Date).Count();

            return getOpenDeliveryOrderDetailsByIds;
        }

        public async Task<string> GenerateODONumber()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var odoNumberEntity = await _tipsWarehouseDbContext.ODONumbers.SingleAsync();
                odoNumberEntity.CurrentValue += 1;
                _tipsWarehouseDbContext.Update(odoNumberEntity);
                await _tipsWarehouseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"ODO-{odoNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            Delete(openDeliveryOrder);
            string result = $"OpenDeliveryOrder details of {openDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var openDeliveryOrderDetails = _tipsWarehouseDbContext.OpenDeliveryOrders
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .Include(itm => itm.OpenDeliveryOrderParts)
            .ToList();
            return openDeliveryOrderDetails;
        }
        public async Task<IEnumerable<OpenDeliveryOrder>> GetAllOpenDeliveryOrderWithItems(OpenDeliveryOrderSearchDto openDeliveryOrderSearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.OpenDeliveryOrders.Include("OpenDeliveryOrderParts");
                if (openDeliveryOrderSearch != null || (openDeliveryOrderSearch.DOType.Any())
                 && openDeliveryOrderSearch.ODONumber.Any() && openDeliveryOrderSearch.CustomerName.Any()
                 && openDeliveryOrderSearch.Description.Any() && openDeliveryOrderSearch.IssuedTo.Any())

                {
                    query = query.Where
                    (po => (openDeliveryOrderSearch.CustomerName.Any() ? openDeliveryOrderSearch.CustomerName.Contains(po.CustomerName) : true)
                   && (openDeliveryOrderSearch.Description.Any() ? openDeliveryOrderSearch.Description.Contains(po.Description) : true)
                   && (openDeliveryOrderSearch.ODONumber.Any() ? openDeliveryOrderSearch.ODONumber.Contains(po.OpenDONumber) : true)
                   && (openDeliveryOrderSearch.DOType.Any() ? openDeliveryOrderSearch.DOType.Contains(po.DOType) : true)
                   && (openDeliveryOrderSearch.IssuedTo.Any() ? openDeliveryOrderSearch.IssuedTo.Contains(po.IssuedTo) : true));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrder([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.OpenDeliveryOrders.Include("OpenDeliveryOrderParts");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.OpenDONumber.Contains(searchParames.SearchValue)
                    || po.CustomerName.Contains(searchParames.SearchValue)
                    || po.CustomerAliasName.Contains(searchParames.SearchValue)
                    || po.CustomerId.Contains(searchParames.SearchValue)
                    || po.DOType.Contains(searchParames.SearchValue)
                    || po.IssuedTo.Contains(searchParames.SearchValue)
                    || po.OpenDeliveryOrderParts.Any(s => s.ItemNumber.Contains(searchParames.SearchValue) ||
                    s.ItemDescription.Contains(searchParames.SearchValue)
                    || s.ItemType.ToString().Contains(searchParames.SearchValue)
                    || s.Location.Contains(searchParames.SearchValue)
                    ));
                }
                return query.ToList();
            }
        }
        public async Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllOpenDeliveryOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.OpenDONumber.Contains(searchParams.SearchValue) ||
                inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue))))
                .Include(x => x.OpenDeliveryOrderParts);
            return PagedList<OpenDeliveryOrder>.ToPagedList(getAllOpenDeliveryOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }


        public async Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id)
        {
            var getOpenDeliveryOrderDetailsById = await _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.Id == id)
                               .Include(x => x.OpenDeliveryOrderParts)

                               .FirstOrDefaultAsync();

            return getOpenDeliveryOrderDetailsById;
        }


        public async Task<List<ODODetailsDto>> GetODODetailsByItemNo(string itemNumber)
        {
            var projectNumbers = await _tipsWarehouseDbContext.Inventory
                                .Where(x => x.PartNumber == itemNumber)
                                .Select(s => new ODODetailsDto()
                                {
                                    ItemNumber = s.ProjectNumber,
                                    ItemType = s.PartType,
                                    UOM = s.UOM,
                                }).Distinct().ToListAsync();

            return projectNumbers;
        }

        public async Task<List<WarehouseDetailsDto>> GetWarehouseODOByItemNo(string itemNumber)
        {
            var projectNumbers = await _tipsWarehouseDbContext.Inventory
                                .Where(x => x.PartNumber == itemNumber)
                                .Select(s => new WarehouseDetailsDto()
                                {
                                    WarehouseName = s.Warehouse,
                                    
                                }).Distinct().ToListAsync();

            return projectNumbers;
        }

        public async Task<List<LocationDetailsDto>> GetLocationODOByItemNo(string itemNumber)
        {
            var projectNumbers = await _tipsWarehouseDbContext.Inventory
                                .Where(x => x.PartNumber == itemNumber)
                                .Select(s => new LocationDetailsDto()
                                {
                                    LocationName = s.Location,
                                    LocationStock = s.Balance_Quantity,

                                }).Distinct().ToListAsync();

            return projectNumbers;
        }

        public async Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            openDeliveryOrder.LastModifiedBy = "Admin";
            openDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(openDeliveryOrder);
            string result = $"openDeliveryOrder of Detail {openDeliveryOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<OpenDeliveryOrderIdNameList>> GetAllOpenDeliveryOrderIdNameList()
        {
            IEnumerable<OpenDeliveryOrderIdNameList> btoIddNameList = await _tipsWarehouseDbContext.OpenDeliveryOrders
                               .Select(x => new OpenDeliveryOrderIdNameList()
                               {
                                   Id = x.Id,

                                   CustomerName = x.CustomerName

                               })
                               .OrderByDescending(x => x.Id)
                             .ToListAsync();

            return btoIddNameList;
        }
    }
}

using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class DeliveryOrderRepository : RepositoryBase<DeliveryOrder>, IDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public DeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            var date = DateTime.Now;
            deliveryOrder.CreatedBy = "Admin";
            deliveryOrder.CreatedOn = date.Date;
            deliveryOrder.Unit = "Bangalore";
            //Guid deliveryOrderNumber = Guid.NewGuid();
            //deliveryOrder.DeliveryOrderNumber = " DO-" + deliveryOrderNumber.ToString();
            var result = await Create(deliveryOrder);
            return result.Id;
        }
        public async Task<int?> GetDONumberAutoIncrementCount(DateTime date)
        {
            var getBTODeliveryOrderDetailsByIds = _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.CreatedOn == date.Date).Count();

            return getBTODeliveryOrderDetailsByIds;
        }
        public async Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            Delete(deliveryOrder);
            string result = $"DeliveryOrder details of {deliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<DeliveryOrder>> GetAllActiveDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var allActiveDeliveryOrderDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                || inv.ProjectNumber.Contains(searchParams.SearchValue)
                || inv.DeliveryOrderNumber.Contains(searchParams.SearchValue)
                || inv.CustomerName.Contains(searchParams.SearchValue))))
                .Include(t => t.DeliveryOrderItems)
                .ThenInclude(y => y.DoSerialNumbers);

            return PagedList<DeliveryOrder>.ToPagedList(allActiveDeliveryOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<DeliveryOrder>> GetAllDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var allDeliveryOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.DeliveryOrderNumber.Contains(searchParams.SearchValue) ||
                inv.ProjectNumber.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue))))
                .Include(t => t.DeliveryOrderItems)
                .ThenInclude(y => y.DoSerialNumbers);

            return PagedList<DeliveryOrder>.ToPagedList(allDeliveryOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<DeliveryOrder> GetDeliveryOrderById(int id)
        {
            var deliveryOrderDetailsbyId = await _tipsWarehouseDbContext.DeliveryOrder.Where(x => x.Id == id)
                              .Include(t => t.DeliveryOrderItems)
                              .ThenInclude(y => y.DoSerialNumbers)
                              .FirstOrDefaultAsync();


            return deliveryOrderDetailsbyId;
        }
        public async Task<IEnumerable<DeliveryOrder>> GetAllDeliveryOrderWithItems(DeliveryOrderSearchDto DeliveryOrderSearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.DeliveryOrder.Include("DeliveryOrderItems");
                if (DeliveryOrderSearch != null || (DeliveryOrderSearch.ProjectNumber.Any())
               && DeliveryOrderSearch.CustomerName.Any() && DeliveryOrderSearch.PONumber.Any())
                {
                    query = query.Where
                    (po => (DeliveryOrderSearch.CustomerName.Any() ? DeliveryOrderSearch.CustomerName.Contains(po.CustomerName) : true)
                   && (DeliveryOrderSearch.ProjectNumber.Any() ? DeliveryOrderSearch.ProjectNumber.Contains(po.ProjectNumber) : true)
                   && (DeliveryOrderSearch.PONumber.Any() ? DeliveryOrderSearch.PONumber.Contains(po.PONumber) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<DeliveryOrder>> SearchDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var DeliveryOrderDetails = _tipsWarehouseDbContext.DeliveryOrder
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .Include(itm => itm.DeliveryOrderItems)
            .ToList();
            return DeliveryOrderDetails;
        }
        public async Task<IEnumerable<DeliveryOrder>> SearchDeliveryOrder([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.DeliveryOrder.Include("DeliveryOrderItems");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.CustomerName.Contains(searchParames.SearchValue)
                    || po.PONumber.Contains(searchParames.SearchValue)
                    || po.ProjectNumber.Contains(searchParames.SearchValue)
                    || po.DeliveryOrderItems.Any(s => s.FGItemNumber.Contains(searchParames.SearchValue)
                    || s.ItemDescription.Contains(searchParames.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            deliveryOrder.LastModifiedBy = "Admin";
            deliveryOrder.LastModifiedOn = DateTime.Now;
            Update(deliveryOrder);
            string result = $"DeliveryOrder of Detail {deliveryOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<DeliveryOrderIdNameList>> GetAllDeliveryOrderIdNameList()
        {
            IEnumerable<DeliveryOrderIdNameList> DeliveryOrderIddNameList = await _tipsWarehouseDbContext.DeliveryOrder
                                .Select(x => new DeliveryOrderIdNameList()
                                {
                                    Id = x.Id,

                                    DeliveryOrderNumber = x.DeliveryOrderNumber

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return DeliveryOrderIddNameList;
        }
    }
}

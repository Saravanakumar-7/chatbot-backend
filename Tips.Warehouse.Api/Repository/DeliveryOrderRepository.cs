using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

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

        public async Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            deliveryOrder.LastModifiedBy = "Admin";
            deliveryOrder.LastModifiedOn = DateTime.Now;
            Update(deliveryOrder);
            string result = $"DeliveryOrder of Detail {deliveryOrder.Id} is updated successfully!";
            return result;
        }

    }
}

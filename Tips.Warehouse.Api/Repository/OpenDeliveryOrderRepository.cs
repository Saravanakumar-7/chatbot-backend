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

        public async Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            Delete(openDeliveryOrder);
            string result = $"OpenDeliveryOrder details of {openDeliveryOrder.Id} is deleted successfully!";
            return result;
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

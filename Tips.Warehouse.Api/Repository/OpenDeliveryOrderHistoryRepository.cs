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
    public class OpenDeliveryOrderHistoryRepository : RepositoryBase<OpenDeliveryOrderHistory>, IOpenDeliveryOrderHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public OpenDeliveryOrderHistoryRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }
        public async Task<long> CreateOpenDeliveryOrderHistory(OpenDeliveryOrderHistory openDeliveryOrderHistory)
        {
            openDeliveryOrderHistory.CreatedBy = "Admin";
            openDeliveryOrderHistory.CreatedOn = DateTime.Now;
            openDeliveryOrderHistory.Unit = "Banglore";
            var result = await Create(openDeliveryOrderHistory);
            return result.Id;
        }

        public async Task<PagedList<OpenDeliveryOrderHistory>> GetAllOpenDeliveryOrderHistoryDetails(PagingParameter pagingParameter,SearchParams searchParams)
        {
            var odo = await _tipsWarehouseDbContext.ReturnOpenDeliveryOrders.Where(odo => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || odo.CustomerId.Contains(searchParams.SearchValue) ||
                odo.CustomerAliasName.Contains(searchParams.SearchValue) || odo.CustomerName.Contains(searchParams.SearchValue)
                || odo.Description.Contains(searchParams.SearchValue))))
              .FirstOrDefaultAsync();

            var getAllOpenDetails = PagedList<OpenDeliveryOrderHistory>.ToPagedList(FindAll()
                    .Where(x => x.UniqeId != null)
                    .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);



            return getAllOpenDetails;
        }

        public async Task<IEnumerable<OpenDeliveryOrderHistory>> GetOpenDeliveryOrderHistoryDetailsByBtoNo(string odoNumber, string uniqueId)
        {
            var openHistoryDetails = await _tipsWarehouseDbContext.OpenDeliveryOrderHistories
              .Where(x => x.ODONumber == odoNumber && x.UniqeId == uniqueId)
                        .ToListAsync();
            return openHistoryDetails;
        }

        public async Task<OpenDeliveryOrderHistory> GetOpenDeliveryOrderHistoryDetailsById(int id)
        {
            var openDeliveryOrderHistoryById = await _tipsWarehouseDbContext.OpenDeliveryOrderHistories.Where(x => x.Id == id)
                                 .FirstOrDefaultAsync();

            return openDeliveryOrderHistoryById;
        }
    }
}

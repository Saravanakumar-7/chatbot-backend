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
    }

}

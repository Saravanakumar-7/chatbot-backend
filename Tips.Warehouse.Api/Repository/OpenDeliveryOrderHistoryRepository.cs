using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities.DTOs;
using System.Linq;
using System.Security.Claims;

namespace Tips.Warehouse.Api.Repository
{
    public class OpenDeliveryOrderHistoryRepository : RepositoryBase<OpenDeliveryOrderHistory>, IOpenDeliveryOrderHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenDeliveryOrderHistoryRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<long> CreateOpenDeliveryOrderHistory(OpenDeliveryOrderHistory openDeliveryOrderHistory)
        {
            openDeliveryOrderHistory.CreatedBy = _createdBy;
            openDeliveryOrderHistory.CreatedOn = DateTime.Now;
            openDeliveryOrderHistory.Unit = _unitname;
            var result = await Create(openDeliveryOrderHistory);
            return result.Id;
        }

        public async Task<PagedList<OpenDeliveryOrderHistory>> GetAllOpenDeliveryOrderHistoryDetails(PagingParameter pagingParameter,SearchParams searchParams)
        {
            var odo = FindAll().OrderByDescending(x=>x.Id).Where(odo => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || odo.CustomerId.Contains(searchParams.SearchValue) ||
                odo.CustomerAliasName.Contains(searchParams.SearchValue) || odo.CustomerName.Contains(searchParams.SearchValue)
                || odo.Description.Contains(searchParams.SearchValue))));
             
            return PagedList<OpenDeliveryOrderHistory>.ToPagedList(odo, pagingParameter.PageNumber, pagingParameter.PageSize);
            //var getAllOpenDetails = PagedList<OpenDeliveryOrderHistory>.ToPagedList(FindAll()
            //        .Where(x => x.UniqeId != null)
            //        .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);



            //return getAllOpenDetails;
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

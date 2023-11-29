using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class OrderTypeRepository : RepositoryBase<OrderType>, IOrderTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OrderTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";


        }

        public async Task<int?> CreateOrderType(OrderType orderType)
        {
            orderType.CreatedBy = _createdBy;
            orderType.CreatedOn = DateTime.Now;
            orderType.Unit = _unitname;
            var result = await Create(orderType);

            return result.Id;
        }

        public async Task<string> DeleteOrderType(OrderType orderType)
        {
            Delete(orderType);
            string result = $"OrderType details of {orderType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<OrderType>> GetAllActiveOrderType([FromQuery] SearchParames searchParams)
        {
            var orderTypeDetails = FindAll()
                              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.OrderTypeName.Contains(searchParams.SearchValue) ||
                                     inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return orderTypeDetails;
        }

        public async Task<IEnumerable<OrderType>> GetAllOrderType([FromQuery] SearchParames searchParams)
        {
            var orderTypeDetails = FindAll().OrderByDescending(x => x.Id)
                              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.OrderTypeName.Contains(searchParams.SearchValue) ||
                                     inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return orderTypeDetails;
        }
        public async Task<OrderType> GetDefaultOrderType(int id)
        {

            var orderTypeDetailsById = await TipsMasterDbContext.OrderTypes
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            orderTypeDetailsById.IsDefault = true;
            Update(orderTypeDetailsById);
             return orderTypeDetailsById;
        }
        public async Task<IEnumerable<OrderType>> GetDefaultOrderTypeValue(int id)
        {
            var updateOrderTypeValue = await TipsMasterDbContext.OrderTypes.Where(x => x.Id != id).ToListAsync();
            return updateOrderTypeValue;
        }

        public async Task<OrderType> GetOrderTypeById(int id)
        {
            var orderTypeDetailsById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return orderTypeDetailsById;
        }

        public async Task<string> UpdateOrderType(OrderType orderType)
        {
            orderType.LastModifiedBy = _createdBy;
            orderType.LastModifiedOn = DateTime.Now;
            orderType.IsDefault = false;
            Update(orderType);
            string result = $"OrderType details of {orderType.Id} is updated successfully!";
            return result;
        }
    }
}

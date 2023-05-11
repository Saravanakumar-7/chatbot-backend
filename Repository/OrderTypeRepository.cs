using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class OrderTypeRepository : RepositoryBase<OrderType>, IOrderTypeRepository
    {
        public OrderTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateOrderType(OrderType orderType)
        {
            orderType.CreatedBy = "Admin";
            orderType.CreatedOn = DateTime.Now;
            orderType.Unit = "Bangalore";
            var result = await Create(orderType);

            return result.Id;
        }

        public async Task<string> DeleteOrderType(OrderType orderType)
        {
            Delete(orderType);
            string result = $"OrderType details of {orderType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<OrderType>> GetAllActiveOrderType()
        {
            var orderTypeDetails = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return orderTypeDetails;
        }

        public async Task<IEnumerable<OrderType>> GetAllOrderType()
        {
            var orderTypeDetails = await FindAll().ToListAsync();
            return orderTypeDetails;
        }

        public async Task<OrderType> GetOrderTypeById(int id)
        {
            var orderTypeDetailsById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return orderTypeDetailsById;
        }

        public async Task<string> UpdateOrderType(OrderType orderType)
        {
            orderType.LastModifiedBy = "Admin";
            orderType.LastModifiedOn = DateTime.Now;
            Update(orderType);
            string result = $"OrderType details of {orderType.Id} is updated successfully!";
            return result;
        }
    }
}

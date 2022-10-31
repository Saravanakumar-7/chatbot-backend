using CommonModels;
using Contracts;

namespace Tips.Master.Api.Services
{
    public interface ICustomerType
    {
        public Task<ServiceResponse<IEnumerable<CustomerType>>> GetAllCustomerTypes();
        public Task<ServiceResponse<IEnumerable<CustomerType>>> GetAllActiveCustomerTypes();
    }
}

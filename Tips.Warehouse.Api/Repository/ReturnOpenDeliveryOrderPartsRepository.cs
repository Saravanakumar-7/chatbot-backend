using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class ReturnOpenDeliveryOrderPartsRepository : RepositoryBase<ReturnOpenDeliveryOrderParts>, IReturnOpenDeliveryOrderPartsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public ReturnOpenDeliveryOrderPartsRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public Task<int?> CreateReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ReturnOpenDeliveryOrderParts>> GetAllReturnOpenDeliveryOrderParts(PagingParameter pagingParameter)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnOpenDeliveryOrderParts> GetReturnOpenDeliveryOrderPartsById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts)
        {
            throw new NotImplementedException();
        }
    }
}

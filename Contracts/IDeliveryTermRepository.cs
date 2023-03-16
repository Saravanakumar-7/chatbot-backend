using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDeliveryTermRepository : IRepositoryBase<DeliveryTerm>
    {
        Task<PagedList<DeliveryTerm>> GetAllDeliveryTerms(PagingParameter pagingParameter, SearchParames searchParams);
        Task<DeliveryTerm> GetDeliveryTermById(int id);
        Task<PagedList<DeliveryTerm>> GetAllActiveDeliveryTerms(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm);
        Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm);
        Task<string> DeleteDeliveryTerm(DeliveryTerm deliveryTerm);
    }
}

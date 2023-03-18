using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomGroupRepository:IRepositoryBase<RfqCustomGroup>
    {
        Task<PagedList<RfqCustomGroup>> GetAllRfqCustomGroup(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqCustomGroup> GetRfqCustomGroupById(int id);
        Task<int?> CreateRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
        Task<string> UpdateRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
        Task<string> DeleteRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
        Task<IEnumerable<ListOfCustomGroupDto>> GetAllCustomGroupList();
    }
}

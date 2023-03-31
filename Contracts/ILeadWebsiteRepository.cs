using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILeadWebsiteRepository: IRepositoryBase<LeadWebsite>
    {
        Task<PagedList<LeadWebsite>> GetAllLeadWebsite(PagingParameter pagingParameter, SearchParames searchParams);
        Task<LeadWebsite> GetLeadWebsiteById(int id);

        Task<PagedList<LeadWebsite>> GetAllActiveLeadWebsite(PagingParameter pagingParameter, SearchParames searchParams);

        Task<int?> CreateLeadWebsite(LeadWebsite leadWebsite);
        Task<string> UpdateLeadWebsite(LeadWebsite leadWebsite);
        Task<string> DeleteLeadWebsite(LeadWebsite leadWebsite);
    }
}

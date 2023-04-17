using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface ICompanyMasterRepository : IRepositoryBase<CompanyMaster>
    {
        Task<PagedList<CompanyMaster>> GetAllCompanyMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CompanyMaster> GetCompanyMasterById(int id);
        Task<IEnumerable<CompanyMaster>> GetAllActiveCompanyMasters();
        Task<int?> CreateCompanyMaster(CompanyMaster companyMaster);
        Task<string> UpdateCompanyMaster(CompanyMaster companyMaster);
        Task<string> DeleteCompanyMaster(CompanyMaster companyMaster);
        Task<IEnumerable<CompanyIdNameListDto>> GetAllActiveCompanyMasterIdNameList();
    }
}

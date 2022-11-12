using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;

namespace Contracts
{
    public interface ICompanyMasterRepository : IRepositoryBase<CompanyMaster>
    {
        Task<IEnumerable<CompanyMaster>> GetAllCompanyMaster();
        Task<CompanyMaster> GetCompanyMasterById(int id);
        Task<IEnumerable<CompanyMaster>> GetAllActiveCompanyMaster();
        Task<int?> CreateCompanyMaster(CompanyMaster companyMaster);
        Task<string> UpdateCompanyMaster(CompanyMaster companyMaster);
        Task<string> DeleteCompanyMaster(CompanyMaster companyMaster);
        Task<IEnumerable<CompanyIdNameListDto>> GetAllActiveCompanyIdNameList();
    }
}

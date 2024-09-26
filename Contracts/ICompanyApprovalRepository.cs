using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyApprovalRepository
    {
        Task<IEnumerable<CompanyApproval>> GetAllCompanyApproval();
        Task<CompanyApproval> GetCompanyApprovalById(int id);
        Task<IEnumerable<CompanyApproval>> GetAllActiveCompanyApproval();
        Task<int?> CreateCompanyApproval(CompanyApproval companyApproval);
        Task<string> UpdateCompanyApproval(CompanyApproval companyApproval);
        Task<string> DeleteCompanyApproval(CompanyApproval companyApproval);
    }
}

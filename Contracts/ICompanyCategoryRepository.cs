using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyCategoryRepository : IRepositoryBase<CompanyCategory>
    {
        Task<IEnumerable<CompanyCategory>> GetAllCompanyCategory();
        Task<CompanyCategory> GetCompanyCategoryById(int id);
        Task<IEnumerable<CompanyCategory>> GetAllActiveCompanyCategory();
        Task<int?> CreateCompanyCategory(CompanyCategory companyCategory);
        Task<string> UpdateCompanyCategory(CompanyCategory companyCategory);
        Task<string> DeleteCompanyCategory(CompanyCategory companyCategory);
    }
}

using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyMasterOtherUploadsRepository : IRepositoryBase<CompanyOtherUploads>
    {
        Task<int?> CreateCompanyOtherUploads(CompanyOtherUploads companyOtherUploads);
        Task<CompanyOtherUploads> GetCompanyMasterOtherUploadsbyCompanyId(int Id);
        Task<string> UpdateCompanyOtherUploads(CompanyOtherUploads companyOtherUploads);
    }
}

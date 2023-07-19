using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyFileUploadRepository : RepositoryBase<CompanyFileUpload>, ICompanyFileUploadRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CompanyFileUploadRepository(TipsMasterDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsMasterDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<int?> CreateCompanyFileUpload(CompanyFileUpload companyFileUpload)
        {
            companyFileUpload.CreatedBy = _createdBy;
            companyFileUpload.CreatedOn = DateTime.Now;
            companyFileUpload.LastModifiedBy = _createdBy;
            companyFileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(companyFileUpload);
            return result.Id;
        }
    }
}

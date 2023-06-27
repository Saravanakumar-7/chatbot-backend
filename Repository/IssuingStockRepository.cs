using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class IssuingStockRepository : RepositoryBase<IssuingStock>, IIssuingStockRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IssuingStockRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateIssuingStock(IssuingStock issueStock)
        {

            issueStock.CreatedBy = _createdBy;
            issueStock.CreatedOn = DateTime.Now;
            issueStock.Unit = _unitname;
            var result = await Create(issueStock); return result.Id;
        }

        public async Task<string> DeleteIssuingStock(IssuingStock issuingStock)
        {
            Delete(issuingStock);
            string result = $"IssuingStock details of {issuingStock.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<IssuingStock>> GetAllActiveIssuingStock()
        {
            var issuingStockDetails = await FindAll().Where(x => x.IsActive == true).ToListAsync();
            return issuingStockDetails;
        }

        public async Task<IEnumerable<IssuingStock>> GetAllIssuingStock()
        {
            var issuingStockDetails = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return issuingStockDetails;
        }

        public async Task<IssuingStock> GetIssuingStockById(int id)
        {
            var issuingStockById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return issuingStockById;
        }

        public async Task<string> UpdateIssuingStock(IssuingStock issuingStock)
        {
            issuingStock.LastModifiedBy = _createdBy;
            issuingStock.LastModifiedOn = DateTime.Now;
            Update(issuingStock);
            string result = $"IssuingStock details of {issuingStock.Id} is updated successfully!";
            return result;
        }
    }
}


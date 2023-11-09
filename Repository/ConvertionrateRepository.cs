using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ConvertionrateRepository : RepositoryBase<Convertionrate>, IConvertionrateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ConvertionrateRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateConvertionrate(Convertionrate convertionrate)
        {
            convertionrate.CreatedBy = _createdBy;
            convertionrate.CreatedOn = DateTime.Now;
            convertionrate.Unit = _unitname;
            var result = await Create(convertionrate);

            return result.Id;
        }

        public async Task<string> DeleteConvertionrate(Convertionrate convertionrate)
        {
            Delete(convertionrate);
            string result = $"Convertionrate details of {convertionrate.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Convertionrate>> GetAllActiveConvertionrate(SearchParames searchParams)
        {
            var convertionrateDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UOC.Contains(searchParams.SearchValue) ||
         inv.Unit.Contains(searchParams.SearchValue))));
            return convertionrateDetails;
        }

        public async Task<IEnumerable<Convertionrate>> GetAllConvertionrate(SearchParames searchParams)
        {
            var convertionrateDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UOC.Contains(searchParams.SearchValue) ||
          inv.Unit.Contains(searchParams.SearchValue))));
            return convertionrateDetails;
        }

        public async Task<Convertionrate> GetConvertionrateById(int id)
        {
            var convertionratebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return convertionratebyId;
        }
        public async Task<decimal> GetLatestConvertionrateByUOC(string currency)
        {
            decimal currentrate = await FindByCondition(x => x.UOC == currency).OrderByDescending(x => x.Date).Select(x=>x.ConvertionRate)
                .FirstOrDefaultAsync();
            return currentrate;
        }
        public async Task<string> UpdateConvertionrate(Convertionrate convertionrate)
        {
            convertionrate.LastModifiedBy = _createdBy;
            convertionrate.LastModifiedOn = DateTime.Now;
            Update(convertionrate);
            string result = $"Convertionrate details of {convertionrate.Id} is updated successfully!";
            return result;
        }
    }
}
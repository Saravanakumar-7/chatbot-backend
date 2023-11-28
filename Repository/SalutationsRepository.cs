using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Repository
{
    public class SalutationsRepository : RepositoryBase<Salutations>, ISalutationsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SalutationsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateSalutations(Salutations salutations)
        {
            salutations.CreatedBy = _createdBy;
            salutations.CreatedOn = DateTime.Now;
            salutations.Unit = _unitname;
            var result = await Create(salutations);
            
            return result.Id;
        }

        public async Task<string> DeleteSalutations(Salutations salutations)
        {

            Delete(salutations);
            string result = $"Salutations details of {salutations.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Salutations>> GetAllActiveSalutations([FromQuery] SearchParames searchParams)
        {
            var salutationDetails = FindAll()
                                    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.SalutationsName.Contains(searchParams.SearchValue) ||
                                   inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return salutationDetails;
        }

        public async Task<IEnumerable<Salutations>> GetAllSalutations([FromQuery] SearchParames searchParams)
        {
            var salutationDetails = FindAll().OrderByDescending(x => x.Id)
                               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.SalutationsName.Contains(searchParams.SearchValue) ||
                           inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return salutationDetails;
        }

        public async Task<Salutations> GetSalutationsById(int id)
        {
            var SalutationsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SalutationsbyId;
        }

        public async Task<string> UpdateSalutations(Salutations salutations)
        {
            salutations.LastModifiedBy = _createdBy;
            salutations.LastModifiedOn = DateTime.Now;
            Update(salutations);
            string result = $"Salutations details of {salutations.Id} is updated successfully!";
            return result;
        }
    }
}

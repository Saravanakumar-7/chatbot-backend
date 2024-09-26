using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ProcurementTypeRepository: RepositoryBase<ProcurementType>, IProcurementTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ProcurementTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateProcurementType(ProcurementType ProcurementType)
        {
            ProcurementType.CreatedBy = _createdBy;
            ProcurementType.CreatedOn = DateTime.Now;
            ProcurementType.Unit = _unitname;
            var result = await Create(ProcurementType);
            
            return result.Id;
        }

        public async Task<string> DeleteProcurementType(ProcurementType ProcurementType)
        {
            Delete(ProcurementType);
            string result = $"ProcurementType details of {ProcurementType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllActiveProcurementType([FromQuery] SearchParames searchParams)
        {
            var procurementTypeDetails = FindAll()
         .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProcurementName.Contains(searchParams.SearchValue) ||
        inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return procurementTypeDetails;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllProcurementType([FromQuery] SearchParames searchParams)
        {
            var procurementTypeDetails = FindAll().OrderByDescending(x => x.Id)
        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProcurementName.Contains(searchParams.SearchValue) ||
       inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return procurementTypeDetails;
        }

        public async Task<ProcurementType> GetProcurementTypeById(int id)
        {
            var ProcurementTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return ProcurementTypebyId;
        }

        public async Task<string> UpdateProcurementType(ProcurementType ProcurementType)
        {
            ProcurementType.LastModifiedBy = _createdBy;
            ProcurementType.LastModifiedOn = DateTime.Now;
            Update(ProcurementType);
            string result = $"ProcurementType details of {ProcurementType.Id} is updated successfully!";
            return result;
        }
    }
}

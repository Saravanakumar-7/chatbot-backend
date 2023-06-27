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

        public async Task<IEnumerable<ProcurementType>> GetAllActiveProcurementType()
        {
            var AllActiveProcurementTypes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveProcurementTypes;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllProcurementType()
        {
            var GetallProcurementTypes = await FindAll().ToListAsync();
            return GetallProcurementTypes;
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

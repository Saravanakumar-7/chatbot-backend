using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PartTypesRepository : RepositoryBase<PartTypes>, IPartTypesRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PartTypesRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePartTypes(PartTypes partTypes)
        {
            partTypes.CreatedBy = _createdBy;
            partTypes.CreatedOn = DateTime.Now;
            partTypes.Unit = _unitname;
            var result = await Create(partTypes);
            
            return result.Id;
        }

        public async Task<string> DeletePartTypes(PartTypes partTypes)
        {
            Delete(partTypes);
            string result = $"PartTypes details of {partTypes.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PartTypes>> GetAllActivePartTypes([FromQuery] SearchParames searchParams)
        {
            var partTypeDetails = FindAll()
          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PartTypeName.Contains(searchParams.SearchValue) ||
         inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return partTypeDetails;
        }

        public async Task<IEnumerable<PartTypes>> GetAllPartTypes([FromQuery] SearchParames searchParams)
        {
            var partTypeDetails = FindAll().OrderBy(x => x.Id)
          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PartTypeName.Contains(searchParams.SearchValue) ||
         inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return partTypeDetails;
        }

        public async Task<PartTypes> GetPartTypesById(int id)
        {
            var PartTypesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PartTypesbyId;
        }

        public async Task<string> UpdatePartTypes(PartTypes partTypes)
        {
            partTypes.LastModifiedBy = _createdBy;
            partTypes.LastModifiedOn = DateTime.Now;
            Update(partTypes);
            string result = $"PartType details of {partTypes.Id} is updated successfully!";
            return result;
        }
    }
}

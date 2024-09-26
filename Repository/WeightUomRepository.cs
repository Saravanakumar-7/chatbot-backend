using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Repository
{
    public class WeightUomRepository : RepositoryBase<WeightUom>, IWeightUomRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public WeightUomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateWeightUom(WeightUom weightUom)
        {
            weightUom.CreatedBy = _createdBy;
            weightUom.CreatedOn = DateTime.Now;
            weightUom.Unit = _unitname;
            var result = await Create(weightUom);
            
            return result.Id;
        }

        public async Task<string> DeleteWeightUom(WeightUom weightUom)
        {
            Delete(weightUom);
            string result = $"Weight Uom details of {weightUom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<WeightUom>> GetAllActiveWeightUom()
        {
            var AllActiveWeightUom= await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveWeightUom;
        }

        public async Task<IEnumerable<WeightUom>> GetAllWeightUom()
        {

            var GetallWeightUom = await FindAll().ToListAsync();

            return GetallWeightUom;
        }

        public async Task<WeightUom> GetWeightUomById(int id)
        {
            var WeightUombyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return WeightUombyId;
        }

        public async Task<string> UpdateWeightUom(WeightUom weightUom)
        {
            weightUom.LastModifiedBy = _createdBy;
            weightUom.LastModifiedOn = DateTime.Now;
            Update(weightUom);
            string result = $"Weight Uom of Detail {weightUom.Id} is updated successfully!";
            return result;
        }
    }
}

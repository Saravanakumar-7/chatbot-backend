using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class TypeOfHomeRepository : RepositoryBase<TypeOfHome>, ITypeOfHomeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public TypeOfHomeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<int?> CreateTypeOfHome(TypeOfHome typeOfHome)
        {
            typeOfHome.CreatedBy = _createdBy;
            typeOfHome.CreatedOn = DateTime.Now;
            typeOfHome.Unit = _unitname;
            var result = await Create(typeOfHome); return result.Id;
        }
        public async Task<string> DeleteTypeOfHome(TypeOfHome typeOfHome)
        {
            Delete(typeOfHome);
            string result = $"typeOfHome details of {typeOfHome.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<TypeOfHome>> GetAllActiveTypeOfHome()
        {
            var AllActivetypeOfHome = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivetypeOfHome;
        }
        public async Task<IEnumerable<TypeOfHome>> GetAllTypeOfHome()
        {
            var GetalltypeOfHome = await FindAll().ToListAsync(); return GetalltypeOfHome;
        }
        public async Task<TypeOfHome> GetTypeOfHomeById(int id)
        {
            var typeOfHomebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return typeOfHomebyId;
        }
        public async Task<string> UpdateTypeOfHome(TypeOfHome typeOfHome)
        {
            typeOfHome.LastModifiedBy = _createdBy;
            typeOfHome.LastModifiedOn = DateTime.Now;
            Update(typeOfHome);
            string result = $"typeOfHome details of {typeOfHome.Id} is updated successfully!";
            return result;
        }
    }
}
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class ArchitectureRepository : RepositoryBase<Architectures>, IArchitectureRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ArchitectureRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateArchitecture(Architectures architecture)
        {
            architecture.CreatedBy = _createdBy;
            architecture.CreatedOn = DateTime.Now;
            architecture.Unit = _unitname;
            var result = await Create(architecture); return result.Id;
        }
        public async Task<string> DeleteArchitecture(Architectures architecture)
        {
            Delete(architecture);
            string result = $"architecture details of {architecture.Id} is deleted successfully!";
            return result;
        }
        public async Task<PagedList<Architectures>> GetAllActiveArchitectures([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var getAllArchitectures = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ArchitectName.Contains(searchParams.SearchValue) ||
            inv.MobileNumber.Contains(searchParams.SearchValue) || inv.FirmName.Contains(searchParams.SearchValue))));
            return PagedList<Architectures>.ToPagedList(getAllArchitectures, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<Architectures>> GetAllArchitectures([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var getAllArchitectDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ArchitectName.Contains(searchParams.SearchValue) ||
                 inv.MobileNumber.Contains(searchParams.SearchValue) || inv.FirmName.Contains(searchParams.SearchValue))));

            return PagedList<Architectures>.ToPagedList(getAllArchitectDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<Architectures>> GetAllArchitecturesDetails()
        {
            var getallArchitectures = await FindAll().ToListAsync();
            return getallArchitectures;
        }

        public async Task<Architectures> GetArchitectureById(int id)
        {
            var architectById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); 
            return architectById;
        }
        public async Task<string> UpdateArchitecture(Architectures architecture)
        {
            architecture.LastModifiedBy = _createdBy;
            architecture.LastModifiedOn = DateTime.Now;
            Update(architecture);
            string result = $"architecture details of {architecture.Id} is updated successfully!";
            return result;
        }
    }
}
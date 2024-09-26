using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoleAccessRepository : RepositoryBase<RoleAccess>, IRoleAccessRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RoleAccessRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRoleAccess(RoleAccess roleAccess)
        {
            var result = await Create(roleAccess);

            return result.Id;
        }

        public async Task<string> DeleteRoleAccess(RoleAccess roleAccess)
        {
            Delete(roleAccess);
            string result = $"RoleAccess details of {roleAccess.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<RoleAccess>> GetAllActiveRoleAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var activeRoleAccessDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Menu.Contains(searchParams.SearchValue) ||
                 inv.FormName.Contains(searchParams.SearchValue))));

            return PagedList<RoleAccess>.ToPagedList(activeRoleAccessDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<RoleAccess>> GetAllRoleAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var roleAccessDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Menu.Contains(searchParams.SearchValue) ||
                 inv.FormName.Contains(searchParams.SearchValue))));

            return PagedList<RoleAccess>.ToPagedList(roleAccessDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RoleAccess> GetRoleAccessById(int id)
        {
            var rolesAccessById = await TipsMasterDbContext.RoleAccesses.Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return rolesAccessById;
        }

        public async Task<List<RoleAccess>> GetRoleAccessByRoleId(int roleId)
        {
            var rolesAccessByRoleId = await TipsMasterDbContext.RoleAccesses.Where(x => x.RoleId == roleId)
                 .ToListAsync();

            return rolesAccessByRoleId;
        }

        public async Task<string> UpdateRoleAccess(RoleAccess roleAccess)
        {
            roleAccess.LastModifiedBy = _createdBy;
            roleAccess.LastModifiedOn = DateTime.Now;
            Update(roleAccess);
            string result = $"RoleAccess of Detail {roleAccess.Id} is updated successfully!";
            return result;
        }
    }
}

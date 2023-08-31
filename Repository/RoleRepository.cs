using System;
using System.Collections.Generic;
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
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RoleRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRole(Role role)
        {
            role.CreatedBy = _createdBy;
            role.CreatedOn = DateTime.Now;
            role.Unit = _unitname;
            var result = await Create(role);

            return result.Id;
        }

        public async Task<string> DeleteRole(Role role)
        {
            Delete(role);
            string result = $"Roles details of {role.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Role>> GetAllActiveRoles([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var activeRolesDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.RoleName.Contains(searchParams.SearchValue) ||
                inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Role>.ToPagedList(activeRolesDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        //public async Task<PagedList<Role>> GetAllRoles([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{

        //    var rolesDetails = FindAll().OrderByDescending(x => x.Id)
        //     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.RoleName.Contains(searchParams.SearchValue) ||
        //        inv.Description.Contains(searchParams.SearchValue))));

        //    return PagedList<Role>.ToPagedList(rolesDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<Role>> GetAllRoles([FromQuery] SearchParames searchParams)
        {
            var rolesDetails = FindAll()
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
                               inv.RoleName.Contains(searchParams.SearchValue) ||
                               inv.Description.Contains(searchParams.SearchValue)))
                .OrderByDescending(x => x.Id);

            return rolesDetails.ToList();
        }

        public async Task<Role> GetRoleById(int id)
        {
            var rolesById = await TipsMasterDbContext.Roles.Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return rolesById;
        }

        public async Task<string> UpdateRole(Role role)
        {
            Update(role);
            string result = $"Roles of Detail {role.Id} is updated successfully!";
            return result;
        }
    }
}

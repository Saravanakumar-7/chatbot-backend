using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoleAccessRepository : RepositoryBase<RoleAccess>, IRoleAccessRepository
    {
        public RoleAccessRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
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

        public async Task<string> UpdateRoleAccess(RoleAccess roleAccess)
        {
            roleAccess.LastModifiedBy = "Admin";
            roleAccess.LastModifiedOn = DateTime.Now;
            Update(roleAccess);
            string result = $"RoleAccess of Detail {roleAccess.Id} is updated successfully!";
            return result;
        }
    }
}

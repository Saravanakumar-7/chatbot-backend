using System;
using System.Collections.Generic;
using System.IO;
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
    public class UserAccessRepository : RepositoryBase<UserAccess>, IUserAccessRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UserAccessRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateUserAccess(UserAccess userAccess)
        {
            var result = await Create(userAccess);

            return result.Id;
        }

        public async Task<string> DeleteUserAccess(UserAccess userAccess)
        {
            Delete(userAccess);
            string result = $"UserAccess details of {userAccess.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<UserAccess>> GetAllActiveUserAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var activeUserAccessDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Menu.Contains(searchParams.SearchValue) ||
                 inv.FormName.Contains(searchParams.SearchValue))));

            return PagedList<UserAccess>.ToPagedList(activeUserAccessDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<UserAccess>> GetAllUserAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var userAccessDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Menu.Contains(searchParams.SearchValue) ||
                inv.FormName.Contains(searchParams.SearchValue))));

            return PagedList<UserAccess>.ToPagedList(userAccessDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<UserAccess> GetUserAccessById(int id)
        {
            var userAccessById = await TipsMasterDbContext.UserAccesses.Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return userAccessById;
        }

        public async Task<List<UserAccess>> GetUserAccessByUserId(int userId)
        {
            var userAccessByUserId = await TipsMasterDbContext.UserAccesses.Where(x => x.UserId == userId)
                 .ToListAsync();

            return userAccessByUserId;
        }
        public async Task<List<RoleAccess>> GetUserRoleAccessByUserId(int roleId)
        {
            var rolesAccessByRoleId = await TipsMasterDbContext.RoleAccesses.Where(x => x.RoleId == roleId)
                 .ToListAsync();

            return rolesAccessByRoleId;
        }

        public async Task<string> UpdateUserAccess(UserAccess userAccess)
        {
            userAccess.LastModifiedBy = _createdBy;
            userAccess.LastModifiedOn = DateTime.Now;
            Update(userAccess);
            string result = $"UserAccess of Detail {userAccess.Id} is updated successfully!";
            return result;
        }
    }
}

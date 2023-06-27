using Contracts;
using Entities;
using Entities.Helper;
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
    public class LightningDesignerRepository : RepositoryBase<LightningDesigner>, ILightningDesignerRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LightningDesignerRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateLightningDesigner(LightningDesigner lightningDesigner)
        {
            lightningDesigner.CreatedBy = _createdBy;
            lightningDesigner.CreatedOn = DateTime.Now;
            lightningDesigner.Unit = _unitname;
            var result = await Create(lightningDesigner); return result.Id;
        }
        public async Task<string> DeleteLightningDesigner(LightningDesigner lightningDesigner)
        {
            Delete(lightningDesigner);
            string result = $"lightningDesigner details of {lightningDesigner.Id} is deleted successfully!";
            return result;
        }
        public async Task<PagedList<LightningDesigner>> GetAllActiveLightningDesigners([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var lightningDesignerDetails = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LightningDesignerName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue) || inv.EmailId.Contains(searchParams.SearchValue))));
            return PagedList<LightningDesigner>.ToPagedList(lightningDesignerDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<LightningDesigner>> GetAllLightningDesigners([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var lightningDesignerDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LightningDesignerName.Contains(searchParams.SearchValue) ||
                   inv.Description.Contains(searchParams.SearchValue) || inv.EmailId.Contains(searchParams.SearchValue))));

            return PagedList<LightningDesigner>.ToPagedList(lightningDesignerDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<LightningDesigner> GetLightningDesignerById(int id)
        {
            var LightningDesignerById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return LightningDesignerById;
        }
        public async Task<string> UpdateLightningDesigner(LightningDesigner lightningDesigner)
        {
            lightningDesigner.LastModifiedBy = _createdBy;
            lightningDesigner.LastModifiedOn = DateTime.Now;
            Update(lightningDesigner);
            string result = $"lightningDesigner details of {lightningDesigner.Id} is updated successfully!";
            return result;
        }
    }
}
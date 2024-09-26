using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tips.SalesService.Api.Repository
{
    public class MaterialTransactionNoteRepository : RepositoryBase<MaterialTransactionNote>, IMaterialTransactionNoteRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialTransactionNoteRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            mtn.CreatedBy = _createdBy;
            mtn.CreatedOn = DateTime.Now;
            mtn.Unit = _unitname;
            var result = await Create(mtn);
            return result.Id;
        }

        public async Task<string> DeleteMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            Delete(mtn);
            string result = $"MaterialTransactionNote details of {mtn.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialTransactionNote>> GetAllMaterialTransactionNote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var materialTransactionNotes = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.MTNNumber.Contains(searchParammes.SearchValue)
              || inv.ProjectNUmber.Contains(searchParammes.SearchValue))))
                          .Include(t => t.MaterialTransactionNoteItems);


            return PagedList<MaterialTransactionNote>.ToPagedList(materialTransactionNotes, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<MaterialTransactionNote> GetMaterialTransactionNoteById(int id)
        {
            var MtnById = await _tipsSalesServiceDbContext.MaterialTransactionNotes.Where(x => x.Id == id)
                               .Include(t => t.MaterialTransactionNoteItems).FirstOrDefaultAsync();
            return MtnById;
        }

        public async Task<string> UpdateMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            mtn.LastModifiedBy = _createdBy;
            mtn.LastModifiedOn = DateTime.Now;
            Update(mtn);
            string result = $"MaterialTransactionNote of Detail {mtn.Id} is updated successfully!";
            return result;
        }
    }
}
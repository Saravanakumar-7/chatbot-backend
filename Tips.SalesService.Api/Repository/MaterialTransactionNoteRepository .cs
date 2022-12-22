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

namespace Tips.SalesService.Api.Repository
{
    public class MaterialTransactionNoteRepository : RepositoryBase<MaterialTransactionNote>, IMaterialTransactionNoteRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public MaterialTransactionNoteRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            mtn.CreatedBy = "Admin";
            mtn.CreatedOn = DateTime.Now;
            mtn.Unit = "Bangalore";
            var result = await Create(mtn);
            return result.Id;
        }

        public async Task<string> DeleteMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            Delete(mtn);
            string result = $"MaterialTransactionNote details of {mtn.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialTransactionNote>> GetAllMaterialTransactionNote(PagingParameter pagingParameter)
        {
            var AllMtn = PagedList<MaterialTransactionNote>.ToPagedList(FindAll()
           .Include(t => t.MaterialTransactionNoteItems)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return AllMtn;
        }

        public async Task<MaterialTransactionNote> GetMaterialTransactionNoteById(int id)
        {
            var MtnId = await _tipsSalesServiceDbContext.materialTransactionNotes.Where(x => x.Id == id)
                               .Include(t => t.MaterialTransactionNoteItems).FirstOrDefaultAsync();
            return MtnId;
        }

        public async Task<string> UpdateMaterialTransactionNote(MaterialTransactionNote mtn)
        {
            mtn.LastModifiedBy = "Admin";
            mtn.LastModifiedOn = DateTime.Now;
            Update(mtn);
            string result = $"MaterialTransactionNote of Detail {mtn.Id} is updated successfully!";
            return result;
        }
    }
}
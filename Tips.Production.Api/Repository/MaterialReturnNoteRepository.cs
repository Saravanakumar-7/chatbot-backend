using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
//using Tips.Warehouse.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialReturnNoteRepository : RepositoryBase<MaterialReturnNote>, IMaterialReturnNoteRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public MaterialReturnNoteRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<int?> CreateMaterialReturnNote(MaterialReturnNote materialReturnNote)
        {
            materialReturnNote.CreatedBy = "Admin";
            materialReturnNote.CreatedOn = DateTime.Now;
            materialReturnNote.Unit = "Bangalore";
            var result = await Create(materialReturnNote);
            return result.Id;
        }

        public async Task<string> DeleteMaterialReturnNote(MaterialReturnNote materialReturnNote)
        {
            Delete(materialReturnNote);
            string result = $"MaterialReturnNote details of {materialReturnNote.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialReturnNote>> GetAllMaterialReturnNotes(PagingParameter pagingParameter)
        {
            var materialReturnNoteDetails = PagedList<MaterialReturnNote>.ToPagedList(FindAll()
                               .Include(x => x.MaterialReturnNoteItems)
              .OrderByDescending(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return materialReturnNoteDetails;
        }

        public async Task<MaterialReturnNote> GetMaterialReturnNoteById(int id)
        {
            var materialReturnNoteDetailById = await _tipsProductionDbContext.MaterialReturnNotes.Where(x => x.Id == id)
                              .Include(x => x.MaterialReturnNoteItems)

                              .FirstOrDefaultAsync();

            return materialReturnNoteDetailById;
        }

        public async Task<string> UpdateMaterialReturnNote(MaterialReturnNote materialReturnNote)
        {
            materialReturnNote.LastModifiedBy = "Admin";
            materialReturnNote.LastModifiedOn = DateTime.Now;
            Update(materialReturnNote);
            string result = $"MaterialReturnNote of Detail {materialReturnNote.Id} is updated successfully!";
            return result;
        }
    }
}

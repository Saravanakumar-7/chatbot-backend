using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
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

        public async Task<int?> GetMRNumberAutoIncrementCount(DateTime date)
        {
            var mRNumberAutoIncrementCount = _tipsProductionDbContext.MaterialReturnNotes.Where(x => x.CreatedOn == date.Date).Count();

            return mRNumberAutoIncrementCount;
        }

        public async Task<string> DeleteMaterialReturnNote(MaterialReturnNote materialReturnNote)
        {
            Delete(materialReturnNote);
            string result = $"MaterialReturnNote details of {materialReturnNote.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialReturnNoteIdNameList>> GetAllMaterialReturnNoteIdNameList()
        {
            IEnumerable<MaterialReturnNoteIdNameList> materialReturnNoteIdNameList = await _tipsProductionDbContext.MaterialReturnNotes
                                .Select(x => new MaterialReturnNoteIdNameList()
                                {
                                    Id = x.Id,

                                    MRNNumber = x.MRNNumber,

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return materialReturnNoteIdNameList;
        }

        public async Task<PagedList<MaterialReturnNote>> GetAllMaterialReturnNotes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            var materialReturnNoteDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProjectNumber.Contains(searchParams.SearchValue) ||
                   inv.MRNNumber.Contains(searchParams.SearchValue))) && inv.MrnStatus == MaterialStatus.open);

            return PagedList<MaterialReturnNote>.ToPagedList(materialReturnNoteDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<MaterialReturnNote> GetMaterialReturnNoteById(int id)
        {
            var materialReturnNoteDetailById = await _tipsProductionDbContext.MaterialReturnNotes.Where(x => x.Id == id)
                              .Include(x => x.MaterialReturnNoteItems)
                              .ThenInclude(s => s.MRNWarehouseList)

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
        public async Task<IEnumerable<MaterialReturnNote>> GetAllMaterialReturnNoteWithItems(MaterialReturnNoteSearchDto materialReturnNoteSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialReturnNotes.Include("MaterialReturnNoteItems");
                if (materialReturnNoteSearch != null || (materialReturnNoteSearch.ProjectNumber.Any())
               && materialReturnNoteSearch.ShopOrderNumber.Any() && materialReturnNoteSearch.FGShopOrderNumber.Any()
               && materialReturnNoteSearch.SAShopOrderNumber.Any())
                {
                    query = query.Where
                    (po => (materialReturnNoteSearch.ProjectNumber.Any() ? materialReturnNoteSearch.ProjectNumber.Contains(po.ProjectNumber) : true)
                   && (materialReturnNoteSearch.ShopOrderNumber.Any() ? materialReturnNoteSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true));
                    //&& (materialReturnNoteSearch.FGShopOrderNumber.Any() ? materialReturnNoteSearch.FGShopOrderNumber.Contains(po.FGShopOrderNumber) : true)
                    //  && (materialReturnNoteSearch.SAShopOrderNumber.Any() ? materialReturnNoteSearch.SAShopOrderNumber.Contains(po.SAShopOrderNumber) : true));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<MaterialReturnNote>> SearchMaterialReturnNote([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialReturnNotes.Include("MaterialReturnNoteItems");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.ProjectNumber.Contains(searchParammes.SearchValue)
                    || po.MRNNumber.Contains(searchParammes.SearchValue)
                    || po.ShopOrderNumber.Contains(searchParammes.SearchValue)
                    || po.MaterialReturnNoteItems.Any(s => s.PartNumber.Contains(searchParammes.SearchValue) ||
                    s.PartDescription.Contains(searchParammes.SearchValue)
                    || s.MftrPartNumber.Contains(searchParammes.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<MaterialReturnNote>> SearchMaterialReturnNoteDate([FromQuery] SearchDateparames searchDatesParams)
        {
            var materialReturnNoteDetails = _tipsProductionDbContext.MaterialReturnNotes
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.MaterialReturnNoteItems)
            .ToList();
            return materialReturnNoteDetails;
        }


    }
}

using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class MaterialRequestsRepository : RepositoryBase<MaterialRequests>, IMaterialRequestsRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        public MaterialRequestsRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<int?> CreateMaterialRequest(MaterialRequests request)
        {

            request.CreatedBy = "Admin";
            request.CreatedOn = DateTime.Now;
            request.Unit = "Bangalore";
            var result = await Create(request);
            return result.Id;
        }

        public async Task<string> DeleteMaterialRequest(MaterialRequests request)
        {
            Delete(request);
            string result = $"MaterialRequest details of {request.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialRequests>> GetAllMaterialRequest(PagingParameter pagingParameter, SearchParamess searchParammes)
        {
            var materialRequests = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.MRNumber.Contains(searchParammes.SearchValue)
              || inv.ProjectNumber.Contains(searchParammes.SearchValue))))
              .Include(t => t.MaterialRequestItems)
              .ThenInclude(v => v.MRStockDetail);



            return PagedList<MaterialRequests>.ToPagedList(materialRequests, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<MaterialRequestIdNoDto>> GetAllOpenMRIdNoList()
        {
            IEnumerable<MaterialRequestIdNoDto> MrNoDetails = await _tipsProductionDbContext.MaterialRequests
                                .Select(x => new MaterialRequestIdNoDto()
                                {
                                    Id = x.Id,
                                    MRNumber = x.MRNumber
                                })
                              .ToListAsync();

            return MrNoDetails;
        }

        public async Task<MaterialRequests> GetMaterialReqByMRNumber(string MRnumber)
        {
            var getMaterialReqbyMRNo = await _tipsProductionDbContext.MaterialRequests

            .Include(t => t.MaterialRequestItems)
            .ThenInclude(v => v.MRStockDetail).Where(x => x.MRNumber == MRnumber)
                    .FirstOrDefaultAsync();
            return getMaterialReqbyMRNo;
        }

        public async Task<MaterialRequests> GetMaterialRequestById(int id)
        {
            var getMRbyId = await _tipsProductionDbContext.MaterialRequests.Where(x => x.Id == id)
                             .Include(t => t.MaterialRequestItems)
                             .ThenInclude(v => v.MRStockDetail)
                             .FirstOrDefaultAsync();
            return getMRbyId;
        }

        public async Task<int?> GetMRNumberAutoIncrementCount(DateTime date)
        {
            var getMRNumberAutoIncrementCount = _tipsProductionDbContext.MaterialRequests.Where(x => x.CreatedOn == date.Date).Count();

            return getMRNumberAutoIncrementCount;
        }

        public async Task<string> UpdateMaterialRequest(MaterialRequests request)
        {
            request.LastModifiedBy = "Admin";
            request.LastModifiedOn = DateTime.Now;
            Update(request);
            string result = $"MaterialRequest of Detail {request.Id} is updated successfully!";
            return result;
        }
    }
}

using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class MaterialRequestRepository : RepositoryBase<MaterialRequest>, IMaterialRequestRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public MaterialRequestRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateMaterialRequest(MaterialRequest request)
        {
            request.CreatedBy = "Admin";
            request.CreatedOn = DateTime.Now;
            request.Unit = "Bangalore";
            var result = await Create(request);
            return result.Id;
        }
        public async Task<int?> GetMRNumberAutoIncrementCount(DateTime date)
        {
            var getMRNumberAutoIncrementCount = _tipsSalesServiceDbContext.MaterialRequests.Where(x => x.CreatedOn == date.Date).Count();

            return getMRNumberAutoIncrementCount;
        }

        public async Task<string> DeleteMaterialRequest(MaterialRequest request)
        {
            Delete(request);
            string result = $"MaterialRequest details of {request.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialRequest>> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var materialRequests = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.MRNumber.Contains(searchParammes.SearchValue) || inv.ProjectNumber.Contains(searchParammes.SearchValue))));

            return PagedList<MaterialRequest>.ToPagedList(materialRequests, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<MaterialRequest> GetMaterialRequestById(int id)
        {
            var getMRbyId = await _tipsSalesServiceDbContext.MaterialRequests.Where(x => x.Id == id)
                               .Include(t => t.MaterialRequestItems).FirstOrDefaultAsync();
            return getMRbyId;
        }

        public async Task<string> UpdateMaterialRequest(MaterialRequest request)
        {
            request.LastModifiedBy = "Admin";
            request.LastModifiedOn = DateTime.Now;
            Update(request);
            string result = $"MaterialRequest of Detail {request.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialRequestIdNoDto>> GetAllOpenMRIdNoList()
        {
            IEnumerable<MaterialRequestIdNoDto> MrNoDetails = await _tipsSalesServiceDbContext.MaterialRequests
                                .Select(x => new MaterialRequestIdNoDto()
                                {
                                    Id = x.Id,
                                    MRNumber = x.MRNumber
                                })
                              .ToListAsync();

            return MrNoDetails;
        }

        public async Task<MaterialRequest> GetMaterialReqByMRNumber(string MRnumber)
        {

            var getMaterialReqbyMRNo = await _tipsSalesServiceDbContext.MaterialRequests

            .Include(t => t.MaterialRequestItems).Where(x => x.MRNumber == MRnumber)
                    .FirstOrDefaultAsync();
            return getMaterialReqbyMRNo;
        }
    }
}
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
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

        public async Task<string> DeleteMaterialRequest(MaterialRequest request)
        {
            Delete(request);
            string result = $"MaterialRequest details of {request.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialRequest>> GetAllMaterialRequest(PagingParameter pagingParameter)
        {
            var AllMR = PagedList<MaterialRequest>.ToPagedList(FindAll()
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return AllMR;
        }

        public async Task<MaterialRequest> GetMaterialRequestById(int id)
        {
            var MatReqId = await _tipsSalesServiceDbContext.materialRequests.Where(x => x.Id == id)
                               .Include(t => t.MaterialRequestItemList).FirstOrDefaultAsync();
            return MatReqId;
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
            IEnumerable<MaterialRequestIdNoDto> MrNoDetails = await _tipsSalesServiceDbContext.materialRequests
                                .Select(x => new MaterialRequestIdNoDto()
                                {
                                    Id = x.Id,
                                    MRNo = x.MRNo
                                })
                              .ToListAsync();

            return MrNoDetails;
        }

        public async Task<MaterialRequest> GetMRNoDetailsById(string MRnumber)
        {

            var Mrnumber = await _tipsSalesServiceDbContext.materialRequests
            .Include(t => t.MaterialRequestItemList).Where(x => x.MRNo == MRnumber)
                    .FirstOrDefaultAsync();
            return Mrnumber;
        }
    }
}
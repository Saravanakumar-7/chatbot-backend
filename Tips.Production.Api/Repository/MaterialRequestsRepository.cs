using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Repository
{
    public class MaterialRequestsRepository : RepositoryBase<MaterialRequests>, IMaterialRequestsRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialRequestsRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateMaterialRequest(MaterialRequests request)
        {

            request.CreatedBy = _createdBy;
            request.CreatedOn = DateTime.Now;
            request.Unit = _unitname;
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
              || inv.ProjectNumber.Contains(searchParammes.SearchValue))));


            return PagedList<MaterialRequests>.ToPagedList(materialRequests, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<MaterialRequests>> GetAllMRStatusOpen(PagingParameter pagingParameter, SearchParamess searchParammes)
        {
            var materialRequestDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.MRNumber.Contains(searchParammes.SearchValue)
              || inv.ProjectNumber.Contains(searchParammes.SearchValue))) && inv.MrStatus == MaterialStatus.Open)
                .Include(s => s.MaterialRequestItems)
                .ThenInclude(m => m.MRStockDetails);


            return PagedList<MaterialRequests>.ToPagedList(materialRequestDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<MaterialRequests>> GetAllMRStatusClose()
        {
            var materialRequestDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(x => x.MrStatus == MaterialStatus.Closed)
                .Include(s => s.MaterialRequestItems)
                .ThenInclude(m => m.MRStockDetails)
                .ToList();

            return materialRequestDetails;
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
            .ThenInclude(v => v.MRStockDetails).Where(x => x.MRNumber == MRnumber)
                    .FirstOrDefaultAsync();
            return getMaterialReqbyMRNo;
        }
        public async Task<MaterialRequests> GetMaterialReqByShopOrderNumber(string ShopOrderNo)
        {
            var getMaterialReqbySONo = await _tipsProductionDbContext.MaterialRequests
          .Include(t => t.MaterialRequestItems)
          .ThenInclude(v => v.MRStockDetails)
          .Where(x => x.ShopOrderNumber == ShopOrderNo)
                  .FirstOrDefaultAsync();
            return getMaterialReqbySONo;
        }
        public async Task<MaterialRequests> GetMaterialRequestById(int id)
        {
            var getMRbyId = await _tipsProductionDbContext.MaterialRequests.Where(x => x.Id == id)
                             .Include(t => t.MaterialRequestItems)
                             .ThenInclude(v => v.MRStockDetails)
                             .FirstOrDefaultAsync();
            return getMRbyId;
        }

        public async Task<int?> GetMRNumberAutoIncrementCount(DateTime date)
        {
            var getMRNumberAutoIncrementCount = _tipsProductionDbContext.MaterialRequests.Where(x => x.CreatedOn == date.Date).Count();

            return getMRNumberAutoIncrementCount;
        }
        public async Task<string> GenerateMRNumberForAvision()
        {
            using var transaction = await _tipsProductionDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsProductionDbContext.MRNumbers.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsProductionDbContext.Update(rfqNumberEntity);
                await _tipsProductionDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                return $"ASPL|MIR|{currentYear:D2}{nextYear:D2}-{rfqNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateMRNumber()
        {
            using var transaction = await _tipsProductionDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var mrNumberEntity = await _tipsProductionDbContext.MRNumbers.SingleAsync();
                mrNumberEntity.CurrentValue += 1;
                _tipsProductionDbContext.Update(mrNumberEntity);
                await _tipsProductionDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"MR-{mrNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> UpdateMaterialRequest(MaterialRequests request)
        {
            request.LastModifiedBy = _createdBy;
            request.LastModifiedOn = DateTime.Now;
            Update(request);
            string result = $"MaterialRequest of Detail {request.Id} is updated successfully!";
            return result;
        } 
        public async Task<IEnumerable<MaterialRequests>> GetAllMaterialRequestsWithItems(MaterialRequestSearchDto materialRequestSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialRequests.Include("MaterialRequestItems");
                if (materialRequestSearch != null || (materialRequestSearch.MRNumber.Any())
                && materialRequestSearch.ShopOrderNumber.Any() && materialRequestSearch.FGShopOrderNumber.Any()
                && materialRequestSearch.ProjectNumber.Any() && materialRequestSearch.SAShopOrderNumber.Any())
                {
                    query = query.Where
                    (po => (materialRequestSearch.MRNumber.Any() ? materialRequestSearch.MRNumber.Contains(po.MRNumber) : true)
                    && (materialRequestSearch.ShopOrderNumber.Any() ? materialRequestSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true)
                   // && (materialRequestSearch.FGShopOrderNumber.Any() ? materialRequestSearch.FGShopOrderNumber.Contains(po.FGShopOrderNumber) : true)
                   //   && (materialRequestSearch.SAShopOrderNumber.Any() ? materialRequestSearch.SAShopOrderNumber.Contains(po.SAShopOrderNumber) : true)
                   && (materialRequestSearch.ProjectNumber.Any() ? materialRequestSearch.ProjectNumber.Contains(po.ProjectNumber) : true))
                    .Include(itm => itm.MaterialRequestItems)
            .ThenInclude(mr => mr.MRStockDetails);
                }
                return (IEnumerable<MaterialRequests>)query.ToList();
            }
        }
        public async Task<IEnumerable<MaterialRequests>> SearchMaterialRequests([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialRequests.Include("MaterialRequestItems");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.MRNumber.Contains(searchParammes.SearchValue)
                    || po.ProjectNumber.Contains(searchParammes.SearchValue)
                    || po.FGItemNumber.Contains(searchParammes.SearchValue)
                    || po.ShopOrderNumber.Contains(searchParammes.SearchValue)
                    || po.MaterialRequestItems.Any(s => s.PartNumber.Contains(searchParammes.SearchValue) ||
                    s.PartDescription.Contains(searchParammes.SearchValue)
                    || s.MftrPartNumber.Contains(searchParammes.SearchValue)))
                        .Include(itm => itm.MaterialRequestItems)
            .ThenInclude(mr => mr.MRStockDetails);
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<MaterialRequests>> SearchMaterialRequestsDate([FromQuery] SearchDateparames searchDatesParams)
        {
            var materialIssueDetails = _tipsProductionDbContext.MaterialRequests
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.MaterialRequestItems)
            .ThenInclude(mr => mr.MRStockDetails)
            .ToList();
            return materialIssueDetails;
        }

    }
}

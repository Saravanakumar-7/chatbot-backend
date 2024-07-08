using System.Linq;
using System.Security.Claims;
using Entities;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueRepository : RepositoryBase<MaterialIssue>, IMaterialIssueRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialIssueRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int> CreateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.CreatedBy = _createdBy;
            materialIssue.CreatedOn = DateTime.Now;
            materialIssue.Unit = _unitname;
            var result = await Create(materialIssue);
            return result.Id;
        }
        public async Task<IEnumerable<MaterialIssueSPReport>> MaterialIssueSPReport()
        {
            var results = _tipsProductionDbContext.Set<MaterialIssueSPReport>()
                        .FromSqlInterpolated($"CALL Material_Issue_Report")
                        .ToList();

            return results;
        }
        public async Task<PagedList<MaterialIssueSPReportForTrans>> GetMaterialIssueSPReportForTrans(PagingParameter pagingParameter)
        {
            var results = _tipsProductionDbContext.Set<MaterialIssueSPReportForTrans>()
                        .FromSqlInterpolated($"CALL Material_Issue_Report_tras")
                        .ToList();

            return PagedList<MaterialIssueSPReportForTrans>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<IEnumerable<MaterialIssueSPReport>> GetMaterialIssueSPReportWithParam(string? shopOrderNo, string? FGitemnumber, string? projectNo,
                                                                                                   string? salesOrderNo)
        {
            var result = _tipsProductionDbContext
            .Set<MaterialIssueSPReport>()
            .FromSqlInterpolated($"Material_Issue_Report_withparameter({shopOrderNo},{FGitemnumber},{projectNo},{salesOrderNo})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<MaterialIssueSPReportForTrans>> GetMaterialIssueSPReportWithParamForTrans(string? WorkorderNo, string? ItemNumber, string? projectNo,
                                                                                                  string? salesOrderNo)
        {
            var result = _tipsProductionDbContext
            .Set<MaterialIssueSPReportForTrans>()
            .FromSqlInterpolated($"Material_Issue_Report_withparameter_tras({WorkorderNo},{ItemNumber},{projectNo},{salesOrderNo})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<MaterialIssueSPReport>> GetMaterialIssueSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsProductionDbContext.Set<MaterialIssueSPReport>()
                      .FromSqlInterpolated($"Material_Issue_Report_withparameter_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<MaterialIssueSPReportForTrans>> GetMaterialIssueSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsProductionDbContext.Set<MaterialIssueSPReportForTrans>()
                      .FromSqlInterpolated($"Material_Issue_Report_withparameter_withdate_tras({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<string> DeleteMaterialIssue(MaterialIssue materialIssue)
        {
            Delete(materialIssue);
            string result = $"MaterialIssue details of {materialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialIssueIdNameList>> GetAllMaterialIssueIdNameList()
        {
            IEnumerable<MaterialIssueIdNameList> DeliveryOrderIddNameList = await _tipsProductionDbContext.MaterialIssue
                                .Select(x => new MaterialIssueIdNameList()
                                {
                                    Id = x.Id,

                                    ShopOrderNumber = x.ShopOrderNumber

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return DeliveryOrderIddNameList;
        }

        public async Task<PagedList<MaterialIssue>> GetAllMaterialIssues([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            PartType? check;
            if (Enum.TryParse<PartType>(searchParams.SearchValue, out PartType result))
            {
                check = result;
            }
            else
            {
                check = null;
            }
            int searchValueAsInt;
            bool isSearchValueNumeric = int.TryParse(searchParams.SearchValue, out searchValueAsInt);
            var materialIssueDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue))
                || (string.IsNullOrEmpty(searchParams.SearchValue)
                || inv.ShopOrderNumber.Contains(searchParams.SearchValue)
                || inv.ItemNumber.Contains(searchParams.SearchValue)
                || (isSearchValueNumeric && inv.ShopOrderQty.Equals(searchValueAsInt))
                || inv.ItemType.Equals(check)
                ));

            return PagedList<MaterialIssue>.ToPagedList(materialIssueDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

           
        }


        public async Task<MaterialIssue> GetMaterialIssueById(int id)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.Id == id)                           
                                    .Include(m=> m.materialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public async Task<MaterialIssue> GetMaterialIssueByShopOrderNo(string shopOrderNo)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.ShopOrderNumber == shopOrderNo)
                                    .Include(m => m.materialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public  async Task<string> UpdateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.LastModifiedBy = _createdBy;
            materialIssue.LastModifiedOn = DateTime.Now;
            Update(materialIssue);
            string result = $"MaterialIssue of Detail {materialIssue.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialIssue>> GetAllMaterialIssueWithItems(MaterialIssueSearchDto materialIssueSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialIssue.Include("materialIssueItems")/*.Include("FGShopOrderMaterialIssues").Include("SAShopOrderMaterialIssues")*/;
                if (materialIssueSearch != null ||/* (materialIssueSearch.ItemType.Any())*/
               /*&&*/ materialIssueSearch.ShopOrderNumber.Any() /*&& materialIssueSearch.FGShopOrderNumber.Any()*/
              /* && materialIssueSearch.SAShopOrderNumber.Any()*/ && materialIssueSearch.ItemNumber.Any() /*&&*/ /*materialIssueSearch.FGItemNumber.Any()*/
              /* && materialIssueSearch.SAItemNumber.Any()*/)
                {
                    query = query.Where
                    (po => /*(materialIssueSearch.ItemType.Any() ? Enum.GetName(typeof(PartTypes), po.ItemType).Contains(po.ItemType.ToString()) : true)*/
                   /*&& */(materialIssueSearch.ShopOrderNumber.Any() ? materialIssueSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true)
                   //  && (materialIssueSearch.FGShopOrderNumber.Any() ? materialIssueSearch.FGShopOrderNumber.Contains(po.FGShopOrderNumber) : true)
                   //&& (materialIssueSearch.SAShopOrderNumber.Any() ? materialIssueSearch.SAShopOrderNumber.Contains(po.SAShopOrderNumber) : true)
                   && (materialIssueSearch.ItemNumber.Any() ? materialIssueSearch.ItemNumber.Contains(po.ItemNumber) : true));
                    // && (materialIssueSearch.FGItemNumber.Any() ? materialIssueSearch.FGItemNumber.Contains(po.FGItemNumber) : true)
                    //  && (materialIssueSearch.SAItemNumber.Any() ? materialIssueSearch.SAItemNumber.Contains(po.SAItemNumber) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<MaterialIssue>> SearchMaterialIssue([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsProductionDbContext)
            {
                 var query = _tipsProductionDbContext.MaterialIssue.Include("materialIssueItems");
                //var query = _tipsProductionDbContext.MaterialIssue.Include("FGShopOrderMaterialIssues").Include("SAShopOrderMaterialIssues");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    //query = query.Where(po => po.ItemType.ToString().Contains(searchParammes.SearchValue)
                    //|| po.ItemNumber.Contains(searchParammes.SearchValue)
                    //|| po.ShopOrderQty.Equals(int.Parse(searchParammes.SearchValue))
                    //|| po.materialIssueItems.Any(s => s.PartNumber.Contains(searchParammes.SearchValue) ||
                    //s.Description.Contains(searchParammes.SearchValue)
                    //|| s.ProjectNumber.Contains(searchParammes.SearchValue)
                    //|| s.Description.Contains(searchParammes.SearchValue)));

                    //string searchValueString = Convert.ToString(searchParammes.SearchValue);

                    //// Perform the search
                    //query = query.Where(po => (
                    //    (po.ItemType == PartType.SA || po.ItemNumber.Contains(searchValueString)) ||
                    //    (po.ItemType == PartType.FG || po.ItemNumber.Contains(searchValueString)) ||
                    //    (po.materialIssueItems.Any(s =>
                    //        (s.PartNumber.Contains(searchValueString) ||
                    //         s.Description.Contains(searchValueString) ||
                    //         s.ProjectNumber.Contains(searchValueString) ||
                    //         s.Description.Contains(searchValueString)) &&
                    //        (po.ItemType == PartType.SA || po.ItemType == PartType.FG)
                    //    )
                    //)));

                    string searchValueString = Convert.ToString(searchParammes.SearchValue);

                    // Perform the search for the searchValueString in ItemNumber or MaterialIssueItem properties,
                    // and also check for the string value of each ItemType enum

                    //   query = query.Where(m => m.ItemNumber.Contains(searchParammes.SearchValue) ||
                    //(m.ItemType == PartType.FG && searchParammes.SearchValue == "FG") ||
                    //(m.ItemType == PartType.PurchasePart && searchParammes.SearchValue == "PurchasePart") ||
                    //(m.ItemType == PartType.TG && searchParammes.SearchValue == "TG") ||
                    //(m.ItemType == PartType.SA && searchParammes.SearchValue == "SA") ||
                    //(m.ItemType == PartType.FRU && searchParammes.SearchValue == "FRU") ||
                    //(m.ItemType == PartType.Phantom && searchParammes.SearchValue == "Phantom") ||
                    //m.materialIssueItems
                    //    .Any(m0 => m0.PartNumber.Contains(searchParammes.SearchValue) ||
                    //               m0.Description.Contains(searchParammes.SearchValue) ||
                    //               m0.ProjectNumber.Contains(searchParammes.SearchValue) ||
                    //               m0.Description.Contains(searchParammes.SearchValue)))
                    //       .ToListAsync();

                    //string searchValueString = searchParammes.SearchValue.ToString();
                    query = query.Where(po => /*Enum.GetName(typeof(PartTypes), po.ItemType).Contains(searchValueString)*/
                                    // po.ItemType.Contains(searchValueString)
                                    /*||*/ po.ItemNumber.Contains(searchParammes.SearchValue)
                                    || po.ShopOrderQty.ToString().Contains(searchParammes.SearchValue)
                                    || po.ShopOrderNumber.Contains(searchParammes.SearchValue)
                                    || po.materialIssueItems.Any(s => s.PartNumber.Contains(searchParammes.SearchValue) ||
                                    s.Description.Contains(searchParammes.SearchValue)
                                    || s.ProjectNumber.Contains(searchParammes.SearchValue)
                                    || s.Description.Contains(searchParammes.SearchValue)));

                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<MaterialIssue>> SearchMaterialIssueDate([FromQuery] SearchDateparames searchDatesParams)
        {
            var materialIssueDetails = _tipsProductionDbContext.MaterialIssue
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.materialIssueItems)
            .ToList();
            return materialIssueDetails;
        }

        public async Task<IEnumerable<PickList>> PickListProductionSPReport(string? ShopOrderNumber)
        {
            var result = _tipsProductionDbContext
          .Set<PickList>()
          .FromSqlInterpolated($"CALL Pick_list_materialissue3({ShopOrderNumber})")
          .ToList();

            return result;
        }
    }
}

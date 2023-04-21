using Entities;
using Entities.Helper;
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
        public MaterialIssueRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<int> CreateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.CreatedBy = "Admin";
            materialIssue.CreatedOn = DateTime.Now;
            materialIssue.Unit = "Bangalore";
            var result = await Create(materialIssue);
            return result.Id;
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
            var materialIssueDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShopOrderNumber.Contains(searchParams.SearchValue) ||
                   inv.ItemNumber.Contains(searchParams.SearchValue) || inv.ShopOrderQty.Equals(int.Parse(searchParams.SearchValue)))));

            return PagedList<MaterialIssue>.ToPagedList(materialIssueDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<MaterialIssue> GetMaterialIssueById(int id)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.Id == id)                           
                                    .Include(m=> m.MaterialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public async Task<MaterialIssue> GetMaterialIssueByShopOrderNo(string shopOrderNo)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.ShopOrderNumber == shopOrderNo)
                                    .Include(m => m.MaterialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public  async Task<string> UpdateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.LastModifiedBy = "Admin";
            materialIssue.LastModifiedOn = DateTime.Now;
            Update(materialIssue);
            string result = $"MaterialIssue of Detail {materialIssue.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialIssue>> GetAllMaterialIssueWithItems(MaterialIssueSearchDto materialIssueSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.MaterialIssue.Include("FGShopOrderMaterialIssues").Include("SAShopOrderMaterialIssues");
                if (materialIssueSearch != null || (materialIssueSearch.ItemType.Any())
               && materialIssueSearch.ShopOrderNumber.Any() && materialIssueSearch.FGShopOrderNumber.Any()
               && materialIssueSearch.SAShopOrderNumber.Any() && materialIssueSearch.ItemNumber.Any() && materialIssueSearch.FGItemNumber.Any()
               && materialIssueSearch.SAItemNumber.Any())
                {
                    query = query.Where
                    (po => (materialIssueSearch.ItemType.Any() ? materialIssueSearch.ItemType.Contains(po.ItemType.ToString()) : true)
                   && (materialIssueSearch.ShopOrderNumber.Any() ? materialIssueSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true)
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
                 var query = _tipsProductionDbContext.MaterialIssue.Include("MaterialIssueItems");
                //var query = _tipsProductionDbContext.MaterialIssue.Include("FGShopOrderMaterialIssues").Include("SAShopOrderMaterialIssues");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    //query = query.Where(po => po.ItemType.ToString().Contains(searchParammes.SearchValue)
                    //|| po.ItemNumber.Contains(searchParammes.SearchValue)
                    //|| po.ShopOrderQty.Equals(int.Parse(searchParammes.SearchValue))
                    //|| po.MaterialIssueItems.Any(s => s.PartNumber.Contains(searchParammes.SearchValue) ||
                    //s.Description.Contains(searchParammes.SearchValue)
                    //|| s.ProjectNumber.Contains(searchParammes.SearchValue)
                    //|| s.Description.Contains(searchParammes.SearchValue)));
                    string searchValueString = searchParammes.SearchValue.ToString();
                    query = query.Where(po => Enum.GetName(typeof(PartTypes), po.ItemType).Contains(searchValueString)
                                    || po.ItemNumber.Contains(searchValueString)
                                    || po.ShopOrderQty.Equals(int.Parse(searchValueString))
                                    || po.MaterialIssueItems.Any(s => s.PartNumber.Contains(searchValueString) ||
                                    s.Description.Contains(searchValueString)
                                    || s.ProjectNumber.Contains(searchValueString)
                                    || s.Description.Contains(searchValueString)));

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
            .Include(itm => itm.MaterialIssueItems)
            .ToList();
            return materialIssueDetails;
        }

    }
}

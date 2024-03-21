using Contracts;
using Entities;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class OQCRepository : RepositoryBase<OQC>, IOQCRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OQCRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateOQC(OQC oqc)
        {

            oqc.CreatedBy = _createdBy;
            oqc.CreatedOn = DateTime.Now;
            oqc.Unit = _unitname;
            var result = await Create(oqc);

            return result.Id;
        }

        public async Task<string> DeleteOQC(OQC oqc)
        {
            Delete(oqc);
            string result = $"OQC details of {oqc.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<OQC>> GetAllOQC([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            var OQCDetails = FindAll().OrderByDescending(x => x.Id)
                           .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || inv.ShopOrderNumber.Contains(searchParamess.SearchValue) ||
                              inv.ItemNumber.Contains(searchParamess.SearchValue))));

            return PagedList<OQC>.ToPagedList(OQCDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<IEnumerable<OQC>> SearchOQC([FromQuery] SearchParamess searchParames)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.oQCs.AsQueryable();
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.ShopOrderNumber.Contains(searchParames.SearchValue)
                    || po.PendingQty.Equals(searchParames.SearchValue)
                    || po.ShopOrderQty.Equals(decimal.Parse(searchParames.SearchValue)));
                }
                return query.ToList();
                //return null;
            }
        }
       
        public async Task<IEnumerable<OQC>> GetAllOQCWithItems(OQCSearchDto oQCSearch)
        {
            using (var context = _tipsProductionDbContext)
            {
                var query = _tipsProductionDbContext.oQCs.AsQueryable();
                if (oQCSearch != null || /*(oQCSearch.FGItemNumber.Any())*/
               /*&&*/ oQCSearch.ShopOrderNumber.Any()/* && oQCSearch.SAItemNumber.Any()*/
               && oQCSearch.PendingQty.Any() && oQCSearch.ShopOrderQty.Any())
                {
                    query = query.Where
                   (po => (oQCSearch.ShopOrderNumber.Any() ? oQCSearch.ShopOrderNumber.Contains(po.ShopOrderNumber) : true)
                   //&& (oQCSearch.SAItemNumber.Any() ? oQCSearch.SAItemNumber.Contains(po.ItemNumber) : true)
                   //&& (oQCSearch.FGItemNumber.Any() ? oQCSearch.FGItemNumber.Contains(po.ItemNumber) : true)
                   && (oQCSearch.PendingQty.Any() ? oQCSearch.PendingQty.Contains(po.PendingQty) : true)
                   && (oQCSearch.ShopOrderQty.Any() ? oQCSearch.ShopOrderQty.Contains(po.ShopOrderQty) : true));
                }
                return await query.ToListAsync();

            }
        }

        public Task<IEnumerable<OQC>> SearchOQCDate([FromQuery] SearchDateparames searchsDateParms)
        {
            var oQcDetails = _tipsProductionDbContext.oQCs
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .ToList();
            return Task.FromResult<IEnumerable<OQC>>(oQcDetails);
        }

        public async Task<IEnumerable<OQCIdNameList>> GetAllOQCIdNameList()
        {
            IEnumerable<OQCIdNameList> btoIddNameList = await _tipsProductionDbContext.oQCs
                               .Select(x => new OQCIdNameList()
                               {
                                   Id = x.Id,

                                   ShopOrderNumber = x.ShopOrderNumber

                               })
                               .OrderByDescending(x => x.Id)
                             .ToListAsync();

            return btoIddNameList;
        }
        public async Task<decimal?> GetOQCAcceptedQty(string Itemnumber, string ShopOrderNumber)
        {
            var AcceptedQty = await _tipsProductionDbContext.oQCs.Where(x => x.ItemNumber == Itemnumber && x.ShopOrderNumber == ShopOrderNumber).SumAsync(x=>x.AcceptedQty);
            return AcceptedQty;
        }
        public async Task<OQC> GetOQCById(int id)
        {
            var oQCDedtailsById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return oQCDedtailsById;
        }

        public async Task<IEnumerable<ShopOrderConfirmationDetailsDto>> GetShopOrderConfirmationDetailsByItemNo(string itemNumber)
        {

            var shopOrderConfirmationDetails = await _tipsProductionDbContext.ShopOrderConfirmations
                .Where(x => x.ItemNumber == itemNumber)
                .Select(s => new ShopOrderConfirmationDetailsDto()
                {
                    ShopOrderNumber = s.ShopOrderNumber,
                    ShopOrderQty = s.ShopOrderReleaseQty,
                    PendingQty = s.ShopOrderReleaseQty,

                })
                .ToListAsync();

            return shopOrderConfirmationDetails;
        }

        public async Task<IEnumerable<ShopOrderConfirmationItemNoListDto>> GetShopOrderConfirmationItemNoByFGItemType()
        {
            var shopOrderConfirmationItmNoList = await _tipsProductionDbContext.ShopOrderConfirmations
               .Where(x => x.ItemType == PartType.FG && x.IsOQCDone == false && x.IsDeleted == false)
               .Select(s => new ShopOrderConfirmationItemNoListDto()
               {
                   ItemNumber = s.ItemNumber,
                   Description = s.Description
               })
               .ToListAsync();

            return shopOrderConfirmationItmNoList;
        }

        public async Task<IEnumerable<ShopOrderConfirmationItemNoListDto>> GetShopOrderConfirmationItemNoBySAItemType()
        {
            var shopOrderConfirmationItmNoList = await _tipsProductionDbContext.ShopOrderConfirmations
                .Where(x => x.ItemType == PartType.SA && x.IsOQCDone == false && x.IsDeleted == false)
                .Select(s => new ShopOrderConfirmationItemNoListDto()
                {
                    ItemNumber = s.ItemNumber,
                    Description = s.Description
                })
                .ToListAsync();

            return shopOrderConfirmationItmNoList;
        }

        public async Task<string> UpdateOQC(OQC oqc)
        {
            oqc.LastModifiedBy = _createdBy;
            oqc.LastModifiedOn = DateTime.Now;
            Update(oqc);
            string result = $"OQC details of {oqc.Id} is updated successfully!";
            return result;
        }

    }
}

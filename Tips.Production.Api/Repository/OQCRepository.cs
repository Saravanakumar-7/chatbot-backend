using Contracts;
using Entities;
using Entities.Enums;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class OQCRepository : RepositoryBase<OQC>, IOQCRepository
    {
        public OQCRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateOQC(OQC oqc)
        {

            oqc.CreatedBy = "Admin";
            oqc.CreatedOn = DateTime.Now;
            oqc.Unit = "Bangalore";
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
                              inv.ItemNumber.Contains(searchParamess.SearchValue) || inv.ShopOrderQty.Equals(int.Parse(searchParamess.SearchValue)) ||
                              inv.ItemType.Equals(int.Parse(searchParamess.SearchValue)))));

            return PagedList<OQC>.ToPagedList(OQCDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
                   Description = s.ItemDescription
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
                    Description = s.ItemDescription
                })
                .ToListAsync();

            return shopOrderConfirmationItmNoList;
        }

        public async Task<string> UpdateOQC(OQC oqc)
        {
            oqc.LastModifiedBy = "Admin";
            oqc.LastModifiedOn = DateTime.Now;
            Update(oqc);
            string result = $"OQC details of {oqc.Id} is updated successfully!";
            return result;
        }

    }
}

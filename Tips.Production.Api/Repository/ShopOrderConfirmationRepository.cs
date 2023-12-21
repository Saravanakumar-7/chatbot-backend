using Entities;
using Entities.Enums;
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
    public class ShopOrderConfirmationRepository : RepositoryBase<ShopOrderConfirmation>, IShopOrderConfirmationRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public ShopOrderConfirmationRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<long> CreateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation)
        {
        
            shopOrderConfirmation.CreatedBy = _createdBy;
            shopOrderConfirmation.CreatedOn = DateTime.Now;
            shopOrderConfirmation.Unit = _unitname;
            var result = await Create(shopOrderConfirmation);
            return result.Id;
        }

        public async Task<PagedList<ShopOrderConfirmation>> GetAllShopOrderConfirmation([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
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
            decimal searchValueAsInt;
            bool isSearchValueNumeric = decimal.TryParse(searchParams.SearchValue, out searchValueAsInt);
            var shopOrderCOnfirmationDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
               || inv.ShopOrderNumber.Contains(searchParams.SearchValue)
               || inv.ItemNumber.Contains(searchParams.SearchValue)
               || inv.ItemType.Equals(check)
               || (isSearchValueNumeric && inv.ShopOrderReleaseQty == searchValueAsInt)
               || (isSearchValueNumeric && inv.WipConfirmedQty == searchValueAsInt)
               )));

            return PagedList<ShopOrderConfirmation>.ToPagedList(shopOrderCOnfirmationDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //var shopOrderCOnfirmationDetails = FindAll().OrderByDescending(x => x.Id)
            //   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShopOrderNumber.Contains(searchParams.SearchValue) ||
            //       inv.ItemType.Equals(int.Parse(searchParams.SearchValue))
            //       || inv.ShopOrderReleaseQty.Equals(int.Parse(searchParams.SearchValue))
            //       || inv.WipConfirmedQty.Equals(int.Parse(searchParams.SearchValue)))));

            //return PagedList<ShopOrderConfirmation>.ToPagedList(shopOrderCOnfirmationDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        } 


        public async Task<ShopOrderConfirmation> GetShopOrderConfirmationById(int id)
        {
            var shopOrderConfirmationDetailById = await 
                            FindByCondition(x => x.Id == id)
                             .FirstOrDefaultAsync();
            return shopOrderConfirmationDetailById;
        }

        public async Task<string> UpdateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation)
        {
            shopOrderConfirmation.LastModifiedBy = _createdBy;
            shopOrderConfirmation.LastModifiedOn = DateTime.Now;
            Update(shopOrderConfirmation);
            string result = $"ShopOrderConfirmation details of {shopOrderConfirmation.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo)
        {
            var shopOrderConfirmationByShopOrderNoList = await FindByCondition(x => x.ShopOrderNumber ==shopOrderNo).ToListAsync();                
            return shopOrderConfirmationByShopOrderNoList;

        }
        
        public async Task<IEnumerable<ShopOrderConfirmation>> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            var openDataForOqcByShopOrderNoList = await FindByCondition(x => x.ShopOrderNumber == shopOrderNo &&  x.IsOQCDone == false).ToListAsync();           
            return openDataForOqcByShopOrderNoList;

        }

        public async Task<IEnumerable<ShopOrderItemNoListDto>> GetShopOrderItemNoByFGItemType()
        {
            var shopOrderItmNoList =await _tipsProductionDbContext.ShopOrders
                .Where(x => x.ItemType == PartType.FG && x.FGDoneStatus != OrderStatus.Closed && x.Status != OrderStatus.Closed && x.IsShortClosed == false)
                .Select(s => new ShopOrderItemNoListDto()
                 {
                    ItemNumber = s.ItemNumber,
                    Description = s.Description
                })
                .ToListAsync();

            return shopOrderItmNoList;
        }

        public async Task<IEnumerable<ShopOrderItemNoListDto>> GetShopOrderItemNoBySAItemType()
        {
            var shopOrderItmNoList = await _tipsProductionDbContext.ShopOrders
                .Where(x => x.ItemType == PartType.SA && x.FGDoneStatus != OrderStatus.Closed && x.Status != OrderStatus.Closed && x.IsShortClosed == false)
                .Select(s => new ShopOrderItemNoListDto()
                {
                    ItemNumber = s.ItemNumber,
                    Description = s.Description
                })
                .ToListAsync();

            return shopOrderItmNoList;
        }

        public async Task<IEnumerable<ShopOrderDetailsDto>> GetShopOrderDetailsByItemNo(string itemNumber)
        {
            var shopOrderDetails = await _tipsProductionDbContext.ShopOrders
                .Where(x => x.ItemNumber == itemNumber)
                .Select(s => new ShopOrderDetailsDto()
                {
                    ShopOrderNumber = s.ShopOrderNumber,
                    ShopOrderReleaseQty = s.TotalSOReleaseQty,
                    WipQty = s.WipQty,
                    OqcQty = s.OqcQty,
                    BOMVersion=s.BomRevisionNo
                })
                .ToListAsync();

            return shopOrderDetails;
        }
    }
    
}

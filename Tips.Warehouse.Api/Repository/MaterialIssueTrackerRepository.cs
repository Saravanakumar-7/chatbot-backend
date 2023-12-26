using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class MaterialIssueTrackerRepository : RepositoryBase<ShopOrderMaterialIssueTracker>,IMaterialIssueTrackerRepository
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public MaterialIssueTrackerRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor, MySqlConnection connection) : base(repositoryContext)
        {
            _connection = connection;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<long?> CreateMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker)
        {
            var date = DateTime.Now;
            shopOrderMaterialIssueTracker.CreatedBy = _createdBy;
            shopOrderMaterialIssueTracker.CreatedOn = date.Date;  
            var result = await Create(shopOrderMaterialIssueTracker);
            return result.Id;
        }
        public async Task<int> AddDataToMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue)
        {
            shopOrderMaterialIssue.CreatedBy = _createdBy;
            shopOrderMaterialIssue.CreatedOn = DateTime.Now;
            shopOrderMaterialIssue.Unit = _unitname;
            var result = await Create(shopOrderMaterialIssue);

            return result.Id;
        }
        public async Task<string> UpdateMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue)
        { 
            Update(shopOrderMaterialIssue);
            string result = $"materialIssue of Detail {shopOrderMaterialIssue.Id} is updated successfully!";
            return result;
        }
        public async Task<List<ShopOrderMaterialIssueTrackerDto>> SOMaterialIssueTrackerDetailsByShopOrderNo(string ShopOrderNo)
        {
            var resultList = (from st in _tipsWarehouseDbContext.ShopOrderMaterialIssueTrackers
                              where st.ShopOrderNumber == ShopOrderNo
                              group st by new { st.PartNumber,st.MRNumber, st.Description,st.Bomversion, st.ShopOrderNumber, st.DataFrom } into g
                              select new ShopOrderMaterialIssueTrackerDto
                              {
                                  PartNumber = g.Key.PartNumber,
                                  MRNumber = g.Key.MRNumber,
                                  Description = g.Key.Description,
                                  ShopOrderNumber = g.Key.ShopOrderNumber,
                                  DataFrom = g.Key.DataFrom,
                                  Bomversion = g.Key.Bomversion,
                                  IssuedQty = g.Sum(st => st.IssuedQty),
                                  ConvertedToFgQty = g.Sum(st => st.ConvertedToFgQty),
                                  BalanceQty = (g.Sum(st => st.IssuedQty) - g.Sum(st => st.ConvertedToFgQty))
                              }).ToList();

            return resultList;
        }

            public async Task<List<ShopOrderMaterialIssueTracker>> GetDetailsByShopOrderNOItemNoLotNo(string PartNumber, string ShopOrderNumber, string LotNumber)
        {
            var shopOrderMaterialIssueTracker = await _tipsWarehouseDbContext.ShopOrderMaterialIssueTrackers
                .Where(x => x.PartNumber == PartNumber && x.ShopOrderNumber == ShopOrderNumber && x.LotNumber == LotNumber 
                    && x.IssuedQty > x.ConvertedToFgQty)
                          .ToListAsync();

            return shopOrderMaterialIssueTracker;
        }

        public async Task<List<MRNIssueTrackerDto>> GetWipQtyFromMaterialIssueTracker(string shopOrderNo, string partNumber,decimal returnedQty)
        {
            List<MRNIssueTrackerDto> mRNIssueTrackerDtos = new List<MRNIssueTrackerDto>();
            var shopOrderMaterialIssueTracker = await _tipsWarehouseDbContext.ShopOrderMaterialIssueTrackers
                .Where(x => x.PartNumber == partNumber && x.ShopOrderNumber == shopOrderNo && (x.IssuedQty-x.ConvertedToFgQty) > 0).ToListAsync();
            // Sort the records by CreatedOn date in descending order (LIFO).
            shopOrderMaterialIssueTracker.Sort((x, y) => y.CreatedOn?.CompareTo(x.CreatedOn) ?? 1);

            // Iterate through the records and add the lot number and quantity to the list.
            for (int i = 0; i < shopOrderMaterialIssueTracker.Count; i++)
            {
                var record = shopOrderMaterialIssueTracker[i];
                decimal wipQty = record.IssuedQty - record.ConvertedToFgQty;
                // Only add the record to the list if the quantity is greater than or equal to the given quantity.
                if (wipQty >= returnedQty)
                {
                    MRNIssueTrackerDto mRNIssueTrackerDto = new MRNIssueTrackerDto
                    {
                        PartNumber = partNumber,
                        WipQty = returnedQty,
                        LotNumber = record.LotNumber
                        
                    };
                    mRNIssueTrackerDtos.Add(mRNIssueTrackerDto);
                    shopOrderMaterialIssueTracker[i].IssuedQty -= returnedQty;
                    returnedQty = 0;                    
                }
                else
                {
                    MRNIssueTrackerDto mRNIssueTrackerDto = new MRNIssueTrackerDto
                    {
                        PartNumber = partNumber,
                        WipQty = wipQty,
                        LotNumber = record.LotNumber
                    };
                    mRNIssueTrackerDtos.Add(mRNIssueTrackerDto);
                    shopOrderMaterialIssueTracker[i].IssuedQty = 0;
                    returnedQty -= wipQty;
                }

                if(returnedQty <= 0)
                {
                    break;
                }
                _tipsWarehouseDbContext.Update(shopOrderMaterialIssueTracker[i]);
            }
            _tipsWarehouseDbContext.SaveChangesAsync();
            return mRNIssueTrackerDtos;
        }
    }
}

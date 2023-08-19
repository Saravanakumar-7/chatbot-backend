using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class MaterialIssueTrackerRepository : RepositoryBase<ShopOrderMaterialIssueTracker>,IMaterialIssueTrackerRepository
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        public MaterialIssueTrackerRepository(TipsWarehouseDbContext repositoryContext, MySqlConnection connection) : base(repositoryContext)
        {
            _connection = connection;
        }
        public async Task<long?> CreateMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker)
        {
            var date = DateTime.Now;
            shopOrderMaterialIssueTracker.CreatedBy = "Admin";
            shopOrderMaterialIssueTracker.CreatedOn = date.Date;  
            var result = await Create(shopOrderMaterialIssueTracker);
            return result.Id;
        }
        public async Task<int> AddDataToMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue)
        {
            shopOrderMaterialIssue.CreatedBy = "Admin";
            shopOrderMaterialIssue.CreatedOn = DateTime.Now;
            shopOrderMaterialIssue.Unit = "Bangalore";
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
                              group st by new { st.PartNumber, st.Description,st.Bomversion, st.ShopOrderNumber, st.DataFrom } into g
                              select new ShopOrderMaterialIssueTrackerDto
                              {
                                  PartNumber = g.Key.PartNumber,
                                  Description = g.Key.Description,
                                  ShopOrderNumber = g.Key.ShopOrderNumber,
                                  DataFrom = g.Key.DataFrom,
                                  Bomversion = g.Key.Bomversion,
                                  IssuedQty = g.Sum(st => st.IssuedQty),
                                  ConvertedToFgQty = g.Sum(st => st.ConvertedToFgQty),
                                  BalanceQty = g.Sum(st => st.IssuedQty) - g.Sum(st => st.ConvertedToFgQty)
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
    }
}

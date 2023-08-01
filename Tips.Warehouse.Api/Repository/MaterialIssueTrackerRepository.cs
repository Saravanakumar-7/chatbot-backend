using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

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

        public async Task<int> AddDataToMaterialIssueTracker(ShopOrderMaterialIssueTracker shopOrderMaterialIssue)
        {
            shopOrderMaterialIssue.CreatedBy = "Admin";
            shopOrderMaterialIssue.CreatedOn = DateTime.Now;
            shopOrderMaterialIssue.Unit = "Bangalore";
            var result = await Create(shopOrderMaterialIssue);

            return result.Id;
        }
    }
}

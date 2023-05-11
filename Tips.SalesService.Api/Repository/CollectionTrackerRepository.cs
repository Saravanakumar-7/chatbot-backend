using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class CollectionTrackerRepository : RepositoryBase<CollectionTracker>, ICollectionTrackerRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public CollectionTrackerRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateCollectionTracker(CollectionTracker collectionTracker)
        {
            collectionTracker.CreatedBy = "Admin";
            collectionTracker.CreatedOn = DateTime.Now;
            collectionTracker.Unit = "Bangalore";
            var result = await Create(collectionTracker);
            return result.Id;
        }

        public async Task<string> DeleteCollectionTracker(CollectionTracker collectionTracker)
        {
            Delete(collectionTracker);
            string result = $"CollectionTracker details of {collectionTracker.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CollectionTracker>> GetAllCollectionTrackers()
        {
            var collectionTrackerDetails = await FindAll().ToListAsync();
            return collectionTrackerDetails;
        }

        public async Task<CollectionTracker> GetCollectionTrackerById(int id)
        {
            var collectionTrackerDetailsById = await FindByCondition(x => x.Id == id)
                .Include(c=>c.SOBreakDown)
                .FirstOrDefaultAsync();
            return collectionTrackerDetailsById;
        }

        public async Task<List<OpenSalesOrderDetailsDto>> GetOpenSODetailsByCustomerId(string customerId)
        {
            var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CustomerId == customerId).Sum(s => s.Total);

            var soBreakDownDetails = _tipsSalesServiceDbContext.SOBreakDowns.Where(x => x.CustomerId == customerId).Select(x => x.AmountAgainstSO).Count();
            if (soBreakDownDetails != 0)
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.CustomerId == customerId
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.SalesOrderNumber equals d.SalesOrderNumber into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                group new { e, SOBreakDown } by new { e.Id, e.SalesOrderNumber, e.Total } into g
                                select new OpenSalesOrderDetailsDto
                                {
                                    SalesOrderId = g.Key.Id,
                                    SalesOrderNo = g.Key.SalesOrderNumber,
                                    TotalValue = g.Key.Total,
                                    PendingValue = g.Key.Total - g.Sum(x => x.SOBreakDown.AmountAgainstSO),
                                    AmountRecieved = g.Sum(x => x.SOBreakDown.AmountAgainstSO)
                                };

                var soData = SODetails.ToList();

                return soData;


            }
            else
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.CustomerId == customerId
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.CustomerId equals d.CustomerId into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                select new OpenSalesOrderDetailsDto
                                {
                                    SalesOrderId = e.Id,
                                    SalesOrderNo = e.SalesOrderNumber,
                                    TotalValue = e.Total,
                                    PendingValue = e.Total,
                                    AmountRecieved = 0
                                };

            var soData = SODetails.ToList();

            return soData; 
            }
        }

            public async Task<CollectionTrackerDetailsDto> GetSOCollectionTrackerByCustomerId(string customerId)
        {
            var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CustomerId == customerId).Sum(s => s.Total);

            var collectionDetails = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Select(x=>x.AlreadyRecieved).Count();
            if (collectionDetails != 0)
            {
                var amountRecieved = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Sum(x=>x.AmountRecieved);
                var alreadyRecieved = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Sum(x => x.AlreadyRecieved);

                var alreadyRecievedData = Convert.ToInt32(alreadyRecieved) + Convert.ToInt32(amountRecieved);

                var collectiveTrackerDetails = await _tipsSalesServiceDbContext.SalesOrders
                                .Select(s => new CollectionTrackerDetailsDto()
                                {
                                    TotalSumOfSOAmount = salesOrderTotalValue,
                                    AlreadyRecieved = Convert.ToDecimal(alreadyRecievedData)

                                }).Distinct().FirstOrDefaultAsync();

                return collectiveTrackerDetails;
            }
            else
            {
                var alreadyRecieved = 0;
                var collectiveTrackerDetails = await _tipsSalesServiceDbContext.SalesOrders
                               .Select(s => new CollectionTrackerDetailsDto()
                               {
                                   TotalSumOfSOAmount = salesOrderTotalValue,
                                   AlreadyRecieved = alreadyRecieved

                               }).Distinct().FirstOrDefaultAsync();

                return collectiveTrackerDetails;
            }

        }

        public async Task<string> UpdateCollectionTracker(CollectionTracker collectionTracker)
        {
            collectionTracker.LastModifiedBy = "Admin";
            collectionTracker.LastModifiedOn = DateTime.Now;
            Update(collectionTracker);
            string result = $"CollectionTracker details of {collectionTracker.Id} is updated successfully!";
            return result;
        }
    }
}

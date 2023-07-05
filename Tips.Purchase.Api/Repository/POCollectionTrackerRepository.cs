using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Misc;
using System.Security.Claims;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class POCollectionTrackerRepository : RepositoryBase<POCollectionTracker>, IPOCollectionTrackerRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public POCollectionTrackerRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public async Task<int?> CreatePOCollectionTracker(POCollectionTracker pocollectionTracker)
        {
            pocollectionTracker.CreatedBy = "Admin";
            pocollectionTracker.CreatedOn = DateTime.Now;
            pocollectionTracker.Unit = "Bangalore";
            var result = await Create(pocollectionTracker);
            return result.Id;
        }

        public async Task<string> DeletePOCollectionTracker(POCollectionTracker pocollectionTracker)
        {
            Delete(pocollectionTracker);
            string result = $"POCollectionTracker details of {pocollectionTracker.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<POCollectionTracker>> GetAllPOCollectionTrackers(PagingParameter pagingParameter, SearchParamess searchParamess)
        {
            var pocollectionTrackerDetails = FindAll().OrderByDescending(x => x.Id)
                 .Where(col => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || col.VendorName.Contains(searchParamess.SearchValue) ||
                  col.VendorId.Contains(searchParamess.SearchValue))));

            return PagedList<POCollectionTracker>.ToPagedList(pocollectionTrackerDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<List<OpenPurchaseOrderDetailsDto>> GetOpenPODetailsByVendorId(string vendorId)
        {

            var purchaseOrderTotalValue = _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.VendorId == vendorId).Sum(s => s.TotalAmount);

            var poBreakDownDetails = _tipsPurchaseDbContext.POBreakDowns.Where(x => x.VendorId ==vendorId).Select(x => x.AmountAgainstPO).Count();
            if (poBreakDownDetails != 0)
            {
                var PODetails = from e in _tipsPurchaseDbContext.PurchaseOrders
                                where e.VendorId == vendorId
                                join d in _tipsPurchaseDbContext.POBreakDowns on e.PONumber equals d.PONumber into dept
                                from POBreakDown in dept.DefaultIfEmpty()
                                group new { e, POBreakDown } by new { e.Id, e.PONumber, e.TotalAmount } into g
                                select new OpenPurchaseOrderDetailsDto
                                {
                                    PurchaseOrderId = g.Key.Id,
                                    PONumber = g.Key.PONumber,
                                    TotalValue = g.Key.TotalAmount,
                                    PendingValue = g.Key.TotalAmount - g.Sum(x => x.POBreakDown.AmountAgainstPO),
                                    AmountRecieved = g.Sum(x => x.POBreakDown.AmountAgainstPO)
                                };

                var poData = PODetails.ToList();

                return poData;


            }
            else
            {
                var PODetails = from e in _tipsPurchaseDbContext.PurchaseOrders
                                where e.VendorId == vendorId
                                join d in _tipsPurchaseDbContext.POBreakDowns on e.VendorId equals d.VendorId into dept
                                from POBreakDown in dept.DefaultIfEmpty()
                                select new OpenPurchaseOrderDetailsDto
                                {
                                    PurchaseOrderId = e.Id,
                                    PONumber = e.PONumber,
                                    TotalValue = e.TotalAmount,
                                    PendingValue = e.TotalAmount,
                                    AmountRecieved = 0
                                };

                var poData = PODetails.ToList();

                return poData;
            }
        }

        public async Task<POCollectionTracker> GetPOCollectionTrackerById(int id)
        {
            var pocollectionTrackerDetailsById = await FindByCondition(x => x.Id == id)
               .Include(c => c.POBreakDown)
               .FirstOrDefaultAsync();
            return pocollectionTrackerDetailsById;
        }

        public async Task<POCollectionTrackerDetailsDto> GetPOCollectionTrackerByVendorId(string vendorId)
        {
            var purchaseOrderTotalValue = _tipsPurchaseDbContext.PurchaseOrders
                     .Where(x => x.VendorId == vendorId)
                         .Sum(s => s.TotalAmount);

            var pocollectionDetails = _tipsPurchaseDbContext.POCollectionTrackers
                .Where(x => x.VendorId == vendorId)
                .Select(x => x.AlreadyRecieved)
                .Count();

            if (pocollectionDetails != 0)
            {
                var amountRecieved = _tipsPurchaseDbContext.POCollectionTrackers
                    .Where(x => x.VendorId == vendorId)
                    .Sum(x => x.AmountRecieved);


                var pocollectiveTrackerDetails = new POCollectionTrackerDetailsDto()
                {
                    TotalSumOfPOAmount = purchaseOrderTotalValue,
                    AlreadyRecieved = Convert.ToDecimal(amountRecieved)
                };

                return pocollectiveTrackerDetails;
            }
            else
            {
                var alreadyRecieved = 0;

                var pocollectiveTrackerDetails = new POCollectionTrackerDetailsDto()
                {
                    TotalSumOfPOAmount = purchaseOrderTotalValue,
                    AlreadyRecieved = alreadyRecieved
                };

                return pocollectiveTrackerDetails;
            }
        }

        public async Task<string> UpdatePOCollectionTracker(POCollectionTracker pocollectionTracker)
        {
            pocollectionTracker.LastModifiedBy = "Admin";
            pocollectionTracker.LastModifiedOn = DateTime.Now;
            Update(pocollectionTracker);
            string result = $"POCollectionTracker details of {pocollectionTracker.Id} is updated successfully!";
            return result;
        }
    }
}
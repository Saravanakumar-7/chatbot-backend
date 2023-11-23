using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Filters;
using Org.BouncyCastle.Asn1.Misc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Tips.Purchase.Api.Repository
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PurchaseOrderRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var date = DateTime.Now;
            purchaseOrder.CreatedBy = _createdBy;
            purchaseOrder.CreatedOn = date.Date;
            // purchaseOrder.LastModifiedBy = _createdBy;
            //purchaseOrder.LastModifiedOn = DateTime.Now;
            //Guid purchaseOrderNumber = Guid.NewGuid();
            //purchaseOrder.PONumber = "PO-" + purchaseOrderNumber.ToString();
            purchaseOrder.Unit = _unitname;
            purchaseOrder.RevisionNumber = 1;
            var result = await Create(purchaseOrder);
            return result.Id;
        }
        public async Task<PurchaseOrder> ChangePurchaseOrderVersion(PurchaseOrder purchaseOrder)
        {
            var getOldPODetails = _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == purchaseOrder.PONumber && x.IsModified == false)
                .FirstOrDefault();

            if (getOldPODetails != null)
            {
                getOldPODetails.IsModified = true;
                getOldPODetails.LastModifiedBy = _createdBy;
                getOldPODetails.LastModifiedOn = DateTime.Now;
                Update(getOldPODetails);
            }

            purchaseOrder.CreatedBy = _createdBy;
            purchaseOrder.CreatedOn = DateTime.Now;
            //purchaseOrder.LastModifiedBy = 
            //purchaseOrder.LastModifiedOn = 
            var getOldRevisionNumber = _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == purchaseOrder.PONumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            purchaseOrder.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(purchaseOrder);
            return result;
        }

        public async Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string poNumber)
        {
            //grin 

            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsPurchaseDbContext.DocumentUploads
                                .Where(b => b.ParentNumber == poNumber)
                                .Select(x => new GetDownloadUrlDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                })
                              .ToListAsync();

            return getDownloadDetails;
        }

        //public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrderWithItems(PurchaseOrderSearchDto purchaseOrderSearch, PoVersion poVersion)
        //{
        //    using (var context = _tipsPurchaseDbContext)
        //    {
        //        var query = _tipsPurchaseDbContext.PurchaseOrders.Include("POItems");
        //        if (purchaseOrderSearch != null || (purchaseOrderSearch.PONumber.Any())
        //       && purchaseOrderSearch.ProcurementType.Any() && purchaseOrderSearch.ShippingMode.Any()
        //       && purchaseOrderSearch.VendorName.Any() && purchaseOrderSearch.PoStatus.Any())
        //        {
        //            query = query.Where
        //            (po => (purchaseOrderSearch.PONumber.Any() ? purchaseOrderSearch.PONumber.Contains(po.PONumber) : true)
        //           && (purchaseOrderSearch.ProcurementType.Any() ? purchaseOrderSearch.ProcurementType.Contains(po.ProcurementType) : true)
        //           && (purchaseOrderSearch.ShippingMode.Any() ? purchaseOrderSearch.ShippingMode.Contains(po.ShippingMode) : true)
        //           && (purchaseOrderSearch.VendorName.Any() ? purchaseOrderSearch.VendorName.Contains(po.VendorName) : true)
        //           && (purchaseOrderSearch.PoStatus.Any() ? purchaseOrderSearch.PoStatus.Contains(po.Status) : true))
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POAddprojects)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POAddDeliverySchedules)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POSpecialInstructions)
        //             .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POConfirmationDates)
        //             .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.PrDetails)
        //            .Include(itm => itm.POIncoTerms);
        //        }
        //        return query.ToList();
        //    }
        //}

        //public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes)
        //{
        //    using (var context = _tipsPurchaseDbContext)
        //    {
        //        var query = _tipsPurchaseDbContext.PurchaseOrders.Include("POItems");
        //        // int searchValueInt;
        //        //bool isSearchValueInt = int.TryParse(searchParammes.SearchValue, out searchValueInt);

        //        if (!string.IsNullOrEmpty(searchParammes.SearchValue))
        //        {
        //            query = query.Where(po => po.PONumber.Contains(searchParammes.SearchValue)
        //            || po.VendorName.Contains(searchParammes.SearchValue)
        //            || po.PODate.ToString().Contains(searchParammes.SearchValue)
        //            //|| po.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
        //            || po.ProcurementType.Contains(searchParammes.SearchValue)
        //            //|| po.VendorId.Contains(searchParammes.SearchValue)
        //            || po.QuotationDate.ToString().Contains(searchParammes.SearchValue)
        //            //|| po.QuotationReferenceNumber.Contains(searchParammes.SearchValue)
        //            || po.ShippingMode.Contains(searchParammes.SearchValue)
        //            || po.PaymentTerms.Contains(searchParammes.SearchValue)
        //            || po.DeliveryTerms.Contains(searchParammes.SearchValue)
        //            || po.POItems.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue) ||
        //            s.Description.Contains(searchParammes.SearchValue)
        //            || s.MftrItemNumber.Contains(searchParammes.SearchValue)
        //            || s.PONumber.Contains(searchParammes.SearchValue)))//||
        //                                                                // (!isSearchValueInt || po.RevisionNumber == searchValueInt))
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POAddprojects)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POAddDeliverySchedules)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POSpecialInstructions)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.POConfirmationDates)
        //            .Include(itm => itm.POItems)
        //            .ThenInclude(po => po.PrDetails)
        //            .Include(itm => itm.POIncoTerms);
        //        }
        //        return query.ToList();
        //    }
        //}
        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrderWithItems(PurchaseOrderSearchDto purchaseOrderSearch, PoVersion poVersion)
        {
            IQueryable<PurchaseOrder> query = _tipsPurchaseDbContext.PurchaseOrders.Include(po => po.POItems);

            if (purchaseOrderSearch != null &&
                (purchaseOrderSearch.PONumber.Any() ||
                 purchaseOrderSearch.ProcurementType.Any() ||
                 purchaseOrderSearch.ShippingMode.Any() ||
                 purchaseOrderSearch.VendorName.Any() ||
                 purchaseOrderSearch.PoStatus.Any()))
            {
                query = query.Where(po =>
                    (purchaseOrderSearch.PONumber.Any() ? purchaseOrderSearch.PONumber.Contains(po.PONumber) : true) &&
                    (purchaseOrderSearch.ProcurementType.Any() ? purchaseOrderSearch.ProcurementType.Contains(po.ProcurementType) : true) &&
                    (purchaseOrderSearch.ShippingMode.Any() ? purchaseOrderSearch.ShippingMode.Contains(po.ShippingMode) : true) &&
                    (purchaseOrderSearch.VendorName.Any() ? purchaseOrderSearch.VendorName.Contains(po.VendorName) : true) &&
                    (purchaseOrderSearch.PoStatus.Any() ? purchaseOrderSearch.PoStatus.Contains(po.Status) : true));
            }

            if (poVersion == PoVersion.LatestVersion)
            {
                var latestRevisions = query.GroupBy(po => po.PONumber)
                    .Select(group => group.OrderByDescending(po => po.RevisionNumber).FirstOrDefault())
                    .ToList();

                var latestRevisionIds = latestRevisions.Select(po => po.Id).ToList();

                query = query.Where(po => latestRevisionIds.Contains(po.Id));
            }

            query = query.Include(po => po.POItems)
                .ThenInclude(po => po.POAddprojects)
                .Include(po => po.POItems)
                .ThenInclude(po => po.POAddDeliverySchedules)
                .Include(po => po.POItems)
                .ThenInclude(po => po.POSpecialInstructions)
                .Include(po => po.POItems)
                .ThenInclude(po => po.POConfirmationDates)
                .Include(po => po.POItems)
                .ThenInclude(po => po.PrDetails)
                .Include(po => po.POIncoTerms);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes, PoVersion poVersion)
        {
            IQueryable<PurchaseOrder> query = _tipsPurchaseDbContext.PurchaseOrders;

            // Apply search criteria
            if (!string.IsNullOrEmpty(searchParammes.SearchValue))
            {
                query = query.Where(po => po.PONumber.Contains(searchParammes.SearchValue)
                    || po.VendorName.Contains(searchParammes.SearchValue)
                    || po.PODate.ToString().Contains(searchParammes.SearchValue)
                    || po.ProcurementType.Contains(searchParammes.SearchValue)
                    || po.QuotationDate.ToString().Contains(searchParammes.SearchValue)
                    || po.ShippingMode.Contains(searchParammes.SearchValue)
                    || po.PaymentTerms.Contains(searchParammes.SearchValue)
                    || po.DeliveryTerms.Contains(searchParammes.SearchValue)
                    || po.POItems.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue)
                        || s.Description.Contains(searchParammes.SearchValue)
                        || s.MftrItemNumber.Contains(searchParammes.SearchValue)
                        || s.PONumber.Contains(searchParammes.SearchValue)));
            }

            if (poVersion == PoVersion.LatestVersion)
            {
                var latestRevisions = query.GroupBy(po => po.PONumber)
                    .Select(group => group.OrderByDescending(po => po.RevisionNumber).FirstOrDefault())
                    .ToList();

                // Get the IDs of the latest revision Purchase Orders
                var latestRevisionIds = latestRevisions.Select(po => po.Id).ToList();

                // Filter by latest revision IDs
                query = query.Where(po => latestRevisionIds.Contains(po.Id));
            }

            // Separate the Include statements
            query = query
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddprojects)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddDeliverySchedules)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POSpecialInstructions)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POConfirmationDates)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.PrDetails)
                .Include(itm => itm.POIncoTerms);

            return await query.ToListAsync();
        }

        //public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams)
        //{
        //    var purchaseOrderDetails = _tipsPurchaseDbContext.PurchaseOrders
        //    .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
        //    inv.CreatedOn <= searchDatesParams.SearchToDate
        //    )))
        //    .Include(itm => itm.POItems)
        //    .ThenInclude(po => po.POAddprojects)
        //    .Include(itm => itm.POItems)
        //    .ThenInclude(po => po.POAddDeliverySchedules)
        //    .Include(itm => itm.POItems)
        //    .ThenInclude(po => po.POSpecialInstructions)
        //    .Include(itm => itm.POItems)
        //    .ThenInclude(po => po.POConfirmationDates)
        //    .Include(itm => itm.POItems)
        //     .ThenInclude(po => po.PrDetails)
        //     .Include(itm => itm.POIncoTerms)
        //    .ToList();
        //    return purchaseOrderDetails;
        //}
        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams, PoVersion poVersion)
        {

            if (poVersion == PoVersion.LatestVersion)
            {
                IQueryable<PurchaseOrder> query = _tipsPurchaseDbContext.PurchaseOrders
                .Where(inv => inv.LastModifiedOn.Value.Date >= searchDatesParams.SearchFromDate.Value.Date
                 && inv.LastModifiedOn.Value.Date <= searchDatesParams.SearchToDate.Value.Date)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddprojects)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddDeliverySchedules)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POSpecialInstructions)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POConfirmationDates)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.PrDetails)
                .Include(itm => itm.POIncoTerms);
                // Filter by the latest revision number if selected
                query = query.GroupBy(inv => inv.PONumber)
                    .Select(group => group.OrderByDescending(inv => inv.RevisionNumber).FirstOrDefault());
                var purchaseOrderDetails = await query.ToListAsync();
                return purchaseOrderDetails;
            }
            else
            {
                IQueryable<PurchaseOrder> query = _tipsPurchaseDbContext.PurchaseOrders
                .Where(inv => (inv.CreatedOn.Value.Date >= searchDatesParams.SearchFromDate.Value.Date && inv.CreatedOn.Value.Date <= searchDatesParams.SearchToDate.Value.Date)
                || (inv.LastModifiedOn.Value.Date >= searchDatesParams.SearchFromDate.Value.Date && inv.LastModifiedOn.Value.Date <= searchDatesParams.SearchToDate.Value.Date))
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddprojects)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POAddDeliverySchedules)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POSpecialInstructions)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.POConfirmationDates)
                .Include(itm => itm.POItems)
                .ThenInclude(po => po.PrDetails)
                .Include(itm => itm.POIncoTerms);
                var purchaseOrderDetails = await query.ToListAsync();
                return purchaseOrderDetails;
            }


        }

        public async Task<IEnumerable<PurchaseOrderRevNoListDto>> GetAllRevisionNumberListByPoNumber(string poNumber)
        {
            IEnumerable<PurchaseOrderRevNoListDto> revNoListbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.PONumber == poNumber).Select(x => new PurchaseOrderRevNoListDto()
            {
                RevisionNumber = x.RevisionNumber,
            }).ToListAsync();

            return revNoListbyPONumber;
        }
        public async Task<PurchaseRequisition> GetPrDetailsByPrNumber(string prNumber)
        {
            var prDetails = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.PrNumber == prNumber)
                .FirstOrDefaultAsync();

            return prDetails;
        }

        public async Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber)
        {

            var query = from pri in _tipsPurchaseDbContext.PrItems
                        join pdu in _tipsPurchaseDbContext.PRItemsDocumentUploads on pri.Id equals pdu.PrItemId into documentUploads
                        from pdu in documentUploads.DefaultIfEmpty() // Left join handling no matching records
                        join pr in _tipsPurchaseDbContext.PurchaseRequisitions on pri.PurchaseRequistionId equals pr.Id
                    where pri.ItemNumber == itemNumber && pr.PrApprovalI && pr.PrApprovalII
                    group new { pri, pr } by new { pr.PrNumber, pri.ItemNumber, pr.RevisionNumber } into grouped
                    select new 
                    {
                        grouped.Key.PrNumber, 
                        grouped.Key.RevisionNumber,
                        Qty = grouped.Sum(item => item.pri.Qty),
                    };
             

// Materialize the query to a list
                var result = query.ToList();             

                var prNoAndQtyList = result.Select(x => new PRNoandQtyListDto
            {
                PRNumber = x.PrNumber,
                RevisionNumber = x.RevisionNumber,
                Qty = x.Qty,
                DocumentNames = _tipsPurchaseDbContext.PRItemsDocumentUploads
        //.Where(pdu => pdu.PrItemId == x.pri.Id)  // as
        .Select(pdu => new PRItemsDocumentUpload
        {
            FileName = pdu.FileName,
            FileExtension = pdu.FileExtension,
            FilePath = pdu.FilePath,
            DocumentFrom = pdu.DocumentFrom,
            ParentNumber = pdu.ParentNumber,
            Checked = pdu.Checked,
            CreatedBy = pdu.CreatedBy,
            CreatedOn = pdu.CreatedOn,
            LastModifiedBy = pdu.LastModifiedBy,
            LastModifiedOn = pdu.LastModifiedOn,
            PrItemId = pdu.PrItemId,
        }).ToList()
            }).ToList();

            return prNoAndQtyList;
    }

        //    public async Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber)
        //    {

        //    var Join = from pri in _tipsPurchaseDbContext.PrItems
        //               where pri.ItemNumber == itemNumber
        //               join pdu in _tipsPurchaseDbContext.PRItemsDocumentUploads on pri.Id equals pdu.PrItemId
        //               join pr in _tipsPurchaseDbContext.PurchaseRequisitions on pri.PurchaseRequistionId equals pr.Id
        //               where pr.PrApprovalI == true && pr.PrApprovalII == true
        //               group new { pri, pr, pdu } by new { pr.PrNumber, pri.ItemNumber,pr.RevisionNumber } into grouped
        //               select new PRNoandQtyListDto
        //               {
        //                   PRNumber = grouped.Key.PrNumber,
        //                   RevisionNumber = grouped.Key.RevisionNumber,
        //                   Qty = grouped.Sum(item => item.pri.Qty),
        //                   //DocumentNames = grouped.Select(item => new PRItemsDocumentUpload
        //                   //{
        //                   //    FileName = item.pdu.FileName,
        //                   //    FileExtension = item.pdu.FileExtension,
        //                   //    FilePath = item.pdu.FilePath,
        //                   //    DocumentFrom = item.pdu.DocumentFrom,
        //                   //    ParentNumber = item.pdu.ParentNumber,
        //                   //    Checked = item.pdu.Checked,
        //                   //    CreatedBy = item.pdu.CreatedBy,
        //                   //    CreatedOn = item.pdu.CreatedOn,
        //                   //    LastModifiedBy = item.pdu.LastModifiedBy,
        //                   //    LastModifiedOn = item.pdu.LastModifiedOn,
        //                   //    PrItemId = item.pdu.PrItemId,
        //                   //}).ToList()
        //               };

        //    var postdata = Join.ToList();
        //        return postdata;
        //}

        //public async Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber)
        //{
        //    var Join = from pri in _tipsPurchaseDbContext.PrItems
        //               where pri.ItemNumber == itemNumber
        //               join pdu in _tipsPurchaseDbContext.PRItemsDocumentUpload on pri.Id equals pdu.PrItemId
        //               join pr in _tipsPurchaseDbContext.PurchaseRequisitions on pri.PurchaseRequistionId equals pr.Id
        //               where pr.PrApprovalI == true && pr.PrApprovalII == true
        //               select new PRNoandQtyListDto
        //               {
        //                   PRNumber = pr.PrNumber,
        //                   Qty = pri.Qty,
        //                   DocumentNames = pri.Upload.Select(d => new PRItemsDocumentUpload
        //                   {
        //                       FileName = d.FileName,
        //                       FileExtension = d.FileExtension,
        //                       FilePath = d.FilePath,
        //                       DocumentFrom = d.DocumentFrom,
        //                       ParentNumber = d.ParentNumber,
        //                       Checked = d.Checked,
        //                       CreatedBy = d.CreatedBy,
        //                       CreatedOn = d.CreatedOn,
        //                       LastModifiedBy = d.LastModifiedBy,
        //                       LastModifiedOn = d.LastModifiedOn,
        //                       PrItemId = d.PrItemId,
        //                   }).ToList()
        //               };

        //    var postdata = Join.ToList();

        //    return postdata;
        //}



        //public async Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber)
        //{

        //    var Join = from pri in _tipsPurchaseDbContext.PrItems
        //               where pri.ItemNumber == itemNumber
        //               join pr in _tipsPurchaseDbContext.PurchaseRequisitions on pri.PurchaseRequistionId equals pr.Id
        //               where pr.PrApprovalI == true && pr.PrApprovalII == true
        //               select new PRNoandQtyListDto
        //               {
        //                   PRNumber = pr.PrNumber,
        //                   Qty = pri.Qty,
        //               };
        //    var postdata = Join.ToList();

        //    return postdata;
        //}

        public async Task<int?> GetPONumberAutoIncrementCount(DateTime date)
        {
            var getPONumberAutoIncrementCount = _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.CreatedOn == date.Date).Count();

            return getPONumberAutoIncrementCount;
        }
        public async Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            Delete(purchaseOrder);
            string result = $"PurchaseOrder details of {purchaseOrder.Id} is deleted successfully!";
            return result;
        }

        //public async Task<PagedList<PurchaseOrder>> GetAllActivePurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        //{



        //    var activePurchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)
        //       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
        //       || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
        //       || inv.PODate.Equals(int.Parse(searchParams.SearchValue)))))
        //                        .Include(o => o.POFiles)
        //                        .Include(t => t.POItems)
        //                        .ThenInclude(x => x.POAddprojects)
        //                        .Include(m => m.POItems)
        //                        .ThenInclude(i => i.POAddDeliverySchedules);
        //    return PagedList<PurchaseOrder>.ToPagedList(activePurchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<string> GeneratePONumber()
        {
            using var transaction = await _tipsPurchaseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsPurchaseDbContext.PONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsPurchaseDbContext.Update(poNumberEntity);
                await _tipsPurchaseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"PO-{poNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> GeneratePONumberForAvision()
        {
            using var transaction = await _tipsPurchaseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsPurchaseDbContext.PONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsPurchaseDbContext.Update(poNumberEntity);
                await _tipsPurchaseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                return $"ASPL|PO|{currentYear:D2}-{nextYear:D2}|{poNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrders()
        {
            var activePurchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)
                                .Where(x => x.Status != Status.Closed)
                                .Include(o => o.POFiles)
                                .Include(t => t.POItems)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItems)
                                .ThenInclude(i => i.POAddDeliverySchedules)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POSpecialInstructions)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POConfirmationDates)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.PrDetails)
                                .Include(itm => itm.POIncoTerms);

            return activePurchaseOrderDetails;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> activePurchaseOrderNameList = await _tipsPurchaseDbContext.PurchaseOrders
                                .Select(x => new PurchaseOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return activePurchaseOrderNameList;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPurchaseOrderNameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> purchaseOrderNameList = await _tipsPurchaseDbContext.PurchaseOrders
                                .Select(x => new PurchaseOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return purchaseOrderNameList;
        }
        public async Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {

            IQueryable<PurchaseOrderIdNameListDto> pendingPOApprovalINameList =  _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.POApprovalI == false && x.IsDeleted == false && x.IsModified == false)
            .Select(g => new PurchaseOrderIdNameListDto()
            {
                Id = g.Id,
                PONumber = g.PONumber,
                PODate = g.PODate,
                RevisionNumber = g.RevisionNumber,
                BillToId = g.BillToId,
                ShipToId = g.ShipToId,
                ProcurementType = g.ProcurementType,
                Currency = g.Currency,
                CompanyAliasName = g.CompanyAliasName,
                PoConfirmationStatus = g.PoConfirmationStatus,
                VendorName = g.VendorName,
                VendorId = g.VendorId,
                QuotationReferenceNumber = g.QuotationReferenceNumber,
                QuotationDate = g.QuotationDate,
                VendorAddress = g.VendorAddress,
                DeliveryTerms = g.DeliveryTerms,
                PaymentTerms = g.PaymentTerms,
                ShippingMode = g.ShippingMode,
                ShipTo = g.ShipTo,
                BillTo = g.BillTo,
                RetentionPeriod = g.RetentionPeriod,
                SpecialTermsAndConditions = g.SpecialTermsAndConditions,
                IsDeleted = g.IsDeleted,
                IsShortClosed = g.IsShortClosed,
                ShortClosedBy = g.ShortClosedBy,
                ShortClosedOn = g.ShortClosedOn,
                TotalAmount = g.TotalAmount,
                POApprovalI = g.POApprovalI,
                POApprovedIDate = g.POApprovedIDate,
                POApprovedIBy = g.POApprovedIBy,
                POApprovalII = g.POApprovalII,
                POApprovedIIDate = g.POApprovedIIDate,
                POApprovedIIBy = g.POApprovedIIBy,
                Unit = g.Unit,
                CreatedBy = g.CreatedBy,
                CreatedOn = g.CreatedOn,
                LastModifiedBy = g.LastModifiedBy,
                LastModifiedOn = g.LastModifiedOn,

            });
            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower(); 

                pendingPOApprovalINameList = pendingPOApprovalINameList
                    .Where(item =>
                        item.PONumber.ToLower().Contains(searchValue) ||
                        item.VendorName.ToLower().Contains(searchValue) ||
                        item.VendorId.ToLower().Contains(searchValue) ||
                        item.VendorAddress.ToLower().Contains(searchValue) ||
                        item.POApprovedIBy.ToLower().Contains(searchValue) ||
                        item.POApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    );
            }

            int totalCount = await pendingPOApprovalINameList.CountAsync();

            var result = await pendingPOApprovalINameList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return new PagedList<PurchaseOrderIdNameListDto>(result, totalCount, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            IQueryable<PurchaseOrderIdNameListDto> pendingPOApprovalIINameList =  _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.POApprovalI == true && x.POApprovalII == false && x.IsDeleted == false && x.IsModified == false)
            .Select(g => new PurchaseOrderIdNameListDto()
            {
                Id = g.Id,
                PONumber = g.PONumber,
                PODate = g.PODate,
                RevisionNumber = g.RevisionNumber,
                BillToId = g.BillToId,
                ShipToId = g.ShipToId,
                ProcurementType = g.ProcurementType,
                Currency = g.Currency,
                CompanyAliasName = g.CompanyAliasName,
                PoConfirmationStatus = g.PoConfirmationStatus,
                VendorName = g.VendorName,
                VendorId = g.VendorId,
                QuotationReferenceNumber = g.QuotationReferenceNumber,
                QuotationDate = g.QuotationDate,
                VendorAddress = g.VendorAddress,
                DeliveryTerms = g.DeliveryTerms,
                PaymentTerms = g.PaymentTerms,
                ShippingMode = g.ShippingMode,
                ShipTo = g.ShipTo,
                BillTo = g.BillTo,
                RetentionPeriod = g.RetentionPeriod,
                SpecialTermsAndConditions = g.SpecialTermsAndConditions,
                IsDeleted = g.IsDeleted,
                IsShortClosed = g.IsShortClosed,
                ShortClosedBy = g.ShortClosedBy,
                ShortClosedOn = g.ShortClosedOn,
                TotalAmount = g.TotalAmount,
                POApprovalI = g.POApprovalI,
                POApprovedIDate = g.POApprovedIDate,
                POApprovedIBy = g.POApprovedIBy,
                POApprovalII = g.POApprovalII,
                POApprovedIIDate = g.POApprovedIIDate,
                POApprovedIIBy = g.POApprovedIIBy,
                Unit = g.Unit,
                CreatedBy = g.CreatedBy,
                CreatedOn = g.CreatedOn,
                LastModifiedBy = g.LastModifiedBy,
                LastModifiedOn = g.LastModifiedOn,

            });

            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower();

                pendingPOApprovalIINameList = pendingPOApprovalIINameList
                    .Where(item =>
                        item.PONumber.ToLower().Contains(searchValue) ||
                        item.VendorName.ToLower().Contains(searchValue) ||
                        item.VendorId.ToLower().Contains(searchValue) ||
                        item.VendorAddress.ToLower().Contains(searchValue) ||
                        item.POApprovedIBy.ToLower().Contains(searchValue) ||
                        item.POApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    );
            }
        

            int totalCount = await pendingPOApprovalIINameList.CountAsync();

            var result = await pendingPOApprovalIINameList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return new PagedList<PurchaseOrderIdNameListDto>(result, totalCount, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalINameList()
        {
            //IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalINameList = await _tipsPurchaseDbContext.PurchaseOrders
            //                .Where(x => x.POApprovalI == false).Select(x => new PurchaseOrderIdNameListDto()
            //                {
            //                    Id = x.Id,
            //                    PONumber = x.PONumber,
            //                }).Distinct().ToListAsync();

            IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalINameList = await _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.POApprovalI == false && x.IsModified == false)
            .GroupBy(x => x.PONumber)
            .Select(g => new PurchaseOrderIdNameListDto()
            {
                Id = g.OrderByDescending(x => x.RevisionNumber).FirstOrDefault().Id,
                PONumber = g.Key
            })
            .ToListAsync();

            return pendingPOApprovalINameList;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIINameList()
        { 
            IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalIINameList = await _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.POApprovalII == false && x.POApprovalI == true && x.IsDeleted == false && x.IsModified == false)
            .GroupBy(x => x.PONumber)
            .Select(g => new PurchaseOrderIdNameListDto()
            {
                Id = g.OrderByDescending(x => x.RevisionNumber).FirstOrDefault().Id,
                PONumber = g.Key,
            })
            .ToListAsync();


            return pendingPOApprovalIINameList;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            int? searchrev;
            DateTime? searchDate;
            try
            {
                searchrev = int.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchrev = null;
            }
            try
            {
                searchDate = DateTime.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchDate = null;
            }

            var purchaseOrderDetails = FindAll().Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
            inv.VendorName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue) ||
            inv.RevisionNumber.Equals(searchrev) || inv.PODate.Equals(searchDate)))
            .OrderByDescending(on => on.Id).Include(o => o.POFiles).Include(t => t.POItems).ThenInclude(x => x.POAddprojects)
            .Include(m => m.POItems).ThenInclude(i => i.POAddDeliverySchedules).Include(itm => itm.POItems)
            .ThenInclude(po => po.POSpecialInstructions).Include(itm => itm.POItems).ThenInclude(po => po.POConfirmationDates)
            .Include(itm => itm.POItems).ThenInclude(po => po.PrDetails).Include(itm => itm.POIncoTerms);

            return PagedList<PurchaseOrder>.ToPagedList(purchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //        var purchaseOrderDetails = FindAll()
            //.Where(inv => (
            //    string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
            //    inv.VendorName.Contains(searchParams.SearchValue) ||
            //    inv.PONumber.Contains(searchParams.SearchValue) ||
            //    inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue)) ||
            //    inv.PODate.Equals(int.Parse(searchParams.SearchValue))
            //))
            //.OrderByDescending(on => on.Id)
            //.Include(o => o.POFiles)
            //.Include(t => t.POItems)
            //    .ThenInclude(x => x.POAddprojects)
            //.Include(m => m.POItems)
            //    .ThenInclude(i => i.POAddDeliverySchedules)
            //.Include(itm => itm.POItems)
            //    .ThenInclude(po => po.POSpecialInstructions)
            //.Include(itm => itm.POItems)
            //    .ThenInclude(po => po.POConfirmationDates)
            //.Include(itm => itm.POItems)
            //    .ThenInclude(po => po.PrDetails)
            //.Include(itm => itm.POIncoTerms);

            //var purchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)

            //   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
            //   || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
            //   || inv.PODate.Equals(int.Parse(searchParams.SearchValue)))))
            //                    .Include(o => o.POFiles)
            //                   .Include(t => t.POItems)                               
            //                   .Include(t => t.POItems)
            //                   .ThenInclude(x => x.POAddprojects)
            //                   .Include(m => m.POItems)
            //                   .ThenInclude(i => i.POAddDeliverySchedules)
            //                   .Include(itm => itm.POItems)
            //                   .ThenInclude(po => po.POSpecialInstructions)
            //                   .Include(itm => itm.POItems)
            //                   .ThenInclude(po => po.POConfirmationDates)
            //                   .Include(itm => itm.POItems)
            //                    .ThenInclude(po => po.PrDetails)
            //                    .Include(itm => itm.POIncoTerms);

            //return PagedList<PurchaseOrder>.ToPagedList(purchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int id)
        {
            var purchaseOrderDetailById = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.Id == id)
                                                .Include(o => o.POFiles)

                .Include(t => t.POItems)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItems)
                                .ThenInclude(i => i.POAddDeliverySchedules)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POSpecialInstructions)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POConfirmationDates)
                                .Include(itm => itm.POItems)
                                 .ThenInclude(po => po.PrDetails)
                                 .Include(itm => itm.POIncoTerms)
                                .FirstOrDefaultAsync();


            return purchaseOrderDetailById;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber)
        {
            var purchaseOrderDetailbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == poNumber && x.IsDeleted == false && x.IsModified == false)
                .Include(o => o.POFiles)
                .Include(t => t.POItems)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItems)
                                .ThenInclude(i => i.POAddDeliverySchedules)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POSpecialInstructions)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POConfirmationDates)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.PrDetails)
                                .Include(itm => itm.POIncoTerms)
                                .FirstOrDefaultAsync();


            return purchaseOrderDetailbyPONumber;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONoAndRevNo(string poNumber, int revisionNumber)
        {
            var purchaseOrderDetail = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == poNumber && x.RevisionNumber == revisionNumber && x.IsDeleted == false)
                .Include(o => o.POFiles)
                .Include(t => t.POItems)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItems)
                                .ThenInclude(i => i.POAddDeliverySchedules)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POSpecialInstructions)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.POConfirmationDates)
                                .Include(itm => itm.POItems)
                                .ThenInclude(po => po.PrDetails)
                                .Include(itm => itm.POIncoTerms)
                                .FirstOrDefaultAsync();


            return purchaseOrderDetail;
        }

        public async Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.LastModifiedBy = _createdBy;
            purchaseOrder.LastModifiedOn = DateTime.Now;
            Update(purchaseOrder);
            string result = $"PurchaseOrder of Detail {purchaseOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<string> UpdatePurchaseOrder_ForApproval(PurchaseOrder purchaseOrder)
        {
           // purchaseOrder.LastModifiedBy = _createdBy;
           // purchaseOrder.LastModifiedOn = DateTime.Now;
            Update(purchaseOrder);
            string result = $"PurchaseOrder of Detail {purchaseOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorName(string vendorName)
        {
            IEnumerable<PurchaseOrderIdNameListDto> pONameListbyVendorName = await _tipsPurchaseDbContext.PurchaseOrders
                           .Where(x => x.VendorName == vendorName).Select(x => new PurchaseOrderIdNameListDto()
                           {
                               Id = x.Id,
                               PONumber = x.PONumber,
                           }).ToListAsync();


            return pONameListbyVendorName;
        }

        public async Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPOItemNumberListByPoNumber(string poNumber)
        {
            int pOItemNoListbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(s => s.PONumber == poNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<PurchaseOrderItemNoListDto> pOItemNumberDetails = await _tipsPurchaseDbContext.PoItems
                          .Where(x => x.PurchaseOrderId == pOItemNoListbyPONumber)
                          .Select(r => new PurchaseOrderItemNoListDto()
                          {
                              Id = r.Id,
                              ItemNumber = r.ItemNumber
                          }).ToListAsync();

            return pOItemNumberDetails;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorId(string vendorId)
        {
            Status[] status = { Status.Open, Status.PartiallyClosed };

            IEnumerable<PurchaseOrderIdNameListDto> pONameListbyVendorId = await _tipsPurchaseDbContext.PurchaseOrders
                           .Where(x => x.VendorId == vendorId && status.Contains(x.Status) && x.POApprovalII == true).Select(x => new PurchaseOrderIdNameListDto()
                           {
                               Id = x.Id,
                               PONumber = x.PONumber
                           }).ToListAsync();


            return pONameListbyVendorId;
        }

        public async Task<decimal> GetOpenPoQuantityByItemNumber(string itemNumber)
        {
            return await _tipsPurchaseDbContext.PoItems
              .Where(poi => poi.ItemNumber == itemNumber && poi.PoPartsStatus == true && poi.BalanceQty > 0
               && poi.PurchaseOrder.IsShortClosed == false && poi.PurchaseOrder.IsDeleted == false)
              .SumAsync(poi => poi.BalanceQty);
        }

    }

    public class UploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContexts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UploadDocumentRepository(TipsPurchaseDbContext tipsPurchaseDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsPurchaseDbContext)
        {
            _tipsPurchaseDbContexts = tipsPurchaseDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateUploadDocumentPO(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = _createdBy;
            documentUpload.CreatedOn = DateTime.Now;
           // documentUpload.LastModifiedBy = _createdBy;
           // documentUpload.LastModifiedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
        public async Task<DocumentUpload> GetUploadDocById(int id)
        {
            var pOUploadDocFileNameById = await _tipsPurchaseDbContext.DocumentUploads
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return pOUploadDocFileNameById;
        }
        public async Task<string> DeleteUploadFile(DocumentUpload documentUpload)
        {
            Delete(documentUpload);
            string result = $"DocumentUpload details of {documentUpload.Id} is deleted successfully!";
            return result;
        }
    }

    public class PurchaseOrderItemRepository : RepositoryBase<PoItem>, IPoItemsRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContexts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PurchaseOrderItemRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContexts = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<PoItem> GetPoItemDetailsById(int poItemId)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContexts.PoItems
                 .Where(x => x.Id == poItemId)
                          .FirstOrDefaultAsync();

            return getPODetailsByPONOandItemNo;
        }
        public async Task<IEnumerable<PoItem>> GetPODetailsByPONumberandItemNo(string ItemNumber, string PONumber)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContexts.PoItems
                 .Where(x => x.ItemNumber == ItemNumber && x.PONumber == PONumber && x.PoPartsStatus != true)
                          .ToListAsync();

            return getPODetailsByPONOandItemNo;
        }
        public async Task<IEnumerable<PoItem>> GetPoItemDetailsByPONumberandItemNo(string ItemNumber, string PONumber,int poItemId)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContexts.PoItems
                 .Where(x => x.ItemNumber == ItemNumber && x.PONumber == PONumber && x.Id == poItemId)
                          .ToListAsync();

            return getPODetailsByPONOandItemNo;
        }
        //aravind


        //public async Task<IEnumerable<PoItem>> GetPODetailsByItemNo(string ItemNumber)
        //{
        //    var podetails = await _tipsPurchaseDbContext.PurchaseOrders.Where(x=>x.Status == Status.Open).Select(x=>x.Id).ToListAsync();

        //    var getPODetailsItemNo = await _tipsPurchaseDbContexts.PoItems
        //         .Where(x => x.ItemNumber == ItemNumber && podetails.Contains(x.PurchaseOrderId))
        //                  .ToListAsync();

        //    return getPODetailsItemNo;
        //}

        //get count po item for particular po number

        public async Task<int?> GetPODetailsByPONumber(string PONumber)
        {
            var getPODetailsByPONO = _tipsPurchaseDbContexts.PoItems.Where(x => x.PONumber == PONumber).Count();

            return getPODetailsByPONO;
        }
        //get tg po details

        public async Task<List<OpenPurchaseOrderDto>> GetOpenPOTGDetailsByItem(string itemNumber)
        {
            List<OpenPurchaseOrderDto> result = await _tipsPurchaseDbContext.PoItems
           .Join(
               _tipsPurchaseDbContext.PurchaseOrders,
               poItem => poItem.PONumber,
               purchaseOrder => purchaseOrder.PONumber,
               (poItem, purchaseOrder) => new { PoItem = poItem, PurchaseOrder = purchaseOrder })
           .Where(joinResult =>
               joinResult.PurchaseOrder.Status == Status.Open || joinResult.PurchaseOrder.Status == Status.PartiallyClosed &&
               joinResult.PurchaseOrder.IsDeleted == false &&
               joinResult.PurchaseOrder.IsShortClosed == false &&
               joinResult.PurchaseOrder.POApprovalI == true &&
               joinResult.PurchaseOrder.POApprovalII == true &&
               joinResult.PurchaseOrder.IsModified == false &&
               joinResult.PoItem.BalanceQty > 0 &&
               joinResult.PoItem.PartType == PoPartType.TG &&
               (joinResult.PoItem.PoStatus == PoStatus.Open || joinResult.PoItem.PoStatus == PoStatus.PartiallyClosed)
               )
           .GroupBy(joinResult => new { joinResult.PoItem.ItemNumber })
           .Select(group => new OpenPurchaseOrderDto
           {
               ItemNumber = group.Key.ItemNumber,
               BalanceQty = group.Sum(c => c.PoItem.BalanceQty)
           }).ToListAsync();

            //        List<OpenPurchaseOrderDto> result = await _tipsPurchaseDbContext.PoItems
            //.Join(_tipsPurchaseDbContext.PurchaseOrders,
            //    poItem => poItem.PONumber,
            //    purchaseOrder => purchaseOrder.PONumber,
            //    (poItem, purchaseOrder) => new { PoItem = poItem, PurchaseOrder = purchaseOrder })
            //.Where(joinResult => joinResult.PoItem.ItemNumber == itemNumber
            //&& joinResult.PurchaseOrder.Status != Status.Closed && (int)joinResult.PoItem.PartType == (int)PartType.TG)
            //.GroupBy(joinResult => new { joinResult.PoItem.ItemNumber, joinResult.PoItem.PONumber })
            //.Select(group => new OpenPurchaseOrderDto
            //{
            //    ItemNumber = group.Key.ItemNumber,
            //    BalanceQty = group.Sum(c => c.PoItem.BalanceQty),
            //    PONumber = group.Key.PONumber
            //}).ToListAsync();

            return result;

        }

        public async Task<List<OpenPurchaseOrderDto>> GetOpenPODetailsByItem(string itemNumber)
        {

            List<OpenPurchaseOrderDto> result = await _tipsPurchaseDbContext.PoItems
            .Join(_tipsPurchaseDbContext.PurchaseOrders,
                poItem => poItem.PONumber,
                purchaseOrder => purchaseOrder.PONumber,
                (poItem, purchaseOrder) => new { PoItem = poItem, PurchaseOrder = purchaseOrder })
            .Where(joinResult => joinResult.PoItem.ItemNumber == itemNumber && joinResult.PurchaseOrder.Status != Status.Closed)
            .GroupBy(joinResult => new { joinResult.PoItem.ItemNumber, joinResult.PoItem.PONumber })
            .Select(group => new OpenPurchaseOrderDto
            {
                ItemNumber = group.Key.ItemNumber,
                BalanceQty = group.Sum(c => c.PoItem.BalanceQty),
                PONumber = group.Key.PONumber
            }).ToListAsync();

            return result;

        }

        public async Task<List<OpenPoQuantityDto>> GetListOfOpenPOQtyByItemNoList(List<string> itemNumberList)
        {
            var poStatus = new List<PoStatus> { PoStatus.Open, PoStatus.PartiallyClosed };

            List<OpenPoQuantityDto> openPoQtyList = await _tipsPurchaseDbContext.PoItems
                .Include(x => x.PurchaseOrder)
                .Where(x => poStatus.Contains(x.PurchaseOrder.PoStatus) && poStatus.Contains(x.PoStatus)
                    && itemNumberList.Contains(x.ItemNumber) && x.PurchaseOrder.IsDeleted== false 
                    && x.PurchaseOrder.IsModified ==false)
                .GroupBy( i=> new { i.ItemNumber })
                .Select(gr => new OpenPoQuantityDto
                {
                    ItemNumber = gr.Key.ItemNumber,
                    OpenPoQty = gr.Sum(x => x.BalanceQty)
                }).ToListAsync();
            return openPoQtyList;

        }
        public async Task<PoItem> ClosePoItemSatusByPoItemId(int poItemId)
        {
            var poItemDetailByPoItemId = await _tipsPurchaseDbContext.PoItems.Where(x => x.Id == poItemId)

                                .FirstOrDefaultAsync();

            return poItemDetailByPoItemId;
        }
        public async Task<int?> GetPoItemOpenStatusCount(int poId)
        {
            var poItemStatusCount = _tipsPurchaseDbContext.PoItems
                                        .Where(x => x.PurchaseOrderId == poId && x.PoStatus == PoStatus.Open).Count();

            return poItemStatusCount;
        }
        public async Task<string> UpdatePOOrderItem(PoItem poItem)
        {
            Update(poItem);
            string result = $"PoItem of Detail {poItem.Id} is updated successfully!";
            return result;
        }
        public async Task<int?> GetPoItemsPartiallyClosedStatusCount(string poNumber)
        {
            var poItemsPartiallyClosedStatusCount = _tipsPurchaseDbContext.PoItems.Where(x => x.PONumber == poNumber 
                                                            && x.PoStatus == PoStatus.PartiallyClosed).Count();

            return poItemsPartiallyClosedStatusCount;
        }




    }
}

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
using Tips.Purchase.Api.Entities.Enums;

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
            purchaseOrder.LastModifiedBy = _createdBy;
            purchaseOrder.LastModifiedOn = DateTime.Now;
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
                .Where(x => x.PONumber == purchaseOrder.PONumber && x.IsModified ==  false)
                .FirstOrDefault();

            if (getOldPODetails != null)
            {
                getOldPODetails.IsModified=true;
                getOldPODetails.LastModifiedBy = _createdBy;
                getOldPODetails.LastModifiedOn = DateTime.Now;
                Update(getOldPODetails);
            }

            purchaseOrder.CreatedBy = purchaseOrder.CreatedBy;
            purchaseOrder.CreatedOn = purchaseOrder.CreatedOn;
            purchaseOrder.LastModifiedBy = _createdBy;
            purchaseOrder.LastModifiedOn = DateTime.Now;
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

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrderWithItems(PurchaseOrderSearchDto purchaseOrderSearch)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseOrders.Include("POItems");
                if (purchaseOrderSearch != null || (purchaseOrderSearch.PONumber.Any())
               && purchaseOrderSearch.ProcurementType.Any() && purchaseOrderSearch.ShippingMode.Any() 
               && purchaseOrderSearch.VendorName.Any() && purchaseOrderSearch.PoStatus.Any())
               {
                    query = query.Where
                    (po => (purchaseOrderSearch.PONumber.Any() ? purchaseOrderSearch.PONumber.Contains(po.PONumber) : true)
                   && (purchaseOrderSearch.ProcurementType.Any() ? purchaseOrderSearch.ProcurementType.Contains(po.ProcurementType) : true)
                   && (purchaseOrderSearch.ShippingMode.Any() ? purchaseOrderSearch.ShippingMode.Contains(po.ShippingMode) : true)
                   && (purchaseOrderSearch.VendorName.Any() ? purchaseOrderSearch.VendorName.Contains(po.VendorName) : true)
                   && (purchaseOrderSearch.PoStatus.Any() ? purchaseOrderSearch.PoStatus.Contains(po.Status) : true))
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
                    .Include(itm => itm.PoIncoTerms);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseOrders.Include("POItems");
                // int searchValueInt;
                //bool isSearchValueInt = int.TryParse(searchParammes.SearchValue, out searchValueInt);

                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.PONumber.Contains(searchParammes.SearchValue)
                    || po.VendorName.Contains(searchParammes.SearchValue)
                    || po.PODate.ToString().Contains(searchParammes.SearchValue) 
                    //|| po.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
                    || po.ProcurementType.Contains(searchParammes.SearchValue)
                    //|| po.VendorId.Contains(searchParammes.SearchValue)
                    || po.QuotationDate.ToString().Contains(searchParammes.SearchValue)
                    //|| po.QuotationReferenceNumber.Contains(searchParammes.SearchValue)
                    || po.ShippingMode.Contains(searchParammes.SearchValue)
                    || po.PaymentTerms.Contains(searchParammes.SearchValue)
                    || po.DeliveryTerms.Contains(searchParammes.SearchValue)
                    || po.POItems.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue) ||
                    s.Description.Contains(searchParammes.SearchValue)
                    || s.MftrItemNumber.Contains(searchParammes.SearchValue)
                    || s.PONumber.Contains(searchParammes.SearchValue)))//||
                   // (!isSearchValueInt || po.RevisionNumber == searchValueInt))
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
                    .Include(itm => itm.PoIncoTerms);
                }
                return query.ToList();
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
            
                     var Join = from pri in _tipsPurchaseDbContext.PrItems
                                where pri.ItemNumber == itemNumber
                                join pr in _tipsPurchaseDbContext.PurchaseRequisitions on pri.PurchaseRequistionId equals pr.Id
                                where pr.PrApprovalI == true && pr.PrApprovalII == true
                                select new PRNoandQtyListDto
                                {
                                    PRNumber =pr.PrNumber,
                                    Qty = pri.Qty,
                                };
                var postdata = Join.ToList();

            return postdata;
        }
        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            var purchaseOrderDetails = _tipsPurchaseDbContext.PurchaseOrders
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
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
             .Include(itm => itm.PoIncoTerms)
            .ToList();
            return purchaseOrderDetails;        
    }

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

        public async Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrders()
        {
            var activePurchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)
                                .Where(x=>x.Status != Status.Closed)
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
                                .Include(itm => itm.PoIncoTerms);

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
            //IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalIINameList = await _tipsPurchaseDbContext.PurchaseOrders
            //                .Where(x => x.POApprovalII == false && x.IsDeleted == false && x.IsModified ==false).Select(x => new PurchaseOrderIdNameListDto()
            //                {
            //                    Id = x.Id,
            //                    PONumber = x.PONumber,
            //                }).ToListAsync();


            IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalIINameList = await _tipsPurchaseDbContext.PurchaseOrders
            .Where(x => x.POApprovalII == false && x.POApprovalI == true && x.IsDeleted == false && x.IsModified == false)
            .GroupBy(x => x.PONumber)
            .Select(g => new PurchaseOrderIdNameListDto()
            {
                Id = g.OrderByDescending(x => x.RevisionNumber).FirstOrDefault().Id,
                PONumber = g.Key
            })
            .ToListAsync();


            return pendingPOApprovalIINameList;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            var purchaseOrderDetails = FindAll()
    .Where(inv => (
        string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
        inv.VendorName.Contains(searchParams.SearchValue) ||
        inv.PONumber.Contains(searchParams.SearchValue) ||
        inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue)) ||
        inv.PODate.Equals(int.Parse(searchParams.SearchValue))
    ))
    .OrderByDescending(on => on.Id)
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
    .Include(itm => itm.PoIncoTerms);

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
            //                    .Include(itm => itm.PoIncoTerms);

            return PagedList<PurchaseOrder>.ToPagedList(purchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int id)
        {
            var purchaseOrderDetailById = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.Id == id)
                                                .Include(o=>o.POFiles)
                
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
                                 .Include(itm => itm.PoIncoTerms)
                                .FirstOrDefaultAsync();


            return purchaseOrderDetailById;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber)
        {
            var purchaseOrderDetailbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == poNumber &&  x.IsDeleted== false && x.IsModified == false)
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
                                .Include(itm => itm.PoIncoTerms)
                                .FirstOrDefaultAsync();


            return purchaseOrderDetailbyPONumber;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONoAndRevNo(string poNumber,int revisionNumber)
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
                                .Include(itm => itm.PoIncoTerms)
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
            documentUpload.LastModifiedBy = _createdBy;
            documentUpload.LastModifiedOn = DateTime.Now;

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

        public async Task<IEnumerable<PoItem>> GetPODetailsByPONumberandItemNo(string ItemNumber, string PONumber)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContexts.PoItems
                 .Where(x => x.ItemNumber == ItemNumber && x.PONumber == PONumber && x.PoPartsStatus != true)
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
                joinResult.PoItem.PartType == PartType.TG &&
                joinResult.PoItem.PoPartsStatus == true)
            .GroupBy(joinResult => new { joinResult.PoItem.ItemNumber, joinResult.PoItem.PONumber })
            .Select(group => new OpenPurchaseOrderDto
            {
                ItemNumber = group.Key.ItemNumber,
                BalanceQty = group.Sum(c => c.PoItem.BalanceQty),
                PONumber = group.Key.PONumber
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
            string result = $"POOrderItem of Detail {poItem.Id} is updated successfully!";
            return result;
        }


        


    }
}

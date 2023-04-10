using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Misc;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PurchaseOrderRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public async Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var date = DateTime.Now;
            purchaseOrder.CreatedBy = "Admin";
            purchaseOrder.CreatedOn = date.Date;
            //Guid purchaseOrderNumber = Guid.NewGuid();
            //purchaseOrder.PONumber = "PO-" + purchaseOrderNumber.ToString();
            purchaseOrder.Unit = "Banglore";
            purchaseOrder.RevisionNumber = 1;
            var result = await Create(purchaseOrder);
            return result.Id;
        }
        public async Task<PurchaseOrder> ChangePurchaseOrderVersion(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.CreatedBy = "Admin";
            purchaseOrder.CreatedOn = DateTime.Now;
            purchaseOrder.Unit = "Bangalore";
            var getOldRevisionNumber = _tipsPurchaseDbContext.PurchaseOrders
                .Where(x => x.PONumber == purchaseOrder.PONumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            var increaseVersionNumber = 1;
            var convertversionnumber = (increaseVersionNumber);
            var version = getOldRevisionNumber + convertversionnumber;
            purchaseOrder.RevisionNumber = (version);
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
                var query = _tipsPurchaseDbContext.PurchaseOrders.Include("POItemList");
                if (purchaseOrderSearch != null || (purchaseOrderSearch.PONumber.Any())
               && purchaseOrderSearch.ProcurementType.Any() && purchaseOrderSearch.ShippingMode.Any() 
               && purchaseOrderSearch.VendorName.Any() && purchaseOrderSearch.Status.Any())
               {
                    query = query.Where
                    (po => (purchaseOrderSearch.PONumber.Any() ? purchaseOrderSearch.PONumber.Contains(po.PONumber) : true)
                   && (purchaseOrderSearch.ProcurementType.Any() ? purchaseOrderSearch.ProcurementType.Contains(po.ProcurementType) : true)
                   && (purchaseOrderSearch.ShippingMode.Any() ? purchaseOrderSearch.ShippingMode.Contains(po.ShippingMode) : true)
                   && (purchaseOrderSearch.VendorName.Any() ? purchaseOrderSearch.VendorName.Contains(po.VendorName) : true)
                   && (purchaseOrderSearch.Status.Any() ? purchaseOrderSearch.Status.Contains(po.Status) : true));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseOrders.Include("PoItems");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.PONumber.Contains(searchParammes.SearchValue)
                    || po.VendorName.Contains(searchParammes.SearchValue)
                    || po.PODate.ToString().Contains(searchParammes.SearchValue)
                    || po.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
                    || po.ProcurementType.Contains(searchParammes.SearchValue)
                    || po.VendorId.Contains(searchParammes.SearchValue)
                    || po.QuotationDate.ToString().Contains(searchParammes.SearchValue)
                    || po.QuotationReferenceNumber.Contains(searchParammes.SearchValue)
                    || po.ShippingMode.Contains(searchParammes.SearchValue)
                    || po.PaymentTerms.Contains(searchParammes.SearchValue)
                    || po.DeliveryTerms.Contains(searchParammes.SearchValue)
                    || po.POItemList.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue) ||
                    s.Description.Contains(searchParammes.SearchValue)
                    || s.MftrItemNumber.Contains(searchParammes.SearchValue)
                    || s.PONumber.Contains(searchParammes.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            var purchaseOrderDetails = _tipsPurchaseDbContext.PurchaseOrders
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.POItemList)
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

        public async Task<PagedList<PurchaseOrder>> GetAllActivePurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {



            var activePurchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
               || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
               || inv.PODate.Equals(int.Parse(searchParams.SearchValue)))))
                                .Include(o => o.POFiles)
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules);
            return PagedList<PurchaseOrder>.ToPagedList(activePurchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalINameList = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalI == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return pendingPOApprovalINameList;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> pendingPOApprovalIINameList = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalII == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return pendingPOApprovalIINameList;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {

            var purchaseOrderDetails = FindAll().OrderByDescending(on => on.Id)

               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
               || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
               || inv.PODate.Equals(int.Parse(searchParams.SearchValue)))))
                                .Include(o => o.POFiles)
                               .Include(t => t.POItemList)
                               .ThenInclude(x => x.POAddprojects)
                               .Include(m => m.POItemList)
                               .ThenInclude(i => i.POAddDeliverySchedules);
            return PagedList<PurchaseOrder>.ToPagedList(purchaseOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int id)
        {
            var purchaseOrderDetailById = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.Id == id)
                                                .Include(o=>o.POFiles)
                
                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderDetailById;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber)
        {
            var purchaseOrderDetailbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.PONumber == poNumber)
                .Include(o => o.POFiles)
                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderDetailbyPONumber;
        }

        public async Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.LastModifiedBy = "Admin";
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
            IEnumerable<PurchaseOrderIdNameListDto> pONameListbyVendorId = await _tipsPurchaseDbContext.PurchaseOrders
                           .Where(x => x.VendorId == vendorId).Select(x => new PurchaseOrderIdNameListDto()
                           {
                               Id = x.Id,
                               PONumber = x.PONumber
                           }).ToListAsync();


            return pONameListbyVendorId;
        }

    }

    public class UploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContexts;
        public UploadDocumentRepository(TipsPurchaseDbContext tipsPurchaseDbContext) : base(tipsPurchaseDbContext)
        {
            _tipsPurchaseDbContexts = tipsPurchaseDbContext;
        }
         
        public async Task<int?> CreateUploadDocumentPO(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = "Admin";
            documentUpload.CreatedOn = DateTime.Now;
            documentUpload.LastModifiedBy = "Admin";
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
        public PurchaseOrderItemRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContexts = repositoryContext;
        }

        public async Task<IEnumerable<PoItem>> GetPODetailsByPONumberandItemNo(string ItemNumber, string PONumber)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContexts.PoItems
                 .Where(x => x.ItemNumber == ItemNumber && x.PONumber == PONumber && x.PoPartsStatus != true)
                          .ToListAsync();

            return getPODetailsByPONOandItemNo;
        }

        //get count po item for particular po number

        public async Task<int?> GetPODetailsByPONumber(string PONumber)
        {
            var getPODetailsByPONO = _tipsPurchaseDbContexts.PoItems.Where(x => x.PONumber == PONumber).Count();

            return getPODetailsByPONO;
        }


        public async Task<string> UpdatePOOrderItem(PoItem poItem)
        {
            Update(poItem);
            string result = $"POOrderItem of Detail {poItem.Id} is updated successfully!";
            return result;
        } 
    }
}

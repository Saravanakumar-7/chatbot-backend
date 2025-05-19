using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Entities;
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Grin.Api.Repository
{
    public class GrinRepository : RepositoryBase<Grins>, IGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public GrinRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<int?> CreateGrin(Grins grins)
        {
            //var date = DateTime.Now;
            grins.CreatedBy = _createdBy;
            grins.CreatedOn = DateTime.Now;
            //Guid grinId = Guid.NewGuid();
            //grins.GrinNumber = "GR-" + grinId.ToString();
            grins.Unit = _unitname;

            var result = await Create(grins);
            return result.Id;
        }

        /// <summary>
        /// This Method should be changed ,because it will create duplicate GrinNumber
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<int?> GetGrinNumberAutoIncrementCount(DateTime date)
        {
            var getGrinNumberAutoIncrementCount = _tipsGrinDbContext.Grins.Where(x => x.CreatedOn == date.Date).Count();

            return getGrinNumberAutoIncrementCount;
        }

        public async Task<int?> GetGrinIqcStatusCount(string grinNo)
        {
            var grinIqcStatusCount = _tipsGrinDbContext.Grins.Where(x => x.GrinNumber == grinNo && x.IsIqcCompleted == true).Count();

            return grinIqcStatusCount;
        }
        public async Task<int?> GetGrinbinningStatusCount(string grinNo)
        {
            var grinBinningStatusCount = _tipsGrinDbContext.Grins.Where(x => x.GrinNumber == grinNo && x.IsBinningCompleted == true).Count();

            return grinBinningStatusCount;
        }

        public async Task<string> GenerateGrinNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.GrinNumbers.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"G-{grinNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        //public async Task<IEnumerable<Grin_ReportSP>> GetGrinSPReportWithParam(string GrinNumber, string VendorName, string PONumber, string KPN, string MPN, string Warehouse, string Location)
        //{
        //    {

        //        if (string.IsNullOrWhiteSpace(GrinNumber)
        //      || string.IsNullOrWhiteSpace(VendorName)
        //      || string.IsNullOrWhiteSpace(PONumber)
        //      || string.IsNullOrWhiteSpace(KPN)
        //      || string.IsNullOrWhiteSpace(MPN)
        //      || string.IsNullOrWhiteSpace(Warehouse)
        //      || string.IsNullOrWhiteSpace(Location)) ;


        //        var result = _tipsGrinDbContext
        //        .Set<Grin_ReportSP>()
        //        .FromSqlInterpolated($"CALL Grin_Report_withparameter({GrinNumber},{VendorName},{PONumber},{KPN},{MPN},{Warehouse},{Location})")
        //        .ToList();

        //        return result;
        //    }
        //}

        public async Task<PagedList<Grin_ReportSP>> GetGrinSPReport(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<Grin_ReportSP>()
                      .FromSqlInterpolated($"CALL Grin_Report")
                      .ToList();

            return PagedList<Grin_ReportSP>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<GrinSPReportForTrans>> GetGrinSPReportForTrans(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<GrinSPReportForTrans>()
                      .FromSqlInterpolated($"CALL Grin_Report_tras")
                      .ToList();

            return PagedList<GrinSPReportForTrans>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<PurchaseInventorySPReport>> GetPurchaseInventorySPReportWithParam(string? InvoiceNumber, string? GRINNumber, string? KPN, string? VendorName)
        {
            var result = _tipsGrinDbContext
            .Set<PurchaseInventorySPReport>()
            .FromSqlInterpolated($"CALL Purchase_With_Inventory({InvoiceNumber},{GRINNumber},{KPN},{VendorName})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<Grin_ReportSP>> GetGrinSPReportWithParam(string? GrinNumber, string? VendorName, string? PONumber,
                                                                                                    string? KPN, string? MPN, string? Warehouse, string? Location)
        {
            var result = _tipsGrinDbContext
            .Set<Grin_ReportSP>()
            .FromSqlInterpolated($"CALL Grin_Report_withparameter({GrinNumber},{VendorName},{PONumber},{KPN},{MPN},{Warehouse},{Location})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<GrinSPReportForTrans>> GetGrinSPReportWithParamForTrans(string? GrinNumber, string? VendorName, string? PONumber,
                                                                                                    string? ItemNumber, string? MPN, string? ProjectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<GrinSPReportForTrans>()
            .FromSqlInterpolated($"CALL Grin_Report_withparameter_tras({GrinNumber},{VendorName},{PONumber},{ItemNumber},{MPN},{ProjectNumber})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<GrinSPReportForAvi>> GetGrinSPReportWithParamForAvi(string? GrinNumber, string? VendorName, string? PONumber,
                                                                                                    string? ItemNumber, string? MPN, string? Warehouse, string? Location, string? ProjectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<GrinSPReportForAvi>()
            .FromSqlInterpolated($"CALL Grin_Report_withparameter_Avi({GrinNumber},{VendorName},{PONumber},{ItemNumber},{MPN},{Warehouse},{Location},{ProjectNumber})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<Grin_ReportSP>> GetGrinSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<Grin_ReportSP>()
                      .FromSqlInterpolated($"CALL Grin_Report_withparameter_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<GrinSPReportForTrans>> GetGrinSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<GrinSPReportForTrans>()
                      .FromSqlInterpolated($"CALL Grin_Report_withparameter_withdate_tras({FromDate},{ToDate})")
                      .ToList();

            return results;
        }

        public async Task<IEnumerable<GrinSPReportForAvi>> GetGrinSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<GrinSPReportForAvi>()
                      .FromSqlInterpolated($"CALL Grin_Report_withparameter_withdate_Avi({FromDate},{ToDate})")
                      .ToList();

            return results;
        }

        public async Task<IEnumerable<PurchaseInventorySPReport>> GetPurchaseInventorySPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<PurchaseInventorySPReport>()
                      .FromSqlInterpolated($"CALL Purchase_With_Inventory_with_Date({FromDate},{ToDate})")
                      .ToList();

            return results;
        }

        public async Task<string> GenerateGrinNumberForAvision()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.GrinNumbers.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                //int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) // Check if the current date is after or equal to April
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100; // Get the last two digits of the current finance year
                int nextYear = (financeYearStart.Year + 1) % 100; // Get the last two digits of the next finance year

                return $"ASPL|GRN|{currentYear:D2}-{nextYear:D2}|{grinNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<List<GrinComsumpReportDto>> GetGrinComsumptionDetialsByPartNos(List<string> PartNoListString)
        {
            var result = await _tipsGrinDbContext.GrinParts
                .Where(x => PartNoListString.Contains(x.ItemNumber))
                .Select(x => new GrinComsumpReportDto
                {
                    GrinNumber = x.Grins.GrinNumber,
                    GrinDate = x.Grins.CreatedOn,
                    VendorName = x.Grins.VendorName,
                    PONumber = x.PONumber,  
                    GrinQty = x.AcceptedQty,
                    GrinUnitPrice = x.UnitPrice,
                    Tax = (x.SGST ?? 0) + (x.IGST ?? 0) + (x.CGST ?? 0) + (x.UTGST ?? 0) + (x.Duties ?? 0),
                    OtherCosts = (x.Grins.Freight ?? 0) + (x.Grins.Insurance ?? 0) + (x.Grins.LoadingorUnLoading ?? 0) + (x.Grins.Transport ?? 0),
                    UOM = x.UOM,
                    UOC = x.UOC,
                    PartNumber = x.ItemNumber 
                })
                .ToListAsync();

            var groupedResult = result
                .GroupBy(r => r.PartNumber)
                .Select(group => new GrinComsumpReportDto
                {
                    PartNumber = group.Key, 
                    GrinNumber = group.FirstOrDefault()?.GrinNumber,
                    GrinDate = group.FirstOrDefault()?.GrinDate,
                    VendorName = group.FirstOrDefault()?.VendorName,
                    PONumber = group.FirstOrDefault()?.PONumber,
                    GrinQty = group.Sum(x => x.GrinQty),
                    GrinUnitPrice = group.Sum(x => x.GrinUnitPrice),
                    Tax = group.Sum(x => x.Tax),
                    OtherCosts = group.Sum(x => x.OtherCosts),
                    UOM = group.FirstOrDefault()?.UOM,
                    UOC = group.FirstOrDefault()?.UOC
                })
                .ToList();

            return groupedResult;
        }



        public async Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string grinNumber)
        {

            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(b => b.ParentId == grinNumber)
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

        public async Task<string> DeleteGrin(Grins grins)
        {
            Delete(grins);
            string result = $"Grin details of {grins.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberWhereBinningComplete()
        {
            IEnumerable<GrinNoForIqcAndBinning> grinNoForBinning = await _tipsGrinDbContext.Grins
                .Where(x => x.IsGrinCompleted == true && x.IsBinningCompleted == true)
                                .Select(x => new GrinNoForIqcAndBinning()
                                {
                                    GrinNumber = x.GrinNumber,
                                    GrinId = x.Id
                                })
                              .ToListAsync();

            return grinNoForBinning;
        }

        //public async Task<PagedList<Grins>> GetAllActiveGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //    var allActiveGrinDetails = FindAll()
        //        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
        //           inv.VendorId.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))))
        //         .Include(t => t.GrinParts);
        //    return PagedList<Grins>.ToPagedList(allActiveGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        public async Task<IEnumerable<Grins>> GetAllActiveGrin()
        {
            var allActiveGrin = await FindAll()
            .Include(t => t.GrinParts)
            .ToListAsync();
            return allActiveGrin;
        }

        public async Task<IEnumerable<GrinNumberListDto>> GetAllActiveGrinNoList()
        {
            IEnumerable<GrinNumberListDto> allActiveGrinNoList = await _tipsGrinDbContext.Grins
                                .Select(x => new GrinNumberListDto()
                                {

                                    Id = x.Id,
                                    GrinNumber = x.GrinNumber,
                                    InvoiceNumber = x.InvoiceNumber
                                })
                              .ToListAsync();

            return allActiveGrinNoList;
        }

        public async Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberForIqc()
        {
            //IEnumerable<GrinNoForIqcAndBinning> grinNoForIqc = await _tipsGrinDbContext.Grins
            //    .Where(x => x.IsGrinCompleted == true && x.IsIqcCompleted == false && x.IsBinningCompleted == false)
            //                    .Select(x => new GrinNoForIqcAndBinning()
            //                    {
            //                        GrinNumber = x.GrinNumber,
            //                        GrinId = x.Id
            //                    })
            //                  .ToListAsync();
            var grinparts = await _tipsGrinDbContext.GrinParts.Where(x => x.IsIqcCompleted == false && x.IsBinningCompleted == false).Select(x => x.GrinsId).ToListAsync();
            var gId = grinparts.Distinct().ToList();
            List<GrinNoForIqcAndBinning> grinNoForIqc = await _tipsGrinDbContext.Grins.Where(x => gId.Contains(x.Id)).Select(x => new GrinNoForIqcAndBinning()
            {
                GrinNumber = x.GrinNumber,
                GrinId = x.Id
            }).ToListAsync();
            return grinNoForIqc;
        }

        public async Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberForBinning()
        {
            //IEnumerable<GrinNoForIqcAndBinning> grinNoForBinning = await _tipsGrinDbContext.Grins
            //    .Where(x => x.IsGrinCompleted == true && x.IsIqcCompleted == true && x.IsBinningCompleted == false)
            //                    .Select(x => new GrinNoForIqcAndBinning()
            //                    {
            //                        GrinNumber = x.GrinNumber,
            //                        GrinId = x.Id
            //                    })
            //                  .ToListAsync();
            var grinparts = await _tipsGrinDbContext.GrinParts.Where(x => x.IsIqcCompleted == true && x.IsBinningCompleted == false).Select(x => x.GrinsId).ToListAsync();
            var gId = grinparts.Distinct().ToList();
            List<GrinNoForIqcAndBinning> grinNoForBinning = await _tipsGrinDbContext.Grins.Where(x => gId.Contains(x.Id)).Select(x => new GrinNoForIqcAndBinning()
            {
                GrinNumber = x.GrinNumber,
                GrinId = x.Id
            }).ToListAsync();

            return grinNoForBinning;
        }

        public async Task<IEnumerable<Grins>> GetAllGrinsWithItems(GrinSearchDto grinSearchDto)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.Grins.Include("GrinParts");
                if (grinSearchDto != null || (grinSearchDto.InvoiceNumber.Any())
               && grinSearchDto.GrinNumber.Any() && grinSearchDto.VendorName.Any() && grinSearchDto.VendorId.Any())
                {
                    query = query.Where
                    (po => (grinSearchDto.GrinNumber.Any() ? grinSearchDto.GrinNumber.Contains(po.GrinNumber) : true)
                   && (grinSearchDto.InvoiceNumber.Any() ? grinSearchDto.InvoiceNumber.Contains(po.InvoiceNumber) : true)
                   && (grinSearchDto.VendorName.Any() ? grinSearchDto.VendorName.Contains(po.VendorName) : true)
                   && (grinSearchDto.VendorId.Any() ? grinSearchDto.VendorId.Contains(po.VendorId) : true))
                    .Include(item => item.GrinParts)
                    .ThenInclude(pr => pr.ProjectNumbers)
                    .Include(item => item.GrinParts)
                    .Include(o => o.OtherCharges);
                    //.Include(item => item.GrinDocuments);
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<Grins>> SearchGrinsDate([FromQuery] SearchDateParames searchParames)
        {
            var grinDetails = _tipsGrinDbContext.Grins
            .Where(inv => ((inv.CreatedOn >= searchParames.SearchFromDate &&
            inv.CreatedOn <= searchParames.SearchToDate
            )))
            .Include(itm => itm.GrinParts)
            .ThenInclude(pr => pr.ProjectNumbers)
            .Include(item => item.GrinParts)
                    .Include(o => o.OtherCharges)
            //.Include(item => item.GrinDocuments)
            .ToList();
            return grinDetails;
        }
        public async Task<IEnumerable<Grins>> SearchGrins([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.Grins.Include("GrinParts");
                if (!string.IsNullOrEmpty(Convert.ToString(searchParames.SearchValue)))
                {
                    query = query.Where(po => po.GrinNumber.Contains(searchParames.SearchValue)
                    || po.VendorName.Contains(searchParames.SearchValue)
                    || po.InvoiceDate.ToString().Contains(searchParames.SearchValue)
                                   //|| po.InvoiceNumber.Equals(int.Parse(searchParames.SearchValue))
                                   || po.VendorId.Contains(searchParames.SearchValue)
                    || po.GrinParts.Any(s => s.ItemNumber.Contains(searchParames.SearchValue) ||
                    s.ItemDescription.Contains(searchParames.SearchValue)
                    || s.MftrItemNumber.Contains(searchParames.SearchValue)
                    || s.PONumber.Contains(searchParames.SearchValue)))
                        .Include(item => item.GrinParts)
                    .ThenInclude(pr => pr.ProjectNumbers)
                    .Include(item => item.GrinParts)
                    //.ThenInclude(pr => pr.CoCUpload)
                    .Include(o => o.OtherCharges);
                    //.Include(item => item.GrinDocuments); 
                }
                return query.ToList();
            }
        }
        public async Task<PagedList<Grins>> GetAllGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
                 inv.InvoiceNumber.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))));
            //.Include(t => t.GrinDocuments)
            //.Include(t => t.GrinParts)
            ////.ThenInclude(c=>c.CoCUpload)
            //.Include(t => t.GrinParts)
            // .ThenInclude(d => d.ProjectNumbers);

            return PagedList<Grins>.ToPagedList(getAllGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //var getAllGrinDetails = FindAll().OrderByDescending(x => x.Id)
            //  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
            //     inv.VendorId.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))));
            //  //.Include(t => t.GrinDocuments)
            //  //.Include(t => t.GrinParts)
            //  ////.ThenInclude(c=>c.CoCUpload)
            //  //.Include(t => t.GrinParts)
            //  // .ThenInclude(d => d.ProjectNumbers);

            //return PagedList<Grins>.ToPagedList(getAllGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<Grins> GetGrinById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.Grins.Where(x => x.Id == id)
              .Include(t => t.GrinParts)
               .ThenInclude(d => d.ProjectNumbers)
               .Include(d => d.OtherCharges).FirstOrDefaultAsync();

            return grinDetailsbyId;
        }
        public async Task<IEnumerable<Grins>> GetGrinDetailsByGrinIds(List<int> grinIds)
        {
            var grinDetailsList = await FindByCondition(x => grinIds.Contains(x.Id)).ToListAsync();
            return grinDetailsList;
        }
        public async Task<Grins> GetGrinByGrinNo(string grinNumber)
        {
            var grinDetailsbyGrin = await _tipsGrinDbContext.Grins.Where(x => x.GrinNumber == grinNumber)

        .Include(t => t.GrinParts)
        .ThenInclude(p => p.ProjectNumbers)

        .FirstOrDefaultAsync();

            return grinDetailsbyGrin;
        }

        public async Task<string> UpdateGrin(Grins grins)
        {
            grins.LastModifiedBy = _createdBy;
            grins.LastModifiedOn = DateTime.Now;
            Update(grins);
            string result = $"Grin Detail {grins.Id} is updated successfully!";
            return result;
        }
        public async Task<string> UpdateGrin_ForTally(Grins grins)
        {
            Update(grins);
            string result = $"Grin Detail {grins.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<Grins>> GetGrinDetailsofPOByGrinIds(List<int> grinIds,string Ponumber)
        {
            var grinDetailsList = await FindByCondition(x => grinIds.Contains(x.Id))
                .Include(item => item.GrinParts.Where(x=>x.PONumber==Ponumber))
                    .ThenInclude(pr => pr.ProjectNumbers)
                    .Include(item => item.GrinParts.Where(x => x.PONumber == Ponumber))
                    .Include(o => o.OtherCharges)
                .ToListAsync();
            return grinDetailsList;
        }

    }
    public class UploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UploadDocumentRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<DocumentUpload> GetUploadDocById(int id)
        {
            var grinUploadDocFileNameById = await _tipsGrinDbContext.DocumentUploads
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return grinUploadDocFileNameById;
        }
        public async Task<string> DeleteUploadFile(DocumentUpload documentUpload)
        {
            Delete(documentUpload);
            string result = $"DocumentUpload details of {documentUpload.Id} is deleted successfully!";
            return result;
        }



        public async Task<int?> CreateUploadDocumentGrin(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = _createdBy;
            documentUpload.CreatedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }


        public async Task<IEnumerable<GetDownloadUrlDto>> GetGrinDownloadUrlDetails(string grinNumber)
        {

            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(x => x.ParentId == grinNumber)
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

        public async Task<IEnumerable<GetDownloadUrlDto>> GetGrinPartsDownloadUrlDetails(string grinNumber)
        {
            var grinnumbers = grinNumber + "-" + "I";
            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(x => x.ParentId == grinnumbers)
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

        public async Task<string> DeleteGrinPartsUploadDocByGrinNo(string grinnumber)
        {
            var documentDetails = await _tipsGrinDbContext.DocumentUploads.Where(x => x.ParentId == grinnumber).FirstOrDefaultAsync();
            Delete(documentDetails);
            string result = $"DocumentUpload details of {documentDetails.Id} is deleted successfully!";
            return result;
        }
        public async Task<int?> GetDocumentDetailsByGrinNo(string grinnumber)
        {
            var grinUploadDocFileNameById = _tipsGrinDbContext.DocumentUploads
               .Where(x => x.ParentId == grinnumber).Count();

            return grinUploadDocFileNameById;
        }
        public async Task<List<DocumentUploadDto>> GetDownloadUrlDetails(string FileIds)
        {
            List<DocumentUploadDto> fileUploads = new List<DocumentUploadDto>();
            if (FileIds != null)
            {
                string[]? ids = FileIds.Split(',');

                for (int i = 0; i < ids.Count(); i++)
                {
                    DocumentUploadDto? getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(b => b.Id == Convert.ToInt32(ids[i]))
                                .Select(x => new DocumentUploadDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                }).FirstOrDefaultAsync();
                    if (getDownloadDetails != null)
                        fileUploads.Add(getDownloadDetails);
                }
            }
            return fileUploads;
        }
    }

}

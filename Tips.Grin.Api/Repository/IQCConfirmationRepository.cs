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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Tips.Grin.Api.Controllers;

namespace Tips.Grin.Api.Repository
{
    public class IQCConfirmationRepository : RepositoryBase<IQCConfirmation>, IIQCConfirmationRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IQCConfirmationRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateIqc(IQCConfirmation iQCConfirmation)
        {
            iQCConfirmation.CreatedBy = _createdBy;
            iQCConfirmation.CreatedOn = DateTime.Now;
            iQCConfirmation.Unit = _unitname;
            var result = await Create(iQCConfirmation);
            return result.Id;
        }

        public async Task<PagedList<IQCConfirmation_SPReport>> GetIQCConfirmationSPReport(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<IQCConfirmation_SPReport>()
                      .FromSqlInterpolated($"CALL iqc_confirmation_without_parameter")
                      .ToList();

            return PagedList<IQCConfirmation_SPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportForTrans(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<IQCConfirmationSPReportForTrans>()
                      .FromSqlInterpolated($"CALL iqc_confirmation_without_parameter_tras")
                      .ToList();

            return PagedList<IQCConfirmationSPReportForTrans>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<IEnumerable<IQCConfirmation_SPReport>> GetIQCConfirmationSPReportWithParam(string? GrinNumber, string? itemNo)
        {
            var result = _tipsGrinDbContext
            .Set<IQCConfirmation_SPReport>()
            .FromSqlInterpolated($"CALL iqc_confirmation_with_parameters({GrinNumber},{itemNo})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportWithParamForTrans(string? GrinNumber, string? itemNo, string? ProjectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<IQCConfirmationSPReportForTrans>()
            .FromSqlInterpolated($"CALL iqc_confirmation_with_parameters_tras({GrinNumber},{itemNo},{ProjectNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<IQCConfirmation_SPReport>> GetIQCConfirmationSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<IQCConfirmation_SPReport>()
                      .FromSqlInterpolated($"CALL iqc_confirmation_with_parameter_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<IQCConfirmationSPReportForTrans>()
                      .FromSqlInterpolated($"CALL iqc_confirmation_with_parameter_withdate_tras({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<IQCConfirmation>> SearchIQCConfirmation([FromQuery] SearchParames searchParames)
        {
           
                var query = _tipsGrinDbContext.IQCConfirmations.Include("IQCConfirmationItems");
                if (!string.IsNullOrEmpty(Convert.ToString(searchParames.SearchValue)))
                {
                    query = query.Where(po => po.GrinNumber.Contains(Convert.ToString(searchParames.SearchValue))
                    || po.IQCConfirmationItems.Any(s => s.ItemNumber.Contains(Convert.ToString(searchParames.SearchValue))));
                }
                return query.ToList();
          
        }
        public async Task<IEnumerable<IQCConfirmation>> SearchIQCConfirmationDate([FromQuery] SearchDateParames searchParames)
        {
            var iQcDetails = _tipsGrinDbContext.IQCConfirmations
            .Where(inv => ((inv.CreatedOn >= searchParames.SearchFromDate &&
            inv.CreatedOn <= searchParames.SearchToDate
            )))
            .Include(itm => itm.IQCConfirmationItems)
            .ToList();
            return iQcDetails;
        }

        public async Task<IEnumerable<IQCConfirmation>> GetAllIQCConfirmationWithItems(IQCConfirmationSearchDto iQCConfirmationSearch)
        {
            var query = _tipsGrinDbContext.IQCConfirmations.Include("IQCConfirmationItems");
            if (iQCConfirmationSearch != null || (iQCConfirmationSearch.InvoiceNumber.Any())
           && iQCConfirmationSearch.GrinNumber.Any() && iQCConfirmationSearch.VendorName.Any()
           && iQCConfirmationSearch.VendorId.Any())

            {
                query = query.Where
                (po => (iQCConfirmationSearch.GrinNumber.Any() ? iQCConfirmationSearch.GrinNumber.Contains(po.GrinNumber) : true));
                   //&& (iQCConfirmationSearch.InvoiceNumber.Any() ? iQCConfirmationSearch.InvoiceNumber.Contains(po.InvoiceNumber) : true)
                   //&& (iQCConfirmationSearch.VendorName.Any() ? iQCConfirmationSearch.VendorName.Contains(po.VendorName) : true)
                   //&& (iQCConfirmationSearch.VendorId.Any() ? iQCConfirmationSearch.VendorId.Contains(po.VendorId) : true));
            }
            return query.ToList();
        }

        //public async Task<PagedList<IQCConfirmation>> GetAllIqcDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //    var getallIQCList = FindAll()
        //        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || (inv.GrinNumber != null && inv.GrinNumber.Contains(searchParams.SearchValue)) ||
        //        (!string.IsNullOrEmpty(inv.VendorName) && inv.VendorName.Contains(searchParams.SearchValue)) || (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))
        //         || (inv.InvoiceNumber != null && inv.InvoiceNumber.Contains(searchParams.SearchValue)))))
        //         .Include(t => t.IQCConfirmationItems);

        //    return PagedList<IQCConfirmation>.ToPagedList(getallIQCList, pagingParameter.PageNumber, pagingParameter.PageSize);

        //}

        public async Task<PagedList<IQCConfirmation>> GetAllIqcDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getallIQCList = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || (inv.GrinNumber != null && inv.GrinNumber.Contains(searchParams.SearchValue)) 
                ||(inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue)
                   || (inv.VendorName != null && inv.VendorName.Contains(searchParams.SearchValue))
                    || (inv.VendorNumber != null && inv.VendorName.Contains(searchParams.SearchValue)) ||
                   (inv.VendorId != null && inv.VendorId.ToString().Contains(searchParams.SearchValue))
                )))).OrderByDescending(x=>x.Id);
                 

            return PagedList<IQCConfirmation>.ToPagedList(getallIQCList, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IQCConfirmation> GetIqcDetailsbyGrinNo(string grinNumber)
        { 
            var iQCDetail = await _tipsGrinDbContext.IQCConfirmations
                .Where(x => x.GrinNumber == grinNumber)
               .Include(t => t.IQCConfirmationItems)
             
                       .FirstOrDefaultAsync();
            return iQCDetail;
        }

        public async Task<string> UpdateIqc(IQCConfirmation iQCConfirmation)
        {
            iQCConfirmation.LastModifiedBy = _createdBy;
            iQCConfirmation.LastModifiedOn = DateTime.Now;
            Update(iQCConfirmation);
            string result = $"IQC details of {iQCConfirmation.Id} is updated successfully!";
            return result;
        }


        public async Task<IQCConfirmation> GetIqcDetailsbyId(int id)
        {
            var iQCDetailsbyId = await _tipsGrinDbContext.IQCConfirmations.Where(x => x.Id == id)
                                .Include(x => x.IQCConfirmationItems)
                                .FirstOrDefaultAsync();
            return iQCDetailsbyId;
        }

        public async Task<IEnumerable<IQCConfirmationIdNameListDto>> GetAllActiveIQCConfirmationNameList()
        {
            IEnumerable<IQCConfirmationIdNameListDto> activeIQCConfirmationNameList = await _tipsGrinDbContext.IQCConfirmations
                                .Select(x => new IQCConfirmationIdNameListDto()
                                {
                                    Id = x.Id,
                                    GrinNumber = x.GrinNumber,
                                })
                              .ToListAsync();

            return activeIQCConfirmationNameList;
        }
        public async Task<List<IQCConfirmation>> GetIqcDetailsByGrinNoAndParts(Dictionary<string, List<string>> Grins)
        {
            var iQCDetails = await _tipsGrinDbContext.IQCConfirmations
                .Where(iqc => Grins.Keys.Contains(iqc.GrinNumber)) // Check if GrinNumber exists in the dictionary keys
                .Include(iqc => iqc.IQCConfirmationItems)          // Include the IQCConfirmationItems
                .ToListAsync();

            // Filter the items based on the PartNumber
            foreach (var iqc in iQCDetails)
            {
                iqc.IQCConfirmationItems = iqc.IQCConfirmationItems
                    .Where(item => Grins[iqc.GrinNumber].Contains(item.ItemNumber)) // Filter parts based on the dictionary values
                    .ToList();
            }

            return iQCDetails;
        }

    }

    public class IQCConfirmationItemsRepository : RepositoryBase<IQCConfirmationItems>, IIQCConfirmationItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public IQCConfirmationItemsRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        //public async Task<PagedList<IQCConfirmationItems>> GetAllIQCConfirmationItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //    var getAllIqcItems = FindAll()
        //     .Where(iqc => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || iqc.ItemNumber.Contains(searchParams.SearchValue) ||
        //     iqc.Description.Contains(searchParams.SearchValue) || iqc.MftrItemNumber.Contains(searchParams.SearchValue) || iqc.ManufactureBatchNumber.Contains(searchParams.SearchValue)
        //     )));

        //    return PagedList<IQCConfirmationItems>.ToPagedList(getAllIqcItems, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<IQCConfirmationItems>> GetAllIQCConfirmationItems()
        {
            var GetAllIQCConfirmationItems = await _tipsGrinDbContext.IQCConfirmationItems.ToListAsync();
            return GetAllIQCConfirmationItems;
        }
        public async Task<int?> CreateIqcItem(IQCConfirmationItems iQCConfirmationItems)
        {
            var result = await Create(iQCConfirmationItems);
            return result.Id;
        }
        public async Task<IQCConfirmationItems> GetIQCConfirmationItemsDetailsbyGrinPartId(int GrinPartId)
        {
            var grinPartsDetails = await _tipsGrinDbContext.IQCConfirmationItems.Where(x => x.GrinPartId == GrinPartId)
                .FirstOrDefaultAsync();
            return grinPartsDetails;
        }
        public async Task<string> UpdateIqcItems(IQCConfirmationItems iqcConfirmationItems)
        {
            Update(iqcConfirmationItems);
            string result = $"IQCItems details of {iqcConfirmationItems.Id} is updated successfully!";
            return result;
        }
        public async Task<int?> GetIQCConformationItemsCount(int iqcId)
        {
            var grinPartsBinningStatusCount = _tipsGrinDbContext.IQCConfirmationItems.Where(x => x.IQCConfirmationId == iqcId).Count();

            return grinPartsBinningStatusCount;
        }
    }
}







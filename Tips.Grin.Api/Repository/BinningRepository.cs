using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace Tips.Grin.Api.Repository
{
    public class BinningRepository : RepositoryBase<Binning>, IBinningRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BinningRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<Binning> CreateBinning(Binning binning)
        {

            binning.CreatedBy = _createdBy;
            binning.CreatedOn = DateTime.Now;
            binning.Unit = _unitname;
            var result = await Create(binning);
            return result;
        }
        //public async Task<PagedList<Binning>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //public async Task<PagedList<GrinAndBinningDetailsDto>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //    var grinDetails = from e in _tipsGrinDbContext.Grins
        //                      join d in _tipsGrinDbContext.Binnings on e.GrinNumber equals d.GrinNumber into dept
        //                      from Binnings in dept.DefaultIfEmpty()
        //                      select new GrinAndBinningDetailsDto
        //                      {
        //                          Id = e.Id,
        //                          GrinNumber = e.GrinNumber,
        //                          InvoiceNumber = e.InvoiceNumber,
        //                          VendorName = e.VendorName,
        //                          LastModifiedOn = Binnings.LastModifiedOn,
        //                          LastModifiedBy = Binnings.LastModifiedBy

        //                      };

        //    if (!string.IsNullOrWhiteSpace(searchParams.SearchValue))
        //    {
        //        string searchTerm = searchParams.SearchValue.Trim();

        //        grinDetails = grinDetails.Where(dto =>
        //            dto.GrinNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        //            dto.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        //            dto.VendorName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        //        );
        //    }

        //    return PagedList<GrinAndBinningDetailsDto>.ToPagedList(grinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        //public async Task<PagedList<GrinAndBinningDetailsDto>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{


        //    var grinDetails = from e in _tipsGrinDbContext.Grins
        //                      join d in _tipsGrinDbContext.Binnings on e.GrinNumber equals d.GrinNumber into dept
        //                      from Binnings in dept.DefaultIfEmpty()
        //                      select new GrinAndBinningDetailsDto
        //                      {
        //                          GrinNumber = e.GrinNumber,
        //                          InvoiceNumber = e.InvoiceNumber,
        //                          VendorName = e.VendorName
        //                      };

        //    return PagedList<GrinAndBinningDetailsDto>.ToPagedList(grinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //    //var getAllBinningDetails = FindAll()
        //    //   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue))));

        //    //   return PagedList<Binning>.ToPagedList(getAllBinningDetails, pagingParameter.PageNumber, pagingParameter.PageSize);


        //}

        public async Task<PagedList<GrinAndBinningDetailsDto>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {

            List<string> grinNumberList = _tipsGrinDbContext.Grins.Select(x => x.GrinNumber).ToList();

            var binningGrinNoList = await _tipsGrinDbContext.Binnings
                                    .Where(x => grinNumberList.Contains(x.GrinNumber))
                                    .Select(x => new { x.GrinNumber, x.Id,x.CreatedOn,x.CreatedBy,x.LastModifiedOn,x.LastModifiedBy })
                                    .Distinct().OrderByDescending(x => x.Id) // Ensure unique pairs of GrinNumber-Id
                                    .ToListAsync();

            var grinNumbers = binningGrinNoList.Select(b => b.GrinNumber).ToList();


            var grinDetails_1 = await _tipsGrinDbContext.Grins.Where(x => grinNumbers.Contains(x.GrinNumber)).ToListAsync();
            var grinDetails = grinDetails_1.Select(grinNumber => new GrinAndBinningDetailsDto
            {
                Id = binningGrinNoList.Where(b => b.GrinNumber == grinNumber.GrinNumber).Select(x=>x.Id).FirstOrDefault(),
                GrinNumber = grinNumber.GrinNumber,
                InvoiceNumber = grinNumber.InvoiceNumber,
                VendorName = grinNumber.VendorName,
                VendorId = grinNumber.VendorId,
                VendorNumber = grinNumber.VendorNumber,
                CreatedBy = binningGrinNoList.Where(b => b.GrinNumber == grinNumber.GrinNumber).Select(x => x.CreatedBy).FirstOrDefault(),
                CreatedOn = binningGrinNoList.Where(b => b.GrinNumber == grinNumber.GrinNumber).Select(x => x.CreatedOn).FirstOrDefault(),
                LastModifiedBy = binningGrinNoList.Where(b => b.GrinNumber == grinNumber.GrinNumber).Select(x => x.LastModifiedBy).FirstOrDefault(),
                LastModifiedOn = binningGrinNoList.Where(b => b.GrinNumber == grinNumber.GrinNumber).Select(x => x.LastModifiedOn).FirstOrDefault()
            }).OrderByDescending(x => x.Id).ToList();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchValue))
            {
                string searchTerm = searchParams.SearchValue.Trim();

                //grinDetails = grinDetails
                //              .Where(dto =>
                //                  dto.GrinNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                //                  dto.VendorId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                //                  dto.VendorNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                //                  dto.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                //                  dto.VendorName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Id).ToList();

                grinDetails = grinDetails
                .Where(dto =>
                    (dto.GrinNumber != null && dto.GrinNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.VendorId != null && dto.VendorId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.VendorNumber != null && dto.VendorNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.InvoiceNumber != null && dto.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (dto.VendorName != null && dto.VendorName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(x => x.Id)
                .ToList();
            }
            return PagedList<GrinAndBinningDetailsDto>.ToPagedList(grinDetails.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo)
        {
            var binningDetailsByGrinNo = await FindByCondition(x => x.GrinNumber == grinNo).ToListAsync();
            return binningDetailsByGrinNo;
        }
        public async Task<Binning> GetExistingBinningDetailsByGrinNo(string grinNo)
        {
            var binningDetailsByGrinNo = await _tipsGrinDbContext.Binnings.Where(x => x.GrinNumber == grinNo)
                                        .Include(x=>x.BinningItems)
                                         .ThenInclude(l=>l.binningLocations)
                                         .FirstOrDefaultAsync();
            return binningDetailsByGrinNo;
        }
        public async Task<Binning> GetBinningDetailsByGrinNumber(string grinNumber)
        {
            var binningDetailsByGrinNo = await _tipsGrinDbContext.Binnings.Where(x => x.GrinNumber == grinNumber)
                                         .FirstOrDefaultAsync();
            return binningDetailsByGrinNo;
        }

        public async Task<IEnumerable<Binning>> GetAllBinningWithItems(BinningSearchDto binningSearchDto)
        {
            
                var query = _tipsGrinDbContext.Binnings.Include("BinningItems");
                if (binningSearchDto != null || (binningSearchDto.InvoiceNumber.Any())
               && binningSearchDto.GrinNumber.Any() && binningSearchDto.VendorName.Any() && binningSearchDto.VendorId.Any())
                {
                    query = query.Where
                    (po => (binningSearchDto.GrinNumber.Any() ? binningSearchDto.GrinNumber.Contains(po.GrinNumber) : true))
                   //&& (binningSearchDto.InvoiceNumber.Any() ? binningSearchDto.InvoiceNumber.Contains(po.InvoiceNumber) : true)
                   //&& (binningSearchDto.VendorName.Any() ? binningSearchDto.VendorName.Contains(po.VendorName) : true)
                   //&& (binningSearchDto.VendorId.Any() ? binningSearchDto.VendorId.Contains(po.VendorId) : true));
                    .Include(p => p.BinningItems)
                    .ThenInclude(l => l.binningLocations);
            }
                return query.ToList();
            
        }
        public async Task<IEnumerable<Binning>> SearchBinning([FromQuery] SearchParames searchParames)
        {
            
                var query = _tipsGrinDbContext.Binnings.Include("BinningItems");
                if (!string.IsNullOrEmpty(Convert.ToString(searchParames.SearchValue)))
                {
                query = query.Where(po => po.GrinNumber.Contains(searchParames.SearchValue)
                //|| po.VendorName.Contains(searchParames.SearchValue)
                //|| po.InvoiceNumber.Contains(searchParames.SearchValue)
                //|| po.VendorId.Contains(searchParames.SearchValue)
                || po.BinningItems.Any(s => s.ItemNumber.Contains(searchParames.SearchValue)))
                    .Include(p=>p.BinningItems)
                    .ThenInclude(l=>l.binningLocations);
                
                }
                return query.ToList();
            
        }
        public async Task<IEnumerable<Binning>> SearchBinningDate([FromQuery] SearchDateParames searchParames)
        {
            var binningDetails = _tipsGrinDbContext.Binnings
            .Where(inv => ((inv.CreatedOn >= searchParames.SearchFromDate &&
            inv.CreatedOn <= searchParames.SearchToDate
            )))
            .Include(itm => itm.BinningItems)
            .ThenInclude(loc => loc.binningLocations)
            .ToList();
            return binningDetails;
        }
        public async Task<string> UpdateBinning(Binning binning)
        {
            binning.LastModifiedBy = _createdBy;
            binning.LastModifiedOn = DateTime.Now;
            Update(binning);
            string result = $"binning details of {binning.Id} is updated successfully!";
            return result;
        }


        public async Task<Binning> GetBinningDetailsbyId(int id)
        {
            var binningDetailsById = await _tipsGrinDbContext.Binnings.Where(x => x.Id == id)
                              .Include(t => t.BinningItems)
                              .ThenInclude(x => x.binningLocations)
                           .FirstOrDefaultAsync();

            return binningDetailsById;
        }

        public async Task<string> DeleteBinning(Binning binning)
        {
            Delete(binning);
            string result = $"binning details of {binning.Id} is deleted successfully!";
            return result;

        }

        public async Task<IEnumerable<BinningIdNameListDto>> GetAllActiveBinningNameList()
        {
            IEnumerable<BinningIdNameListDto> activeBinningNameList = await _tipsGrinDbContext.Binnings
                                .Select(x => new BinningIdNameListDto()
                                {
                                    Id = x.Id,
                                    GrinNumber = x.GrinNumber,
                                })
                              .ToListAsync();

            return activeBinningNameList;
        }

    }



    public class BinningItemsRepository : RepositoryBase<BinningItems>, IBinningItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BinningItemsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        //public async Task<PagedList<BinningItems>> GetAllBinningItems()
        //{
        //    var getAllBinningItems = FindAll()
        //       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue)
        //       )))
        //      .Include(t => t.binningLocations);


        //    return PagedList<BinningItems>.ToPagedList(getAllBinningItems, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<BinningItems> CreateBinningItems(BinningItems binningItems)
        {

            var result = await Create(binningItems);
            return result;
        }

        public async Task<IEnumerable<BinningItems>> GetAllBinningItems()
        {
            var binningItemsDetails = FindAll().OrderByDescending(x => x.Id)
            .Include(v => v.binningLocations);
            return binningItemsDetails;
        }
        public async Task<string> UpdateBinningItems(BinningItems binningItems)
        {
            Update(binningItems);
            string result = $"binningItems details of {binningItems.Id} is updated successfully!";
            return result;
        }

    }

    public class BinningLocationRepository : RepositoryBase<BinningLocation>, IBinningLocationRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BinningLocationRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<IEnumerable<BinningLocation>> GetBinningLocationDetailsbyGrinPartId(int id)
        {
            var binningDetailsById = await _tipsGrinDbContext.BinningLocations.Where(x => x.BinningItemsId == id)
                             
                           .ToListAsync();

            return binningDetailsById;
        }
        public async Task<string> UpdateBinning(BinningLocation binningLocation)
        {
            binningLocation.LastModifiedBy = _createdBy;
            binningLocation.LastModifiedOn = DateTime.Now;
            Update(binningLocation);
            string result = $"binningLocation details of {binningLocation.Id} is updated successfully!";
            return result;
        }
        //public async Task<BinningQuantityDto> GetListOfBinningQtyByItemNoListByProjectNo(string projectNo, List<string> itemNumberList)
        //{
        //    var binningItemDetails = await _tipsGrinDbContext.BinningItem.Where(x => itemNumberList.Contains( x.ItemNumber))
        //                                     .Select(x => new { x.ItemNumber, x.Id })
        //                                    .ToListAsync();

        //    var binningItemIds = binningItemDetails.Select(b => b.Id).ToList();

        //    var binningQuantityList = await _tipsGrinDbContext.BinningLocations
        //        .Where(x => x.ProjectNumber == projectNo && binningItemIds.Contains(x.BinningItemsId))
        //         .GroupBy(i => new { i.BinningItemsId })
        //        .Select(gr => new BinningQuantityDto
        //        {
        //            ItemNumber = binningItemDetails.Where(b => b.Id == gr.First().BinningItemsId).Select(x => x.ItemNumber).FirstOrDefault(),
        //            BinningQty = gr.Sum(x => x.Qty)
        //        }).FirstOrDefaultAsync();

        //    return binningQuantityList;

        //}

        public async Task<List<BinningQuantityDto>> GetListOfBinningQtyByItemNo(string itemNumber)
        {
            var binningItemIds = await _tipsGrinDbContext.BinningItem.Where(x => x.ItemNumber == itemNumber)
                                             .Select(x => x.Id)
                                            .ToListAsync();

            List<BinningQuantityDto> binningQuantityList = await _tipsGrinDbContext.BinningLocations
                .Where(x =>  binningItemIds.Contains(x.BinningItemsId))
                .GroupBy(x => new { x.ProjectNumber })
                .Select(gr => new BinningQuantityDto
                {
                    ItemNumber = itemNumber,
                    BinningQty = gr.Sum(x => x.Qty)
                })
                .ToListAsync();

            return binningQuantityList;
        }
        public async Task<List<BinningQuantityDto>> GetListOfBinningQtyByItemNoListByProjectNo(string projectNo, string itemNumber)
        {
            var binningItemIds = await _tipsGrinDbContext.BinningItem.Where(x => x.ItemNumber == itemNumber)
                                             .Select(x => x.Id )
                                            .ToListAsync();

            List<BinningQuantityDto> binningQuantityList = await _tipsGrinDbContext.BinningLocations
                .Where(x => x.ProjectNumber == projectNo && binningItemIds.Contains(x.BinningItemsId))
                .GroupBy(x => new { x.ProjectNumber })
                .Select(gr => new BinningQuantityDto
                {
                    ItemNumber = itemNumber,
                    BinningQty = gr.Sum(x => x.Qty)
                })
                .ToListAsync();

            return binningQuantityList;
        }
        public async Task<List<BinningQuantityDto>> GetListOfBinningQtyByItemNoListByMultipleProjectNo(string itemNumber, List<string> projectNo)
        {
            var binningItemIds = await _tipsGrinDbContext.BinningItem.Where(x => x.ItemNumber == itemNumber)
                                             .Select(x => x.Id)
                                            .ToListAsync();

            List<BinningQuantityDto> binningQuantityList = await _tipsGrinDbContext.BinningLocations
                .Where(x => projectNo.Contains( x.ProjectNumber) && binningItemIds.Contains(x.BinningItemsId))
                .GroupBy(x => new { x.ProjectNumber })
                .Select(gr => new BinningQuantityDto
                {
                    ItemNumber = itemNumber,
                    BinningQty = gr.Sum(x => x.Qty)
                })
                .ToListAsync();

            return binningQuantityList;
        }
        public async Task<IEnumerable<BinningSPReportAvi>> GetBinningSPReportWithParamForAvi(string? ponumber, string? grinnumber, string? itemnumber,
                                                                                                    string? projectnumber)
        {
            var result = _tipsGrinDbContext
            .Set<BinningSPReportAvi>()
            .FromSqlInterpolated($"CALL Binning_Report_With_Parameter_Avi({ponumber},{grinnumber},{itemnumber},{projectnumber})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<BinningSPReportAvi>> GetBinningSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<BinningSPReportAvi>()
                      .FromSqlInterpolated($"CALL Binning_Report_Withdate_Avi({FromDate},{ToDate})")
                      .ToList();

            return results;
        }

    }
}
using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class BinningRepository : RepositoryBase<Binning>, IBinningRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public BinningRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        public async Task<Binning> CreateBinning(Binning binning)
        {

            binning.CreatedBy = "Admin";
            binning.CreatedOn = DateTime.Now;
            binning.Unit = "Bangalore";
            var result = await Create(binning);
            return result;
        }
        public async Task<PagedList<Binning>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {

            var getAllBinningDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue))))
                 .Include(t => t.BinningItems)
                 .ThenInclude(t => t.BinningLocations);
                return PagedList<Binning>.ToPagedList(getAllBinningDetails, pagingParameter.PageNumber, pagingParameter.PageSize);


        }
        public async Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo)
        {
            var binningDetailsByGrinNo = await FindByCondition(x => x.GrinNumber == grinNo).ToListAsync();
            return binningDetailsByGrinNo;
        }

        public async Task<IEnumerable<Binning>> GetAllBinningWithItems(BinningSearchDto binningSearchDto)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.Binnings.Include("BinningItems");
                if (binningSearchDto != null || (binningSearchDto.InvoiceNumber.Any())
               && binningSearchDto.GrinNumber.Any() && binningSearchDto.VendorName.Any() && binningSearchDto.VendorId.Any())
                {
                    query = query.Where
                    (po => (binningSearchDto.GrinNumber.Any() ? binningSearchDto.GrinNumber.Contains(po.GrinNumber) : true));
                   //&& (binningSearchDto.InvoiceNumber.Any() ? binningSearchDto.InvoiceNumber.Contains(po.InvoiceNumber) : true)
                   //&& (binningSearchDto.VendorName.Any() ? binningSearchDto.VendorName.Contains(po.VendorName) : true)
                   //&& (binningSearchDto.VendorId.Any() ? binningSearchDto.VendorId.Contains(po.VendorId) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<Binning>> SearchBinning([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.Binnings.Include("BinningItems");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.GrinNumber.Contains(searchParames.SearchValue)
                    //|| po.VendorName.Contains(searchParames.SearchValue)
                    //|| po.InvoiceNumber.Contains(searchParames.SearchValue)
                    //|| po.VendorId.Contains(searchParames.SearchValue)
                    || po.BinningItems.Any(s => s.ItemNumber.Contains(searchParames.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<Binning>> SearchBinningDate([FromQuery] SearchDateParames searchParames)
        {
            var binningDetails = _tipsGrinDbContext.Binnings
            .Where(inv => ((inv.CreatedOn >= searchParames.SearchFromDate &&
            inv.CreatedOn <= searchParames.SearchToDate
            )))
            .Include(itm => itm.BinningItems)
            .ToList();
            return binningDetails;
        }
        public async Task<string> UpdateBinning(Binning binning)
        {
            binning.LastModifiedBy = "Admin";
            binning.LastModifiedOn = DateTime.Now;
            Update(binning);
            string result = $"binning details of {binning.Id} is updated successfully!";
            return result;
        }


        public async Task<Binning> GetBinningDetailsbyId(int id)
        {
            var binningDetailsById = await _tipsGrinDbContext.Binnings.Where(x => x.Id == id)
                              .Include(t => t.BinningItems)
                              .ThenInclude(x => x.BinningLocations)
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

        public BinningItemsRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        //public async Task<PagedList<BinningItems>> GetAllBinningItems()
        //{
        //    var getAllBinningItems = FindAll()
        //       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue)
        //       )))
        //      .Include(t => t.BinningLocations);


        //    return PagedList<BinningItems>.ToPagedList(getAllBinningItems, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<IEnumerable<BinningItems>> GetAllBinningItems()
        {
            var binningItemsDetails = FindAll().OrderByDescending(x => x.Id)
            .Include(v => v.BinningLocations);
            return binningItemsDetails;
        }

    }
}
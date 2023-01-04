using Entities.Helper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class BinningRepository : RepositoryBase<Binning>, IBinningRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public BinningRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        public async Task<int?> CreateBinning(Binning binning)
        {
           
            binning.CreatedBy = "Admin";
            binning.CreatedOn = DateTime.Now;
            binning.Unit = "Bangalore";
            var result = await Create(binning);
            return result.Id;
        }
        public async Task<IEnumerable<Binning>> GetAllBinningDetails()
        {
            var GetallBinnings = await FindAll().ToListAsync();
            return GetallBinnings;

        }
        public async Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo)
        {
            var BinningbyGrinNo = await FindByCondition(x => x.GrinNumber == grinNo).ToListAsync();
            return BinningbyGrinNo;
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
            var BinningbyId = await _tipsGrinDbContext.Binnings.Where(x => x.Id == id)
                              .Include(t => t.BinningItems)
                              .ThenInclude(x => x.BinningLocations)
                           .FirstOrDefaultAsync();

            return BinningbyId;
        }

        public async Task<string> DeleteBinning(Binning binning)
        {
            Delete(binning);
            string result = $"binning details of {binning.Id} is deleted successfully!";
            return result;

        }
    }
}
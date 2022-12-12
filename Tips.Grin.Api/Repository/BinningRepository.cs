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
            binning.LastModifiedBy = "Admin";
            binning.LastModifiedOn = DateTime.Now;
            binning.CreatedBy = "Admin";
            binning.CreatedOn = DateTime.Now;
            var result = await Create(binning);
            return result.Id;
        }
        public async Task<IEnumerable<Binning>> GetAllBinningDetails()
        {
            var binnings = await FindAll().ToListAsync();
            return binnings;

        }
        public async Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo)
        {
            var binningList = await FindByCondition(x => x.SelectGrinNo == grinNo).ToListAsync();
            return binningList;
        }

        public async Task<string> UpdateBinning(Binning binning)
        {
            binning.LastModifiedBy = "Admin";
            binning.LastModifiedOn = DateTime.Now;
            Update(binning);
            string result = $"LeadTime details of {binning.Id} is updated successfully!";
            return result;
        }


        public async Task<Binning> GetBinningDetailsbyId(int id)
        {
            var binnings = await _tipsGrinDbContext.Binnings.Where(x => x.Id == id)
                              .Include(t => t.binningItems)
                              .ThenInclude(x => x.binningLocations)
                           .FirstOrDefaultAsync();

            return binnings;
        }

        public async Task<string> DeleteBinning(Binning binning)
        {
            Delete(binning);
            string result = $"binning details of {binning.Id} is deleted successfully!";
            return result;

        }
    }
}
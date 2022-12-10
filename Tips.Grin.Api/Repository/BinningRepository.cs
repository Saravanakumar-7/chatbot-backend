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
            var binningList = await FindAll().ToListAsync();
            return (binningList);

        }
        public async Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo)
        {
            var binningList = await FindByCondition(x => x.GrinNo == grinNo).ToListAsync();
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
            var binningList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return binningList;
        }

    }
}

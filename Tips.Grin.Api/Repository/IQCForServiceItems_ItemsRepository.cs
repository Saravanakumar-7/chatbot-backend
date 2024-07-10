using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class IQCForServiceItems_ItemsRepository : RepositoryBase<IQCForServiceItems_Items>, IIQCForServiceItems_ItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        public IQCForServiceItems_ItemsRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
        }
        public async Task<int?> CreateIqcForServiceItems_Items(IQCForServiceItems_Items iqcForServiceItems_Items)
        {
            var result = await Create(iqcForServiceItems_Items);
            return result.Id;
        }
        public async Task<int?> GetIQCForServiceItems_ItemsCount(int iqcId)
        {
            var grinPartsBinningStatusCount = _tipsGrinDbContext.IQCForServiceItems_Items.Where(x => x.IQCForServiceItemsId == iqcId).Count();

            return grinPartsBinningStatusCount;
        }
    }
}

using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCForServiceItems_ItemsRepository : IRepositoryBase<IQCForServiceItems_Items>
    {
        Task<int?> CreateIqcForServiceItems_Items(IQCForServiceItems_Items iqcForServiceItems_Items);
        Task<int?> GetIQCForServiceItems_ItemsCount(int iqcId);
    }
}

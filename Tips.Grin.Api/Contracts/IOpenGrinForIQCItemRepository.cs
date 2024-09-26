using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForIQCItemRepository : IRepositoryBase<OpenGrinForIQCItems>
    {
        Task<int?> CreateOpenGrinForIQCItems(OpenGrinForIQCItems openGrinForIQCItems);
        Task<int?> GetOpenGrinForIQCItemsCount(int openGrinForIQCId);
        Task<OpenGrinForIQCItems> GetOpenGrinForIQCItemsDetailsbyOpenGrinForGrinItemId(int openGrinForGrinItemId);
        Task<string> UpdateOpenGrinForIQCItems(OpenGrinForIQCItems openGrinForIQCItems);
}
}

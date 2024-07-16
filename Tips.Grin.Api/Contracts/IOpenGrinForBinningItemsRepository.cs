using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForBinningItemsRepository : IRepositoryBase<OpenGrinForBinningItems>
    {
        Task<string> UpdateOpenGrinForBinningItems(OpenGrinForBinningItems openGrinForBinningItems);
        Task<OpenGrinForBinningItems> CreateOpenGrinForBinningItems(OpenGrinForBinningItems openGrinForBinningItems);
    }
}

using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForGrinItemRepository : IRepositoryBase<OpenGrinForGrinItems>
    {
        Task<int?> CreateOpenGrinForGrinItems(OpenGrinForGrinItems openGrinForGrinItems);
        Task<OpenGrinForGrinItems> GetOpenGrinForGrinItemById(int id);
        Task<string> UpdateOpenGrinForGrinItem(OpenGrinForGrinItems openGrinForGrinItems);
        Task<OpenGrinForGrinItems> GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(int openGrinForGrinItemId);
        Task<int?> GetOpenGrinForGrinItemsIqcStatusCount(int grinId);
        Task<OpenGrinForGrinItems> UpdateOpenGrinForGrinItemsQty(int openGrinForGrinPartId, string AcceptedQty, string RejectedQty);
        Task<int?> GetOpenGrinForGrinItemsCount(int openGrinForGrinId);
        Task<int?> GetOpenGrinForGrinItemsBinningStatusCount(int openGrinForGrinId);
    }
}

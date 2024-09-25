using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinsForServiceItemsPartsRepository : IRepositoryBase<GrinsForServiceItemsParts>
    {
        Task<GrinsForServiceItemsParts> GetGrinsForServiceItemsPartsById(int id);
        Task<string> UpdateGrinsForServiceItemsParts(GrinsForServiceItemsParts grinsForServiceItemsParts);
        Task<GrinsForServiceItemsParts> GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(int GrinsForServiceItemsPartsId);
        Task<int?> GetGrinsForServiceItemsPartsCount(int grinForServiceItemsId);
        Task<GrinsForServiceItemsParts> UpdateGrinsForServiceItemsPartsQty(int GrinsForServiceItemsPartsId, string AcceptedQty, string RejectedQty);
        Task<List<int>?> GetGrinForServiceItemsIdsByPonumber(string Ponumber);
    }
}

using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IKIT_GRINPartsRepository : IRepositoryBase<KIT_GRINParts>
    {
        Task<KIT_GRINParts> GetKIT_GRINPartsById(int id);
        Task<string> UpdateKIT_GRINPartsQty(KIT_GRINParts grinParts);
    }
}

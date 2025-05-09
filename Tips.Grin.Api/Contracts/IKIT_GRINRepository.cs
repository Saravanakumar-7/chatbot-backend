using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IKIT_GRINRepository : IRepositoryBase<KIT_GRIN>
    {
        Task<string> GenerateKIT_GrinNumberForAvision();
        Task<string> GenerateKIT_GrinNumber();
        Task<int?> CreateKIT_Grin(KIT_GRIN grins);
    }
}

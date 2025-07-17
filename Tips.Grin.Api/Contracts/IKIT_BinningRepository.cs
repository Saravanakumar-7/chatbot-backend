using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IKIT_BinningRepository : IRepositoryBase<KIT_Binning>
    {
        Task<int?> CreateKIT_Binning(KIT_Binning kIT_Binning);
        Task<KIT_Binning?> GetKIT_BinningbyKIT_GrinNumber(string kIT_GrinNumber);
        Task<string> UpdateKIT_Binning(KIT_Binning kIT_Binning);
        Task<PagedList<KIT_Binning>> GetAllKIT_Binning(PagingParameter pagingParameter, SearchParams searchParams);
        Task<KIT_Binning> GetKIT_BinningbyId(int Id);
    }
}

using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IKIT_IQCRepository : IRepositoryBase<KIT_IQC>
    {
        Task<int?> CreateKIT_IQC(KIT_IQC kIT_IQC);
        Task<KIT_IQC?> GetKIT_IQCbyKIT_GrinNumber(string kIT_GrinNumber);
        Task<string> UpdateKIT_IQC(KIT_IQC kIT_IQC);
        Task<PagedList<KIT_IQC>> GetAllKIT_IQC(PagingParameter pagingParameter,SearchParams searchParams);
        Task<KIT_IQC> GetKIT_IQCbyId(int Id);
    }
}


using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISaOqcRepository : IRepositoryBase<SaOqc>
    {
        public Task<PagedList<SaOqc>> GetAllSaOqc(PagingParameter pagingParameter);
        Task<SaOqc> GetSaOqcById(int id);
        Task<int?> CreateSaOqc(SaOqc saOqc);
        Task<string> UpdateSaOqc(SaOqc saOqc);
        Task<string> DeleteSaOqc(SaOqc saOqc);
    }
}


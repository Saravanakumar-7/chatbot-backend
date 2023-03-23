
using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IFinalOqcRepository : IRepositoryBase<FinalOqc>
    {
        public Task<PagedList<FinalOqc>> GetAllFinalOqc(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<FinalOqc> GetFinalOqcById(int id);
        Task<int?> CreateFinalOqc(FinalOqc finalOqc);
        Task<string> UpdateFinalOqc(FinalOqc finalOqc);
        Task<string> DeleteFinalOqc(FinalOqc finalOqc);
    }
}


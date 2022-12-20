using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IFgOqcRepository : IRepositoryBase<FgOqc>
    {

        public Task<PagedList<FgOqc>> GetAllFgOqcs(PagingParameter pagingParameter);
        Task<FgOqc> GetFgOqcById(int id);
        Task<int?> CreateFgOqc(FgOqc fgOqc);
        Task<string> UpdateFgOqc(FgOqc fgOqc);
        Task<string> DeleteFgOqc(FgOqc fgOqc);
    }
}

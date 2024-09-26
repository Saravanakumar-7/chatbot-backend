using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinPartsRepository : IRepositoryBase<OpenGrinParts>
    {
        Task<PagedList<OpenGrinParts>> GetAllOpenGrinPartsDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<string> UpdateOpenGrinParts(OpenGrinParts openGrinParts);
        Task<OpenGrinParts> GetOpenGrinPartsDetailsbyId(int id);
        Task<OpenGrinParts> CreateOpenGrinParts(OpenGrinParts openGrinParts);
        Task<string> DeleteOpenGrinParts(OpenGrinParts openGrinParts);
    }
}

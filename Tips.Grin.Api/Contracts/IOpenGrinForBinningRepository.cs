using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForBinningRepository : IRepositoryBase<OpenGrinForBinning>
    {
        Task<PagedList<OpenGrinForBinning>> GetAllOpenGrinForBinningDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<OpenGrinForBinning> GetOpenGrinForBinningDetailsByOpenGrinNo(string openGrinNo);
        Task<OpenGrinForBinning> GetOpenGrinForBinningDetailsbyId(int id);
        Task<OpenGrinForBinning> GetExistingOpenGrinForBinningDetailsByOpenGrinNo(string openGrinNo);
        Task<string> UpdateOpenGrinForBinning(OpenGrinForBinning openGrinForBinning);
        Task<OpenGrinForBinning> CreateOpenGrinForBinning(OpenGrinForBinning openGrinForBinning);
    }
}

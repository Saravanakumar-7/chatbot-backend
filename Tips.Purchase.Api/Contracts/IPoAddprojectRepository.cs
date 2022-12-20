using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoAddprojectRepository
    {
        Task<IEnumerable<PoAddProject>> GetAllPoAddproject();
        Task<PoAddProject> GetPoAddprojectById(int id);
        Task<IEnumerable<PoAddProject>> GetAllActivePoAddproject();
        Task<int?> CreatePoAddproject(PoAddProject poAddproject);
        Task<string> UpdatePoAddproject(PoAddProject poAddproject);
        Task<string> DeletePoAddproject(PoAddProject poAddproject);
    }
}

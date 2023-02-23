using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrAddprojectRepository
    {
        Task<IEnumerable<PrAddProject>> GetAllPrAddprojects();
        Task<PrAddProject> GetPrAddprojectById(int id);
        Task<IEnumerable<PrAddProject>> GetAllActivePrAddprojects();
        Task<int?> CreatePrAddproject(PrAddProject prAddProject);
        Task<string> UpdatePrAddproject(PrAddProject prAddProject);
        Task<string> DeletePrAddproject(PrAddProject prAddProject);
    }
}

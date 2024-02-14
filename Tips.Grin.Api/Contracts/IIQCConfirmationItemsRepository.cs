using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCConfirmationItemsRepository : IRepositoryBase<IQCConfirmationItems>
    {
        Task<IEnumerable<IQCConfirmationItems>> GetAllIQCConfirmationItems();
        Task<int?> CreateIqcItem(IQCConfirmationItems iQCConfirmationItems);
        Task<IQCConfirmationItems> GetIQCConfirmationItemsDetailsbyGrinPartId(int GrinPartId);
        Task<string> UpdateIqcItems(IQCConfirmationItems iqcConfirmationItems);
        Task<int?> GetIQCConformationCount(int iqcId);
    }
}

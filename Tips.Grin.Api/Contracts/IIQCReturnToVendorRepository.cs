using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCReturnToVendorRepository : IRepositoryBase<IQCReturnToVendor>
    {
        Task<int?> CreateIQCReturnToVendor(IQCReturnToVendor iQCReturnToVendor);
    }
}

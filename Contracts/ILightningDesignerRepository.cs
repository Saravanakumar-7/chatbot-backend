using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ILightningDesignerRepository : IRepositoryBase<LightningDesigner>
    {
        Task<PagedList<LightningDesigner>> GetAllLightningDesigners(PagingParameter pagingParameter, SearchParames searchParames);
        Task<LightningDesigner> GetLightningDesignerById(int id);
        Task<PagedList<LightningDesigner>> GetAllActiveLightningDesigners(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLightningDesigner(LightningDesigner lightningDesigner);
        Task<string> UpdateLightningDesigner(LightningDesigner lightningDesigner);
        Task<string> DeleteLightningDesigner(LightningDesigner lightningDesigner);
    }
}
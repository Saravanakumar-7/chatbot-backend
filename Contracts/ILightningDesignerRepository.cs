using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ILightningDesignerRepository : IRepositoryBase<LightningDesigner>
    {
        Task<IEnumerable<LightningDesigner>> GetAllLightningDesigners();
        Task<LightningDesigner> GetLightningDesignerById(int id);
        Task<IEnumerable<LightningDesigner>> GetAllActiveLightningDesigners();
        Task<int?> CreateLightningDesigner(LightningDesigner lightningDesigner);
        Task<string> UpdateLightningDesigner(LightningDesigner lightningDesigner);
        Task<string> DeleteLightningDesigner(LightningDesigner lightningDesigner);
    }
}
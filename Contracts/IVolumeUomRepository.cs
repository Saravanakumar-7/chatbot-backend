using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVolumeUomRepository
    {
        Task<IEnumerable<VolumeUom>> GetAllVolumeUom();
        Task<VolumeUom> GetVolumeUomById(int id);
        Task<IEnumerable<VolumeUom>> GetAllActiveVolumeUom();
        Task<int?> CreateVolumeUom(VolumeUom volumeUom);
        Task<string> UpdateVolumeUom(VolumeUom volumeUom);
        Task<string> DeleteVolumeUom(VolumeUom volumeUom);

    }
}

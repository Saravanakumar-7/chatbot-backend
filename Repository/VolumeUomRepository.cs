using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VolumeUomRepository : RepositoryBase<VolumeUom>, IVolumeUomRepository
    {
        public VolumeUomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateVolumeUom(VolumeUom volumeUom)
        {
            volumeUom.CreatedBy = "Admin";
            volumeUom.CreatedOn = DateTime.Now;
            var result = await Create(volumeUom);
            return result.Id;

            //throw new NotImplementedException();
        }

        public async Task<string> DeleteVolumeUom(VolumeUom volumeUom)
        {
            Delete(volumeUom);
            string result = $"Volume Uom details of {volumeUom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VolumeUom>> GetAllActiveVolumeUom()
        {
            var VolumeUomList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return VolumeUomList;
        }

        public async Task<IEnumerable<VolumeUom>> GetAllVolumeUom()
        {
            var volumeUoms = await FindAll().ToListAsync();

            return volumeUoms;
        }

        public async Task<VolumeUom> GetVolumeUomById(int id)
        {
            var volumeUom = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return volumeUom;
        }

        public async Task<string> UpdateVolumeUom(VolumeUom volumeUom)
        {
            volumeUom.LastModifiedBy = "Admin";
            volumeUom.LastModifiedOn = DateTime.Now;
            Update(volumeUom);
            string result = $"Volume Uom of Detail {volumeUom.Id} is updated successfully!";
            return result;
        }
    }
}

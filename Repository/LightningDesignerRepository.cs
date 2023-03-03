using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class LightningDesignerRepository : RepositoryBase<LightningDesigner>, ILightningDesignerRepository
    {
        public LightningDesignerRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateLightningDesigner(LightningDesigner lightningDesigner)
        {
            lightningDesigner.CreatedBy = "Admin";
            lightningDesigner.CreatedOn = DateTime.Now;
            lightningDesigner.Unit = "Bangalore";
            var result = await Create(lightningDesigner); return result.Id;
        }
        public async Task<string> DeleteLightningDesigner(LightningDesigner lightningDesigner)
        {
            Delete(lightningDesigner);
            string result = $"lightningDesigner details of {lightningDesigner.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<LightningDesigner>> GetAllActiveLightningDesigners()
        {
            var AllActivelightningDesigner = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivelightningDesigner;
        }
        public async Task<IEnumerable<LightningDesigner>> GetAllLightningDesigners()
        {
            var GetallLightningDesigner = await FindAll().ToListAsync(); return GetallLightningDesigner;
        }
        public async Task<LightningDesigner> GetLightningDesignerById(int id)
        {
            var LightningDesignerById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return LightningDesignerById;
        }
        public async Task<string> UpdateLightningDesigner(LightningDesigner lightningDesigner)
        {
            lightningDesigner.LastModifiedBy = "Admin";
            lightningDesigner.LastModifiedOn = DateTime.Now;
            Update(lightningDesigner);
            string result = $"lightningDesigner details of {lightningDesigner.Id} is updated successfully!";
            return result;
        }
    }
}
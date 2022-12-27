using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class WeightUomRepository : RepositoryBase<WeightUom>, IWeightUomRepository
    {
        public WeightUomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateWeightUom(WeightUom weightUom)
        {
            weightUom.CreatedBy = "Admin";
            weightUom.CreatedOn = DateTime.Now;
            var result = await Create(weightUom);
            weightUom.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteWeightUom(WeightUom weightUom)
        {
            Delete(weightUom);
            string result = $"Weight Uom details of {weightUom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<WeightUom>> GetAllActiveWeightUom()
        {
            var WeightUomList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return WeightUomList;
        }

        public async Task<IEnumerable<WeightUom>> GetAllWeightUom()
        {

            var weightUoms = await FindAll().ToListAsync();

            return weightUoms;
        }

        public async Task<WeightUom> GetWeightUomById(int id)
        {
            var weightUom = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return weightUom;
        }

        public async Task<string> UpdateWeightUom(WeightUom weightUom)
        {
            weightUom.LastModifiedBy = "Admin";
            weightUom.LastModifiedOn = DateTime.Now;
            Update(weightUom);
            string result = $"Weight Uom of Detail {weightUom.Id} is updated successfully!";
            return result;
        }
    }
}

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
    public class DemoStatusRepository : RepositoryBase<DemoStatus>, IDemoStatusRepository
    {
        public DemoStatusRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateDemoStatus(DemoStatus demoStatus)
        {
            demoStatus.CreatedBy = "Admin";
            demoStatus.CreatedOn = DateTime.Now;
            demoStatus.Unit = "Bangalore";
            var result = await Create(demoStatus);
           
            return result.Id;

        }

        public async Task<string> DeleteDemoStatus(DemoStatus demoStatus)
        {
            Delete(demoStatus);
            string result = $"DemoStatus details of {demoStatus.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<DemoStatus>> GetAllActiveDemoStatus()
        {

            var AllActiveDemoStatus = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveDemoStatus;
        }

        public async Task<IEnumerable<DemoStatus>> GetAllDemoStatus()
        {

            var GetallDemoStatus= await FindAll().ToListAsync();

            return GetallDemoStatus;
        }

        public async Task<DemoStatus> GetDemoStatusById(int id)
        {
            var DemoStatusbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DemoStatusbyId;
        }
        public async Task<string> UpdateDemoStatus(DemoStatus demoStatus)
        {

            demoStatus.LastModifiedBy = "Admin";
            demoStatus.LastModifiedOn = DateTime.Now;
            Update(demoStatus);
            string result = $"demoStatus details of {demoStatus.Id} is updated successfully!";
            return result;
        }
    }
}

using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class GST_PercentageRepository : RepositoryBase<GST_Percentage>, IGST_PercentageRepository
    {
        public  GST_PercentageRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateGST_Percentage(GST_Percentage gst_Percentage)
        {
            gst_Percentage.CreatedBy = "Admin";
            gst_Percentage.CreatedOn = DateTime.Now;
            var result = await Create(gst_Percentage);
            gst_Percentage.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteGST_Percentage(GST_Percentage gst_Percentage)
        {
            Delete(gst_Percentage);
            string result = $"GST_Percentage details of {gst_Percentage.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<GST_Percentage>> GetAllActiveGST_Percentages()
        {
            var gst_PercentageList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return gst_PercentageList;
        }

        public async Task<IEnumerable<GST_Percentage>> GetAllGST_Percentages()
        {
            var gst_PercentageList = await FindAll().ToListAsync();

            return gst_PercentageList;
        }

        public async Task<GST_Percentage> GetGST_PercentageById(int id)
        {
            var gst_Percentage = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return gst_Percentage;
        }

        public async Task<string> UpdateGST_Percentage(GST_Percentage gst_Percentage)
        {

            gst_Percentage.LastModifiedBy = "Admin";
            gst_Percentage.LastModifiedOn = DateTime.Now;
            Update(gst_Percentage);
            string result = $"GST_Percentage details of {gst_Percentage.Id} is updated successfully!";
            return result;
        }
    }
}

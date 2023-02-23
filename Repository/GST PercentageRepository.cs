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
            gst_Percentage.Unit = "Bangalore";
            var result = await Create(gst_Percentage);
           
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
            var AllActiveGst_PercentageList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveGst_PercentageList;
        }

        public async Task<IEnumerable<GST_Percentage>> GetAllGST_Percentages()
        {
            var GetallGst_PercentageList = await FindAll().ToListAsync();

            return GetallGst_PercentageList;
        }

        public async Task<GST_Percentage> GetGST_PercentageById(int id)
        {
            var Gst_PercentagebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return Gst_PercentagebyId;
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

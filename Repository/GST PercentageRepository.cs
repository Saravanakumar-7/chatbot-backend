using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GST_PercentageRepository : RepositoryBase<GST_Percentage>, IGST_PercentageRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public  GST_PercentageRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateGST_Percentage(GST_Percentage gst_Percentage)
        {
            gst_Percentage.CreatedBy = _createdBy;
            gst_Percentage.CreatedOn = DateTime.Now;
            gst_Percentage.Unit = _unitname;
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
            var gstPercentageDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return gstPercentageDetails;
        }

        public async Task<IEnumerable<GST_Percentage>> GetAllGST_Percentages()
        {
            var gstPercentageDetails = await FindAll().ToListAsync();
            return gstPercentageDetails;
        }
        public async Task<GST_Percentage> GetGST_PercentageById(int id)
        {
            var Gst_PercentagebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return Gst_PercentagebyId;
        }

        public async Task<string> UpdateGST_Percentage(GST_Percentage gst_Percentage)
        {

            gst_Percentage.LastModifiedBy = _createdBy;
            gst_Percentage.LastModifiedOn = DateTime.Now;
            Update(gst_Percentage);
            string result = $"GST_Percentage details of {gst_Percentage.Id} is updated successfully!";
            return result;
        }
    }
}

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
    public class ExportUnitTypeRepository : RepositoryBase<ExportUnitType>, IExportUnitTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ExportUnitTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateExportUnitType(ExportUnitType exportUnitType)
        {
            exportUnitType.CreatedBy = _createdBy;
            exportUnitType.CreatedOn = DateTime.Now;
            exportUnitType.Unit = _unitname;
            var result = await Create(exportUnitType);
            
            return result.Id;
        }

        public async Task<string> DeleteExportUnitType(ExportUnitType exportUnitType)
        {
            Delete(exportUnitType);
            string result = $"ExportUnitType details of {exportUnitType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ExportUnitType>> GetAllActiveExportUnitTypes([FromQuery] SearchParames searchParams)
        {
            var exportUnitDetails = FindAll()
                              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ExportUnitTypeName.Contains(searchParams.SearchValue) ||
                        inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return exportUnitDetails;
        }

        public async Task<IEnumerable<ExportUnitType>> GetAllExportUnitTypes([FromQuery] SearchParames searchParams)
        {
            var exportUnitDetails = FindAll().OrderByDescending(x => x.Id)
                                          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ExportUnitTypeName.Contains(searchParams.SearchValue) ||
                                    inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return exportUnitDetails;
        }

        public async Task<ExportUnitType> GetExportUnitTypeById(int id)
        {
            var ExportUnitTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ExportUnitTypebyId;
        }

        public async Task<string> UpdateExportUnitType(ExportUnitType exportUnitType)
        {
            exportUnitType.LastModifiedBy = _createdBy;
            exportUnitType.LastModifiedOn = DateTime.Now;
            Update(exportUnitType);
            string result = $"ExportUnitType details of {exportUnitType.Id} is updated successfully!";
            return result;
        }
    }
}

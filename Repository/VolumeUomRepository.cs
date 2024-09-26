using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VolumeUomRepository : RepositoryBase<VolumeUom>, IVolumeUomRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public VolumeUomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateVolumeUom(VolumeUom volumeUom)
        {
            volumeUom.CreatedBy = _createdBy;
            volumeUom.CreatedOn = DateTime.Now;
            volumeUom.Unit = _unitname;
            var result = await Create(volumeUom);

            return result.Id;

            
        }

        public async Task<string> DeleteVolumeUom(VolumeUom volumeUom)
        {
            Delete(volumeUom);
            string result = $"Volume Uom details of {volumeUom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<VolumeUom>> GetAllActiveVolumeUom()
        {
            var AllActiveVolumeUomList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveVolumeUomList;
        }

        public async Task<IEnumerable<VolumeUom>> GetAllVolumeUom()
        {
            var GetallVolumeUom = await FindAll().ToListAsync();

            return GetallVolumeUom;
        }

        public async Task<VolumeUom> GetVolumeUomById(int id)
        {
            var VolumeUombyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return VolumeUombyId;
        }

        public async Task<string> UpdateVolumeUom(VolumeUom volumeUom)
        {
            volumeUom.LastModifiedBy = _createdBy;
            volumeUom.LastModifiedOn = DateTime.Now;
            Update(volumeUom);
            string result = $"Volume Uom of Detail {volumeUom.Id} is updated successfully!";
            return result;
        }
    }
}

using Contracts;
using Entities;
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
    public class ShipmentModeRepository : RepositoryBase<ShipmentMode>, IShipmentModeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ShipmentModeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateShipmentMode(ShipmentMode shipmentMode)
        {
            shipmentMode.CreatedBy = _createdBy;
            shipmentMode.CreatedOn = DateTime.Now;
            shipmentMode.Unit = _unitname;
            var result = await Create(shipmentMode);

            return result.Id;
        }

        public async Task<string> DeleteShipmentMode(ShipmentMode shipmentMode)
        {
            Delete(shipmentMode);
            string result = $"ShipmentMode details of {shipmentMode.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ShipmentMode>> GetAllActiveShipmentModes([FromQuery] SearchParames searchParams)
        {
            var shipmentModeDetails = FindAll()
                             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShipmentModeName.Contains(searchParams.SearchValue) ||
                                    inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return shipmentModeDetails;
        }

        public async Task<IEnumerable<ShipmentMode>> GetAllShipmentModes([FromQuery] SearchParames searchParams)
        {
            var shipmentModeDetails = FindAll()
                              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShipmentModeName.Contains(searchParams.SearchValue) ||
                                     inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return shipmentModeDetails;
        }

        public async Task<ShipmentMode> GetShipmentModeById(int id)
        {
            var ShipmentModebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ShipmentModebyId;
        }

        public async Task<string> UpdateShipmentMode(ShipmentMode shipmentMode)
        {
            shipmentMode.LastModifiedBy = _createdBy;
            shipmentMode.LastModifiedOn = DateTime.Now;
            Update(shipmentMode);
            string result = $"shipmentMode details of {shipmentMode.Id} is updated successfully!";
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ShipmentInstructionsRepository : RepositoryBase<ShipmentInstructions>, IShipmentInstructionsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ShipmentInstructionsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";


        }

        public async Task<int?> CreateShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            shipmentInstructions.CreatedBy = _createdBy;
            shipmentInstructions.CreatedOn = DateTime.Now;
            shipmentInstructions.Unit = _unitname;
            var result = await Create(shipmentInstructions);

            return result.Id;
        }

        public async Task<string> DeleteShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            Delete(shipmentInstructions);
            string result = $"ShipmentInstructions details of {shipmentInstructions.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ShipmentInstructions>> GetAllActiveShipmentInstructions([FromQuery] SearchParames searchParams)
        {
            var shipmentInstructionDetails = FindAll()
                                        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShipmentInstructionsName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return shipmentInstructionDetails;
        }

        public async Task<IEnumerable<ShipmentInstructions>> GetAllShipmentInstructions([FromQuery] SearchParames searchParams)
        {
            var shipmentInstructionDetails = FindAll()
                                        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ShipmentInstructionsName.Contains(searchParams.SearchValue) ||
                                  inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return shipmentInstructionDetails;
        }

        public async Task<ShipmentInstructions> GetShipmentInstructionsById(int id)
        {
            var ShipmentInstructionsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return ShipmentInstructionsbyId;
        }

        public async Task<string> UpdateShipmentInstructions(ShipmentInstructions shipmentInstructions)
        {
            shipmentInstructions.LastModifiedBy = _createdBy;
            shipmentInstructions.LastModifiedOn = DateTime.Now;
            Update(shipmentInstructions);
            string result = $"ShipmentInstructions details of {shipmentInstructions.Id} is updated successfully!";
            return result;
        }
    }
}

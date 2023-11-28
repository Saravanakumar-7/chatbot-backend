using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using Entities.Helper;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Repository
{
    public class LocationsRepository : RepositoryBase<Locations>, ILocationsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LocationsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateLocations(Locations locations)
        {
            locations.CreatedBy = _createdBy;
            locations.CreatedOn = DateTime.Now;
            locations.Unit = _unitname;
            var result = await Create(locations);

            return result.Id;
        }

        public async Task<string> DeleteLocations(Locations locations)
        {
            Delete(locations);
            string result = $"Locations details of {locations.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Locations>> GetAllActiveLocations([FromQuery] SearchParames searchParams)
        {
            var LocationsDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LocationName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            
            return LocationsDetails;
        }

        public async Task<IEnumerable<Locations>> GetAllLocations([FromQuery] SearchParames searchParams)
        {
            var LocationsDetails = FindAll().OrderByDescending(x => x.Id)
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.LocationName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return LocationsDetails;
        }
        public async Task<Locations> GetLocationsById(int id)
        {
            var LocationsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return LocationsbyId;
        }

        public async Task<string> UpdateLocations(Locations locations)
        {
            locations.LastModifiedBy = _createdBy;
            locations.LastModifiedOn = DateTime.Now;
            Update(locations);
            string result = $"Locations details of {locations.Id} is updated successfully!";
            return result;
        }
        //public async Task<Locations> GetListofLocationsByWarehouse(string Warehouse)
        //{
        //    var locationsbyWh = await TipsMasterDbContext.Locations

        //        .Where(x => x.Warehouse == Warehouse).FirstOrDefaultAsync();

        //    return locationsbyWh;
        //}

        //public async Task<Locations> GetListofLocationsByWarehouse(string Warehouse)
        //{
        //    var locationsBywh = await FindByCondition(x => x.Warehouse == Warehouse)
        //        .ToList();

        //    return locationsBywh;
        //}
        public async Task<IEnumerable<Locations>> GetListofLocationsByWarehouse(string Warehouse)
        {
            IEnumerable<Locations> locationbyWh = await TipsMasterDbContext.Locations
             .Where(x => x.Warehouse == Warehouse ).ToListAsync();

            return locationbyWh;

            
        }
    }
    
}

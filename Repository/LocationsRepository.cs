using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LocationsRepository : RepositoryBase<Locations>, ILocationsRepository
    {
        public LocationsRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateLocations(Locations locations)
        {
            locations.CreatedBy = "Admin";
            locations.CreatedOn = DateTime.Now;
            var result = await Create(locations);
            return result.Id;
        }

        public async Task<string> DeleteLocations(Locations locations)
        {
            Delete(locations);
            string result = $"Locations details of {locations.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Locations>> GetAllActiveLocations()
        {
            var LocationsList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return LocationsList;
        }

        public async Task<IEnumerable<Locations>> GetAllLocations()
        {

            var LocationsList = await FindAll().ToListAsync();
            return LocationsList;
        }

        public async Task<Locations> GetLocationsById(int id)
        {
            var LocationsList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return LocationsList;
        }

        public async Task<string> UpdateLocations(Locations locations)
        {
            locations.LastModifiedBy = "Admin";
            locations.LastModifiedOn = DateTime.Now;
            Update(locations);
            string result = $"Locations details of {locations.Id} is updated successfully!";
            return result;
        }
    }
}

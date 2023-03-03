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
    public class CityRepository : RepositoryBase<City>, ICityRepository
    {
        public CityRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateCity(City city)
        {
            city.CreatedBy = "Admin";
            city.CreatedOn = DateTime.Now;
            city.Unit = "Bangalore";
            var result = await Create(city); return result.Id;
        }
        public async Task<string> DeleteCity(City city)
        {
            Delete(city);
            string result = $"city details of {city.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<City>> GetAllActiveCities()
        {
            var AllActivecities = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivecities;
        }
        public async Task<IEnumerable<City>> GetAllCities()
        {
            var GetallCities = await FindAll().ToListAsync(); return GetallCities;
        }
        public async Task<City> GetCityById(int id)
        {
            var cityById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return cityById;
        }
        public async Task<string> UpdateCity(City city)
        {
            city.LastModifiedBy = "Admin";
            city.LastModifiedOn = DateTime.Now;
            Update(city);
            string result = $"city details of {city.Id} is updated successfully!";
            return result;
        }
    }
}
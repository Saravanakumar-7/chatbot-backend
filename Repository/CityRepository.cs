using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<PagedList<City>> GetAllActiveCities([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var cITYDetails = FindAll()
                                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CityName.Contains(searchParams.SearchValue) ||
                                  inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<City>.ToPagedList(cITYDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<City>> GetAllCities([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var cITYDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CityName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<City>.ToPagedList(cITYDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
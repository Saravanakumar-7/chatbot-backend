using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ICityRepository : IRepositoryBase<City>
    {
        Task<PagedList<City>> GetAllCities(PagingParameter pagingParameter, SearchParames searchParams);
        Task<City> GetCityById(int id);
        Task<PagedList<City>> GetAllActiveCities(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCity(City city);
        Task<string> UpdateCity(City city);
        Task<string> DeleteCity(City city);
    }
}
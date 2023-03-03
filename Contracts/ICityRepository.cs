using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface ICityRepository : IRepositoryBase<City>
    {
        Task<IEnumerable<City>> GetAllCities();
        Task<City> GetCityById(int id);
        Task<IEnumerable<City>> GetAllActiveCities();
        Task<int?> CreateCity(City city);
        Task<string> UpdateCity(City city);
        Task<string> DeleteCity(City city);
    }
}
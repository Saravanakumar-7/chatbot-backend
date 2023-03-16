using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface ILocationsRepository : IRepositoryBase<Locations>
    {
        Task<PagedList<Locations>> GetAllLocations(PagingParameter pagingParameter, SearchParames searchParames);
        Task<Locations> GetLocationsById(int id);
        Task<PagedList<Locations>> GetAllActiveLocations(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateLocations(Locations locations);
        Task<string> UpdateLocations(Locations locations);
        Task<string> DeleteLocations(Locations locations);
        Task<IEnumerable<Locations>> GetListofLocationsByWarehouse(string Warehouse);

    }
}

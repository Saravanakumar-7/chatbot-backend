using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinPartsRepository : IRepositoryBase<GrinParts>
    { 
        Task<GrinParts> UpdateGrinPartsQty(int GrinPartId, string AcceptedQty, string RejectedQty);
        Task<string> UpdateGrinQty(GrinParts grinparts);
        Task<GrinParts> GetGrinPartsById(int id);
        Task<GrinParts> DeleteGrinPartsById(int id);
        Task<PagedList<GrinParts>> GetAllGrinParts( PagingParameter pagingParameter, SearchParams searchParams);
        Task<GrinParts> GetGrinPartsDetailsbyGrinPartId(int GrinPartId);
        Task<string> DeleteGrinParts(GrinParts grinParts);
        Task<IEnumerable<GrinParts>> GetGrinPartsDetailsByGrinPartIds(List<int> grinPartIds);
        Task<GrinParts> GetGrinPartsByItemNo(string itemNumber);
        Task<int?> GetGrinPartsIqcStatusCount(int grinId);
        Task<int?> GetGrinPartsBinningStatusCount(int grinId);
        Task<int?> GetGrinPartsCount(int grinId);
    }
}


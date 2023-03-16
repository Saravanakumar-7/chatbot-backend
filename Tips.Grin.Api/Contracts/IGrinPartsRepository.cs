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

        Task<PagedList<GrinParts>> GetAllGrinParts([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<GrinParts> GetGrinPartsDetailsbyGrinPartId(int GrinPartId);
        Task<GrinParts> GetGrinPartsById(int id);
        Task<string> DeleteGrinParts(GrinParts grinParts);

    }
}


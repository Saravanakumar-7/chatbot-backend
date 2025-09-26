using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    
       public interface IDiscountRangesRepository
    {
        Task<int?> CreateDiscountRanges(DiscountRanges discountRanges);
        Task<DiscountRanges> GetDiscountRangesById(int id);
        Task<DiscountRanges> GetDiscountRangesByAmount(decimal amount);
        Task<PagedList<DiscountRanges>> GetAllDiscountRanges(PagingParameter pagingParameter);
        Task UpdateDiscountRanges(DiscountRanges discountRanges);
    }
}

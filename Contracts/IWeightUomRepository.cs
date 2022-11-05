using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IWeightUomRepository
    {
        Task<IEnumerable<WeightUom>> GetAllWeightUom();
        Task<WeightUom> GetWeightUomById(int id);
        Task<IEnumerable<WeightUom>> GetAllActiveWeightUom();
        Task<int?> CreateWeightUom(WeightUom weightUom);
        Task<string> UpdateWeightUom(WeightUom weightUom);
        Task<string> DeleteWeightUom(WeightUom weightUom);

    }
}

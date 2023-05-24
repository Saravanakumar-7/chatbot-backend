using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductTypeRepository : IRepositoryBase<ProductType>
    {
        Task<IEnumerable<ProductType>> GetAllProductType();
        Task<ProductType> GetProductTypeById(int id);
        Task<int?> CreateProductType(ProductType productType);
        Task<string> UpdateProductType(ProductType productType);
        Task<string> DeleteProductType(ProductType productType);
        Task<IEnumerable<ProductType>> GetListOfTypeSolutionByProductType(string typeSolution);
    }
}
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
    public class ProductTypeRepository : RepositoryBase<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateProductType(ProductType productType)
        {
            productType.CreatedBy = "Admin";
            productType.CreatedOn = DateTime.Now;
            productType.Unit = "Bangalore";
            var result = await Create(productType);

            return result.Id;
        }

        public async Task<string> DeleteProductType(ProductType productType)
        {
            Delete(productType);
            string result = $"productType details of {productType.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<ProductType>> GetListOfTypeSolutionByProductType(string typeSolution)
        {
            IEnumerable<ProductType> typeSolutionByProductType = await TipsMasterDbContext.ProductTypes
             .Where(x => x.TypeSolution == typeSolution).ToListAsync();

            return typeSolutionByProductType;
        }
        public async Task<IEnumerable<ProductType>> GetAllProductType()
        {
            var getAllproductType = await FindAll().OrderByDescending(x => x.Id).ToListAsync();

            return getAllproductType;
        }

        public async Task<ProductType> GetProductTypeById(int id)
        {
            var productTypeByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return productTypeByid;
        }

        public async Task<string> UpdateProductType(ProductType productType)
        {
            productType.LastModifiedBy = "Admin";
            productType.LastModifiedOn = DateTime.Now;
            Update(productType);
            string result = $"productType details of {productType.Id} is updated successfully!";
            return result;
        }
    }
}
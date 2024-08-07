using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductTypeRepository : RepositoryBase<ProductType>, IProductTypeRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ProductTypeRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateProductType(ProductType productType)
        {
            productType.CreatedBy = _createdBy;
            productType.CreatedOn = DateTime.Now;
            productType.Unit = _unitname;
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
        public async Task<IEnumerable<ProductType>> GetAllActiveProductType()
        {
            var getAllActiveproductType = await FindAll().Where(x=>x.IsActive==true).OrderByDescending(x => x.Id).ToListAsync();

            return getAllActiveproductType;
        }

        public async Task<ProductType> GetProductTypeById(int id)
        {
            var productTypeByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return productTypeByid;
        }

        public async Task<string> UpdateProductType(ProductType productType)
        {
            productType.LastModifiedBy = _createdBy;
            productType.LastModifiedOn = DateTime.Now;
            Update(productType);
            string result = $"productType details of {productType.Id} is updated successfully!";
            return result;
        }
    }
}
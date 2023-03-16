using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DeliveryTermRepository : RepositoryBase<DeliveryTerm>, IDeliveryTermRepository
    {
        public DeliveryTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.CreatedBy = "Admin";
            deliveryTerm.CreatedOn = DateTime.Now;
            deliveryTerm.Unit = "Bangalore";
            var result = await Create(deliveryTerm);
            return result.Id;
        }

        public async Task<string> DeleteDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            Delete(deliveryTerm);
            string result = $"Delivery Terms details of {deliveryTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<DeliveryTerm>> GetAllActiveDeliveryTerms([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var deliveryTermDetails = FindAll()
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.DeliveryTermName.Contains(searchParams.SearchValue) ||
                      inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<DeliveryTerm>.ToPagedList(deliveryTermDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<PagedList<DeliveryTerm>> GetAllDeliveryTerms([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var deliveryTermDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.DeliveryTermName.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<DeliveryTerm>.ToPagedList(deliveryTermDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<DeliveryTerm> GetDeliveryTermById(int id)
        {
            var DeliveryTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DeliveryTermbyId;
          
        }

        public async Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.LastModifiedBy = "Admin";
            deliveryTerm.LastModifiedOn = DateTime.Now;
            Update(deliveryTerm);
            string result = $"Delivery Term of Detail {deliveryTerm.Id} is updated successfully!";
            return result;
        
        }
    }
}

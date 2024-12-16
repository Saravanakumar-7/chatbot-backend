using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    class FieldInformationRepository : RepositoryBase<FieldInformation>, IFieldInformationRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public FieldInformationRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }


        public async Task<FieldInformation> CreateFieldInformation(FieldInformation fieldInformation)
        {
            var date = DateTime.Now;
            fieldInformation.CreatedBy = _createdBy;
            fieldInformation.CreatedOn = date;
            fieldInformation.Unit = _unitname;
            var result = await Create(fieldInformation);

            return result;
        }


        public async Task<string> DeleteFieldInformation(FieldInformation fieldInformation)
        {
            Delete(fieldInformation);
            string result = $"lead details of {fieldInformation.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<FieldInformation>> GetAllActiveFieldInformation(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var fieldInformationDetails = FindAll()
                                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.TabName.Contains(searchParams.SearchValue) ||
                                  inv.FieldInfo.Contains(searchParams.SearchValue))));
            return PagedList<FieldInformation>.ToPagedList(fieldInformationDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<FieldInformation>> GetAllFieldInformation(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var fieldInformationDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.TabName.Contains(searchParams.SearchValue) ||
                 inv.FieldInfo.Contains(searchParams.SearchValue))));

            return PagedList<FieldInformation>.ToPagedList(fieldInformationDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<FieldInformation>> GetFieldInformationByIds(List<int> fieldIds)
        {
            var fieldInformationDetails = await TipsMasterDbContext.FieldInformations.Where(x => fieldIds.Contains(x.FieldId))
                               .ToListAsync();

            return fieldInformationDetails;
        }


        public async Task<string> UpdateFieldInformation(FieldInformation fieldInformation)
        {
            fieldInformation.LastModifiedBy = _createdBy;
            fieldInformation.LastModifiedOn = DateTime.Now;
            Update(fieldInformation);
            string result = $"FieldInformation details of {fieldInformation.Id} is updated successfully!";
            return result;
        }

    }
}

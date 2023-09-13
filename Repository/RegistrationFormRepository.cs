using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RegistrationFormRepository : RepositoryBase<RegistrationForm>, IRegistrationFormRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public RegistrationFormRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            //_unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRegistrationForm(RegistrationForm registrationForm)
        {
            registrationForm.CreatedBy = _createdBy;
            registrationForm.CreatedOn = DateTime.Now;
            //registrationForm.Unit = _unitname;
            var result = await Create(registrationForm);

            return result.Id;
        }

        public async Task<string> DeleteRegistrationForm(RegistrationForm registrationForm)
        {
            Delete(registrationForm);
            string result = $"RegistrationForm details of {registrationForm.Id} is deleted successfully!";
            return result;
        }

        public async Task<List<RegistrationForm>> GetAllActiveRegistrationForm()
        {
            var activeRegistrationFormDetails = FindAll().OrderByDescending(x => x.Id);

            return await activeRegistrationFormDetails.ToListAsync(); // Assuming you're using Entity Framework
        }


        //public async Task<PagedList<RegistrationForm>> GetAllActiveRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var activeRegistrationFormDetails = FindAll().OrderByDescending(x => x.Id)
        //      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UserName.Contains(searchParams.SearchValue) ||
        //         inv.EmailId.Contains(searchParams.SearchValue) || inv.FirstName.Contains(searchParams.SearchValue) ||
        //         inv.LastName.Contains(searchParams.SearchValue))));

        //    return PagedList<RegistrationForm>.ToPagedList(activeRegistrationFormDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        public async Task<IEnumerable<RegistrationForm>> GetAllRegistrationForm([FromQuery] SearchParames searchParams)
        {
            var registrationFormDetails = FindAll()
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
                               inv.UserName.Contains(searchParams.SearchValue) ||
                               inv.EmailId.Contains(searchParams.SearchValue) ||
                               inv.FirstName.Contains(searchParams.SearchValue) ||
                               inv.LastName.Contains(searchParams.SearchValue)))
                .OrderByDescending(x => x.Id);

            return registrationFormDetails.ToList();
        }

        //public async Task<PagedList<RegistrationForm>> GetAllRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        //{
        //    var registrationFormDetails = FindAll().OrderByDescending(x => x.Id)
        //     .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UserName.Contains(searchParams.SearchValue) ||
        //        inv.EmailId.Contains(searchParams.SearchValue) || inv.FirstName.Contains(searchParams.SearchValue) ||
        //        inv.LastName.Contains(searchParams.SearchValue))));

        //    return PagedList<RegistrationForm>.ToPagedList(registrationFormDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}

        public async Task<RegistrationForm> GetRegistrationFormById(int id)
        {
            var registrationFormById = await TipsMasterDbContext.RegistrationForms.Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return registrationFormById;
        }
        public async Task<int> GetRegistrationUserById(int id)
        {
            var getRoleId = await TipsMasterDbContext.RegistrationForms.Where(x => x.Id == id)
                .Select(x=>x.RoleId)
                 .FirstOrDefaultAsync();

            return getRoleId;
        }
        //test
        public async Task<RegistrationForm> GetRegistrationFormByUserNameandPassword(string username,string password)
        {
            var registrationFormDetails = await TipsMasterDbContext.RegistrationForms.Where(x => x.UserName == username && x.Password == password)
                 .FirstOrDefaultAsync();

            return registrationFormDetails;
        }

        public async Task<IEnumerable<RegistrationFormDetailsDto>> GetAllActiveRegistrationFormList()
        {
            IEnumerable<RegistrationFormDetailsDto> registrationFormList = await TipsMasterDbContext.RegistrationForms
                           .Select(x => new RegistrationFormDetailsDto()
                           {
                               Id = x.Id,
                               UserName = x.UserName,
                               RoleId = x.RoleId,
                               RoleName = x.RoleName,
                           }).ToListAsync();


            return registrationFormList;

        }

        public async Task<string> UpdateRegistrationForm(RegistrationForm registrationForm)
        {
            registrationForm.LastModifiedBy = _createdBy;
            registrationForm.LastModifiedOn = DateTime.Now;
            Update(registrationForm);
            string result = $"RegistrationForm of Detail {registrationForm.Id} is updated successfully!";
            return result;
        }
    }
}

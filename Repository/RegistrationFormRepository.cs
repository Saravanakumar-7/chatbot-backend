using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RegistrationFormRepository : RepositoryBase<RegistrationForm>, IRegistrationFormRepository
    {
        public RegistrationFormRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateRegistrationForm(RegistrationForm registrationForm)
        {
            registrationForm.CreatedBy = "Admin";
            registrationForm.CreatedOn = DateTime.Now;
            registrationForm.Unit = "Bangalore";
            var result = await Create(registrationForm);

            return result.Id;
        }

        public async Task<string> DeleteRegistrationForm(RegistrationForm registrationForm)
        {
            Delete(registrationForm);
            string result = $"RegistrationForm details of {registrationForm.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<RegistrationForm>> GetAllActiveRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var activeRegistrationFormDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UserName.Contains(searchParams.SearchValue) ||
                 inv.EmailId.Contains(searchParams.SearchValue) || inv.FirstName.Contains(searchParams.SearchValue) ||
                 inv.LastName.Contains(searchParams.SearchValue))));

            return PagedList<RegistrationForm>.ToPagedList(activeRegistrationFormDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<RegistrationForm>> GetAllRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var registrationFormDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UserName.Contains(searchParams.SearchValue) ||
                inv.EmailId.Contains(searchParams.SearchValue) || inv.FirstName.Contains(searchParams.SearchValue) ||
                inv.LastName.Contains(searchParams.SearchValue))));

            return PagedList<RegistrationForm>.ToPagedList(registrationFormDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RegistrationForm> GetRegistrationFormById(int id)
        {
            var registrationFormById = await TipsMasterDbContext.RegistrationForms.Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return registrationFormById;
        }

        public async Task<string> UpdateRegistrationForm(RegistrationForm registrationForm)
        {
            registrationForm.LastModifiedBy = "Admin";
            registrationForm.LastModifiedOn = DateTime.Now;
            Update(registrationForm);
            string result = $"RegistrationForm of Detail {registrationForm.Id} is updated successfully!";
            return result;
        }
    }
}

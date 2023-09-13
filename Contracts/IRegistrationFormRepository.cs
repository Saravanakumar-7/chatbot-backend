using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface IRegistrationFormRepository : IRepositoryBase<RegistrationForm>
    {
        //Task<PagedList<RegistrationForm>> GetAllRegistrationForm(PagingParameter pagingParameter, SearchParames searchParams);

        Task<IEnumerable<RegistrationForm>> GetAllRegistrationForm(SearchParames searchParams);
        Task<RegistrationForm> GetRegistrationFormById(int id);
        Task<List<RegistrationForm>> GetAllActiveRegistrationForm(); 
        Task<int?> CreateRegistrationForm(RegistrationForm registrationForm);

        Task<int> GetRegistrationUserById(int id);
        Task<string> UpdateRegistrationForm(RegistrationForm registrationForm);
        Task<string> DeleteRegistrationForm(RegistrationForm registrationForm);
        Task<IEnumerable<RegistrationFormDetailsDto>> GetAllActiveRegistrationFormList();
        Task<RegistrationForm> GetRegistrationFormByUserNameandPassword(string username, string password);
    }
}

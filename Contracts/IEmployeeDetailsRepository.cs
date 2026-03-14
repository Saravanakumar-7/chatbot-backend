using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeDetailsRepository : IRepositoryBase<EmployeeDetails>
    {
        Task<IEnumerable<EmployeeDetails>> GetAllEmployeeDetails(SearchParames searchparams);
        Task<EmployeeDetails> GetEmployeeDetailsById(int id);
        Task<int> CreateEmployeeDetails(EmployeeDetails employeeDetails);
        Task<string> UpdateEmployeeDetails(EmployeeDetails employeeDetails);
        Task<string> DeleteEmployeeDetails(EmployeeDetails employeeDetails);

    }
}

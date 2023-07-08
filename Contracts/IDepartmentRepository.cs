using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDepartmentRepository : IRepositoryBase<Department>
    {
        Task<IEnumerable<Department>> GetAllDepartment();
        Task<Department> GetDepartmentById(int id);
        Task<IEnumerable<Department>> GetAllActiveDepartment();
        Task<int?> CreateDepartment(Department department);
        Task<string> UpdateDepartment(Department department);
        Task<string> DeleteDepartment(Department department);
    }
}

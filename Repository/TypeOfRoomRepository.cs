using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;

namespace Repository
{
    public class TypeOfRoomRepository : RepositoryBase<TypeOfRoom>, ITypeOfRoomRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public TypeOfRoomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateTypeOfRoom(TypeOfRoom typeOfRoom)
        {
            typeOfRoom.CreatedBy = _createdBy;
            typeOfRoom.CreatedOn = DateTime.Now;
            typeOfRoom.Unit = _unitname;
            var result = await Create(typeOfRoom);

            return result.Id;
        }

        public async Task<string> DeleteTypeOfRoom(TypeOfRoom typeOfRoom)
        {
            Delete(typeOfRoom);
            string result = $"TypeOfRoom details of {typeOfRoom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<TypeOfRoom>> GetAllActiveTypeOfRoom()
        {
            var AllActiveTypeOfRoom = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return AllActiveTypeOfRoom;
        }

        public async Task<IEnumerable<TypeOfRoom>> GetAllTypeOfRoom()
        {
            var GetallTypeOfRoom = await FindAll().ToListAsync();
            return GetallTypeOfRoom;
        }

        public async Task<TypeOfRoom> GetTypeOfRoomById(int id)
        {
            var TypeOfRoombyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return TypeOfRoombyId;
        }

        public async Task<string> UpdateTypeOfRoom(TypeOfRoom typeOfRoom)
        {
            typeOfRoom.LastModifiedBy = "Admin";
            typeOfRoom.LastModifiedOn = DateTime.Now;
            Update(typeOfRoom);
            string result = $"UOM details of {typeOfRoom.Id} is updated successfully!";
            return result;
        }
    }
}

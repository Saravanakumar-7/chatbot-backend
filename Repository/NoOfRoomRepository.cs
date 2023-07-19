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
    public class NoOfRoomRepository : RepositoryBase<NoOfRoom>, INoOfRoomRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public NoOfRoomRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateNoOfRoom(NoOfRoom noOfRoom)
        {
            noOfRoom.CreatedBy = _createdBy;
            noOfRoom.CreatedOn = DateTime.Now;
            noOfRoom.Unit = _unitname;
            var result = await Create(noOfRoom);

            return result.Id;
        }

        public async Task<string> DeleteNoOfRoom(NoOfRoom noOfRoom)
        {
            Delete(noOfRoom);
            string result = $"NoOfRoom details of {noOfRoom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<NoOfRoom>> GetAllActiveNoOfRoom()
        {
            var AllActiveNoOfRoom = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return AllActiveNoOfRoom;
        }

        public async Task<IEnumerable<NoOfRoom>> GetAllNoOfRoom()
        {
            var GetallNoOfRoom = await FindAll().ToListAsync();
            return GetallNoOfRoom;
        }

        public async Task<NoOfRoom> GetNoOfRoomById(int id)
        {
            var NoOfRoombyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return NoOfRoombyId;
        }

        public async Task<string> UpdateNoOfRoom(NoOfRoom noOfRoom)
        {
            noOfRoom.LastModifiedBy = "Admin";
            noOfRoom.LastModifiedOn = DateTime.Now;
            Update(noOfRoom);
            string result = $"NoOfRoom details of {noOfRoom.Id} is updated successfully!";
            return result;
        }
    }
}

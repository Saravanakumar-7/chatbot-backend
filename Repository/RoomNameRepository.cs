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
    public class RoomNameRepository : RepositoryBase<RoomNames>, IRoomNameRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RoomNameRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRoomName(RoomNames roomName)
        {
            roomName.CreatedBy = _createdBy;
            roomName.CreatedOn = DateTime.Now;
            roomName.Unit = _unitname;
            var result = await Create(roomName);

            return result.Id;
        }

        public async Task<string> DeleteRoomName(RoomNames roomName)
        {
            Delete(roomName);
            string result = $"typeSolution details of {roomName.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RoomNames>> GetAllRoomNames()
        {
            var getAllRoomNames = await FindAll().OrderByDescending(x => x.Id).ToListAsync();

            return getAllRoomNames;
        }

        public async Task<RoomNames> GetRoomNameById(int id)
        {
            var roomNameByid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return roomNameByid;
        }

        public async Task<string> UpdateRoomName(RoomNames roomName)
        {
            roomName.LastModifiedBy = _createdBy;
            roomName.LastModifiedOn = DateTime.Now;
            Update(roomName);
            string result = $"typeSolution details of {roomName.Id} is updated successfully!";
            return result;
        }
    }
}

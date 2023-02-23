using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RoomNameRepository : RepositoryBase<RoomNames>, IRoomNameRepository
    {
        public RoomNameRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateRoomName(RoomNames roomName)
        {
            roomName.CreatedBy = "Admin";
            roomName.CreatedOn = DateTime.Now;
            roomName.Unit = "Bangalore";
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
            roomName.LastModifiedBy = "Admin";
            roomName.LastModifiedOn = DateTime.Now;
            Update(roomName);
            string result = $"typeSolution details of {roomName.Id} is updated successfully!";
            return result;
        }
    }
}

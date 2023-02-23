using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRoomNameRepository : IRepositoryBase<RoomNames>
    {
        Task<IEnumerable<RoomNames>> GetAllRoomNames();
        Task<RoomNames> GetRoomNameById(int id);
        Task<int?> CreateRoomName(RoomNames roomName);
        Task<string> UpdateRoomName(RoomNames roomName);
        Task<string> DeleteRoomName(RoomNames roomName);
    }
}

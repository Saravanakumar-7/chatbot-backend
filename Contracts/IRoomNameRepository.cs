using Entities;

namespace Contracts
{
    public interface IRoomNameRepository : IRepositoryBase<RoomNames>
    {
        Task<IEnumerable<RoomNames>> GetAllRoomNames(SearchParames searchParams);
        Task<RoomNames> GetRoomNameById(int id);
        Task<int?> CreateRoomName(RoomNames roomName);
        Task<string> UpdateRoomName(RoomNames roomName);
        Task<string> DeleteRoomName(RoomNames roomName);
    }
}

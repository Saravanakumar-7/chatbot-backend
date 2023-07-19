using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITypeOfRoomRepository
    {
        Task<IEnumerable<TypeOfRoom>> GetAllTypeOfRoom();
        Task<TypeOfRoom> GetTypeOfRoomById(int id);
        Task<IEnumerable<TypeOfRoom>> GetAllActiveTypeOfRoom();
        Task<int?> CreateTypeOfRoom(TypeOfRoom typeOfRoom);
        Task<string> UpdateTypeOfRoom(TypeOfRoom typeOfRoom);
        Task<string> DeleteTypeOfRoom(TypeOfRoom typeOfRoom);
    }
}

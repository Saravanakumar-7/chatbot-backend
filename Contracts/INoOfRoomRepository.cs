using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface INoOfRoomRepository
    {
        Task<IEnumerable<NoOfRoom>> GetAllNoOfRoom();
        Task<NoOfRoom> GetNoOfRoomById(int id);
        Task<IEnumerable<NoOfRoom>> GetAllActiveNoOfRoom();
        Task<int?> CreateNoOfRoom(NoOfRoom noOfRoom);
        Task<string> UpdateNoOfRoom(NoOfRoom noOfRoom);
        Task<string> DeleteNoOfRoom(NoOfRoom noOfRoom);
    }
}

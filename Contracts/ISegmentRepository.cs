using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ISegmentRepository : IRepositoryBase<Segment>
    {
        Task<IEnumerable<Segment>> GetAllSegment();
        Task<Segment> GetSegmentById(int id);
        Task<IEnumerable<Segment>> GetAllActiveSegment();
        Task<int?> CreateSegment(Segment segment);
        Task<string> UpdateSegment(Segment segment);
        Task<string> DeleteSegment(Segment segment);
    }

}

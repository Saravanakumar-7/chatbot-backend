using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class SegmentRepository : RepositoryBase<Segment>, ISegmentRepository
    {
        public SegmentRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateSegment(Segment segment)
        {
            segment.CreatedBy = "Admin";
            segment.CreatedOn = DateTime.Now;
            segment.Unit = "Bangalore";
            var result = await Create(segment);
          
            return result.Id;
        }

        public async Task<string> DeleteSegment(Segment segment)
        {
            Delete(segment);
            string result = $"Segment details of {segment.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Segment>> GetAllActiveSegment()
        {
            var AllActiveSegmentList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return AllActiveSegmentList;
        }

        public async Task<IEnumerable<Segment>> GetAllSegment()
        {
            var GetallsegmentList = await FindAll().ToListAsync();
            return GetallsegmentList;
        }

        public async Task<Segment> GetSegmentById(int id)
        {
            var SegmentbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return SegmentbyId;
        }

        public async Task<string> UpdateSegment(Segment segment)
        {
            segment.LastModifiedBy = "Admin";
            segment.LastModifiedOn = DateTime.Now;
            Update(segment);
            string result = $"Segment details of {segment.Id} is updated successfully!";
            return result;
        }
    }
}

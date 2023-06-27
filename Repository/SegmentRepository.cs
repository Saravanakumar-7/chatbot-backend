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

namespace Repository
{
    public class SegmentRepository : RepositoryBase<Segment>, ISegmentRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SegmentRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";


        }

        public async Task<int?> CreateSegment(Segment segment)
        {
            segment.CreatedBy = _createdBy;
            segment.CreatedOn = DateTime.Now;
            segment.Unit = _unitname;
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
            segment.LastModifiedBy = _createdBy;
            segment.LastModifiedOn = DateTime.Now;
            Update(segment);
            string result = $"Segment details of {segment.Id} is updated successfully!";
            return result;
        }
    }
}

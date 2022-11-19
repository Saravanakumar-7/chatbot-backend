using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EngineeringBomRepository : RepositoryBase<EnggBom>, IEnggBomRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EngineeringBomRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;   
        }

        public async Task<int?> CreateEnggBom(EnggBom enggBom)
        {
            enggBom.CreatedBy = "Admin";
            enggBom.CreatedOn = DateTime.Now;
            enggBom.LastModifiedBy = "Admin";
            enggBom.LastModifiedOn = DateTime.Now;
            enggBom.Unit = "Bangalore";

            var result = await Create(enggBom);
            return result.Id;
        }

        public async Task<string> DeleteEnggBom(EnggBom enggBom)
        {
            Delete(enggBom);
            string result = $"BOM details of {enggBom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<EnggBom>> GetAllActiveEnggBom()
        {
            var bomDetails = await FindAll().ToListAsync();
            return bomDetails;
        }

        public async Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter)
        {
            var bomDetails = PagedList<EnggBom>.ToPagedList(FindAll()
                                .Include(t => t.EnggChildItems)
                                .ThenInclude(s => s.EnggAlternates)
                                .Include(m => m.EnggChildItems)
                                .ThenInclude(i => i.NREConsumable)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);


            return bomDetails;
        }

        public async Task<EnggBom> GetEnggBomById(int id)
        {
            var bomDetails = await _tipsMasterDbContext.EnggBoms.Where(x => x.Id == id)
                              .Include(t => t.EnggChildItems)
                                .ThenInclude(x => x.EnggAlternates)
                                .Include(m => m.EnggChildItems)
                                .ThenInclude(i => i.NREConsumable)
                              .FirstOrDefaultAsync();

            return bomDetails;
        }

        public async Task<string> UpdateEnggBom(EnggBom enggBom)
        {
            enggBom.LastModifiedBy = "Admin";
            enggBom.LastModifiedOn = DateTime.Now;
            Update(enggBom);
            string result = $"Engineering BOM Detail {enggBom.Id} is updated successfully!";
            return result;
        }
    }
}

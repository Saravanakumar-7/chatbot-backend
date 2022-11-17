using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class TipsProductionDbContext : DbContext
    {

        public TipsProductionDbContext(DbContextOptions<TipsProductionDbContext> options) : base(options)
        {

        }

    }
}

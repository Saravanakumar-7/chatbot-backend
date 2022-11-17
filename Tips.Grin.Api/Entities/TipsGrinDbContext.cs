using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities
{
    public class TipsGrinDbContext : DbContext
    {

        public TipsGrinDbContext(DbContextOptions<TipsGrinDbContext> options) : base(options)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Tips.AiAgent.Api.Entities
{
    public class TipsAiAgentDbContext : DbContext
    {
        public TipsAiAgentDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Register Entities here
        }
    }
}

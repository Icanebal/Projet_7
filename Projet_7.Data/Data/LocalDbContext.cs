using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;

namespace Projet_7.Data.Data
{
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<CurvePoint> CurvePoints { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RuleName> RuleNames { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<User> Users { get; set;}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        
    }
}
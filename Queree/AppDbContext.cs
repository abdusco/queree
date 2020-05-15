using Microsoft.EntityFrameworkCore;
using Queree.Seed;

namespace Queree
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public AppDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=queree.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(user => user.Id);
                builder.Property(user => user.Id).ValueGeneratedOnAdd();
                builder.Property(user => user.Name).IsRequired();
            });
        }
    }
}
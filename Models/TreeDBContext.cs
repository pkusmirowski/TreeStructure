using Microsoft.EntityFrameworkCore;

namespace TreeStructure.Models
{
    public class TreeDBContext : DbContext
    {
        public TreeDBContext(DbContextOptions<TreeDBContext> options) : base(options) { }

        public DbSet<Tree> Trees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tree>()
                .HasMany(t => t.InverseParent)
                .WithOne(t => t.Parent)
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or ClientSetNull

            base.OnModelCreating(modelBuilder);
        }
    }
}
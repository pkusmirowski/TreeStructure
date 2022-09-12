using Microsoft.EntityFrameworkCore;

namespace TreeStructure.Models
{
    public partial class TreeDBContext : DbContext
    {
        public TreeDBContext()
        {
        }

        public TreeDBContext(DbContextOptions<TreeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tree> Trees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tree>(entity =>
            {
                entity.ToTable("Tree");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Folder)
                    .HasMaxLength(99)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK__Tree__ParentID__24927208");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

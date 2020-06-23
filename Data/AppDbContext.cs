using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Plantagoo.Entities;
using Plantagoo.Entities.ManyToMany;

namespace Plantagoo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<ProjectModel> Projects { get; set; }
        public virtual DbSet<TaskModel> Tasks { get; set; }
        public virtual DbSet<TagModel> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var guidConverter = new GuidToStringConverter();

            modelBuilder.Entity<UserModel>().HasKey(k => k.Id);
            modelBuilder.Entity<UserModel>().Property(p => p.Id).HasConversion(guidConverter);
            modelBuilder.Entity<UserModel>().HasIndex(b => b.Email).IsUnique();

            modelBuilder.Entity<ProjectModel>().HasKey(k => k.Id);
            modelBuilder.Entity<ProjectModel>().Property(p => p.Id).HasConversion(guidConverter);

            modelBuilder.Entity<TaskModel>().HasKey(k => k.Id);
            modelBuilder.Entity<TaskModel>().Property(p => p.Id).HasConversion(guidConverter);

            modelBuilder.Entity<TagModel>().HasKey(k => k.Id);
            modelBuilder.Entity<TagModel>().Property(p => p.Id).HasConversion(guidConverter);

            modelBuilder.Entity<UserModel>()
                .HasMany(c => c.Tags)
                .WithOne(s => s.Owner)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(c => c.Projects)
                .WithOne(s => s.Owner)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectModel>()
                .HasMany(c => c.Tasks)
                .WithOne(s => s.Project)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MMProjectTag>().HasKey(sc => new { sc.ProjectId, sc.TagId });

            modelBuilder.Entity<MMProjectTag>()
            .HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTags)
            .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<MMProjectTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProjectTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}

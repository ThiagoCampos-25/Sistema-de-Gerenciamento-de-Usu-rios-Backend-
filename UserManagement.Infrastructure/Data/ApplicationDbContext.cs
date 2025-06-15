using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.CreateAt).IsRequired();
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            //Configure Role entity
            modelBuilder.Entity<Role>(entity => 
            { 
                entity.HasIndex(r => r.Name).IsUnique();
            });

            //Configure UserRole many-to-many relationship
            modelBuilder.Entity<UserRole>(entity => 
            { 
                entity.HasKey(ur => new {ur.UserId, ur.RoleId});

                entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            });

            //Seed initial roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("c0b8d7e0-0b0a-4b0c-8c0d-0e0f10111213"), Name = "Admin" },
                new Role { Id = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef"), Name = "User" }
            );
        }
    }
}

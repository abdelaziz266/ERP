using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Enums;

namespace ERP.Modules.Users.Infrastructure.Data
{
    public class UsersDbContext : IdentityDbContext<User, Role, Guid>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.Gender)
                    .HasConversion<int>();

                entity.Property(u => u.Language)
                    .HasConversion<string>()
                    .HasMaxLength(10)
                    .HasDefaultValue(Language.en);

                entity.Property(u => u.Birthday)
                    .HasColumnType("date");

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.IsDeleted)
                    .HasDefaultValue(false);

                entity.HasIndex(u => u.UserName).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(r => r.IsDeleted)
                    .HasDefaultValue(false);
            });
        }
    }
}

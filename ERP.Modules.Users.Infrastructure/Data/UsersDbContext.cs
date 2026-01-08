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
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<RolePage> RolePages { get; set; } = null!;

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

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Pages");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.NameAr)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(p => p.NameEn)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(p => p.Key)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.IsDeleted)
                    .HasDefaultValue(false);

                entity.HasOne(p => p.Parent)
                    .WithMany(p => p.SubPages)
                    .HasForeignKey(p => p.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.NameAr).IsUnique().HasFilter("IsDeleted = 0");
                entity.HasIndex(p => p.NameEn).IsUnique().HasFilter("IsDeleted = 0");
                entity.HasIndex(p => p.Key).IsUnique().HasFilter("IsDeleted = 0");
            });

            modelBuilder.Entity<RolePage>(entity =>
            {
                entity.ToTable("RolePages");

                entity.HasKey(rp => rp.Id);

                entity.Property(rp => rp.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(rp => rp.IsDeleted)
                    .HasDefaultValue(false);

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePages)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Page)
                    .WithMany(p => p.RolePages)
                    .HasForeignKey(rp => rp.PageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rp => new { rp.RoleId, rp.PageId })
                    .IsUnique()
                    .HasFilter("IsDeleted = 0");
            });
        }
    }
}

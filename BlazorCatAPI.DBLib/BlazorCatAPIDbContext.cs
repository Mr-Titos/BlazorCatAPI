using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCatAPI.DBLib
{
    public class BlazorCatAPIDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlite("Data Source=BlazorCatAPI.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(u => u.NameIdentifier);

                u.Property(u => u.GivenName)
                    .HasMaxLength(20)
                    .IsUnicode();

                u.Property(u => u.Surname)
                    .HasMaxLength(40)
                    .IsUnicode();

                u.Property(u => u.Password)
                    .HasMaxLength(256)
                    .HasColumnType("text");

                u.HasIndex(u => u.Email).IsUnique(true);

                u.Property(u => u.isDarkMode).HasDefaultValue(false);

                u.Property(u => u.IsAuthenticated).HasDefaultValue(false);

                u.HasMany(u => u.Favorites).WithOne(u => u.User);
            });

            modelBuilder.Entity<Favorite>(f =>
            {
                f.HasKey(f => f.Id);

                f.Property(f => f.ImageId);

                f.HasOne(f => f.User).WithMany(u => u.Favorites);
            });
        }
    }
}

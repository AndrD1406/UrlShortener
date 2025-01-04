using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UrlShortener.DataAccess.Models;
using UrlShortener.DataAccess.Models.Configurations;
using Microsoft.AspNetCore.Identity;

namespace UrlShortener.DataAccess
{
    public class UrlShortenerDbContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public UrlShortenerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public UrlShortenerDbContext() : base() { }

        public virtual DbSet<Url> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UrlConfiguration());

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    NormalizedName = "USER"
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=localhost;Port=3306;Database=urlshortenerdb;Uid=root;Pwd=mysql1406maga;";

                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39)));
            }
        }
    }
}

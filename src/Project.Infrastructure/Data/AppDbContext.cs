using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Interfaces;

namespace Project.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {  }
        
        public DbSet<Language> Languages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

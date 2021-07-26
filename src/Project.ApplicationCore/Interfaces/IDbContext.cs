using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Entities;

namespace Project.ApplicationCore.Interfaces
{
    public interface IDbContext
    {
        DbSet<AppUser> Users { get; set; }
        DbSet<Language> Languages { get; set; }
    }
}
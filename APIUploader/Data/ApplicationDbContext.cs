using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
namespace WebApplication1.Data
{
    

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define aquí tus DbSet para las entidades
        public DbSet<User> Users { get; set; }

        public DbSet<Archivo> Archivos { get; set; }    
    }
}

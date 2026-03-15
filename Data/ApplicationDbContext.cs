using Microsoft.EntityFrameworkCore;
using ColegioPrivado.Models;

namespace ColegioPrivado.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<SolicitudesAcceso> SolicitudesAccesos { get; set; }
        public DbSet<HistorialPassword> HistorialPassword { get; set; }

    }
}
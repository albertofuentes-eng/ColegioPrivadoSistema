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

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        public DbSet<DevolucionCompra> DevolucionCompras { get; set; }
        public DbSet<DetalleDevolucion> DetalleDevoluciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 DECIMALES
            modelBuilder.Entity<Compra>()
                .Property(c => c.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.PrecioCompra)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioCompra)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVenta)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DevolucionCompra>()
                .Property(d => d.TotalCredito)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Proveedor>()
                .Property(p => p.CreditoDisponible)
                .HasColumnType("decimal(18,2)");

            // 🔹 MULTIEMPRESA (IMPORTANTE)
            modelBuilder.Entity<Proveedor>()
                .Property(p => p.EmpresaId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Producto>()
                .Property(p => p.EmpresaId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Compra>()
                .Property(c => c.EmpresaId)
                .HasDefaultValue(1);
        }
    }
}
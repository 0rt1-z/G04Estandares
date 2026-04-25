using ConsultoriaApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultoriaApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<TareaDependencia> TareaDependencias { get; set; }
    public DbSet<RegistroTiempo> RegistroTiempos { get; set; }
    public DbSet<Alerta> Alertas { get; set; }
    public DbSet<ProyectoUsuario> ProyectoUsuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Mapear tablas de Identity a los nombres del script SQL ──
        builder.Entity<ApplicationUser>()
            .ToTable("AppUsers");
        builder.Entity<IdentityRole>()
            .ToTable("AppRoles");
        builder.Entity<IdentityUserRole<string>>()
            .ToTable("AppUserRoles");
        builder.Entity<IdentityUserClaim<string>>()
            .ToTable("AppUserClaims");
        builder.Entity<IdentityUserLogin<string>>()
            .ToTable("AppUserLogins");
        builder.Entity<IdentityRoleClaim<string>>()
            .ToTable("AppRoleClaims");
        builder.Entity<IdentityUserToken<string>>()
            .ToTable("AppUserTokens");

        // ── ProyectoUsuario — clave compuesta ───────────────────────
        builder.Entity<ProyectoUsuario>()
            .HasKey(pu => new { pu.ProyectoId, pu.UsuarioId });

        builder.Entity<ProyectoUsuario>()
            .HasOne(pu => pu.Proyecto)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(pu => pu.ProyectoId);

        builder.Entity<ProyectoUsuario>()
            .HasOne(pu => pu.Usuario)
            .WithMany(u => u.Proyectos)
            .HasForeignKey(pu => pu.UsuarioId);

        // ── TareaDependencia — clave compuesta ──────────────────────
        builder.Entity<TareaDependencia>()
            .HasKey(td => new { td.TareaId, td.DependeDeTareaId });

        builder.Entity<TareaDependencia>()
            .HasOne(td => td.Tarea)
            .WithMany(t => t.Dependencias)
            .HasForeignKey(td => td.TareaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TareaDependencia>()
            .HasOne(td => td.DependeDe)
            .WithMany(t => t.DependientesDe)
            .HasForeignKey(td => td.DependeDeTareaId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Columna calculada RegistroTiempo ────────────────────────
        builder.Entity<RegistroTiempo>()
            .Property(r => r.HorasTotales)
            .HasComputedColumnSql(
                "CAST(DATEDIFF(MINUTE, HoraInicio, HoraFin) AS DECIMAL(10,2)) / 60",
                stored: true);

        // ── Precisión de decimales ───────────────────────────────────
        builder.Entity<Proyecto>()
            .Property(p => p.Presupuesto).HasPrecision(18, 2);
        builder.Entity<Proyecto>()
            .Property(p => p.TarifaHora).HasPrecision(10, 2);
        builder.Entity<ApplicationUser>()
            .Property(u => u.TarifaHora).HasPrecision(10, 2);
        builder.Entity<Tarea>()
            .Property(t => t.HorasEstimadas).HasPrecision(10, 2);
        builder.Entity<RegistroTiempo>()
            .Property(r => r.HorasTotales).HasPrecision(10, 2);
    }
}
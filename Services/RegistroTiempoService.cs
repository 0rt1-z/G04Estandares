using ConsultoriaApp.Data;
using ConsultoriaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultoriaApp.Services;

public class RegistroTiempoService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IProyectoService _proyectoSvc;

    public RegistroTiempoService(ApplicationDbContext ctx, IProyectoService proyectoSvc)
    {
        _ctx = ctx;
        _proyectoSvc = proyectoSvc;
    }

    public async Task<List<RegistroTiempo>> ObtenerPorTareaAsync(int tareaId) =>
        await _ctx.RegistroTiempos
            .Include(rt => rt.Usuario)
            .Where(rt => rt.TareaId == tareaId)
            .OrderByDescending(rt => rt.Fecha)
            .ToListAsync();

    public async Task RegistrarAsync(RegistroTiempo registro)
    {
        _ctx.RegistroTiempos.Add(registro);
        await _ctx.SaveChangesAsync();

        var tarea = await _ctx.Tareas.FindAsync(registro.TareaId);
        if (tarea != null)
            await _proyectoSvc.VerificarAlertasAsync(tarea.ProyectoId);
    }

    public async Task<ReporteProyectoVM> GenerarReporteAsync(int proyectoId,
        DateTime? desde, DateTime? hasta)
    {
        var proyecto = await _ctx.Proyectos.FindAsync(proyectoId)
            ?? throw new Exception("Proyecto no encontrado");

        var query = _ctx.RegistroTiempos
            .Include(rt => rt.Usuario)
            .Include(rt => rt.Tarea)
            .Where(rt => rt.Tarea!.ProyectoId == proyectoId);

        if (desde.HasValue) query = query.Where(rt => rt.Fecha >= desde);
        if (hasta.HasValue) query = query.Where(rt => rt.Fecha <= hasta);

        var registros = await query.ToListAsync();
        var costoReal = registros.Sum(rt => rt.HorasTotales) * proyecto.TarifaHora;

        return new ReporteProyectoVM
        {
            Proyecto = proyecto,
            Registros = registros,
            HorasTotales = registros.Sum(rt => rt.HorasTotales),
            CostoReal = costoReal,
            Diferencia = proyecto.Presupuesto - costoReal
        };
    }
}

// ── ViewModels para reportes ─────────────────────────────────
public class ReporteProyectoVM
{
    public Proyecto Proyecto { get; set; } = null!;
    public List<RegistroTiempo> Registros { get; set; } = new();
    public decimal HorasTotales { get; set; }
    public decimal CostoReal { get; set; }
    public decimal Diferencia { get; set; }
}
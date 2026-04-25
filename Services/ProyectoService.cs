using ConsultoriaApp.Data;
using ConsultoriaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultoriaApp.Services;

public class ProyectoService : IProyectoService
{
    private readonly ApplicationDbContext _ctx;

    public ProyectoService(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<Proyecto>> ObtenerTodosAsync() =>
        await _ctx.Proyectos
            .Include(p => p.Lider)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();

    public async Task<Proyecto?> ObtenerPorIdAsync(int id) =>
        await _ctx.Proyectos
            .Include(p => p.Lider)
            .Include(p => p.Usuarios).ThenInclude(u => u.Usuario)
            .Include(p => p.Tareas).ThenInclude(t => t.Asignado)
            .Include(p => p.Alertas)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task CrearAsync(Proyecto proyecto)
    {
        _ctx.Proyectos.Add(proyecto);
        await _ctx.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Proyecto proyecto)
    {
        _ctx.Proyectos.Update(proyecto);
        await _ctx.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var proyecto = await _ctx.Proyectos.FindAsync(id);
        if (proyecto != null)
        {
            _ctx.Proyectos.Remove(proyecto);
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task<decimal> ObtenerCostoRealAsync(int proyectoId)
    {
        var proyecto = await _ctx.Proyectos.FindAsync(proyectoId);
        if (proyecto == null) return 0;

        var horas = await _ctx.RegistroTiempos
            .Where(rt => rt.Tarea!.ProyectoId == proyectoId)
            .SumAsync(rt => rt.HorasTotales);

        return horas * proyecto.TarifaHora;
    }

    public async Task VerificarAlertasAsync(int proyectoId)
    {
        var proyecto = await _ctx.Proyectos.FindAsync(proyectoId);
        if (proyecto == null) return;

        var costoReal = await ObtenerCostoRealAsync(proyectoId);
        var porcentaje = proyecto.Presupuesto > 0
            ? (costoReal / proyecto.Presupuesto) * 100 : 0;

        // Alerta: sobrecosto
        if (porcentaje >= 100)
            await CrearAlertaAsync(proyectoId, "PresupuestoAgotado",
                $"⚠️ Proyecto [{proyecto.Nombre}]: presupuesto AGOTADO. Costo real: ${costoReal:N2}");
        else if (porcentaje >= 80)
            await CrearAlertaAsync(proyectoId, "Sobrecosto",
                $"⚠️ Proyecto [{proyecto.Nombre}]: {porcentaje:N1}% del presupuesto consumido.");

        // Alerta: retraso
        if (DateTime.Today > proyecto.FechaFin && proyecto.Estado == "Activo")
            await CrearAlertaAsync(proyectoId, "Retraso",
                $"📅 Proyecto [{proyecto.Nombre}]: fecha límite vencida desde {proyecto.FechaFin:dd/MM/yyyy}.");

        await _ctx.SaveChangesAsync();
    }

    private async Task CrearAlertaAsync(int proyectoId, string tipo, string mensaje)
    {
        bool existe = await _ctx.Alertas
            .AnyAsync(a => a.ProyectoId == proyectoId && a.Tipo == tipo && !a.Leida);
        if (!existe)
            _ctx.Alertas.Add(new Alerta
            {
                ProyectoId = proyectoId,
                Tipo = tipo,
                Mensaje = mensaje
            });
    }
}
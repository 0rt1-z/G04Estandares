using ConsultoriaApp.Data;
using ConsultoriaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConsultoriaApp.Controllers;

[Authorize]
public class TareasController : Controller
{
    private readonly ApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userMgr;

    public TareasController(ApplicationDbContext ctx, UserManager<ApplicationUser> userMgr)
    {
        _ctx = ctx; _userMgr = userMgr;
    }

    public async Task<IActionResult> Crear(int proyectoId)
    {
        await CargarSelectsAsync(proyectoId);
        return View(new Tarea
        {
            ProyectoId = proyectoId,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(7)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Tarea tarea)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(tarea.ProyectoId); return View(tarea); }
        _ctx.Tareas.Add(tarea);
        await _ctx.SaveChangesAsync();
        TempData["Exito"] = "Tarea creada.";
        return RedirectToAction("Detalle", "Proyectos", new { id = tarea.ProyectoId });
    }

    public async Task<IActionResult> Editar(int id)
    {
        var tarea = await _ctx.Tareas.FindAsync(id);
        if (tarea == null) return NotFound();
        await CargarSelectsAsync(tarea.ProyectoId);
        return View(tarea);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Tarea tarea)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(tarea.ProyectoId); return View(tarea); }
        _ctx.Tareas.Update(tarea);
        await _ctx.SaveChangesAsync();
        TempData["Exito"] = "Tarea actualizada.";
        return RedirectToAction("Detalle", "Proyectos", new { id = tarea.ProyectoId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string estado)
    {
        var tarea = await _ctx.Tareas.FindAsync(id);
        if (tarea != null) { tarea.Estado = estado; await _ctx.SaveChangesAsync(); }
        return RedirectToAction("Detalle", "Proyectos", new { id = tarea?.ProyectoId });
    }

    private async Task CargarSelectsAsync(int proyectoId)
    {
        var usuarios = await _ctx.ProyectoUsuarios
            .Where(pu => pu.ProyectoId == proyectoId)
            .Include(pu => pu.Usuario)
            .Select(pu => pu.Usuario!)
            .ToListAsync();
        ViewBag.Usuarios = usuarios.Select(u => new SelectListItem(u.FullName, u.Id));

        var tareas = await _ctx.Tareas
            .Where(t => t.ProyectoId == proyectoId)
            .ToListAsync();
        ViewBag.Tareas = tareas.Select(t => new SelectListItem(t.Nombre, t.Id.ToString()));
        ViewBag.ProyectoId = proyectoId;
    }
}
using ConsultoriaApp.Data;
using ConsultoriaApp.Models;
using ConsultoriaApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultoriaApp.Controllers;

[Authorize]
public class RegistroTiemposController : Controller
{
    private readonly RegistroTiempoService _svc;
    private readonly ApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userMgr;

    public RegistroTiemposController(RegistroTiempoService svc,
        ApplicationDbContext ctx, UserManager<ApplicationUser> userMgr)
    {
        _svc = svc; _ctx = ctx; _userMgr = userMgr;
    }

    public async Task<IActionResult> Crear(int tareaId)
    {
        var tarea = await _ctx.Tareas.Include(t => t.Proyecto).FirstOrDefaultAsync(t => t.Id == tareaId);
        ViewBag.Tarea = tarea;
        return View(new RegistroTiempo
        {
            TareaId = tareaId,
            Fecha = DateTime.Today,
            HoraInicio = TimeSpan.FromHours(8),
            HoraFin = TimeSpan.FromHours(9)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(RegistroTiempo registro)
    {
        var usuario = await _userMgr.GetUserAsync(User);
        registro.UsuarioId = usuario!.Id;

        if (registro.HoraFin <= registro.HoraInicio)
            ModelState.AddModelError("HoraFin", "La hora fin debe ser mayor a la hora inicio.");

        if (!ModelState.IsValid)
        {
            var tarea = await _ctx.Tareas.FindAsync(registro.TareaId);
            ViewBag.Tarea = tarea;
            return View(registro);
        }

        await _svc.RegistrarAsync(registro);
        TempData["Exito"] = "Tiempo registrado correctamente.";
        var t = await _ctx.Tareas.FindAsync(registro.TareaId);
        return RedirectToAction("Detalle", "Proyectos", new { id = t?.ProyectoId });
    }
}
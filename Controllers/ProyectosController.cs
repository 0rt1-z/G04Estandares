using ConsultoriaApp.Models;
using ConsultoriaApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultoriaApp.Data;

namespace ConsultoriaApp.Controllers;

[Authorize]
public class ProyectosController : Controller
{
    private readonly IProyectoService _svc;
    private readonly ApplicationDbContext _ctx;
    private readonly UserManager<ApplicationUser> _userMgr;

    public ProyectosController(IProyectoService svc,
        ApplicationDbContext ctx,
        UserManager<ApplicationUser> userMgr)
    {
        _svc = svc; _ctx = ctx; _userMgr = userMgr;
    }

    public async Task<IActionResult> Index()
    {
        var proyectos = await _svc.ObtenerTodosAsync();
        // Adjuntar costo real a cada proyecto para la vista
        foreach (var p in proyectos)
            ViewData[$"Costo_{p.Id}"] = await _svc.ObtenerCostoRealAsync(p.Id);
        return View(proyectos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var proyecto = await _svc.ObtenerPorIdAsync(id);
        if (proyecto == null) return NotFound();
        ViewBag.CostoReal = await _svc.ObtenerCostoRealAsync(id);
        return View(proyecto);
    }

    [Authorize(Roles = "Administrador,Lider")]
    public async Task<IActionResult> Crear()
    {
        await CargarLideresAsync();
        return View(new Proyecto { FechaInicio = DateTime.Today, FechaFin = DateTime.Today.AddMonths(1) });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador,Lider")]
    public async Task<IActionResult> Crear(Proyecto proyecto)
    {
        if (!ModelState.IsValid) { await CargarLideresAsync(); return View(proyecto); }
        await _svc.CrearAsync(proyecto);
        TempData["Exito"] = "Proyecto creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador,Lider")]
    public async Task<IActionResult> Editar(int id)
    {
        var proyecto = await _svc.ObtenerPorIdAsync(id);
        if (proyecto == null) return NotFound();
        await CargarLideresAsync();
        return View(proyecto);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador,Lider")]
    public async Task<IActionResult> Editar(Proyecto proyecto)
    {
        if (!ModelState.IsValid) { await CargarLideresAsync(); return View(proyecto); }
        await _svc.ActualizarAsync(proyecto);
        TempData["Exito"] = "Proyecto actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _svc.EliminarAsync(id);
        TempData["Exito"] = "Proyecto eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarLideresAsync()
    {
        var lideres = await _userMgr.GetUsersInRoleAsync("Lider");
        ViewBag.Lideres = lideres.Select(u => new SelectListItem(u.FullName, u.Id));
    }
}
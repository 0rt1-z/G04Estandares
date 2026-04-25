using ConsultoriaApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultoriaApp.Controllers;

[Authorize(Roles = "Administrador,Lider")]
public class ReportesController : Controller
{
    private readonly RegistroTiempoService _svc;

    public ReportesController(RegistroTiempoService svc) => _svc = svc;

    public IActionResult Index() => View();

    public async Task<IActionResult> Proyecto(int proyectoId,
        DateTime? desde, DateTime? hasta)
    {
        var reporte = await _svc.GenerarReporteAsync(proyectoId, desde, hasta);
        return View(reporte);
    }
}
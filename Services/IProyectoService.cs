using ConsultoriaApp.Models;

namespace ConsultoriaApp.Services;

public interface IProyectoService
{
    Task<List<Proyecto>> ObtenerTodosAsync();
    Task<Proyecto?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Proyecto proyecto);
    Task ActualizarAsync(Proyecto proyecto);
    Task EliminarAsync(int id);
    Task<decimal> ObtenerCostoRealAsync(int proyectoId);
    Task VerificarAlertasAsync(int proyectoId);
}
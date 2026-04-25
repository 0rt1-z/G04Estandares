using System.Threading;

namespace ConsultoriaApp.Models;

public class Proyecto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Presupuesto { get; set; }
    public decimal TarifaHora { get; set; }
    public string Estado { get; set; } = "Activo";
    public string LiderId { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public ApplicationUser? Lider { get; set; }
    public ICollection<ProyectoUsuario> Usuarios { get; set; } = new List<ProyectoUsuario>();
    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}
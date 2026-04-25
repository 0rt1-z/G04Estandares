namespace ConsultoriaApp.Models;

public class Tarea
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal HorasEstimadas { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string Prioridad { get; set; } = "Media";
    public string? AsignadoId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public Proyecto? Proyecto { get; set; }
    public ApplicationUser? Asignado { get; set; }
    public ICollection<RegistroTiempo> RegistroTiempos { get; set; } = new List<RegistroTiempo>();
    public ICollection<TareaDependencia> Dependencias { get; set; } = new List<TareaDependencia>();
    public ICollection<TareaDependencia> DependientesDe { get; set; } = new List<TareaDependencia>();
}
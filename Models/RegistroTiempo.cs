namespace ConsultoriaApp.Models;

public class RegistroTiempo
{
    public int Id { get; set; }
    public int TareaId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public decimal HorasTotales { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public Tarea? Tarea { get; set; }
    public ApplicationUser? Usuario { get; set; }
}
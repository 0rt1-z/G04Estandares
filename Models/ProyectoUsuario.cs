namespace ConsultoriaApp.Models;

public class ProyectoUsuario
{
    public int ProyectoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;

    public Proyecto? Proyecto { get; set; }
    public ApplicationUser? Usuario { get; set; }
}
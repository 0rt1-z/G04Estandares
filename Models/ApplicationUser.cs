using Microsoft.AspNetCore.Identity;

namespace ConsultoriaApp.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Rol { get; set; } = "Consultor"; // Administrador | Lider | Consultor
    public decimal TarifaHora { get; set; }

    // Navegación
    public ICollection<ProyectoUsuario> Proyectos { get; set; } = new List<ProyectoUsuario>();
    public ICollection<RegistroTiempo> RegistroTiempos { get; set; } = new List<RegistroTiempo>();
}
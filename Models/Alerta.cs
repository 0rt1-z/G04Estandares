namespace ConsultoriaApp.Models;

public class Alerta
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Tipo { get; set; } = string.Empty;   // Sobrecosto | Retraso | PresupuestoAgotado
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public Proyecto? Proyecto { get; set; }
}
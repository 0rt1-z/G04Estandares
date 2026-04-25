namespace ConsultoriaApp.Models;

public class TareaDependencia
{
    public int TareaId { get; set; }
    public int DependeDeTareaId { get; set; }
    public Tarea? Tarea { get; set; }
    public Tarea? DependeDe { get; set; }
}
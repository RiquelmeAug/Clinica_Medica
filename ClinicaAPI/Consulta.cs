namespace ClinicaAPI.Models;

public class Consulta
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public int PacienteId { get; set; }
    public string? DataHora { get; set; } // manter como string por simplicidade
}

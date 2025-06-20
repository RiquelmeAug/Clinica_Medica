namespace ClinicaAPI.Models;

public class Medico : Pessoa
{
    public CRM? CRM { get; set; }               // Value Object
    public string? Especialidade { get; set; }
}

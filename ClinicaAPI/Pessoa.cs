namespace ClinicaAPI.Models;

public class Pessoa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public Genero? Genero { get; set; } // Value Object
}

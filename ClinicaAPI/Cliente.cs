namespace ClinicaAPI.Models;

public class Cliente : Pessoa
{
    public CPF? CPF { get; set; }                       // Value Object
    public DataNascimento? DataNascimento { get; set; } // Value Object
}

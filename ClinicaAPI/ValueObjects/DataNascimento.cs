namespace ClinicaAPI
{
using System;

/// <summary>
/// Representa uma data de nascimento válida.
/// </summary>
public class DataNascimento
{
    public DateTime Valor { get; private set; }

    public DataNascimento(DateTime data)
    {
        if (data > DateTime.Today)
            throw new ArgumentException("A data de nascimento não pode estar no futuro.");

        if (data < new DateTime(1900, 1, 1))
            throw new ArgumentException("A data de nascimento é irrealista.");

        Valor = data;
    }

    public int CalcularIdade()
    {
        var hoje = DateTime.Today;
        int idade = hoje.Year - Valor.Year;
        if (Valor.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }

    public override string ToString() => Valor.ToString("dd/MM/yyyy");
}

}
namespace ClinicaAPI
{
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Representa um número de CRM válido (ex: 123456-SP).
/// </summary>
public class CRM
{
    public string Numero { get; private set; }

    public CRM(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("O CRM não pode ser vazio.");

        numero = numero.Trim().ToUpper();

        if (!Regex.IsMatch(numero, @"^\d{4,6}-[A-Z]{2}$"))
            throw new ArgumentException("Formato inválido de CRM. Use o formato '123456-SP'.");

        Numero = numero;
    }

    public override string ToString() => Numero;
}

}
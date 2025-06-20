namespace ClinicaAPI
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Representa um endereço de e-mail válido.
    /// </summary>
    public class Email
    {
        public string Endereco { get; private set; }

        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("O endereço de e-mail não pode ser vazio.");

            endereco = endereco.Trim();

            // Regex simples para validar e-mail (não cobre todos os casos, mas boa para a maioria)
            string padraoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(endereco, padraoEmail, RegexOptions.IgnoreCase))
                throw new ArgumentException("Endereço de e-mail inválido.");

            Endereco = endereco;
        }

        public override string ToString() => Endereco;
    }

}
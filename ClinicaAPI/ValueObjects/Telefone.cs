namespace ClinicaAPI
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Representa um número de telefone válido (com DDD e formato brasileiro).
    /// Exemplo válido: (11) 98765-4321 ou 11987654321.
    /// </summary>
    public class Telefone
    {
        public string Numero { get; private set; }

        public Telefone(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O número de telefone não pode ser vazio.");

            // Remove espaços, traços, parênteses, etc.
            string limpo = Regex.Replace(numero, @"[^\d]", "");

            // Verifica se tem 10 ou 11 dígitos (ex: 11987654321 ou 1132654321)
            if (!Regex.IsMatch(limpo, @"^\d{10,11}$"))
                throw new ArgumentException("Número de telefone inválido. Deve conter DDD e 8 ou 9 dígitos.");

            Numero = limpo;
        }

        /// <summary>
        /// Retorna o telefone formatado: (XX) XXXXX-XXXX
        /// </summary>
        public override string ToString()
        {
            if (Numero.Length == 10)
                return $"({Numero.Substring(0, 2)}) {Numero.Substring(2, 4)}-{Numero.Substring(6, 4)}";
            else if (Numero.Length == 11)
                return $"({Numero.Substring(0, 2)}) {Numero.Substring(2, 5)}-{Numero.Substring(7, 4)}";
            else
                return Numero;
        }
    }

}
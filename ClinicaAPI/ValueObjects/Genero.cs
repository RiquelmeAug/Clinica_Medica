namespace ClinicaAPI
{
    using System;

    /// <summary>
    /// Representa o Gênero de uma pessoa (M, F, O).
    /// </summary>
    public class Genero
    {
        public char Valor { get; private set; }

        public Genero(char valor)
        {
            valor = char.ToUpper(valor);
            if (valor != 'M' && valor != 'F' && valor != 'O')
                throw new ArgumentException("Gênero inválido. Use 'M' (Masculino), 'F' (Feminino) ou 'O' (Outro).");

            Valor = valor;
        }

        public override string ToString()
        {
            return Valor switch
            {
                'M' => "Masculino",
                'F' => "Feminino",
                'O' => "Outro",
                _ => "Desconhecido"
            };
        }
    }

}
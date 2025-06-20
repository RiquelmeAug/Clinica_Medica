namespace ClinicaAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classe com Métodos de Validação de CPF
    /// </summary>
    public class CPF
    {
        /// <summary>
        /// Construtor da Classe para formatação de Dígitos
        /// </summary>
        /// <param name="digitos">Dígitos do CPF</param>
        /// <exception cref="ArgumentException">Erro adquirido ao inserir dígitos que não correspondam a um CPF</exception>
        public CPF(string digitos)
        {
            //Verificação do Formato do CPF Inserido
            digitos = digitos.Replace(".", "").Replace("-", "").Trim();
            if (String.IsNullOrEmpty(digitos))
                throw new ArgumentException("O CPF não pode ser vazio.");
            if (digitos.Length != 11)
                throw new ArgumentException("O CPF deve possuir todos os 11 dígitos.");
            if (!long.TryParse(digitos, out long digitosFormatados))
                throw new ArgumentException("O CPF deve ser composto somente por dígitos numéricos.");

            //Guardar os Dígitos do CPF
            this.Digitos = digitos;
        }

        /// <summary>
        /// Propriedade que guarda todos os Dígitos do CPF
        /// </summary>
        public string Digitos { get; set; }

        /// <summary>
        /// Método que retorna a validade do Documento CPF
        /// </summary>
        /// <returns>Retorna a Validade do Documento</returns>
        public bool Validar()
        {
            //Caso haja somente números iguais, retornar que é Falso o Documento
            if (this.Digitos == new string(Convert.ToChar(this.Digitos.Substring(0, 1)), 11))
                return false;

            //Gerar o CPF Válido e Padrão
            int digitoVerificador1 = GerarDigitoVerificador(this.Digitos.Substring(0, 9));
            int digitoVerificador2 = GerarDigitoVerificador(this.Digitos.Substring(0, 9) + digitoVerificador1);
            string CPFReal = Digitos.Substring(0, 9) + digitoVerificador1.ToString() + digitoVerificador2.ToString();

            //Enviar resposta da Validação
            return this.Digitos == CPFReal ? true : false;
        }

        //Método que realiza os cálculos para gerar os Dígitos Verificadores
        private int GerarDigitoVerificador(string digitos)
        {
            //Calcular as Multiplicações de cada Dígito do CPF
            int soma = 0;
            int multiplicador = digitos.Length + 1;
            for (int indice = 0; indice < digitos.Length; indice++)
            {
                soma += int.Parse(digitos.Substring(indice, 1)) * multiplicador;
                multiplicador--;
            }

            //Retornar o Dígito Validador
            int resto = soma % 11;
            if (resto > 1)
                return 11 - resto;
            else
                return 0;
        }
        
        public override string ToString()
        {
            return Digitos;
        }

    }

}
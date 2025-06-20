using Microsoft.Data.Sqlite;

namespace ClinicaAPI;

public class Database
{
    public static void Inicializar()
    {
        using var connection = new SqliteConnection("Data Source=clinica.db");
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Pessoa (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Genero TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Medico (
                PessoaId INTEGER PRIMARY KEY,
                CRM TEXT NOT NULL,
                Especialidade TEXT NOT NULL,
                FOREIGN KEY (PessoaId) REFERENCES Pessoa(Id)
            );

            CREATE TABLE IF NOT EXISTS Cliente (
                PessoaId INTEGER PRIMARY KEY,
                CPF TEXT NOT NULL,
                DataNascimento TEXT NOT NULL,
                FOREIGN KEY (PessoaId) REFERENCES Pessoa(Id)
            );

            CREATE TABLE IF NOT EXISTS Consulta (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MedicoId INTEGER NOT NULL,
                PacienteId INTEGER NOT NULL,
                DataHora TEXT NOT NULL,
                FOREIGN KEY (MedicoId) REFERENCES Medico(PessoaId),
                FOREIGN KEY (PacienteId) REFERENCES Cliente(PessoaId)
            );
        ";
        cmd.ExecuteNonQuery();
    }
}

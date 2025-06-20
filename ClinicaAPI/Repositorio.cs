using Microsoft.Data.Sqlite;
using ClinicaAPI.Models;

namespace ClinicaAPI;

public class Repositorio
{
    private const string connectionString = "Data Source=clinica.db";

    public static void InserirMedico(Medico medico)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmdPessoa = connection.CreateCommand();
        cmdPessoa.CommandText = "INSERT INTO Pessoa (Nome, Genero) VALUES ($nome, $genero);";
        cmdPessoa.Parameters.AddWithValue("$nome", medico.Nome);
        cmdPessoa.Parameters.AddWithValue("$genero", medico.Genero?.Valor ?? throw new ArgumentException("Gênero não informado"));
        cmdPessoa.ExecuteNonQuery();

        var cmdId = connection.CreateCommand();
        cmdId.CommandText = "SELECT last_insert_rowid();";

        var result = cmdId.ExecuteScalar();
        if (result is null)
            throw new Exception("Erro ao recuperar o ID da pessoa.");
        var pessoaId = (long)(long?)result;

        var cmdMedico = connection.CreateCommand();
        cmdMedico.CommandText = @"
            INSERT INTO Medico (PessoaId, CRM, Especialidade)
            VALUES ($id, $crm, $esp);";
        cmdMedico.Parameters.AddWithValue("$id", pessoaId);
        cmdMedico.Parameters.AddWithValue("$crm", medico.CRM?.Numero ?? throw new ArgumentException("CRM não informado"));
        cmdMedico.Parameters.AddWithValue("$esp", medico.Especialidade);
        cmdMedico.ExecuteNonQuery();
    }



    public static List<Medico> ListarMedicos()
    {
        var lista = new List<Medico>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT p.Id, p.Nome, p.Genero, m.CRM, m.Especialidade
            FROM Pessoa p
            JOIN Medico m ON p.Id = m.PessoaId;
        ";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var medico = new Medico
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Genero = new Genero(reader.GetString(2)[0]),         // VO
                CRM = new CRM(reader.GetString(3)),               // VO
                Especialidade = reader.GetString(4)
            };
            lista.Add(medico);
        }
        return lista;
    }

    public static void InserirCliente(Cliente cliente)
    {
        // Verificações antes de acessar propriedades
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));

        if (string.IsNullOrWhiteSpace(cliente.Nome))
            throw new ArgumentException("Nome do cliente não pode ser vazio.");

        if (cliente.Genero == null)
            throw new ArgumentNullException(nameof(cliente.Genero));

        if (cliente.CPF == null)
            throw new ArgumentNullException(nameof(cliente.CPF));

        if (cliente.DataNascimento == null)
            throw new ArgumentNullException(nameof(cliente.DataNascimento));

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmdPessoa = connection.CreateCommand();
        cmdPessoa.CommandText = "INSERT INTO Pessoa (Nome, Genero) VALUES ($nome, $genero);";
        cmdPessoa.Parameters.AddWithValue("$nome", cliente.Nome);
        cmdPessoa.Parameters.AddWithValue("$genero", cliente.Genero.Valor);
        cmdPessoa.ExecuteNonQuery();

        var cmdId = connection.CreateCommand();
        cmdId.CommandText = "SELECT last_insert_rowid();";

        var result = cmdId.ExecuteScalar();
        if (result == null)
            throw new Exception("Falha ao obter o ID gerado para Pessoa.");
        var pessoaId = (long)(long?)result;

        var cmdCliente = connection.CreateCommand();
        cmdCliente.CommandText = @"
            INSERT INTO Cliente (PessoaId, CPF, DataNascimento)
            VALUES ($id, $cpf, $nascimento);";
        cmdCliente.Parameters.AddWithValue("$id", pessoaId);
        cmdCliente.Parameters.AddWithValue("$cpf", cliente.CPF.Digitos);
        cmdCliente.Parameters.AddWithValue("$nascimento", cliente.DataNascimento.Valor.ToString("yyyy-MM-dd"));
        cmdCliente.ExecuteNonQuery();
    }


    public static List<Cliente> ListarClientes()
    {
        var lista = new List<Cliente>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT p.Id, p.Nome, p.Genero, c.CPF, c.DataNascimento
            FROM Pessoa p
            JOIN Cliente c ON p.Id = c.PessoaId;
        ";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var cliente = new Cliente
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Genero = new Genero(reader.GetString(2)[0]),
                CPF = new CPF(reader.GetString(3)),
                DataNascimento = new DataNascimento(DateTime.Parse(reader.GetString(4)))
            };
            lista.Add(cliente);
        }

        return lista;
    }

    public static void InserirConsulta(Consulta consulta)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Consulta (MedicoId, PacienteId, DataHora)
            VALUES ($medico, $paciente, $data);";
        cmd.Parameters.AddWithValue("$medico", consulta.MedicoId);
        cmd.Parameters.AddWithValue("$paciente", consulta.PacienteId);
        cmd.Parameters.AddWithValue("$data", consulta.DataHora);
        cmd.ExecuteNonQuery();
    }

    public static List<string> ListarConsultas()
    {
        var lista = new List<string>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT c.Id, c.DataHora, p1.Nome AS NomeMedico, p2.Nome AS NomePaciente
            FROM Consulta c
            JOIN Medico m ON c.MedicoId = m.PessoaId
            JOIN Cliente cl ON c.PacienteId = cl.PessoaId
            JOIN Pessoa p1 ON m.PessoaId = p1.Id
            JOIN Pessoa p2 ON cl.PessoaId = p2.Id;
        ";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var texto = $"Consulta #{reader.GetInt32(0)} - {reader.GetString(1)} | Médico: {reader.GetString(2)} | Paciente: {reader.GetString(3)}";
            lista.Add(texto);
        }

        return lista;
    }



    //TESTE
    public static void Seed()
    {
        // Evita duplicações simples
        if (ListarMedicos().Count > 0) return;

        var m1 = new Medico
        {
            Nome = "Dra. Carla",
            Genero = new Genero('F'),
            CRM = new CRM("123456-SP"),
            Especialidade = "Cardiologia"
        };
        InserirMedico(m1);

        var c1 = new Cliente
        {
            Nome = "João da Silva",
            Genero = new Genero('M'),
            CPF = new CPF("12345678909"),
            DataNascimento = new DataNascimento(new DateTime(1985, 6, 10))
        };
        InserirCliente(c1);

        var medicos = ListarMedicos();
        var clientes = ListarClientes();

        var consulta = new Consulta
        {
            MedicoId = medicos[0].Id,
            PacienteId = clientes[0].Id,
            DataHora = "2025-07-10 14:00"
        };
        InserirConsulta(consulta);
    }

}

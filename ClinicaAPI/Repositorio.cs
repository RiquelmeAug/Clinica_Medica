using Microsoft.Data.Sqlite;
using ClinicaAPI.Models;

namespace ClinicaAPI;

public class Repositorio 
{
    private const string connectionString = "Data Source=clinica.db";

    //Recebe um objeto da classe Medico
    //Conecta ao banco
    //Tenta inserir no banco e trata erros
    //Se tudo certo insere no banco e não retorna nada
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


    //Lista todos os médicos registrados
    //Faz JOIN entre Pessoa e Medico
    //Retorna uma lista de objetos Medico preenchidos com os dados do banco
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
                Genero = new Genero(reader.GetString(2)[0]),
                CRM = new CRM(reader.GetString(3)),
                Especialidade = reader.GetString(4)
            };
            lista.Add(medico);
        }
        return lista;
    }


    //Recebe um objeto da classe Cliente
    //Conecta ao banco
    //Insere em Pessoa, depois em Cliente
    //Se tudo certo insere no banco e não retorna nada
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


    //Lista todos os clientes registrados
    //Faz JOIN entre Pessoa e Cliente
    //Retorna uma lista de objetos Cliente preenchidos com os dados do banco
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


    //Recebe um objeto da classe Consulta
    //Conecta ao banco
    //Insere no banco a consulta, relacionando médico e cliente por seus IDs
    //Se tudo certo insere no banco e não retorna nada
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


    //Lista todas as consultas marcadas
    //Faz JOIN para incluir nomes de médico e paciente
    //Retorna uma lista de objetos Consulta com os campos preenchidos
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


    //Busca todos os médicos cujo nome contenha o texto informado
    //Ignora maiúsculas/minúsculas
    //Retorna uma lista com os médicos encontrados ou vazia se nenhum for achado
    public static List<Medico> BuscarMedicosPorNome(string nome)
    {
        var lista = new List<Medico>();

        if (string.IsNullOrWhiteSpace(nome))
            return lista;

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        SELECT p.Id, p.Nome, p.Genero, m.CRM, m.Especialidade
        FROM Pessoa p
        JOIN Medico m ON p.Id = m.PessoaId
        WHERE LOWER(p.Nome) LIKE '%' || LOWER($nome) || '%';";
        cmd.Parameters.AddWithValue("$nome", nome);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var medico = new Medico
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Genero = new Genero(reader.GetString(2)[0]),
                CRM = new CRM(reader.GetString(3)),
                Especialidade = reader.GetString(4)
            };
            lista.Add(medico);
        }

        return lista;
    }


    //Busca todos os médicos cuja especialidade contenha o texto informado
    //Ignora maiúsculas/minúsculas
    //Retorna uma lista com os médicos encontrados ou vazia se nenhum for achado
    public static List<Medico> BuscarMedicosPorEspecialidade(string especialidade)
    {
        var lista = new List<Medico>();

        if (string.IsNullOrWhiteSpace(especialidade))
            return lista;

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        SELECT p.Id, p.Nome, p.Genero, m.CRM, m.Especialidade
        FROM Pessoa p
        JOIN Medico m ON p.Id = m.PessoaId
        WHERE LOWER(m.Especialidade) LIKE '%' || LOWER($esp) || '%';";
        cmd.Parameters.AddWithValue("$esp", especialidade);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var medico = new Medico
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Genero = new Genero(reader.GetString(2)[0]),
                CRM = new CRM(reader.GetString(3)),
                Especialidade = reader.GetString(4)
            };
            lista.Add(medico);
        }

        return lista;
    }


    //Busca todos os clientes cujo nome contenha o texto informado
    //Ignora maiúsculas/minúsculas
    //Retorna uma lista com os clientes encontrados ou vazia se nenhum for achado
    public static List<Cliente> BuscarClientesPorNome(string nome)
    {
        var lista = new List<Cliente>();

        if (string.IsNullOrWhiteSpace(nome))
            return lista;

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT p.Id, p.Nome, p.Genero, c.CPF, c.DataNascimento
            FROM Pessoa p
            JOIN Cliente c ON p.Id = c.PessoaId
            WHERE LOWER(p.Nome) LIKE '%' || LOWER($nome) || '%';";
        cmd.Parameters.AddWithValue("$nome", nome);

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


    //Busca um cliente específico pelo ID da tabela Pessoa
    //Retorna o cliente se encontrado, ou null caso não exista
    public static Cliente? BuscarClientePorId(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        SELECT p.Id, p.Nome, p.Genero, c.CPF, c.DataNascimento
        FROM Pessoa p
        JOIN Cliente c ON p.Id = c.PessoaId
        WHERE p.Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Cliente
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Genero = new Genero(reader.GetString(2)[0]),
                CPF = new CPF(reader.GetString(3)),
                DataNascimento = new DataNascimento(DateTime.Parse(reader.GetString(4)))
            };
        }

        return null;
    }


    //Retorna todas as consultas registradas para um paciente específico
    //Recebe o ID da pessoa (cliente) e devolve uma lista de objetos Consulta
    public static List<Consulta> BuscarConsultaPorPaciente(int pacienteId)
    {
        var lista = new List<Consulta>();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, MedicoId, PacienteId, DataHora
            FROM Consulta
            WHERE PacienteId = $id;";
        cmd.Parameters.AddWithValue("$id", pacienteId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var consulta = new Consulta
            {
                Id = reader.GetInt32(0),
                MedicoId = reader.GetInt32(1),
                PacienteId = reader.GetInt32(2),
                DataHora = reader.GetString(3)
            };
            lista.Add(consulta);
        }

        return lista;
    }


    //Retorna todas as consultas associadas a um médico específico
    //Recebe o ID do médico (PessoaId) e devolve uma lista de objetos Consulta
    public static List<Consulta> BuscarConsultaPorMedico(int medicoId)
    {
        var lista = new List<Consulta>();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        SELECT Id, MedicoId, PacienteId, DataHora
        FROM Consulta
        WHERE MedicoId = $id;";
        cmd.Parameters.AddWithValue("$id", medicoId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var consulta = new Consulta
            {
                Id = reader.GetInt32(0),
                MedicoId = reader.GetInt32(1),
                PacienteId = reader.GetInt32(2),
                DataHora = reader.GetString(3)
            };
            lista.Add(consulta);
        }

        return lista;
    }


    //Remove uma consulta do banco com base no ID da consulta
    //Não retorna nada, apenas executa o DELETE
    public static void CancelarConsulta(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Consulta WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);

        cmd.ExecuteNonQuery();
    }


    //Verifica se uma consulta já existe com médico, paciente e horário exatos
    //Retorna true se existir (evita duplicações), false se for nova
    public static bool VerificarSeConsultaExiste(int medicoId, int pacienteId, string dataHora)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        SELECT COUNT(*)
        FROM Consulta
        WHERE MedicoId = $medico
          AND PacienteId = $paciente
          AND DataHora = $dataHora;";
        cmd.Parameters.AddWithValue("$medico", medicoId);
        cmd.Parameters.AddWithValue("$paciente", pacienteId);
        cmd.Parameters.AddWithValue("$dataHora", dataHora);

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result) > 0;
    }

}

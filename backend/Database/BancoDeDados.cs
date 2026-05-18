namespace BankSystem.Database;

using System.Collections.Generic;
using MySql.Data.MySqlClient;
using BankSystem.Models;

public class BancoDeDados
{
    private readonly string _connectionString;

    public BancoDeDados(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<ContaBancaria> ListarContas()
    {
        //Cria uma lista vazia na memória
        var contas = new List<ContaBancaria>();

        //Abre uma conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //prepara o comando sql select
        using var command = new MySqlCommand("SELECT id, titular, saldo, tipo FROM contas", connection);
        //Executa o Select e recebe um 'reader" (leitor de linhas)
        using var reader = command.ExecuteReader();

        //para cada linha que o banco retornar
        while (reader.Read())
        {
            //cria um objeto ContaBancaria com os dados daquela linha
            var conta = new ContaBancaria(
                reader.GetInt32("id"), //pega coluna id
                reader.GetString("titular"), //pega coluna titular
                reader.GetDecimal("saldo"), //pega coluna saldo
                reader.GetString("tipo")
            );
            //Adiciona o objto na lista contas que foi criada lá em cima
            contas.Add(conta);

            //Como estra dentro de um loop while, faz isso para todas linhas do banco
        }
        //retorna a lista de contas, criada pelo loop while
        return contas;
    }

    public ContaBancaria? BuscarContaPorId(int id)
    {
        //Abre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando de SQL de insert
        using var command = new MySqlCommand("SELECT id, titular, saldo, tipo FROM contas WHERE id = @id", connection);
        //Substitui o @id pelos valor real
        command.Parameters.AddWithValue("@id", id);

        //executa e recebe o reader
        using var reader = command.ExecuteReader();

        //tenta ler uma linha(se existir)
        if (reader.Read())
        {
            //se encontrou, cria o objeto e retorna
            return new ContaBancaria(
                reader.GetInt32("id"),
                reader.GetString("titular"),
                reader.GetDecimal("saldo"),
                reader.GetString("tipo")
            );
        }
        //se não encontrou nenhuma conta retorna null
        return null;
    }

    public int CriarConta(string titular, decimal saldoInicial, string tipo = "corrente")
    {
        // ABre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando SQL de Insert
        using var command = new MySqlCommand(
            "INSERT INTO contas (titular, saldo, tipo) VALUES (@titular, @saldo, @tipo)",
            connection
        );

        //Substitui os @parametros pelos valores reais
        command.Parameters.AddWithValue("@titular", titular);
        command.Parameters.AddWithValue("@saldo", saldoInicial);
        command.Parameters.AddWithValue("@tipo", tipo);

        //Executa o insert(não retorna dada, só insere)
        command.ExecuteNonQuery();
        
        //Retorna o ultimo id criado, para usarmos na a conta recem criada e conectala com o usuario
        return(int)command.LastInsertedId;
    }

    public void AtualizarSaldo(int id, decimal novoSaldo)
    {   
        //Abre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando SQL de update
        using var command = new MySqlCommand(
            "UPDATE contas SET saldo = @saldo WHERE id = @id",
            connection
        );

        //Substitui os @parametros pelos valores reais
        command.Parameters.AddWithValue("@saldo", novoSaldo);
        command.Parameters.AddWithValue("@id", id);

        //executa o update
        command.ExecuteNonQuery();
    }

    public void CriarUsuario(string email, string senha, int contaId)
    {
        //Abre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //prepara o comando sql para criar usuario
        using var command = new MySqlCommand(
            "INSERT INTO usuarios (email, senha, conta_id) VALUES (@email, @senha, @contaId)",
            connection
        );
        
        //substitui os parametros pelos valores reais
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@senha", senha);
        command.Parameters.AddWithValue("@contaId", contaId);

        //executa o create
        command.ExecuteNonQuery();

    }

    public Usuarios? BuscarUsuarioPorEmail(string email)
    {   
        //abre conexão com o MySql
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //prepara o comando sql para buscar usuario por email
        using var command = new MySqlCommand(
            "SELECT id, email, senha, conta_id FROM usuarios WHERE email = @email",
        connection);

        //substitui o parametro pelo valor real
        command.Parameters.AddWithValue("@email", email);

        //executa e recebe os dados
        using var reader = command.ExecuteReader();
        
        //tenta ler uma linha (se existir)
        if (reader.Read())
        {   
            //retorna o usuario
            return new Usuarios (
                reader.GetInt32("id"),
                reader.GetString("email"),
                reader.GetString("senha"),
                reader.GetInt32("conta_id")
            );
        } 
        else {
            return null;
        }
    }

    public void RegistrarTransacao(int contaId, string tipo, decimal valor){
        //Abre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando SQL
        using var command = new MySqlCommand(
            "INSERT INTO transacoes (conta_id, tipo, valor) VALUES (@contaId, @tipo, @valor)",
            connection
        );

        //substitui os parametros pelos valores reais
        command.Parameters.AddWithValue("@contaId", contaId);
        command.Parameters.AddWithValue("@tipo", tipo);
        command.Parameters.AddWithValue("@valor", valor);
        
        //executa o comando SQL
        command.ExecuteNonQuery();
    }

    public  List<Transacoes> ListarTransacoes(int contaId){
        //cria uma nova lista de transações
        var transacoes = new List<Transacoes>();

        //Abre conexão com o MySQL
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //prepara o comando SQL
        using var command = new MySqlCommand(
            "SELECT id, conta_id, tipo, valor, data_hora FROM transacoes WHERE conta_id = @contaId ORDER BY data_hora DESC",
            connection
        );

        //substitui os parametros pelos valores reais
        command.Parameters.AddWithValue("@contaId", contaId);

        //executa o comando e recebe os dados
        using var reader = command.ExecuteReader();

        //loop para ler cada linha que retorna do select
        while(reader.Read()){
            //variavel que ficará com a transação montada com os dados da linha
            var transacao= new Transacoes(
                reader.GetInt32("id"),
                reader.GetInt32("conta_id"),
                reader.GetString("tipo"),
                reader.GetDecimal("valor"),
                reader.GetDateTime("data_hora")
            );

            //adiciona os dados da variavél na lista, uma de cada vez, como esta dentro de um while
            transacoes.Add(transacao);

        }

        //retorna a lista de transações
        return transacoes;
    }

    public void AplicarRendimento()
    {
        //Abre conexão com o banco
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando SQL para atualizar o saldo das contas poupanças
        using var command = new MySqlCommand(
            "UPDATE contas SET saldo = saldo * 1.01 WHERE tipo = 'poupanca'",
            connection
        );

        //Executa o comando
        command.ExecuteNonQuery();
    }

    public void AtualizarSenha(string email, string novaSenha)
    {
        //Abre conexão com o banco
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        //Prepara o comando SQL para atualizar a nova senha
        using var command = new MySqlCommand(
            "UPDATE usuarios SET senha = @senha WHERE email = @email",
            connection
        );

        //Substitui os parametros pelos valores reais
        command.Parameters.AddWithValue("@senha", novaSenha);
        command.Parameters.AddWithValue("@email", email);

        //Executa o comando
        command.ExecuteNonQuery();
    }
}

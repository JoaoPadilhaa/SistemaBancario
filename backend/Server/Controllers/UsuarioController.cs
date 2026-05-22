namespace BankSystem.Server.Controllers;

using System.Net;
using System.Text.Json;
using System.IO;
using BankSystem.Database;
using System;

public static class UsuarioController
{
    public static void HandleRegistrar(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        string email = dados.GetProperty("email").GetString();
        string senha = dados.GetProperty("senha").GetString();
        string titular = dados.GetProperty("titular").GetString();
        decimal saldoInicial = dados.GetProperty("saldoInicial").GetDecimal();
        string tipo = dados.GetProperty("tipo").GetString() ?? "corrente";
        string senhaHash = HashSenha(senha);

        int contaId = banco.CriarConta(titular, saldoInicial, tipo);
        banco.CriarUsuario(email, senhaHash, contaId);

        string json = JsonSerializer.Serialize(new {
            id = contaId,
            titular = titular,
            saldo = saldoInicial,
            email = email,
            tipo = tipo,
            mensagem = "Usuário registrado com sucesso"
        });
        enviarResposta(context, json, 201);
    }

    public static void HandleLogin(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        string email = dados.GetProperty("email").GetString();
        string senha = dados.GetProperty("senha").GetString();

        var usuario = banco.BuscarUsuarioPorEmail(email);

        if (usuario == null)
        {
            enviarResposta(context, "{\"erro\":\"Usuário não encontrado\"}", 404);
            return;
        }

        if (HashSenha(senha) == usuario.Senha)
        {
            var conta = banco.BuscarContaPorId(usuario.ContaId);
            string json = JsonSerializer.Serialize(new {
                id = conta.Id,
                titular = conta.Titular,
                saldo = conta.Saldo,
                email = usuario.Email,
                tipo = conta.Tipo
            });
            enviarResposta(context, json, 200);
        }
        else
        {
            enviarResposta(context, "{\"erro\":\"Senha incorreta\"}", 400);
        }
    }

    public static void HandleBuscarUsuarioPorEmail(HttpListenerContext context, BancoDeDados banco, string email, Action<HttpListenerContext, string, int> enviarResposta)
    {
        string emailDecodificado = Uri.UnescapeDataString(email);
        var usuario = banco.BuscarUsuarioPorEmail(emailDecodificado);

        if (usuario == null)
        {
            enviarResposta(context, "{\"erro\":\"Usuário não encontrado\"}", 404);
            return;
        }

        var conta = banco.BuscarContaPorId(usuario.ContaId);
        string json = JsonSerializer.Serialize(new {
            id = conta.Id,
            email = usuario.Email,
            titular = conta.Titular,
            saldo = conta.Saldo,
            tipo = conta.Tipo
            
        });
        enviarResposta(context, json, 200);
    }

    public static void HandleAtualizarSenha(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        string email = dados.GetProperty("email").GetString();
        string senhaAtual = dados.GetProperty("senhaAtual").GetString();
        string novaSenha = dados.GetProperty("novaSenha").GetString();

        var usuario = banco.BuscarUsuarioPorEmail(email);

        if (usuario == null)
        {
            enviarResposta(context, "{\"erro\":\"Usuário não encontrado\"}", 404);
            return;
        }

        if (usuario.Senha != HashSenha(senhaAtual))
        {
            enviarResposta(context, "{\"erro\":\"Senha atual incorreta\"}", 400);
            return;
        }

        banco.AtualizarSenha(email, HashSenha(novaSenha));
        enviarResposta(context, "{\"mensagem\":\"Senha alterada com sucesso\"}", 200);
    }

    private static string HashSenha(string senha){
        using var sha = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }

    public static void HandleProvisionar(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        //Lê o body recebido do front
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        //Converte o JSON em um objeto JsonElement para facilitar a extração dos dados
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        //Extrai cada dado do body
        string email = dados.GetProperty("email").GetString();
        string titular = dados.GetProperty("titular").GetString();
        string tipo = dados.GetProperty("tipo").GetString();
        decimal saldoInicial = dados.GetProperty("saldoInicial").GetDecimal();

        //Busca usuario existente com o email enviado
        var usuarioExistente = banco.BuscarUsuarioPorEmail(email);
        //se existir, busca a conta do usuario, converte os dados, e retorna os dados convertidos
        //Se usuarioExistente não encontrar a conta por email, ele vai ser nulo, logo vai pular esse if
        if(usuarioExistente != null){
            var contaExistente = banco.BuscarContaPorId(usuarioExistente.ContaId);
            string jsonExistente = JsonSerializer.Serialize(new {
                id = contaExistente.Id,
                titular = contaExistente.Titular,
                saldo = contaExistente.Saldo,
                tipo = contaExistente.Tipo
            });

            //envia json convertido 
            enviarResposta(context, jsonExistente, 200);
            return;
        }

        //Cria conta e usuario sem senha
        int contaId = banco.CriarConta(titular, saldoInicial, tipo);
        banco.CriarUsuario(email, "", contaId);//Senha vazia, pois a autenticação é com o keycloak
        //Busca conta recem criada e converte os dados para json
        var conta = banco.BuscarContaPorId(contaId);
        string json = JsonSerializer.Serialize(new {
            id = conta.Id,
            email = email,
            titular = conta.Titular,
            saldo = conta.Saldo,
            tipo = conta.Tipo
        });
        //Envia a conta recem criada, com os dados convertidos em json
        enviarResposta(context, json, 201);
    }
}

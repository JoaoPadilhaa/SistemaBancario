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
            email = usuario.Email,
            titular = conta.Titular,
            contaId = conta.Id
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
}

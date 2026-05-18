namespace BankSystem.Server.Controllers;

using System;
using System.Net;
using System.Text.Json;
using System.IO;
using BankSystem.Database;

public static class ContaController
{
    public static void HandleListarContas(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        var contas = banco.ListarContas();
        string json = JsonSerializer.Serialize(contas);
        enviarResposta(context, json, 200);
    }

    public static void HandleCriarConta(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        string titular = dados.GetProperty("titular").GetString() ?? "";
        decimal saldoInicial = dados.GetProperty("saldoInicial").GetDecimal();

        banco.CriarConta(titular, saldoInicial);

        string json = JsonSerializer.Serialize(new { mensagem = "Conta criada com sucesso" });
        enviarResposta(context, json, 201);
    }

    public static void HandleBuscarConta(HttpListenerContext context, BancoDeDados banco, int id, Action<HttpListenerContext, string, int> enviarResposta)
    {
        var conta = banco.BuscarContaPorId(id);
        if (conta == null)
        {
            enviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }
        string json = JsonSerializer.Serialize(conta);
        enviarResposta(context, json, 200);
    }

    public static void HandleDepositar(HttpListenerContext context, BancoDeDados banco, int id, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);
        decimal valor = dados.GetProperty("valor").GetDecimal();

        var conta = banco.BuscarContaPorId(id);
        if (conta == null)
        {
            enviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }

        conta.Depositar(valor);
        banco.AtualizarSaldo(id, conta.Saldo);
        banco.RegistrarTransacao(id, "deposito", valor);

        string json = JsonSerializer.Serialize(conta);
        enviarResposta(context, json, 200);
    }

    public static void HandleSacar(HttpListenerContext context, BancoDeDados banco, int id, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);
        decimal valor = dados.GetProperty("valor").GetDecimal();

        var conta = banco.BuscarContaPorId(id);
        if (conta == null)
        {
            enviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }

        if (conta.Saldo < valor)
        {
            string erroJson = JsonSerializer.Serialize(new { erro = "Saldo insuficiente" });
            enviarResposta(context, erroJson, 400);
            return;
        }

        conta.Sacar(valor);
        banco.AtualizarSaldo(id, conta.Saldo);
        banco.RegistrarTransacao(id, "saque", valor);

        string json = JsonSerializer.Serialize(conta);
        enviarResposta(context, json, 200);
    }

    public static void HandleTransferir(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        int idOrigem = dados.GetProperty("idOrigem").GetInt32();
        int idDestino = dados.GetProperty("idDestino").GetInt32();
        decimal valor = dados.GetProperty("valor").GetDecimal();

        var origem = banco.BuscarContaPorId(idOrigem);
        var destino = banco.BuscarContaPorId(idDestino);

        if (origem == null || destino == null)
        {
            enviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }

        if (origem.Saldo < valor)
        {
            enviarResposta(context, "{\"erro\":\"Saldo insuficiente\"}", 400);
            return;
        }

        origem.Sacar(valor);
        destino.Depositar(valor);
        banco.AtualizarSaldo(idOrigem, origem.Saldo);
        banco.AtualizarSaldo(idDestino, destino.Saldo);
        banco.RegistrarTransacao(idOrigem, "transferencia", valor);

        string json = JsonSerializer.Serialize(new { mensagem = "Transferência realizada com sucesso", saldo = origem.Saldo });
        enviarResposta(context, json, 200);
    }
}

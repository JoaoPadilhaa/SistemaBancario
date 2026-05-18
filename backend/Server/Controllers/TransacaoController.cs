namespace BankSystem.Server.Controllers;

using System.Net;
using System.Text.Json;
using BankSystem.Database;
using System;

public static class TransacaoController
{
    public static void HandleListarTransacoes(HttpListenerContext context, BancoDeDados banco, int id, Action<HttpListenerContext, string, int> enviarResposta)
    {
        var transacoes = banco.ListarTransacoes(id);
        string json = JsonSerializer.Serialize(transacoes);
        enviarResposta(context, json, 200);
    }
}

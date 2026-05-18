namespace BankSystem.Server.Controllers;

using System.Net;
using System.Text.Json;
using System.IO;
using BankSystem.Database;
using System;

public static class PixController
{
    public static void HandlePix(HttpListenerContext context, BancoDeDados banco, Action<HttpListenerContext, string, int> enviarResposta)
    {
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        int idOrigem = dados.GetProperty("idOrigem").GetInt32();
        string emailDestino = dados.GetProperty("emailDestino").GetString();
        decimal valor = dados.GetProperty("valor").GetDecimal();

        var usuarioDestino = banco.BuscarUsuarioPorEmail(emailDestino);

        if (usuarioDestino == null)
        {
            enviarResposta(context, "{\"erro\":\"Destinatário não encontrado\"}", 404);
            return;
        }

        int idDestino = usuarioDestino.ContaId;
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
        banco.RegistrarTransacao(idOrigem, "pix", valor);
        banco.RegistrarTransacao(idDestino, "pix recebido", valor);

        string json = JsonSerializer.Serialize(new { mensagem = "Pix enviado com sucesso!", saldo = origem.Saldo });
        enviarResposta(context, json, 200);
    }
}

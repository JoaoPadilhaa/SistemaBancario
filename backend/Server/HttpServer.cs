namespace BankSystem.Server;

using System;
using System.Net;
using System.Text;
using System.Text.Json;
using BankSystem.Database;
using System.IO;
using System.Threading.Tasks;
using BankSystem.Server.Controllers;
using System.Collections.Generic;

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly BancoDeDados _banco;

    public HttpServer(BancoDeDados banco)
    {
        _banco = banco;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:5000/");
    }

    public void Iniciar()
    {
        _listener.Start();
        Console.WriteLine("Servidor iniciado em http://localhost:5000/");

        while (true)
        {
            var context = _listener.GetContext();
            Task.Run(() => ProcessarRequisicao(context));
        }
    }

    private async Task<bool> ValidarToken(string token)
    {
        try{
            using var client = new System.Net.Http.HttpClient();
            string jwkUrl = "http://localhost:8080/realms/banco-sistema/protocol/openid-connect/certs";
            string jwkJson = await client.GetStringAsync(jwkUrl);

            var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwkJson);

            var parametros = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:8080/realms/banco-sistema",
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKeys = jwks.GetSigningKeys()
            };
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            handler.ValidateToken(token, parametros, out _);
            return true;
        }
        catch {
            return false;
        }
    }

    public async void ProcessarRequisicao(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PATCH, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.Close();
            return;
        }

        //Verifica se Header existe
        string authHeader = request.Headers["Authorization"] ?? "";
        if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            response.StatusCode = 401;
            response.Close();
            return;
        }
        //Extrai o token
        string token = authHeader.Replace("Bearer ", "");

        //Valida o token com o Keycloak < NOVO
        bool tokenValido = await ValidarToken(token);
        if(!tokenValido)
        {
            response.StatusCode = 401;
            response.Close();
            return;
        }

        try
        {
            string caminho = request.Url!.AbsolutePath;
            string metodo = request.HttpMethod;

            if (metodo == "GET" && caminho == "/api/contas")
                ContaController.HandleListarContas(context, _banco, EnviarResposta);

            else if (metodo == "POST" && caminho == "/api/contas")
                ContaController.HandleCriarConta(context, _banco, EnviarResposta);

            else if (metodo == "GET" && caminho.EndsWith("/transacoes"))
                TransacaoController.HandleListarTransacoes(context, _banco, ExtrairId(caminho), EnviarResposta);

            else if (metodo == "GET" && caminho.StartsWith("/api/contas") && !caminho.EndsWith("/transacoes"))
                ContaController.HandleBuscarConta(context, _banco, ExtrairId(caminho), EnviarResposta);

            else if (metodo == "POST" && caminho.EndsWith("/depositar"))
                ContaController.HandleDepositar(context, _banco, ExtrairId(caminho), EnviarResposta);

            else if (metodo == "POST" && caminho.EndsWith("/sacar"))
                ContaController.HandleSacar(context, _banco, ExtrairId(caminho), EnviarResposta);

            else if (metodo == "POST" && caminho == "/api/contas/transferir")
                ContaController.HandleTransferir(context, _banco, EnviarResposta);

            else if (metodo == "POST" && caminho == "/api/registrar")
                UsuarioController.HandleRegistrar(context, _banco, EnviarResposta);

            else if (metodo == "POST" && caminho == "/api/login")
                UsuarioController.HandleLogin(context, _banco, EnviarResposta);

            else if (metodo == "GET" && caminho.StartsWith("/api/usuarios/buscar"))
            {
                string email = context.Request.QueryString["email"] ?? "";
                UsuarioController.HandleBuscarUsuarioPorEmail(context, _banco, email, EnviarResposta);
            }

            else if (metodo == "POST" && caminho == "/api/pix")
                PixController.HandlePix(context, _banco, EnviarResposta);

            else if (metodo == "PATCH" && caminho == "/api/perfil")
                UsuarioController.HandleAtualizarSenha(context, _banco, EnviarResposta);
            
            else if(metodo == "POST" && caminho == "/api/provisionar")
                UsuarioController.HandleProvisionar(context, _banco, EnviarResposta);

            else
            {
                response.StatusCode = 404;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            response.StatusCode = 500;
            response.Close();
        }
    }

    private int ExtrairId(string caminho)
    {
        string[] partes = caminho.Split('/');
        return int.Parse(partes[3]);
    }

    private void EnviarResposta(HttpListenerContext context, string json, int statusCode)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();
    }
}

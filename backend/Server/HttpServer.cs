namespace BankSystem.Server;

using System;
using System.Net;
using System.Text;
using System.Text.Json;
using BankSystem.Database;
using System.IO;
using System.Threading.Tasks;

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly BancoDeDados _banco;

    public HttpServer(BancoDeDados banco)
    {
        //guarda a referência do banco para usar nos handlers
        _banco = banco;
        //Cria o listener http (o "ouvido" que vai escutar as requisições)
        _listener = new HttpListener();
        //diz em qual endereço ele vai escutar
        _listener.Prefixes.Add("http://localhost:5000/");
    }

    public void Iniciar()
    {
        //Liga o servidos e começa a aceitar conexões
        _listener.Start();
        Console.WriteLine("Servidor iniciado em http://localhost:5000/");

        //loop infinito - fica esperando requisições para sempre 
        while (true)
        {
            //trava aqui até alguem fazer uma requisição
            var context = _listener.GetContext();
            //QUando chega uma requisição, processa em paralelo sem travar o loop
            Task.Run(() => ProcessarRequisicao(context));
            //Task.run faz com que a requisição seja processada em paralelo
            //sem ele, a segunda requisição teria que esperar a primeira ser processada
        }
    }

    public void ProcessarRequisicao(HttpListenerContext context)
    {
        var request = context.Request; //o que o front mandou
        var response = context.Response;// o que vamos devolver

        // adiciona headers CORS, permite o frontend em ooutra porta acessar
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        //se for OPTIONS, é só o navegador perguntando "se pode", responde ok e sai
        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.Close();
            return;
        }

        try
        {
            //pega a URL e o método HTTP
            string caminho = request.Url!.AbsolutePath;
            string metodo = request.HttpMethod;

            //Roteia - decide qual handler chamar baseado na URL + método
            if (metodo == "GET" && caminho == "/api/contas")
            {
                HandleListarContas(context);
            }
            else if (metodo == "POST" && caminho == "/api/contas")
            {
                HandleCriarConta(context);
            }
            else if(metodo == "GET" && caminho.EndsWith("/transacoes"))
            {
                int id = ExtrairId(caminho);
                HandleListrarTransacoes(context, id);
            }
            else if(metodo == "GET" && caminho.StartsWith("/api/contas") && !caminho.EndsWith("/transacoes"))
            {
                int id = ExtrairId(caminho);
                HandleBuscarConta(context, id);
            }
            else if (metodo == "POST" && caminho.EndsWith("/depositar"))
            {
                int id = ExtrairId(caminho);
                HandleDepositar(context, id);
            }
            else if (metodo == "POST" && caminho.EndsWith("/sacar"))
            {
                int id = ExtrairId(caminho);
                HandleSacar(context, id);
            }
            else if(metodo == "POST" && caminho =="/api/contas/transferir")
            {
                HandleTransferir(context);
            }
            else if(metodo == "POST" && caminho == "/api/registrar")
            {
                HandleRegistrar(context);
            }
            else if(metodo == "POST" && caminho == "/api/login")
            {
                HandleLogin(context);
            }            
            else
            {
                //nenhuma rota bateu, retorna 404
                response.StatusCode = 404;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            //Se qualquer coisa explodir, retorna 500(erro interno)
            Console.WriteLine($"Erro: {ex.Message}");
            response.StatusCode = 500;
            response.Close();
        }
    }

    private void HandleRegistrar(HttpListenerContext context)
    {
        //lê o body que o front enviou
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();

        //Transforma o json em algo legível
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        //extrai os campos do json
        string email = dados.GetProperty("email").GetString();
        string senha = dados.GetProperty("senha").GetString();
        string titular = dados.GetProperty("titular").GetString();
        decimal saldoInicial = dados.GetProperty("saldoInicial").GetDecimal();
        string tipo = dados.GetProperty("tipo").GetString() ?? "corrente";

        //cria conta bancaria
        int contaId = _banco.CriarConta(titular, saldoInicial, tipo);

        //cria usuario e vincula a conta bancaria
        //pois a variavel acima retorna o id da conta recem criada, e aqui apontamos pra ele
        _banco.CriarUsuario(email, senha, contaId);

        //envia resposta
        //o new{} é um objeto anônimo, só pra organizar melhor
        string json = JsonSerializer.Serialize(new{ 
            id = contaId,
            titular = titular,
            saldo = saldoInicial,
            email = email,
            mensagem = "Usuário registrado com sucesso"});
        EnviarResposta(context, json, 201);

    }

    public void HandleLogin(HttpListenerContext context)
    {
        //Lê o body que o front mandou
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();

        //transforma json em algo legível
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        //extrai os campos do json
        string email = dados.GetProperty("email").GetString();
        string senha = dados.GetProperty("senha").GetString();

        var usuario = _banco.BuscarUsuarioPorEmail(email);

        if (usuario == null)
        {
            EnviarResposta(context, "{\"erro\":\"Usuário não encontrado\"}", 404);
            return;
        }
        if (senha == usuario.Senha)
        {
            var conta = _banco.BuscarContaPorId(usuario.ContaId);

            string json = JsonSerializer.Serialize(new{
                id = conta.Id,
                titular = conta.Titular,
                saldo = conta.Saldo,
                email = usuario.Email
            });
            EnviarResposta(context, json, 200);
        }
        else{
            
            EnviarResposta(context, "{\"erro\":\"Senha incorreta\"}", 400);

        }

    }

    private void HandleTransferir(HttpListenerContext context)
    {   
        //Lê o body que o front mandou
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();

        //transforma o json em algo legível
        var dados = JsonSerializer.Deserialize<JsonElement>(body);
        //extrai os campos do json
        int idOrigem = dados.GetProperty("idOrigem").GetInt32();
        int idDestino = dados.GetProperty("idDestino").GetInt32();
        decimal valor = dados.GetProperty("valor").GetDecimal();

        // Busca as duas contas
        var origem = _banco.BuscarContaPorId(idOrigem);
        var destino = _banco.BuscarContaPorId(idDestino);

        //valida se é nulo
        if (origem == null || destino == null)
        {
            EnviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }
        //valida se o saldo de origem é menor que o valor a enviar
        if (origem.Saldo < valor)
        {
            EnviarResposta(context, "{\"erro\":\"Saldo insuficiente\"}", 400);
            return;
        }

        // Saca da origem, deposita no destino
        origem.Sacar(valor);
        destino.Depositar(valor);

        // Salva os dois saldos no banco
        _banco.AtualizarSaldo(idOrigem, origem.Saldo);
        _banco.AtualizarSaldo(idDestino, destino.Saldo);
        //registra a transação no banco
        _banco.RegistrarTransacao(idOrigem, "transferencia", valor);

        string json = JsonSerializer.Serialize(new { mensagem = "Transferência realizada com sucesso",
         saldo = origem.Saldo});
        EnviarResposta(context, json, 200);
    }


    private void HandleListarContas(HttpListenerContext context)
    {   
        //Pede pro banco todas as contas
        var contas = _banco.ListarContas();
        //Transforma a lista de objetos em json/serialização
        string json = JsonSerializer.Serialize(contas);
        //Manda pro fronted
        EnviarResposta(context, json, 200);
    }

    private void HandleCriarConta(HttpListenerContext context)
    {
        //Lê o body que o frontend mandou 
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        //transforma o texto json em algo que podemos ler/Deserialização
        var dados = JsonSerializer.Deserialize<JsonElement>(body);

        //Extrai os campos do json
        string titular = dados.GetProperty("titular").GetString() ?? "";
        decimal saldoInicial = dados.GetProperty("saldoInicial").GetDecimal();

        //Manda pro banco criar
        _banco.CriarConta(titular, saldoInicial);

        //Responde pro frontend que deu certo a operação
        string json = JsonSerializer.Serialize(new { mensagem = "Conta criada com sucesso" });
        EnviarResposta(context, json, 201);
    }

    private void HandleDepositar(HttpListenerContext context, int id)
    {
        //Lê o body
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);
        decimal valor = dados.GetProperty("valor").GetDecimal();

        //Busca a conta no banco
        var conta = _banco.BuscarContaPorId(id);
        //se a conta não existe, retorna 404
        if (conta == null)
        {
            EnviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }

        //executa o depósito no objeto(soma o valor ao saldo)
        conta.Depositar(valor);
        //Salva novo saldo no banco de dados
        _banco.AtualizarSaldo(id, conta.Saldo);

        //registra o deposito no banco
        _banco.RegistrarTransacao(id, "deposito", valor);
        //retorna a conta atualizada
        string json = JsonSerializer.Serialize(conta);
        EnviarResposta(context, json, 200);
    }

    private void HandleSacar(HttpListenerContext context, int id)
    {
        //Lê o body
        using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
        string body = reader.ReadToEnd();
        var dados = JsonSerializer.Deserialize<JsonElement>(body);
        decimal valor = dados.GetProperty("valor").GetDecimal();

        //Busca a conta no banco(mesmo coisa que depositar)
        var conta = _banco.BuscarContaPorId(id);
        //se não existe, retorna 404
        if (conta == null)
        {
            EnviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        }

        //verifica se tem saldo suficiente antes de sacar
        if (conta.Saldo < valor)
        {
            //não deixa sacar - retorna erro 400
            string erroJson = JsonSerializer.Serialize(new { erro = "Saldo insuficiente" });
            EnviarResposta(context, erroJson, 400);
            return; // para aqui e não executa o saque
        }

        //se tiver saldo executa o saldo
        conta.Sacar(valor);
        //atualiza o saldo atual no banco
        _banco.AtualizarSaldo(id, conta.Saldo);

        //registra o saque no banco
        _banco.RegistrarTransacao(id, "saque", valor);

        //retorna a conta atualizada
        string json = JsonSerializer.Serialize(conta);
        EnviarResposta(context, json, 200);
    }

    private int ExtrairId(string caminho)
    {
        // URL: "/api/contas/3/depositar" → split: ["", "api", "contas", "3", "depositar"]
        string[] partes = caminho.Split('/');
        return int.Parse(partes[3]);//pega 3 e converte para int
    }

    private void EnviarResposta(HttpListenerContext context, string json, int statusCode)
    {
        //Converte o texto json em bytes ( o http trabalha com bytes)
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        //define o status(200, 201, 400, 404, 500
        context.Response.StatusCode = statusCode;
        //diz que o conteúdo é json
        context.Response.ContentType = "application/json";
        //Diz o tamanho da resposta
        context.Response.ContentLength64 = buffer.Length;
        //Escreve em bytes no "cano" de saída
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        //Fecha a conexão - Resposta enviada
        context.Response.Close();
    }

    private void HandleListrarTransacoes(HttpListenerContext context, int id)
    {
        var transacoes = _banco.ListarTransacoes(id);
        string json = JsonSerializer.Serialize(transacoes);
        EnviarResposta(context, json, 200);
    }

    private void HandleBuscarConta(HttpListenerContext context, int id)
    {
        var conta = _banco.BuscarContaPorId(id);
        if(conta == null)
        {
            EnviarResposta(context, "{\"erro\":\"Conta não encontrada\"}", 404);
            return;
        };
        string json = JsonSerializer.Serialize(conta);
        EnviarResposta(context, json, 200);
    }
}

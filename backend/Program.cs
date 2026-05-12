using BankSystem.Server;
using BankSystem.Database;
using System;

var banco = new BancoDeDados("Server=localhost;Database=banco_sistema;User=root;Password=1234;");

var servidor = new HttpServer(banco);

//chama o metodo AplicarRendimento do banco a cada 20 segundos, rendendo o saldo em conta
var timer = new System.Threading.Timer(_ =>
{
    banco.AplicarRendimento();
    Console.WriteLine("Rendimento aplicado!");
}, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));

Console.WriteLine("Servidor rodando em http://localhost:5000/");
servidor.Iniciar();

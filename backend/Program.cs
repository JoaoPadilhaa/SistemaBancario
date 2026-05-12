using BankSystem.Server;
using BankSystem.Database;
using System;

var banco = new BancoDeDados("Server=localhost;Database=banco_sistema;User=root;Password=1234;");

var servidor = new HttpServer(banco);

Console.WriteLine("Servidor rodando em http://localhost:5000/");
servidor.Iniciar();

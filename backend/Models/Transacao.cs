namespace BankSystem.Models;

using System;

public class Transacoes{
    private int id;
    private int contaId;
    private string tipo;
    private decimal valor;
    private DateTime data;

    public Transacoes(int id, int contaId, string tipo, decimal valor, DateTime data)
    {
        this.id = id;
        this.contaId = contaId;
        this.tipo = tipo;
        this.valor = valor;
        this.data = data;
    }

    public int Id {get => id; set => id = value;}
    public int ContaId {get => contaId; set => contaId = value;}
    public string Tipo {get => tipo; set => tipo = value;}
    public decimal Valor {get => valor; set => valor = value;}
    public DateTime Data {get => data; set => data = value;}
}
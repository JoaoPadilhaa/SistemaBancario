namespace BankSystem.Models;

using System;
//classe que representa uma conta bancária.

public class ContaBancaria
{
    private int id;
    private string titular;
    private decimal saldo;

    public ContaBancaria(int id, string titular, decimal saldo){
        this.id = id;
        this.titular = titular;
        this.saldo = saldo;
    }
    public int Id { get => id; set => id = value;}
    public string Titular { get => titular; set => titular = value;}
    public decimal Saldo { get => saldo; private set => saldo = value;}

    public void Sacar(decimal valor) {
        if (valor > 0 && valor <= saldo)
            {
               saldo -= valor;
            } else {
                throw new ArgumentException("Valor digitado inválido!");
            }
        
    }   

    public void Depositar(decimal valor) {
        if (valor > 0)
            {
                saldo += valor;
            } else {
                throw new ArgumentException("Valor digitado inválido!");
            }
        
        
    }
}
